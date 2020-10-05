using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Visualizer : NarseseParser
{
    Dictionary<string, Concept> conceptTable; //holds concepts, accessed by term
    Dictionary<string, Inheritance> inheritTable; //holds whether a certain inheritance exists
    Dictionary<Concept, Queue<Concept>> termLinkTable; //holds whether a certain termlink exists

    Queue<string> newStatementQueue = new Queue<string>(); //new statements to parse
    Queue<string> newConceptQueue = new Queue<string>(); //new concepts queued for visualization
    Queue<Statement> newInheritQueue = new Queue<Statement>(); //new inheritances queued for visualization (subject, predicate)
    Queue<Statement> newTermLinkQueue = new Queue<Statement>(); //new statements to parse

    public GameObject conceptPrefab, inheritancePrefab, termLinkPrefab;

    Text hideShowStatementTermsBtnTxt, hideShowCompoundTermsBtnTxt;

    // Start is called before the first frame update
    void Start()
    {
        conceptTable = new Dictionary<string, Concept>();
        inheritTable = new Dictionary<string, Inheritance>();
        termLinkTable = new Dictionary<Concept, Queue<Concept>>();

        hideShowStatementTermsBtnTxt = GameObject.Find("HideShowStatementTermsBtnTxt").GetComponent<Text>();
        hideShowCompoundTermsBtnTxt = GameObject.Find("HideShowCompoundTermsBtnTxt").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

        if(newStatementQueue.Count > 0)
        {
            string outputStr = newStatementQueue.Dequeue();
            if (!(outputStr.Contains("#") || outputStr.Contains("$")))
            {
                Debug.Log("Try parsing pending statement");
                VisualizeStatement(outputStr);
            }
        }

        if (newConceptQueue.Count > 0)
        {
            Debug.Log("Try visualize pending concept");
            string newConceptName = newConceptQueue.Dequeue();
            VisualizeNewConcept(newConceptName);
        }

        if (newInheritQueue.Count > 0)
        {
            Debug.Log("Try visualize pending inheritance");
            Statement newInherit = newInheritQueue.Dequeue();
            bool success = VisualizeNewInherit(newInherit);
            if (!success)
            {
                newInheritQueue.Enqueue(newInherit);
            }
        }

        if (newTermLinkQueue.Count > 0)
        {
            Debug.Log("Try visualize pending inheritance");
            Statement newInherit = newTermLinkQueue.Dequeue();
            bool success = VisualizeNewTermLink(newInherit);
            if (!success)
            {
                newInheritQueue.Enqueue(newInherit);
            }
        }

        //MoveTermLinksTogether();
        UpdateShowHideVisualization();
    }

    public void MoveTermLinksTogether()
    {
        foreach (KeyValuePair<Concept, Queue<Concept>> entry in termLinkTable)
        {
            foreach (Concept concept in entry.Value)
            {
                entry.Key.transform.position = Vector3.MoveTowards(entry.Key.transform.position, concept.transform.position, .001f);
                concept.transform.position = Vector3.MoveTowards(concept.transform.position, entry.Key.transform.position, .001f);
            }
        }
    }

    public Concept GetConcept(string conceptName)
    {
        if (conceptTable.ContainsKey(conceptName)) return conceptTable[conceptName];
        return null;
    }

    //Show/Hide options
    bool _updateVisualization = false;
    bool _areStatementTermsHidden;
    bool _areCompoundTermsHidden;
    string STATEMENT_STR = "statement terms";
    string COMPOUND_STR = "compound terms";

    public void UpdateShowHideVisualization()
    {
        if (_updateVisualization)
        {
            if (_areStatementTermsHidden)
            {
                HideStatementTerms();
            }
            else
            {
                ShowStatementTerms();
            }

            if (_areCompoundTermsHidden)
            {
                HideCompoundTerms();
            }
            else
            {
                ShowCompoundTerms();
            }

            _updateVisualization = false;
        }
    }

    public void ToggleShowHideStatementTerms()
    {
        _areStatementTermsHidden = !_areStatementTermsHidden;
        _updateVisualization = true;
    }

    private void HideStatementTerms()
    {
        foreach (KeyValuePair<string, Concept> entry in conceptTable)
        {
            if (entry.Value.isStatementTerm())
            {
                entry.Value.gameObject.SetActive(false);
            }
        }
        hideShowStatementTermsBtnTxt.text = "Show " + STATEMENT_STR;
        _areStatementTermsHidden = true;
    }

    private void ShowStatementTerms()
    {
        foreach (KeyValuePair<string, Concept> entry in conceptTable)
        {
            if (entry.Value.isStatementTerm())
            {
                if(_areCompoundTermsHidden && entry.Value.isCompoundTerm()) { continue; }
                entry.Value.gameObject.SetActive(true);
            }
        }
        hideShowStatementTermsBtnTxt.text = "Hide " + STATEMENT_STR;
        _areStatementTermsHidden = false;
    }

    public void ToggleShowHideCompoundTerms()
    {
        _areCompoundTermsHidden = !_areCompoundTermsHidden;
        _updateVisualization = true;
    }

    private void HideCompoundTerms()
    {
        //hide compound terms
        foreach (KeyValuePair<string, Concept> entry in conceptTable)
        {
            if (entry.Value.isCompoundTerm())
            {
                entry.Value.gameObject.SetActive(false);
            }
        }

        //hide inheritances connecting compound terms
        foreach (KeyValuePair<string, Inheritance> entry in inheritTable)
        {
            if (entry.Value.ConnectsCompoundTerm())
            {
                entry.Value.gameObject.SetActive(false);
            }
        }

        hideShowCompoundTermsBtnTxt.text = "Show " + COMPOUND_STR;
        _areCompoundTermsHidden = true;
    }

    private void ShowCompoundTerms()
    {
        //show compound terms
        foreach (KeyValuePair<string, Concept> entry in conceptTable)
        {
            if (entry.Value.isCompoundTerm())
            {
                if (_areStatementTermsHidden && entry.Value.isStatementTerm()) { continue; }
                entry.Value.gameObject.SetActive(true);
            }
        }

        //show inheritances connecting compound terms
        foreach (KeyValuePair<string, Inheritance> entry in inheritTable)
        {
            if (entry.Value.ConnectsCompoundTerm())
            {
                entry.Value.gameObject.SetActive(true);
            }
        }

        hideShowCompoundTermsBtnTxt.text = "Hide " + COMPOUND_STR;
        _areCompoundTermsHidden = false;
    }


    //actually create the new visualization
    public void VisualizeNewConcept(string conceptName)
    {

        Vector3 offset = new Vector3(Random.Range(-10f - conceptTable.Count / 10f, 10f + conceptTable.Count/10f), Random.Range(-10f - conceptTable.Count / 10f, 10f + conceptTable.Count / 10f), Random.Range(-10f - conceptTable.Count / 10f, 10f + conceptTable.Count / 10f));

        GameObject newConceptGO = Instantiate(conceptPrefab, offset, Quaternion.identity);
        Concept newConcept = newConceptGO.GetComponent<Concept>();
        newConcept.Init(conceptName);

        if (newConcept.isStatementTerm())
        {
            if(_areStatementTermsHidden)
                newConceptGO.SetActive(false);

            QueueVisualizeNewTermLink(new Statement(conceptName + "%1;1%"), newConcept);
        }


        if (newConcept.isCompoundTerm() && _areCompoundTermsHidden)
        {
            newConceptGO.SetActive(false);
        }

        conceptTable.Add(conceptName, newConceptGO.GetComponent<Concept>());
    }

    public bool VisualizeNewInherit(Statement statement)
    {
        string subjectKey = statement.subjectPredicate._subject;
        string predicateKey = statement.subjectPredicate._predicate;

        string statementKey = GetInheritanceString(statement.subjectPredicate);

        if(!(conceptTable.ContainsKey(subjectKey) && conceptTable.ContainsKey(predicateKey)))
        {
            return false;
        }

        if (inheritTable.ContainsKey(statementKey)) {
            return true;
        }

        Concept subject = conceptTable[subjectKey];
        Concept predicate = conceptTable[predicateKey];

        GameObject newInheritanceGO = Instantiate(inheritancePrefab);
        Inheritance newInheritance = newInheritanceGO.GetComponent<Inheritance>();
        newInheritance.Init(statement.truthValue, subject, predicate);

        if (newInheritance.ConnectsCompoundTerm() && _areCompoundTermsHidden)
        {
            newInheritanceGO.SetActive(false);
        }

        inheritTable.Add(statementKey, newInheritance);

        Debug.Log("MADE INHERIT " + statementKey);
        return true;
    }

    //queue the new visualization (since creating a new visualization directly from output is broken)
    public void QueueVisualizeNewInherit(Statement statement)
    {
        string statementKey = GetInheritanceString(statement.subjectPredicate);
        if (!inheritTable.ContainsKey(statementKey) && !newInheritQueue.Contains(statement))
        {
            Debug.Log("QUEUE INHERIT " + statementKey);
            newInheritQueue.Enqueue(statement);
        }else if (inheritTable.ContainsKey(statementKey))
        {
            inheritTable[statementKey].SetTruthValue(statement.truthValue);
        }
    }

    public void QueueVisualizeNewConcept(string conceptName)
    {
        if (!conceptTable.ContainsKey(conceptName) && !newConceptQueue.Contains(conceptName))
        {
            Debug.Log("QUEUE CONCEPT " + conceptName + conceptTable.ContainsKey(conceptName) + newConceptQueue.Contains(conceptName));
            newConceptQueue.Enqueue(conceptName);
        }else if (conceptTable.ContainsKey(conceptName))
        {
            conceptTable[conceptName].Fire();
        }
    }

    public void QueueVisualizeNewTermLink(Statement statement, Concept statementConcept)
    {
        if (!termLinkTable.ContainsKey(statementConcept) && !newTermLinkQueue.Contains(statement))
        {
            newTermLinkQueue.Enqueue(statement);
        }
    }


    //queue the new visualization (since creating a new visualization directly from output is broken)
    //returns success
    public bool VisualizeNewTermLink(Statement statement)
    {
        string statementKey = GetInheritanceString(statement.subjectPredicate);
        if (!conceptTable.ContainsKey(statementKey)) { return false; }
        Concept statementConcept = conceptTable[statementKey];

        if (conceptTable.ContainsKey(statement.subjectPredicate._subject) && conceptTable.ContainsKey(statement.subjectPredicate._predicate)) {
            Concept subjectConcept = conceptTable[statement.subjectPredicate._subject];
            Concept predicateConcept = conceptTable[statement.subjectPredicate._predicate];

            if (!termLinkTable.ContainsKey(statementConcept)) { 
                Queue<Concept> links = new Queue<Concept>();
                termLinkTable.Add(statementConcept, links);
            }

            if (!termLinkTable.ContainsKey(subjectConcept))
            {
                Queue<Concept> links = new Queue<Concept>();
                termLinkTable.Add(subjectConcept, links);
            }

            if (!termLinkTable.ContainsKey(predicateConcept))
            {
                Queue<Concept> links = new Queue<Concept>();
                termLinkTable.Add(predicateConcept, links);
            }

            if (!termLinkTable[statementConcept].Contains(subjectConcept))
            {
                termLinkTable[statementConcept].Enqueue(subjectConcept);
            }

            if (!termLinkTable[subjectConcept].Contains(statementConcept))
            {
                termLinkTable[subjectConcept].Enqueue(statementConcept);
            }

            if (!termLinkTable[statementConcept].Contains(predicateConcept))
            {
                termLinkTable[statementConcept].Enqueue(predicateConcept);
            }

            if (!termLinkTable[predicateConcept].Contains(statementConcept))
            {
                termLinkTable[predicateConcept].Enqueue(statementConcept);
            }

            GameObject newTermLinkGO = Instantiate(termLinkPrefab);
            TermLink termLink = newTermLinkGO.GetComponent<TermLink>();
            termLink.Init(statementConcept, subjectConcept);

            GameObject newTermLinkGO2 = Instantiate(termLinkPrefab);
            TermLink termLink2 = newTermLinkGO2.GetComponent<TermLink>();
            termLink2.Init(statementConcept, predicateConcept);

            return true;
        }

        return false;
    }

    public void QueueStatement(string statement)
    {
        newStatementQueue.Enqueue(statement);
    }

    void VisualizeStatement(string statement)
    {
        Statement parsedStatement = new Statement(statement);
        if (parsedStatement.type == Statement.StatementType.Unsupported) { return; } //failed to create statement; unsupported statement

        SubjectPredicate subjectPredicate = parsedStatement.subjectPredicate;
        TruthValue truthValue = parsedStatement.truthValue;

        //visualize subject, predicate as terms

        QueueVisualizeNewConcept(subjectPredicate._subject);
        QueueVisualizeNewConcept(subjectPredicate._predicate);

        string statementKey = "";
        //visualize inheritance relation
        if (parsedStatement.type == Statement.StatementType.Inheritance)
        {
            statementKey = GetInheritanceString(subjectPredicate);
            QueueVisualizeNewConcept(statementKey);
            if (inheritTable.ContainsKey(statementKey) && inheritTable[statementKey].GetTruthValue().GetHashCode() == truthValue.GetHashCode()) { return; }

            QueueVisualizeNewInherit(parsedStatement);
        }
        else if (parsedStatement.type == Statement.StatementType.Similarity)
        {
            statementKey = GetSimilarityString(subjectPredicate);
            QueueVisualizeNewConcept(statementKey);

            Statement parsedReverseStatement = NarseseParser.GetReverseStatement(statement);

            string forwardStatementKey = GetInheritanceString(subjectPredicate);
            string reverseStatementKey = GetInheritanceString(parsedReverseStatement.subjectPredicate);

            if (inheritTable.ContainsKey(forwardStatementKey) && inheritTable[forwardStatementKey].GetTruthValue().GetHashCode() == truthValue.GetHashCode()) { return; }
            QueueVisualizeNewInherit(parsedStatement);

            if (inheritTable.ContainsKey(reverseStatementKey) && inheritTable[reverseStatementKey].GetTruthValue().GetHashCode() == parsedReverseStatement.truthValue.GetHashCode()) { return; }
            QueueVisualizeNewInherit(parsedReverseStatement);
        }
        else
        {
            return;
        }

        
    }

    private string GetInheritanceString(SubjectPredicate subjectPredicate)
    {
        return "<" + subjectPredicate._subject + NarseseParser.INHERITANCE_COPULA + subjectPredicate._predicate + ">";
    }

    private string GetSimilarityString(SubjectPredicate subjectPredicate)
    {
        return "<" + subjectPredicate._subject + NarseseParser.SIMILARITY_COPULA + subjectPredicate._predicate + ">";
    }

}

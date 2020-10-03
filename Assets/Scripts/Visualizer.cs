using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    Dictionary<string, Concept> conceptTable;
    Dictionary<string, bool> inheritTable;
    Dictionary<(int, int), bool> spaceAvailableTable;

    Queue<string> newConceptQueue = new Queue<string>();
    Queue<QueuedNewInherit> newInheritQueue = new Queue<QueuedNewInherit>();

    public GameObject conceptPrefab, inheritancePrefab;

    public class QueuedNewInherit
    {

        public string Subj, Pred;
        public QueuedNewInherit(string subj, string pred)
        {
            Subj = subj;
            Pred = pred;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        conceptTable = new Dictionary<string, Concept>();
        inheritTable = new Dictionary<string, bool>();
    }

    // Update is called once per frame
    void Update()
    {
        if (newConceptQueue.Count > 0)
        {
            string newConceptName = newConceptQueue.Dequeue();
            VisualizeNewConcept(newConceptName);
        }else if(newInheritQueue.Count > 0)
        {
            QueuedNewInherit newInherit = newInheritQueue.Peek();
            bool success = VisualizeNewInherit(newInherit.Subj, newInherit.Pred);
            if (success)
            {
                newInheritQueue.Dequeue();
            }
        }
    }


    //actually create the new visualization
    public void VisualizeNewConcept(string conceptName)
    {
        GameObject newConcept = Instantiate(conceptPrefab, new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f), Quaternion.identity);
        newConcept.GetComponent<Concept>().Init(conceptName);
        conceptTable.Add(conceptName, newConcept.GetComponent<Concept>());
    }

    public bool VisualizeNewInherit(string subject, string predicate)
    {
        if (!(conceptTable.ContainsKey(subject) && conceptTable.ContainsKey(predicate))) return false;
        if (inheritTable.ContainsKey(subject + "-->" + predicate)) return true;

        Transform subjGO = conceptTable[subject].transform;
        Transform predGO = conceptTable[predicate].transform;

        GameObject newInheritance = Instantiate(inheritancePrefab);

        Vector3[] positions = new Vector3[2];
        positions[0] = subjGO.position;
        positions[1] = predGO.position;
        newInheritance.GetComponent<LineRenderer>().SetPositions(positions);

        inheritTable.Add(subject + "-->" + predicate, true);
        Debug.Log("MADE INHERIT " + subject + " --> " + predicate);
        return true;
    }

    //queue the new visualization (since creating a new visualization directly from output is broken)
    public void QueueVisualizeNewInherit(string subject, string predicate)
    {
        newInheritQueue.Enqueue(new QueuedNewInherit(subject, predicate));
    }

    public void QueueVisualizeNewConcept(string conceptName)
    {
        if (!conceptTable.ContainsKey(conceptName) && !newConceptQueue.Contains(conceptName))
        {
            newConceptQueue.Enqueue(conceptName);
        }
    }
}

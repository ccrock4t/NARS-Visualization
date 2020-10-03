using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    Dictionary<string, Concept> conceptTable; //holds concepts, accessed by term
    Dictionary<string, bool> inheritTable; //holds whether a certain inheritance exists
    Dictionary<(int, int), bool> spaceAvailableTable;

    Queue<string> newConceptQueue = new Queue<string>(); //new concepts queued for visualization
    Queue<(string, string)> newInheritQueue = new Queue<(string, string)>(); //new inheritances queued for visualization (subject, predicate)

    public GameObject conceptPrefab, inheritancePrefab;

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
            Debug.Log("Try visualize pending concept");
            string newConceptName = newConceptQueue.Dequeue();
            VisualizeNewConcept(newConceptName);
        }
        
        if(newInheritQueue.Count > 0)
        {
            Debug.Log("Try visualize pending inheritance");
            (string, string) newInherit = newInheritQueue.Dequeue();
            bool success = VisualizeNewInherit(newInherit.Item1, newInherit.Item2);
            if (!success)
            {
                newInheritQueue.Enqueue(newInherit);
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
        if (inheritTable.ContainsKey(GetInheritanceString(subject, predicate))) return true;

        Transform subjGO = conceptTable[subject].transform;
        Transform predGO = conceptTable[predicate].transform;

        GameObject newInheritance = Instantiate(inheritancePrefab);

        Vector3[] positions = new Vector3[2];
        positions[0] = subjGO.position;
        positions[1] = predGO.position;
        newInheritance.GetComponent<LineRenderer>().SetPositions(positions);

        inheritTable.Add(GetInheritanceString(subject, predicate), true);
        Debug.Log("MADE INHERIT " + GetInheritanceString(subject, predicate));
        return true;
    }

    //queue the new visualization (since creating a new visualization directly from output is broken)
    public void QueueVisualizeNewInherit(string subject, string predicate)
    {
        if (!inheritTable.ContainsKey(GetInheritanceString(subject, predicate)) && !newInheritQueue.Contains((subject, predicate)))
        {
            Debug.Log("QUEUE INHERIT " + GetInheritanceString(subject, predicate));
            newInheritQueue.Enqueue((subject, predicate));
        }
    }

    public void QueueVisualizeNewConcept(string conceptName)
    {
        if (!conceptTable.ContainsKey(conceptName) && !newConceptQueue.Contains(conceptName))
        {
            Debug.Log("QUEUE CONCEPT " + conceptName);
            newConceptQueue.Enqueue(conceptName);
        }
    }

    private string GetInheritanceString(string subject, string predicate)
    {
        return subject + NARSHost.INHERIT_STRING + predicate;
    }
}

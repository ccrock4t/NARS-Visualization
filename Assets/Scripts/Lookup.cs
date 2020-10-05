using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lookup : MonoBehaviour
{
    TMP_InputField inputTextField;

    // Start is called before the first frame update
    void Start()
    {
        inputTextField = GameObject.Find("LookupField").GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LookupConcept(string input)
    {
        Concept concept = this.GetComponent<Visualizer>().GetConcept(input);
        if (concept != null) {
            Camera.main.transform.position = new Vector3(concept.transform.position.x, concept.transform.position.y, -3f);
        }
    }

    public void AddInputText()
    {
        LookupConcept(inputTextField.text);
        inputTextField.text = "";
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Concept : MonoBehaviour
{
    public TextMeshPro nameText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(string name)
    {
        nameText.text = name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

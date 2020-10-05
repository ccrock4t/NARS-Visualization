using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Materials : MonoBehaviour
{
    static Materials _instance;
    public Material conceptMaterial;
    public Material cyanMaterial;

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Materials GetInstance()
    {
        return _instance;
    }
}

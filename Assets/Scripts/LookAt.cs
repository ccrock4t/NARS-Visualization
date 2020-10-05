using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;
    public bool lookAtCamera;
    public bool lookAtOrigin;

    private void Start()
    {
        if (lookAtCamera)
        {
            target = Camera.main.transform;
        }
    }
    void Update()
    {
        // Rotate the camera every frame so it keeps looking at the target
        if (lookAtOrigin)
        {
            transform.LookAt(Vector3.zero);
        }
        else
        {
            transform.LookAt(target);
        }

        // Same as above, but setting the worldUp parameter to Vector3.left in this example turns the camera on its side
        //transform.LookAt(target, Vector3.left);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    float dragSpeed = -2;
    private Vector3 dragOrigin;


    void Update()
    {
        //zoom
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            transform.Translate(new Vector3(0,0,Input.GetAxis("Mouse ScrollWheel")*5f*Mathf.Abs(Camera.main.transform.position.z)), Space.World);
        }

        if(Camera.main.transform.position.z >= 0f)
        {
            Camera.main.transform.position += new Vector3(0, 0, -.5f);
        }

        //drag
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

        transform.Translate(move, Space.World);
    }

    
}

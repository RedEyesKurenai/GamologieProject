using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardHealth : MonoBehaviour
{
    public Transform cam; //la camera

    // Update is called once per frame
    void LateUpdate() //pour suivre la caméra
    {
        transform.LookAt(transform.position + cam.forward);
        
    }
}

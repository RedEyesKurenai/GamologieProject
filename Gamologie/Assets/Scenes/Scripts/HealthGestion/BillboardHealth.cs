using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BillboardHealth : MonoBehaviour
{
    //reference vers la camera
    public Transform cam; 

    /*
     LateUpdate is called once per frame
     Fonction permettant de toujours positionner l'objet face � la cam�ra 
    */
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
        
    }
}

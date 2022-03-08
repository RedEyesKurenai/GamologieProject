using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaypointFollower : MonoBehaviour
{
    [SerializeField] GameObject waypoint;
    [SerializeField] GameObject sphereCheck;
    [SerializeField] LayerMask player;
    [SerializeField] float Speed = 1f;
 

    

    void Update()
    {
        if (IsRange())
        {
            

            transform.position = Vector3.MoveTowards(transform.position, waypoint.transform.position, Speed * Time.deltaTime);

        }
    }

    bool IsRange()
    {
        return Physics.CheckSphere(sphereCheck.transform.position, sphereCheck.GetComponent<SphereCollider>().radius, player);
    }
}

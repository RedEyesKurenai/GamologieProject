using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Camera cam; //la camera

    public UnityEngine.AI.NavMeshAgent agent; //le player
   
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) //0 c'est click gauche normal
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); //enregistre le bouton
            RaycastHit hit; //objet touch� par le ray cast

            if(Physics.Raycast(ray, out hit)) //si on a touch� qlq chose un objet et non le ciel
            {
                agent.SetDestination(hit.point); //si touch� alors il se d�place vers cette destination

            }

        }
        
    }
}

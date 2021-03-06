using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    /*
     Vitesse de rotation des axes
     */

    [SerializeField] float speedX= 1;
    [SerializeField] float speedY=1 ;
    [SerializeField] float speedZ=1 ;


    /*
     * Update is called once per frame
     * Fonction permettant la rotation de l'objet 
     */
    void Update()
    {
        transform.Rotate(360 * speedX * Time.deltaTime, 360 * speedY * Time.deltaTime, 360 * speedZ * Time.deltaTime);
    }
}
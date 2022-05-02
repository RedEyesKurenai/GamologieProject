using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class HealthBar : MonoBehaviour
{
    /* référence à la barre de vie */
    public Slider slider;

    /*Fonction permettant de modifier la valeur maximal de la barre de vie et la valeur initiale */
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

    }

    /*Fonction permettant de modifier la valeur courante de la barre de vie */
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}

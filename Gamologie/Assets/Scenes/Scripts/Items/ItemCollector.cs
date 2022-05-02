using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemCollector : MonoBehaviour
{
   
    /*
    valeur initiale du nombre des pi�ces et de la sant�
    */
    int coins = 0;
    int health = 300;

    /*
     R�f�rence � l'affichage textuel du nombre de pi�ces et de sant�
     */
    [SerializeField] Text coinsText;
    [SerializeField] Text healthText;

    /*
     Fonction permettant de detruire l'item s'il est touch� et de mettre � jour les valeurs sur l'affichage textuel
        l'objet other est l'item touch�
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            coins++;
            Debug.Log("+1 coin");
            coinsText.text = "Coins :" + coins;
        }

        if (other.gameObject.CompareTag("Hlth"))
        {
            Destroy(other.gameObject);
            health = Mathf.Min(health + 100, 1000);
            Debug.Log("+100 health");
            healthText.text ="Health :" + health;
        }

        if (other.gameObject.CompareTag("Poison"))
        {
            Destroy(other.gameObject);
            health = Mathf.Max(health - 100, 0);
            Debug.Log("-100 health");
            healthText.text = "Health :" + health;
            
        }

    }

    
}

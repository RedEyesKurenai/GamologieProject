using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemCollector : MonoBehaviour
{
    int coins = 0;
    int health = 300;

    [SerializeField] Text coinsText;
    [SerializeField] Text healthText;

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

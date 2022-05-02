using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerLife : MonoBehaviour
{

    public float health;

    //health bar
    public HealthBar healthbar;

    /*
     * Fonction qui initialise la vie maximal de HealthBar
     */
    void Start()
    {
        healthbar.SetMaxHealth( (int)health );
    }

    /*
     Fonction qui met à jour l' Health Bar
     */
    void Update()
    {
        healthbar.SetHealth((int)health);
    }


    /*
     * diminue la vie de la tour
     * */
    public void TakeDamage(int damage)
    {

        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    /*
     * fonction qui detruit de la tour 
     * */
    private void DestroyEnemy()
    {

        Destroy(gameObject);

    }

}

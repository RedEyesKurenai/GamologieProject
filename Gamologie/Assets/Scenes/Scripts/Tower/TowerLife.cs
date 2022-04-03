using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerLife : MonoBehaviour
{

    public float health;

    //health bar
    public HealthBar healthbar;

      void Start()
    {
        //HealthBar
        healthbar.SetMaxHealth( (int)health );
    }

    void Update()
    {
        //Health Bar
        healthbar.SetHealth((int)health);
    }



        public void TakeDamage(int damage)
    {

        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }


    private void DestroyEnemy()
    {

        Destroy(gameObject);

    }

}

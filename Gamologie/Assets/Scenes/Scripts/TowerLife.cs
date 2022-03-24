using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerLife : MonoBehaviour
{

    public float health;


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

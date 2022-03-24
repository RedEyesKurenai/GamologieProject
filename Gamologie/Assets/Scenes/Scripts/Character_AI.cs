using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character_AI : LeadManagement
{
    public NavMeshAgent agent;

    public LeadManagement lead;

    public List<GameObject> Allies;

    private GameObject target;
    private GameObject nearestAlly;
    private GameObject tower;
    public LayerMask whatIsGround;
    public Animator animator;


    public float health;
    public float radiusChase;
    public float radiusAttack;
    private float Distance;

    //Patroling
    private Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange = 0f;

    //Attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;

    //States
    private bool playerInSightRange, playerInAttackRange;
    private bool towerInAttackRange;

    private void Start()
    {
        this.lead = transform.parent.gameObject.GetComponent<LeadManagement>();
    }

    private void Move(Vector3 position)
    {
        animator.SetBool("isAttacking", false);
        animator.SetBool("isRunning", true);
        transform.LookAt(position);
        agent.SetDestination(position);
    }

    private GameObject GetNearestEnemy ()
    {
        
        List<GameObject> enemies = this.lead.GetEnnemies();

        GameObject temp = null;
        float distMin = Mathf.Infinity;
        foreach (GameObject any in enemies)
        {
            float d = Vector3.Distance(transform.position, any.transform.position);
            
            if (d < distMin)
            {
                distMin = d;
                temp = any;
            }
        }

        return temp;
    }

    private GameObject GetNearestAlly()
    {
        Allies = lead.GetComponent<LeadManagement>().GetAllies();

        GameObject temp = null;
        float distMin = Mathf.Infinity;
        foreach (GameObject any in Allies)
        {
            if (any != transform.gameObject)
            {
                float d = Vector3.Distance(transform.position, any.transform.position);

                if (d < distMin)
                {
                    distMin = d;
                    temp = any;
                }
            }
        }

        return temp;
    }

    void Update()
    {

        nearestAlly = GetNearestAlly();
        target = GetNearestEnemy();

        if (target != null)
        {

            Distance = Vector3.Distance(transform.position, target.transform.position);
            
            if ((agent.CompareTag("Team1") && target.CompareTag("Team2")) || (agent.CompareTag("Team2") && target.CompareTag("Team1")))
            {
                if (Distance < radiusChase)
                {
                    playerInSightRange = true;
                    CallAlly();
                }
                else playerInSightRange = false;

                if (Distance < radiusAttack)
                {
                    playerInAttackRange = true;
                }
                else playerInAttackRange = false;

            }
        }
        

        
        //Check for sight and attack range
        
        if (!playerInSightRange && !playerInAttackRange) GoToTower();

        if (playerInSightRange && !playerInAttackRange) Chase();

        if (playerInAttackRange && playerInSightRange) AttackEnemy();
    }

    /*
    private int FilterAllies()
    {
        int indice = 0;

        for(int i=0; i < Allies.Capacity; i++)
        {
            if(nearestAlly == Allies[i])
            {
                indice = i;
            }
        }

        return indice;
    }*/

    private void CallAlly()
    {
        
        if ( nearestAlly != null && target != null)
        {
            Character_AI script = nearestAlly.gameObject.GetComponent<Character_AI>();
            script.Move(target.transform.position);
        }
        else { GoToTower();  }
    }


    private void GoToTower()
    {
         animator.SetBool("isAttacking", false);

        if(agent.CompareTag("Team1"))
        {
            tower = GameObject.FindGameObjectWithTag("Tower2");
        }

        if (agent.CompareTag("Team2"))
        {
            tower = GameObject.FindGameObjectWithTag("Tower1");
        }

        if (tower != null)
        {
            Move(tower.transform.position);

            float DistanceT = Vector3.Distance(transform.position, tower.transform.position);
            if (DistanceT < radiusAttack)
            {
                AttackTower(tower);
            }
        }


    }


    private void AttackTower( GameObject tower)
    {
        //Make sure enemy doesn't move
        animator.SetBool("isRunning", false);
        agent.ResetPath();

        if (tower != null)
        {

            transform.LookAt(tower.transform.position);

            if (!alreadyAttacked)
            {
                ///Attack code here

                animator.SetBool("isAttacking", true);


                TowerLife scriptTower = tower.GetComponent<TowerLife>();
                scriptTower.TakeDamage(1);

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
    }
    

    private void Chase()
    {
        if (target != null)
        {
            Move(target.transform.position);
        }

    }


    private void AttackEnemy()
    {
        //Make sure enemy doesn't move
        animator.SetBool("isRunning", false);
        agent.ResetPath();

        if (target != null)
        {

            transform.LookAt(target.transform.position);

            if (!alreadyAttacked)
            {
                ///Attack code here

                animator.SetBool("isAttacking", true);


                Character_AI script_player = target.GetComponent<Character_AI>();
                script_player.TakeDamage(1);

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
        else
        {
            GoToTower();
        }
    }


    private void ResetAttack()
    {
        alreadyAttacked = false;
        animator.SetBool("isAttacking", false);
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

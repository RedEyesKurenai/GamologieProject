using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character_AI : MonoBehaviour
{
    public NavMeshAgent agent;

    private List<GameObject> Allies;

    private GameObject target;
    private GameObject companion;
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
    

    public void getEnnemies ()
    {
        List<GameObject> Enemies = new List<GameObject>();

        Transform pere = this.gameObject.transform.parent;
        Transform grand_pere = pere.parent;
        foreach (Transform sibling in grand_pere)
        {
            if (sibling.gameObject != pere.gameObject)
            {
                
                foreach (Transform fils in sibling)
                {
                    Enemies.Add(fils.gameObject);
                }
            }
        }

        foreach (GameObject temp in Enemies)
        {
            target = temp;
        }
    }

    public void getAllies()
    {
        Allies = new List<GameObject>();

        Transform pere = this.gameObject.transform.parent;
        foreach (Transform sibling in pere)
        {
            Allies.Add(sibling.gameObject);
        }
        /*
        foreach (GameObject alie in Allies)
        {
            companion = alie;
        }
        */
    }

    void Update()
    {
        getEnnemies();
        getAllies();

        if (target != null)
        {

            Distance = Vector3.Distance(transform.position, target.transform.position);


            if ((agent.CompareTag("Team1") && target.CompareTag("Team2")) || (agent.CompareTag("Team2") && target.CompareTag("Team1")))
            {
                if (Distance < radiusChase)
                {
                    playerInSightRange = true;

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
        
        if (!playerInSightRange && !playerInAttackRange) Patroling();

        if (playerInSightRange && !playerInAttackRange) Chase();

        if (playerInAttackRange && playerInSightRange) Attack();
    }

    private void Patroling()
    {
        // animator.SetBool("isAttacking", false);

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            animator.SetBool("isAttacking", false);
            animator.SetBool("isRunning", true);
            transform.LookAt(walkPoint);
            agent.SetDestination(walkPoint);


        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void Chase()
    {
        if (target != null)
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("isRunning", true);
            transform.LookAt(target.transform.position);
            agent.SetDestination(target.transform.position);
        }

    }


    private void Attack()
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

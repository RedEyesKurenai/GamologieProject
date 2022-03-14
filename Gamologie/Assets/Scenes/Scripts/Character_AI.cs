using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_AI : MonoBehaviour
{
    [SerializeField] GameObject waypoint;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] LayerMask player;
    [SerializeField] float Speed = 4f;
    Animator animator;
    

    public float health;
    public float radiusChase;
    public float radiusAttack;

    //private bool isMoving ;

    //Patroling
    private Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange = 0f;

    //Attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;
    public GameObject projectile;

    //States
    private bool playerInSightRange, playerInAttackRange;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, radiusChase, player);
        playerInAttackRange = Physics.CheckSphere(transform.position, radiusAttack, player);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
 
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
       // animator.SetBool("isAttacking", false);

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            animator.SetBool("isRunning", true);
            transform.LookAt(walkPoint);
            transform.position = Vector3.MoveTowards(transform.position, walkPoint, Speed * Time.deltaTime);
        
            
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

    private void ChasePlayer()
    {
        //animator.SetBool("isAttacking", false);
        animator.SetBool("isRunning", true);
        transform.LookAt(waypoint.transform.position);
        transform.position = Vector3.MoveTowards(transform.position, waypoint.transform.position, Speed * Time.deltaTime);
    }


    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        animator.SetBool("isRunning", false);
        transform.position = transform.position;


        transform.LookAt(waypoint.transform);

        if (!alreadyAttacked)
        {
            ///Attack code here

            animator.SetBool("isAttacking", true);
            /*Projectile
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            */
            ///End of attack code
            ///

            Character_AI script_player = waypoint.GetComponent<Character_AI>();
            script_player.TakeDamage(1);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
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

        // if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    /*
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    */

}

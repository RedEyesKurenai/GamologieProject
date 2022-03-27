using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Character_AI : LeadManagement
{
    //[SerializeField] GameObject CharaAIGameObjet;

    public LeadManagement lead;
    
    public int id = 0;
    
    public NavMeshAgent agent;

    public DataChara datachara;
    public string nom;
    public int damage;

    public List<Message> localLetterBox;

    public bool isLeader;

    protected GameObject target;
    private GameObject companion;
    public LayerMask whatIsGround;
    public Animator animator;


    public float health;
    public float maxHealth;
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

    //Items collector
    int coinsTeam1 = 0;
    int coinsTeam2 = 0;

    public Text coinsTextTeam1 ;
    public Text coinsTextTeam2 ;







    public void importData()
    {
        nom = datachara.charaName;
        maxHealth = datachara.maxHealth;
        damage = datachara.damage;
        timeBetweenAttacks = datachara.timeBetweenAttack;
        agent.speed = datachara.speed;
        health = maxHealth;
    }

    public bool leaderOrNot() { return isLeader; }

    public void makeLeaderOrNot(bool leader) { this.isLeader = leader; }

    public void setId(int id) { this.id = id; }

    public GameObject GetEnemy()
    {
        List<GameObject> enemies = getEnnemies();
        
        GameObject temp = null;
        float distMin = Mathf.Infinity;
        foreach (GameObject o in enemies)
        {
            float d = Vector3.Distance(transform.position, o.transform.position);
            if(d < distMin)
            {
                distMin = d;
                temp = o;
            }
        }
        return temp;
    }
    public void sendMessage(int id, int dest, int subject, string content)
    {
        //Message message = new Message(id, dest, false, subject, content);
        Message message = ScriptableObject.CreateInstance<Message>();
        message.sourceID = id;
        message.targetID = dest;
        message.isSupported = false;
        message.subject = subject;
        message.content = content;
        lead.globalLetterBox.Add(message);
    }

    public List<Message> receiveMessage()
    {
        List<Message> boitAuLettre = new List<Message>();
        foreach(Message m in globalLetterBox)
        {
            if ((m.targetID == this.id) || (m.targetID == 0))
                boitAuLettre.Add(m);
        }
        return boitAuLettre;
    }

    void Start()
    {

        if (datachara != null)
            importData();
        
        localLetterBox = new List<Message>();
        /*
        if (isLeader)
        {
            sendMessage(this.id, 0, (int)Content.BEGIN, "start");
            sendMessage(this.id, 2, (int)Content.TEST, "tst");
        }
        */
        if(this.id == 2)
            sendMessage(this.id, 3, (int)Content.BEGIN, "start");
        if (this.id == 2)
            sendMessage(this.id, 1, (int)Content.TEST, "test");
        localLetterBox = receiveMessage();

    }

    void Update()
    {
        Allies = getAllies();
        Enemies = getEnnemies();
        globalLetterBox = lead.globalLetterBox;
        localLetterBox = receiveMessage();
        //target = Enemies[id - 1];
        target = GetEnemy();

        if (target != null)
        {

            Distance = Vector3.Distance(transform.position, target.transform.position);


            if ((agent.CompareTag("Team1") && target.CompareTag("Team2")) || (agent.CompareTag("Team2") && target.CompareTag("Team1")))
            {
                playerInSightRange = true;

                if (Distance < radiusAttack)
                {
                    playerInAttackRange = true;

                }
                else playerInAttackRange = false;

            }
        }

        //Check for sight and attack range
        
        // if (!playerInSightRange && !playerInAttackRange) Patroling();

        if (playerInSightRange && !playerInAttackRange) Chase();

        if (playerInAttackRange && playerInSightRange) Attack();
    }


    //Items Coins touched
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            if (agent.CompareTag("Team1"))
            {
                coinsTeam1++;
                Debug.Log("+1 coin for Team 1");
                coinsTextTeam1.text = "Team 1 Coins :" + coinsTeam1;
            }
            if (agent.CompareTag("Team2"))
            {
                coinsTeam2++;
                Debug.Log("+1 coin for Team 2");
                coinsTextTeam2.text = "Team 2 Coins :" + coinsTeam2;
            }

        }

         /*if (other.gameObject.CompareTag("Hlth"))
        {
            Destroy(other.gameObject);
            health = Mathf.Min(health + 100, 1000);
            Debug.Log("+100 health");
            healthText.text = "Health :" + health;
        }

        if (other.gameObject.CompareTag("Poison"))
        {
            Destroy(other.gameObject);
            health = Mathf.Max(health - 100, 0);
            Debug.Log("-100 health");
            healthText.text = "Health :" + health;
            if (health <= 0)
            {
                //ReloadLevel();
            }
        }*/

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
                script_player.TakeDamage(this.damage);

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

        if (health <= 0) { 
            Invoke(nameof(DestroyEnemy), 0.5f); 
        }
    }

    
    private void DestroyEnemy()
    {
        Destroy(gameObject);
        
    }
    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private GameObject towerEnemy;
    private GameObject towerAlly;
    private GameObject respawnPoint;
    private float initial_health;


    public float health;
    public float maxHealth;
    public float radiusChase;
    public float radiusAttack;
    private float Distance;


    //Attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;

    //States
    private bool playerInSightRange, playerInAttackRange;

    

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
        List<GameObject> enemies = this.lead.getEnnemies();
        
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

    private void Move(Vector3 position)
    {
        animator.SetBool("isAttacking", false);
        animator.SetBool("isRunning", true);
        transform.LookAt(position);
        agent.SetDestination(position);
    }

    void Start()
    {
        this.lead = transform.parent.gameObject.GetComponent<LeadManagement>();
        initial_health = health;

        if (agent.CompareTag("Team1"))
        {
            towerEnemy = GameObject.FindGameObjectWithTag("Tower2");
            towerAlly = GameObject.FindGameObjectWithTag("Tower1");
            respawnPoint = GameObject.Find("RespawnPoint1");
        }

        if (agent.CompareTag("Team2"))
        {
            towerEnemy = GameObject.FindGameObjectWithTag("Tower1");
            towerAlly = GameObject.FindGameObjectWithTag("Tower2");
            respawnPoint = GameObject.Find("RespawnPoint2");
        }

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
        Allies = this.lead.getAllies();
        Enemies = this.lead.getEnnemies();
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
        
        if (!playerInSightRange && !playerInAttackRange) GoToTower();

        if (playerInSightRange && !playerInAttackRange) Chase();

        if (playerInAttackRange && playerInSightRange) Attack();
    }

    private void GoToTower()
    {
        animator.SetBool("isAttacking", false);



        if (towerEnemy != null)
        {
            Move(towerEnemy.transform.position);

            float DistanceT = Vector3.Distance(transform.position, towerEnemy.transform.position);
            if (DistanceT < radiusAttack)
            {
                AttackTower(towerEnemy);
            }
        }


    }


    private void AttackTower(GameObject tower)
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

        if (health <= 0) { 
            Invoke(nameof(DestroyEnemy), 0.1f); 
        }
    }

    private void Respawn()
    {

        agent.transform.position = respawnPoint.transform.position;
        health = initial_health;

    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
        
    }
    

}

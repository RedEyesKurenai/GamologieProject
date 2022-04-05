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
    public bool isListeningToOrder = false;


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

    //HealthBar
    public HealthBar healthbar;


    //Attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;

    //States
    private bool playerInSightRange, playerInAttackRange;

    //Items collector
    static int coinsTeam1 ;
    static int coinsTeam2 ;

    //Avoid Zone
    List <Vector3> Avoid_zone;

    //[SerializeField]
    [SerializeField]  Text coinsTextTeam1 ;
    [SerializeField]  Text coinsTextTeam2 ;





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

    public void sendMessage(int id, int dest, int subject, string content, Vector3 position, GameObject zone)
    {
        //Message message = new Message(id, dest, false, subject, content);
        Message message = ScriptableObject.CreateInstance<Message>();
        message.sourceID = id;
        message.targetID = dest;
        message.isSupported = false;
        message.subject = subject;
        message.content = content;
        message.position = position;
        message.zone = zone;
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

    public void supportMessage()
    {
        foreach (Message m in localLetterBox)
        {
            if (!m.isSupported)
            {
                isListeningToOrder = true;
                switch (m.subject)
                {
                    case (int)Subject.BEGIN:
                        {
                            m.isSupported = true;
                        }
                        break;
                    case (int)Subject.TEST:
                        {
                            m.isSupported = true;
                        }
                        break;
                    case (int)Subject.GO_TO_TOWER:
                        {
                            GoToTower();
                            if (towerEnemy.GetComponent<TowerLife>().health <= 0.0f)
                                m.isSupported = true;
                        }
                        break;
                    default:
                        isListeningToOrder = false;
                        break;
                }
                isListeningToOrder = false;
            }
        }
    }

    void Start()
    {

        //Item initial
        coinsTeam1 = 0;
        coinsTeam2 = 0;

        //Avoid Zone initial
        Avoid_zone = new List<Vector3>();

        this.lead = transform.parent.gameObject.GetComponent<LeadManagement>();
        initial_health = health;

        //HealthBar
        healthbar.SetMaxHealth( (int)maxHealth );

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
        if(this.id == 2)
            sendMessage(this.id, 3, (int)Subject.BEGIN, "start", Vector3.zero, null);
        if (this.id == 2)
            sendMessage(this.id, 1, (int)Subject.TEST, "test", Vector3.zero, null );

        /*
        if (this.isLeader)
        {
            sendMessage(this.id, 0, (int)Content.BEGIN, "start");
            sendMessage(this.id, 2, (int)Content.TEST, "tst");
        }
        */
        /*
        if (this.id == 1)
            sendMessage(this.id, 0, (int)Content.BEGIN, "start");
        if (this.id == 2)
            sendMessage(this.id, 0, (int)Content.BEGIN, "start");
        if (this.id == 3)
            sendMessage(this.id, 0, (int)Content.BEGIN, "start");
        if (this.id == 2)
            sendMessage(this.id, 0, (int)Content.BEGIN, "start");
        if (this.id == 1)
            sendMessage(this.id, 0, (int)Content.BEGIN, "start");
        */
        //if(this.id == 1)
        //  sendMessage(this.id, 2, (int)Content.SPLIT_TO_TOWER, "nothing more");

        localLetterBox = receiveMessage();
        if (localLetterBox != null)
            supportMessage();

    }

    void Update()
    {

        //Health Bar
        healthbar.SetHealth((int)health);

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

        if (!isListeningToOrder)
        {
            target = GetEnemy();

            if (!playerInSightRange && !playerInAttackRange) GoToTower();

            if (playerInSightRange && !playerInAttackRange) Chase();

            if (playerInAttackRange && playerInSightRange) Attack();
        }
        else
        {

        }

        //Check if in Avoid zone
        /*
         * if (Avoid_zone.Count != 0)
        {
            foreach (Vector3 z in Avoid_zone)
            {
                if (Vector3.Distance(this.transform.position, z) < 0.8)
                {
                    Vector3 diff = z - this.transform.position; //Arrivé - départ : pour avoir le vecteur direct 
                    Vector3 rotatedVector;
                    rotatedVector = Quaternion.AngleAxis(90, Vector3.left) * diff;
                    Move(rotatedVector);//tourner de 90 degré pour eviter l'objet et continuer les chemin
                    
                }
            }
        }
        */

        //Analysis of the messages

        foreach (Message m in localLetterBox)
        {
            if (!m.isSupported)
            {
                if (m.subject == (int)Subject.HEALTH_MOVE_AT)
                {
                    if (health < maxHealth)
                    {
                        int pills_health_number = m.zone.transform.childCount;
                        if (pills_health_number > 0)
                        {
                            if (Vector3.Distance(transform.position, m.zone.transform.GetChild(pills_health_number - 1).position) > 1)
                            {
                                Move(m.zone.transform.GetChild(pills_health_number - 1).position);
                               
                            }

                            else
                            {
                                m.isSupported = true;
                            }
                        }
                        else
                        {
                            m.isSupported = true;
                        }

                    }                    
                }

                if (m.subject == (int)Subject.POISON_AVOID_THIS)
                {
                    Avoid_zone.Add(m.position);
                    m.isSupported = true;
                }
            }
        }
    }


    //Items Coins touched
    private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Coin"))
            {
                Destroy(other.gameObject);
                if (agent.CompareTag("Team1"))
                {
                    coinsTeam1 = coinsTeam1 + 100;
                    Debug.Log("+1 coin for Team 1");
                    coinsTextTeam1.text = "Team 1 Coins :" + coinsTeam1;
                    Debug.Log(coinsTeam1 + "Team 1");
                }
                if (agent.CompareTag("Team2"))
                {
                    coinsTeam2 = coinsTeam2 + 100;
                    Debug.Log("+1 coin for Team 2");
                    coinsTextTeam2.text = "Team 2 Coins :" + coinsTeam2;
                    Debug.Log(coinsTeam2 + "Team 1");
                }

            }
        

        int health_ecart =(int) (health * 0.5) ; //50% de la sante

        if (other.gameObject.CompareTag("Hlth"))
       {
           Destroy(other.gameObject);
            health =   Mathf.Min(health + health_ecart, maxHealth); //rajouter 50% de vie
           Debug.Log("+50% health");
            //Envoyer un message à toutes l'équipe que pillule bonus ici et venir si peu de vie 
            Vector3 health_position= other.transform.position;
            //envoie à tous le monde de l'équipe 0
            sendMessage(this.id, 0, (int)Subject.HEALTH_MOVE_AT,"I Found health", health_position, other.transform.parent.gameObject);
            
        }

       if (other.gameObject.CompareTag("Poison"))
       {
           Destroy(other.gameObject);
           this.TakeDamage(health_ecart);
           Debug.Log("-50% health");

            //Envoyer un message à toutes l'équipe que pilule mallus ici et éviter à tout prix cette zone
            Vector3 poison_position = other.transform.position;
            //envoie à tous le monde de l'équipe 0
            sendMessage(this.id, 0, (int)Subject.POISON_AVOID_THIS, "I Found poison", poison_position, other.transform.parent.gameObject);
           


        }

    }

    private void GoToTower()
    {
        animator.SetBool("isAttacking", false);



        if (towerEnemy != null)
        {
            Move(towerEnemy.transform.position);
            Debug.Log("ICIIIIIXXXX");

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


    //retourne une liste des alliés rangés du plus proche au plus loin de mainUnit, fais avec algo tri bulle
    /*public List<GameObject> GetAlliesOrderedByDistance(GameObject mainUnit)
    {
        List<GameObject> tempUnits = members;

        for (int i = tempUnits.Count - 1; i > 1; i--)
        {
            for (int j = 0; j < i; j++)
            {
                float d1 = Vector3.Distance(mainUnit.transform.position, tempUnits[j].transform.position) - tempUnits[j].GetComponent<LeaderNPC>().attackRange;
                float d2 = Vector3.Distance(mainUnit.transform.position, tempUnits[j + 1].transform.position) - tempUnits[j + 1].GetComponent<LeaderNPC>().attackRange;
                if (d1 > d2)
                {
                    GameObject tmp = tempUnits[j];
                    tempUnits[j] = tempUnits[j + 1];
                    tempUnits[j + 1] = tmp;
                }
            }
        }
        return tempUnits;
    }*/


}

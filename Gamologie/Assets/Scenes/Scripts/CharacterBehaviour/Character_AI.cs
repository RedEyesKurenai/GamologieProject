using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Character_AI : LeadManagement
{
    

    private LeadManagement lead;
    
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


    //Stats du personnage
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
    List<Vector3> Avoid_zone;

    //reference à l'affichage textuel des pièces des équipes
    [SerializeField]  Text coinsTextTeam1 ;
    [SerializeField]  Text coinsTextTeam2 ;


    public Group grp;


    /*importe les données du personnages*/
    public void importData()
    {
        nom = datachara.charaName;
        maxHealth = datachara.maxHealth;
        damage = datachara.damage;
        timeBetweenAttacks = datachara.timeBetweenAttack;
        agent.speed = datachara.speed;
        health = maxHealth;
    }

    /*test si le personnage est un leader ou non*/
    public bool leaderOrNot() { return isLeader; }

    /*  */
    public void makeLeaderOrNot(bool leader)
    {
        if (!this.grp.HasGroup())
        {
            this.isLeader = leader;
        }
    }

    /* modifie l'id du personnage */
    public void setId(int id) { this.id = id; }

    /* récupère un allié grâce à son id */
    public GameObject getAlly(int identity)
    {
        foreach (GameObject obj in Allies)
        {
            Character_AI script = obj.GetComponent<Character_AI>();
            if (script.id == identity)
            {
                return obj;
            }
        }

        return null;

    }

    /*Récupère un ennemi proche*/
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

    /**
     * Fonction permettant d'envoyer des messages dans la boîte aux lettres globale
     */
    public void SendMessage(int id, int dest, int subject, string content, Vector3 position, GameObject zone)
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

    /**
     * Fonction permettant de recevoir les messages concernant le personnage
     */
    public List<Message> ReceiveMessage()
    {
        List<Message> boitAuLettre = new List<Message>();
        foreach(Message m in globalLetterBox)
        {
            if ((m.targetID == this.id) || (m.targetID == 0))
                boitAuLettre.Add(m);
        }
        return boitAuLettre;
    }

    /*Fonction permettant de deplacer le personnage vers une position en animant ses mouvements */
    private void Move(Vector3 position)
    {
        animator.SetBool("isAttacking", false);
        animator.SetBool("isRunning", true);
        transform.LookAt(position);
        agent.SetDestination(position);
    }

    void Start()
    {
        
        this.grp = new Group(this.gameObject);
        
        //Item initial
        coinsTeam1 = 0;
        coinsTeam2 = 0;

        //Avoid Zone initial
        Avoid_zone = new List<Vector3>();

        this.lead = transform.parent.gameObject.GetComponent<LeadManagement>();
        initial_health = health;

        //HealthBar
        healthbar.SetMaxHealth( (int)maxHealth );

        //repérer la tour ennemi
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

        //importer les données du personnages
        if (datachara != null)
            importData();
        
        //Initialiser la boitre au lettre locale
        localLetterBox = new List<Message>();  
        localLetterBox = ReceiveMessage();

    }

    void Update()
    {

        //Health Bar
        healthbar.SetHealth((int) health);

        Allies = this.lead.getAllies();
        Enemies = this.lead.getEnnemies();
        globalLetterBox = lead.globalLetterBox;
        localLetterBox = ReceiveMessage();
        
        target = GetEnemy();
        CreateGroup();
        FollowTeamTarget();
        BuyHealthTeam();

        
        /*
         Gestion de l'attaque du personnage
         */
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
        else { playerInAttackRange = true; }


        /*
         * Check for sight and attack range
         * Definit les actions du personnages et ses priorités
         */
        
        if (!playerInSightRange && !playerInAttackRange) GoToTower();

        if (playerInSightRange && !playerInAttackRange) Chase();

        if (playerInAttackRange && playerInSightRange) Attack();

        //Gestion de contournement des pillules rouges non fonctionnelle
        /*if (Avoid_zone.Count != 0)
        {

            foreach (Vector3 z in Avoid_zone)
            {
                Debug.Log("distance" + Vector3.Distance(this.transform.position, z) + "XXXX");
                if (Vector3.Distance(this.transform.position, z) < 5)
                {

                    Vector3 diff = z - this.transform.position; //Arrivé - départ : pour avoir le vecteur direct 
                    Vector3 rotatedVector;
                    rotatedVector = Quaternion.AngleAxis(90, Vector3.left) * diff;
                    Debug.Log("rotate !!! ");
                    Move(rotatedVector);//tourner de 90 degré pour eviter l'objet et continuer les chemin


                     }
            }

            
        }*/

        GoToHealth();

        //askForHelp();
        //giveHelp();
    }


    /*
     Gestion de la priorité de la santé: 
    Se dirige vers une pilule verte connue via la messagerie si le personnage possède peu de vie. 
    Rajoute la position d'une pilule rouge connue via la messagerie dans l'avoid zone
     */
    private void GoToHealth()
    {
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
                                Debug.Log("Go to Health ");

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
    
    /*
     Fonction qui demande de l'aide aux alliés via la messagerie 
     */
    void AskForHelp()
    {
        if (target != null)
        {

            if (this.playerInSightRange )
            {
                foreach (GameObject obj in Allies)
                {
                    if (this.gameObject != obj)
                    {
                        Character_AI script = obj.GetComponent<Character_AI>();
                        SendMessage(this.id, script.id, (int)Subject.HELP, "Help", target.transform.position, target);
                    }
                }
            }
        }
        else GoToTower();

    }

    /*
     Fonction qui fait deplacer le personnage vers la l'allié qui a demandé de l'aide via la messagerie 
     */
    void GiveHelp()
    {
        if (localLetterBox != null )
        {
            foreach (Message mes in localLetterBox)
            {
                if (mes.subject == 3 && mes.content == "Help")
                {

                    if (mes.zone != null)
                    {
                        Move(mes.position);
                    }
                    else GoToTower();
                }
            }
        }
        else GoToTower();
    }

    /*
     * Fonction qui rajoute un membre dans le groupe 
     */
    void AddMember(GameObject any)
    {
        if (any != null)
        {
            Character_AI script = any.GetComponent<Character_AI>();
            if (!script.grp.HasGroup())
            {
                this.grp.AddToGroup(any);
                script.grp = this.grp;
            }
        }
    }

    /*
     Fonction qui enlève un membre du groupe 
     */
    void RemoveMember(GameObject any)
    {
        if (any != null)
        {
            Character_AI script = any.GetComponent<Character_AI>();
            if (script.grp.HasGroup() && script.grp == this.grp)
            {
                this.grp.RemoveFromGroup(any);
                script.grp = null;
            }
        }
        
    }

    /*
     Fonction qui crée un groupe
     */
    void CreateGroup()
    {
        
        if (this.isLeader)
        {
            
            foreach(GameObject any in Allies)
            {
                float d = Vector3.Distance(transform.position, any.transform.position);
                if (d < 10)
                {
                    AddMember(any);
                }
                
            }
        }
    }

    /*
     Fonction permettant à un groupe de suivre une même cible 
     */
    void FollowTeamTarget()
    {
        if(this.grp.HasGroup() && this.isLeader)
        {
            foreach(GameObject obj in this.grp.GetMembres())
            {
                Character_AI script = obj.GetComponent<Character_AI>();
                if (target != null)
                {
                    script.target = this.target;
                }
                script.Move(this.transform.position);
            }
        }
    }

    
    /*
     *Fonction permettant de gerer le comportement lorsque le personnage touche un item:
     *il envoie un message si c'est une pillule avec le sujet adapté.
     *On augmente le nombre de pièce de l'équipe si c'est une pièce
     */
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


        int health_ecart = (int)(health * 0.5); //50% de la sante

        if (other.gameObject.CompareTag("Hlth"))
        {
            Destroy(other.gameObject);
            health = Mathf.Min(health + health_ecart, maxHealth); //rajouter 50% de vie
            Debug.Log("+50% health");
            //Envoyer un message à toutes l'équipe que pillule bonus ici et venir si peu de vie 
            Vector3 health_position = other.transform.position;
            //envoie à tous le monde de l'?quipe 0
            SendMessage(this.id, 0, (int)Subject.HEALTH_MOVE_AT, "I Found health", health_position, other.transform.parent.gameObject);

        }

        if (other.gameObject.CompareTag("Poison"))
        {
            Destroy(other.gameObject);
            this.TakeDamage(health_ecart);
            Debug.Log("-50% health");

            //Envoyer un message à toutes l'équipe que pilule mallus ici et éviter é tout prix cette zone
            Vector3 poison_position = other.transform.position;
            //envoie à tous le monde de l'équipe 0
            SendMessage(this.id, 0, (int)Subject.POISON_AVOID_THIS, "I Found poison", poison_position, other.transform.parent.gameObject);



        }

    }

    
    /*
     Fonction permettant le regain de santé de toute l'équipe s'il atteint un certain nombre de pièce
     */
    private void BuyHealthTeam()
    {
        if( agent.CompareTag("Team1") && coinsTeam1 >=500)
        {
            foreach(GameObject obj in Allies )
            {
                if(obj != null)
                obj.GetComponent<Character_AI>().health = initial_health;
            }
            coinsTeam1 = 0;
        }

        if (agent.CompareTag("Team2") && coinsTeam2 >= 500)
        {
            foreach (GameObject obj in Allies)
            {
                if (obj != null)
                obj.GetComponent<Character_AI>().health = initial_health;
            }
            coinsTeam2 = 0;
        }


    }


    /*Fonction permettant au personnage de se dirriger vers la tour */
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


    /* Fonction permettant au personnage d'attaquer la tour */
    private void AttackTower(GameObject tower)
    {
        animator.SetBool("isRunning", false);
        agent.ResetPath();

        if (tower != null)
        {

            transform.LookAt(tower.transform.position);

            if (!alreadyAttacked)
            {
                

                animator.SetBool("isAttacking", true);


                TowerLife scriptTower = tower.GetComponent<TowerLife>();
                scriptTower.TakeDamage(1);

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
    }

    /*Fonction permettant de suivre une cible */
    private void Chase()
    {
        if (target != null)
        {
            Move(target.transform.position);
        }

    }


    /* Fonction permettant au personnage d'attaquer */
    private void Attack()
    {    
        animator.SetBool("isRunning", false);
        agent.ResetPath();

        if (target != null)
        {

            transform.LookAt(target.transform.position);

            if (!alreadyAttacked)
            {
                

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

    /*Fonction permettant au personnage d' annuler une attaque */
    private void ResetAttack()
    {
        alreadyAttacked = false;
        animator.SetBool("isAttacking", false);
    }

    /*Fonction permettant au personnage de perdre de la vie */
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) { 
            Invoke(nameof(DestroyEnemy), 0.5f); 
        }
    }

    private void Respawn()
    {

        agent.transform.position = respawnPoint.transform.position;
        health = initial_health;

    }

    /*Fonction permettant au personnage de mourir*/

    private void DestroyEnemy()
    {
        Destroy(gameObject);
        
    }
    

}

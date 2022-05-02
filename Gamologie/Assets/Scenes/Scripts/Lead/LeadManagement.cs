using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LeadManagement : MonoBehaviour
{
    private int nb = 0;

    //[SerializeField] GameObject ceGameObjet;

    public List<Message> globalLetterBox;

    public List<GameObject> Leaders;
    public List<GameObject> Allies;
    public List<GameObject> Enemies;
    


    /*Enumération des sujets de message possible*/
    public enum Subject
    {
        BEGIN = 0,
        TEST = 1,
        IS_DEAD = 2,
        HELP = 3,
        HEALTH_MOVE_AT = 4,
        POISON_AVOID_THIS = 5
    }

    /*
     Fonction qui récupère la liste des ennemies
     */
    public List<GameObject> getEnnemies()
    {
        List<GameObject> enemies = new List<GameObject>();
        Transform pere = this.gameObject.transform.parent;
        foreach (Transform sibling in pere)
        {
            if (sibling.gameObject != this.gameObject)
            {
                foreach (Transform fils in sibling)
                {
                    enemies.Add(fils.gameObject);
                    
                }
            }
        }
        return enemies;
        
    }

    /*
     Fonction qui récupère la liste des alliés
     */
    public List<GameObject> getAllies()
    {
        List<GameObject> allies = new List<GameObject>();
        foreach (Transform sibling in this.gameObject.transform)
        {
            allies.Add(sibling.gameObject);
        }
        return allies;
    }

    /*
     Fonction qui récupère la liste des leaders
     */
    public List<GameObject> getLeaders()
    {
        List<GameObject> leaders = new List<GameObject>();
        if (Allies != null)
        {
            foreach (GameObject any in Allies)
            {
                if (any != null)
                {
                    Character_AI script = any.GetComponent<Character_AI>();
                    if (script.isLeader)
                    {
                        leaders.Add(any);
                    }
                }
            }
        }
        return leaders;
    }

    /*
     Fonction qui répartie les Id aux personnage
     */
    public void giveId()
    {
        foreach (Transform sibling in this.gameObject.transform)
        {
            nb++;
            sibling.gameObject.GetComponent<Character_AI>().setId(nb);
        }
    }

    public List<GameObject> getAlliesArray() { return Allies; }

    public List<GameObject> getEnemyArray() { return Enemies; }

    
    /*
     Fonction qui définit un leader
     */
    public void defineLeader(GameObject any)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.parent = any.transform;
        sphere.transform.localPosition = new Vector3(0, 2, 0);
        sphere.transform.localScale = new Vector3(0.25083f, 0.25083f, 0.25083f);
        sphere.transform.localRotation = new Quaternion(0, 0, 0, 0);
    }


    /*
     * Procédure qui gère l'affectation des leaders
     */
    public void setLeader()
    {
        
        if (Allies != null)
        {
            if (Allies[0] != null)
            {
                Allies[0].GetComponent<Character_AI>().makeLeaderOrNot(true);
                defineLeader(Allies[0]);
                //Debug.Log(Allies[0].GetComponent<Character_AI>().grp.HasGroup());
            }

            Debug.Log(leadersHaveGroup());

            foreach(GameObject any in Allies)
            {
                if (any != null)
                {
                    Character_AI script = any.GetComponent<Character_AI>();
                    if (leadersHaveGroup() && !script.grp.HasGroup() && !script.isLeader)
                    {
                        script.makeLeaderOrNot(true);
                        defineLeader(any);
                        break;

                    }
                    else break;
                    
                    
                }
            }
        }
    }


    /*
     * Fonction booléenne qui vérifie que tous les leaders ont un groupe
     * */
    public bool leadersHaveGroup()
    {
        if(Leaders == null)
        {
            return false;
        }
        
        foreach (GameObject any in Leaders)
         {
                Character_AI script = any.GetComponent<Character_AI>();
            
                if (!script.grp.HasGroup())
                {
                    return false;
                }
         }

       return true;
        
        
    }

    /*
     Fonction qui met à jour la boite au lettre
     */
    public List<Message> globalLetterBoxReloading() {

        List<Message> boitAuLettre = new List<Message>();
        foreach(Message m in globalLetterBox)
        {
            boitAuLettre.Add(m);
        }
        return boitAuLettre; 
    }

    // Start is called before the first frame update
    void Start()
    {
        giveId();
        
        globalLetterBox = new List<Message>();
        Allies = getAllies();
        Leaders = getLeaders();
        Enemies = getEnnemies();
        globalLetterBox = globalLetterBoxReloading();
        setLeader();
    }

    // Update is called once per frame
    void Update()
    {
        globalLetterBox = globalLetterBoxReloading();
        Leaders = getLeaders();
        setLeader();
    }
}

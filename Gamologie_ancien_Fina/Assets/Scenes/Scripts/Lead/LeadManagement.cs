using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LeadManagement : MonoBehaviour
{
    private int nb = 0;

    //[SerializeField] GameObject ceGameObjet;

    public List<Message> globalLetterBox;

    public List<GameObject> Allies;
    public List<GameObject> Enemies;

    public GameObject leaderAlie;
    public GameObject leaderEnemi;

    public enum Subject
    {
        BEGIN = 0,
        TEST = 1,
        IS_DEAD = 2,
        HEALTH_MOVE_AT = 3,
        POISON_AVOID_THIS = 4
    }

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
        /*
        foreach (GameObject temp in Enemies)
        {
            target = temp;
        }
        */
    }

    public List<GameObject> getAllies()
    {
        List<GameObject> allies = new List<GameObject>();
        foreach (Transform sibling in this.gameObject.transform)
        {
            allies.Add(sibling.gameObject);
        }
        return allies;
    }

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

    public void defineLeader()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.parent = leaderAlie.transform;
        sphere.transform.localPosition = new Vector3(0, 2, 0);
        sphere.transform.localScale = new Vector3(0.25083f, 0.25083f, 0.25083f);
        sphere.transform.localRotation = new Quaternion(0, 0, 0, 0);
    }

    public void setLeader()
    {
        for(int i = 0; i < Allies.Count; i++)
        {
            if(Allies[i] != null)
            {
                leaderAlie = Allies[i];
                leaderAlie.gameObject.GetComponent<Character_AI>().makeLeaderOrNot(true);
                defineLeader();
                break;
            }
        }
    }

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
        Enemies = new List<GameObject>();
        Allies = new List<GameObject>();
        globalLetterBox = new List<Message>();
        Allies = getAllies();
        Enemies = getEnnemies();
        globalLetterBox = globalLetterBoxReloading();
        setLeader();
    }

    // Update is called once per frame
    void Update()
    {
        globalLetterBox = globalLetterBoxReloading();
        setLeader();
    }
}

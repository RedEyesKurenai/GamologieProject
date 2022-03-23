using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LeadManagement : MonoBehaviour
{
    private int nb = 0;

    [SerializeField] GameObject ceGameObjet;

    public List<GameObject> Allies;
    public List<GameObject> Enemies;

    public GameObject leaderAlie;
    public GameObject leaderEnemi;

    public GameObject leaderModel;

    private GameObject leaderAlieTrans;
    private GameObject leaderEnemiTrans;

    public void getEnnemies()
    {
        Transform pere = ceGameObjet.transform.parent;
        foreach (Transform sibling in pere)
        {
            if (sibling.gameObject != ceGameObjet)
            {
                foreach (Transform fils in sibling)
                {
                    Enemies.Add(fils.gameObject);
                    
                }
            }
        }
        /*
        foreach (GameObject temp in Enemies)
        {
            target = temp;
        }
        */
    }

    public void getAllies()
    {
        foreach (Transform sibling in ceGameObjet.transform)
        {
            Allies.Add(sibling.gameObject);
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
                defineLeader();
                break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Enemies = new List<GameObject>();
        Allies = new List<GameObject>();
        getAllies();
        getEnnemies();
        setLeader();
    }

    // Update is called once per frame
    void Update()
    {
        setLeader();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LeadManagement : MonoBehaviour
{
    //private int nb = 0;
    /*
    public GameObject leaderAlie;
    public GameObject leaderEnemi;

    public GameObject leaderModel;

    private GameObject leaderAlieTrans;
    private GameObject leaderEnemiTrans;*/


   
    public List<GameObject> GetEnnemies()
    {
        List<GameObject> enemies = new List<GameObject>();
        Transform pere = this.gameObject.transform.parent;

        
        foreach (Transform any in pere)
        {
            if (any.gameObject != this.gameObject)
            {
                
                foreach (Transform fils in any)
                {
                    enemies.Add(fils.gameObject);
                }
            }
        }

        return enemies;

    }

    public List<GameObject> GetAllies()
    {
        List<GameObject> Allies = new List<GameObject>();

        foreach (Transform sibling in this.gameObject.transform)
        {
            Allies.Add(sibling.gameObject);
            //nb++;
            //sibling.gameObject.GetComponent<Character_AI>().setId(nb);
        }

        return Allies;
    }

    

    /*
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
        for (int i = 0; i < Allies.Count; i++)
        {
            if (Allies[i] != null)
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

    */
}

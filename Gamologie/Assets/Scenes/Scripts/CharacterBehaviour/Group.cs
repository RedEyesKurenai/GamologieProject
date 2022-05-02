using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    private List<GameObject> membres;
    private bool groupe;  //si le groupe est remplie 

    /*
     constructeur de groupe
     */
    public Group()
    {
        membres = new List<GameObject>();
        groupe = false;
    }

    /*
     constructeur de groupe avec paramètre
     */
    public Group(GameObject obj)
    {
        membres = new List<GameObject>();
        membres.Add(obj);
        groupe = false;
    }

    /*
     Fonction qui récupère la liste des membres du groupe
     */
    public List<GameObject> GetMembres()
    {
        return membres;
    }


    /*Fonction qui teste si le groupe est vide ou non*/
    public bool HasGroup()
    {
        return groupe;
    }

    /*Fonction qui met à jour si le groupe a des membres ou non */
    public void CheckGroup()
    {
        if(membres == null || membres.Count == 1)
        {
            groupe = false;
        }
        else if(membres.Count > 1) { groupe = true; }
    }

    /*Fonction qui teste si le personnage any fait partie du groupe ou non*/
    public bool IsInGroup(GameObject any)
    {
        foreach(GameObject obj in membres)
        {
            if(any == obj )
            {
                return true;
            }
        }
        return false;
    }

    /*Fonction qui ajoute le personnage any au groupe */
    public void AddToGroup(GameObject any)
    {
        if (any != null)
        {
            if (!IsInGroup(any))
            {
                membres.Add(any);
            }
        }
    }

    /* Fonction qui enlève le personnage any du groupe*/
    public void RemoveFromGroup(GameObject any)
    {
        if (any != null)
        {
            if (IsInGroup(any))
            {
                membres.Remove(any);
            }
        }
    }

    private void Update()
    {
        CheckGroup();
    }



}

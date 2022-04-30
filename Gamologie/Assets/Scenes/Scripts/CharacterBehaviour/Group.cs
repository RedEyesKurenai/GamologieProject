using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    private List<GameObject> membres;
    private bool groupe;

    public Group()
    {
        membres = new List<GameObject>();
        groupe = false;
    }

    public Group(GameObject obj)
    {
        membres = new List<GameObject>();
        membres.Add(obj);
        groupe = false;
    }

    public List<GameObject> GetMembres()
    {
        return membres;
    }


    public bool HasGroup()
    {
        return groupe;
    }

    
    public void CheckGroup()
    {
        if(membres == null || membres.Count == 1)
        {
            groupe = false;
        }
        else if(membres.Count > 1) { groupe = true; }
    }

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

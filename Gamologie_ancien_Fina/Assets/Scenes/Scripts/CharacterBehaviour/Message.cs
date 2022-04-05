using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message : ScriptableObject
{
    public int sourceID;
    public int targetID;
    public bool isSupported;
    public int subject;
    public string content;
    public Vector3 position;
    public GameObject zone;
}
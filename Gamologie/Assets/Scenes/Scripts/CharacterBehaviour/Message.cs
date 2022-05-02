using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message : ScriptableObject
{
    public int sourceID;       //id de l'envoyeur
    public int targetID;       //id du recepteur
    public bool isSupported;   //lu ou non lu
    public int subject;        //sujet du message
    public string content;     //contenu du message
    public Vector3 position;   //position sur la carte
    public GameObject zone;    //ensemble d'Objet qui représente un zone ex: ligne de pillule
}
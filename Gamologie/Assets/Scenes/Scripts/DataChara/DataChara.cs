using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CharacterData", menuName ="Character/New Data")]
public class DataChara : ScriptableObject
{
    public string charaName;          //nom du personnage
    public float maxHealth;           //vie maximale
    public int damage;                //dommage reçu
    public float speed;               //vitesse du personnage
    public float timeBetweenAttack;   //temps de latence entre des attaques
}

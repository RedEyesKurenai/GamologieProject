using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CharacterData", menuName ="Character/New Data")]
public class DataChara : ScriptableObject
{
    public string charaName;
    public float maxHealth;
    public int damage;
    public float speed;
    public float timeBetweenAttack;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CharacterData", menuName ="Character/New Data")]
public class DataChara : ScriptableObject
{
    public string charaName;
    public int maxHealth;
    public int damage;
    public int speedRunning;
    public int speedDamage;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Sciptable Object/Character Data", order = -1)]
public class CharacterData : ScriptableObject
{
    [SerializeField] public int Level;
    [SerializeField] public string CharName;
    [SerializeField] public float MaxHP;
    [SerializeField] public float MoveSpeed;
    [SerializeField] public float AttackRange;
    [SerializeField] public float SkillRange;
    [SerializeField] public float AttackSpeed;
    [SerializeField] public float AttackDamage;
    [SerializeField] public float Stability;
    [SerializeField] public float CostRecover;
    [SerializeField] public float CritPer;
    [SerializeField] public float CritDamage;
    [SerializeField] public float DefencePower;
    [SerializeField] public float Healing;
    [SerializeField] public string SpriteName;
    [SerializeField] public string IconSpriteName;
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Sciptable Object/Character Data", order = -1)]
public class CharacterData : ScriptableObject
{
    [SerializeField] float maxHP;
    [SerializeField] float moveSpeed;
    [SerializeField] float attackRange;
    [SerializeField] float attackSpeed;
    [SerializeField] float attackDamage;
    [SerializeField] float stability;
    public float MaxHP { get { return maxHP; } set { maxHP = value; } }
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public float AttackRange { get { return attackRange; } set { attackRange = value; } }
    public float AttackSpeed { get { return attackSpeed; } set{ attackSpeed = value; } }
    public float AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    public float Stability { get { return stability; } set { stability = value; } } 
}



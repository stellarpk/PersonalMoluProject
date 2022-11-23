using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStat : MonoBehaviour
{
    public CharacterData myData;
    [field: SerializeField] public float MaxHP { get; set; }
    [field: SerializeField] public float CurHP { get; set; }
    [field: SerializeField] public float MoveSpeed { get; set; }
    [field: SerializeField] public float AttackRange { get; set; }
    [field: SerializeField] public float AttackSpeed { get; set; }
    [field: SerializeField] public float AttackDamage { get; set; }
    [field: SerializeField] public float Stability { get; set; }
    private void Start()
    {

    }

    public void Initialize()
    {
        MaxHP = myData.MaxHP;
        MoveSpeed = myData.MoveSpeed;
        AttackRange = myData.AttackRange;
        AttackSpeed = myData.AttackSpeed;
        AttackDamage = myData.AttackDamage;
        Stability = myData.Stability;
    }

    public void SetHP()
    {
        CurHP = MaxHP;
    }

    public void UpdateHP(float value)
    {
        CurHP = Mathf.Clamp(CurHP + value, 0.0f, MaxHP);
    }
}

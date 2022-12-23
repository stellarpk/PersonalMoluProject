using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStat : MonoBehaviour
{
    public CharacterData myData;
    public BossData bossData;
    [field: SerializeField] public float MaxHP { get; set; }
    [field: SerializeField] public float CurHP { get; set; }
    [field: SerializeField] public float MoveSpeed { get; set; }
    [field: SerializeField] public float AttackRange { get; set; }
    [field: SerializeField] public float SkillRange { get; set; }
    [field: SerializeField] public float AttackSpeed { get; set; }
    [field: SerializeField] public float AttackDamage { get; set; }
    [field: SerializeField] public float Stability { get; set; }
    [field: SerializeField] public float CritRate { get; set; }
    [field: SerializeField] public float CritDmg { get; set; }
    [field: SerializeField] public float DefencePower { get; set; }
    [field: SerializeField] public float StunGauge { get; set; }
    [field: SerializeField] public float TimeLimit { get; set; }

    public void Initialize()
    {
        MaxHP = myData.MaxHP;
        MoveSpeed = myData.MoveSpeed;
        AttackRange = myData.AttackRange;
        SkillRange = myData.SkillRange;
        AttackSpeed = myData.AttackSpeed;
        AttackDamage = myData.AttackDamage;
        Stability = myData.Stability;
        CritRate = myData.CritPer;
        CritDmg = myData.CritDamage;
        DefencePower = myData.DefencePower;

        StunGauge = bossData.StunGauge;
        TimeLimit = bossData.TimeLimit;
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

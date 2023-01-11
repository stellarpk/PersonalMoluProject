using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossDifficulty
{
    NORMAL, HARD, VERYHARD, HARDCORE
}

public class BossStat : MonoBehaviour
{
    public BossData bossData;
    [field: SerializeField] public BossDifficulty difficulty;
    [field: SerializeField] public float MaxHP { get; set; }
    [field: SerializeField] public float CurHP { get; set; }
    [field: SerializeField] public float AttackRange { get; set; }
    [field: SerializeField] public float SkillRange { get; set; }
    [field: SerializeField] public float AttackSpeed { get; set; }
    [field: SerializeField] public float AttackDamage { get; set; }
    [field: SerializeField] public float Stability { get; set; }
    [field: SerializeField] public float CritRate { get; set; }
    [field: SerializeField] public float CritDmg { get; set; }
    [field: SerializeField] public float DefencePower { get; set; }
    [field: SerializeField] public float TimeLimit { get; set; }

    public void Initialize(BossDifficulty diff)
    {
        MaxHP = bossData.MaxHP[(int)diff];
        AttackRange = bossData.AttackRange;
        SkillRange = bossData.SkillRange;
        AttackSpeed = bossData.AttackSpeed;
        AttackDamage = bossData.AttackDamage[(int)diff];
        Stability = bossData.Stability;
        DefencePower = bossData.DefencePower[(int)diff];
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Data", menuName = "Sciptable Object/Boss Data", order = -1)]
public class BossData : ScriptableObject
{
    [field: SerializeField] public float[] MaxHP { get; set; }
    [field: SerializeField] public float CurHP { get; set; }
    [field: SerializeField] public float AttackRange { get; set; }
    [field: SerializeField] public float SkillRange { get; set; }
    [field: SerializeField] public float AttackSpeed { get; set; }
    [field: SerializeField] public float[] AttackDamage { get; set; }
    [field: SerializeField] public float Stability { get; set; }
    [field: SerializeField] public float[] DefencePower { get; set; }
    [field: SerializeField] public float TimeLimit { get; set; }
    [field: SerializeField] public string BossIcon { get; set; }
}

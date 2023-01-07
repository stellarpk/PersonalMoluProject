using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Sciptable Object/Skill Data", order = -3)]
public class SkillData : ScriptableObject
{
    [SerializeField] public int SkillCost;
    [SerializeField] public float CoolTime;
    [SerializeField] public int SkillLevel;
    [SerializeField] public float[] percentage;
    [SerializeField] public float[] percentage_2;
    [SerializeField] public bool isBuff;
    [SerializeField] public float BuffTime;
    [Multiline (3)]
    [SerializeField] public string SkillExplain;
    [SerializeField] public string SkillName;
    [SerializeField] public string SkillIcon;
    public float GetPercentage(int lv)
    {
        if (lv < 1 || lv > percentage.Length) return 0.0f;
        return percentage[lv - 1];
    }
    public float GetPercentage_2(int lv)
    {
        if (percentage_2 == null) return 0.0f;
        if (lv < 1 || lv > percentage_2.Length) return 0.0f;
        return percentage_2[lv - 1];
    }
}

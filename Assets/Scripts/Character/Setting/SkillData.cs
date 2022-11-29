using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Sciptable Object/Skill Data", order = -3)]
public class SkillData : ScriptableObject
{
    [SerializeField] int skillCost;
    [SerializeField] float coolTime;
    [SerializeField] int skillLevel;
    [SerializeField] float[] percentage;

    public int SkillCost { get { return skillCost; } set { skillCost = value; } }
    public float CoolTime { get { return coolTime; } set { coolTime = value; } }
    public float GetBuffPercentage(int lv)
    {
        if (lv < 1 || lv > percentage.Length) return 0.0f;
        return percentage[lv - 1];
    }
    public int SkillLevel { get { return skillLevel; } set { skillLevel = value; } }
}

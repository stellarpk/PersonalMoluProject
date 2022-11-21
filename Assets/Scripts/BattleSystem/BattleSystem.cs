using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattle
{
    void OnDamage(float damage);
    Transform transform { get; }
    bool IsLive
    {
        get;
    }
}
[Serializable]
public struct Skill
{
    public SkillData sData;
    public float coolTime;
    public float Percentage;
    public int SkillLevel;
    public Buff buff;

    public void Initialize(SkillData SDB)
    {
        coolTime = SDB.CoolTime;
        SkillLevel = SDB.SkillLevel;
        Percentage = SDB.GetBuffPercentage(SDB.SkillLevel);
    }
}

[Serializable]
public struct Buff
{
    public BuffData bData;
    public float buffTime;
    public bool buffType; // Buff 1, DeBuff 0
    public bool isBuffOn; // On 1, Off 0

    public void Initailize(BuffData BDB)
    {
        buffTime = BDB.BuffTime;
        buffType = BDB.BuffType;
    }
}

public enum SType { Buff, Active }



public interface ISkill
{
    void EX_Skill();
    void Normal_Skill();
    void Passive_Skill();
    void Sub_Skill();
}

public class BattleSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
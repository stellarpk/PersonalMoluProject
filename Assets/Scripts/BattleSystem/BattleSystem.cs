using JetBrains.Annotations;
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
public struct Weapon
{
    public WeaponData weaponData;
    public float Staibility;
    public float BulletDamage;
    public float BulletSpeed;
    public float ReloadTime;
    public int MaxMagazine;

    public void Initialize(Character owner)
    {
        BulletDamage = owner.myStat.AttackDamage;
        Staibility = owner.myStat.Stability;
        BulletSpeed = weaponData.BulletSpeed;
        ReloadTime = weaponData.ReloadTime;
        MaxMagazine = weaponData.MaxMagazine;
    }
}

public enum BuffType
{
    AttackDamage, AttackSpeed, HP, MoveSpeed
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
    // 노말 - 액티브형
    // 패시브 - 단순 스탯 증가
    // 서브 - 조건부 스탯 변화/추가데미지,도트데미지,아군스탯증가
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

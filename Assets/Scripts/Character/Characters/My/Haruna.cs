using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haruna : Character, ISkill
{
    bool NormalReady;
    bool ActionEX;
    public HarunaEX drawEX;
    private void Start()
    {
        myStat.Initialize();
        InitializeSkill();
        Passive_Skill();
        myWeapon.weapon.Initialize(this);
        myWeapon.curMagazine = myWeapon.weapon.MaxMagazine;
        myStat.SetHP();
        fire = () => Shooting();
        scanner.FindTarget += () => { if (Changable()) ChangeState(STATE.Battle); };
        scanner.LostTarget += () => { if (Changable()) ChangeState(STATE.Move); };
        scanner.Range.radius = myStat.AttackRange / 10.0f;

        ChangeState(STATE.Wait);
        StartCoroutine(ToMoveState());

        Normal_Skill();
        Sub_Skill();
    }


    public override void TurnOnIndicator()
    {
        base.TurnOnIndicator();
        if (coEX != null) StopCoroutine(coEX);
        else coEX = StartCoroutine(UseEX());
    }

    public override void TurnOffIndicator()
    {
        base.TurnOffIndicator();
        if (coEX != null)
        {
            StopCoroutine(coEX);
            coEX = null;
        }
    }

    public override void EndReload()
    {
        base.EndReload();
        if (NormalReady)
        {
            Invoke("Normal", 0.5f);
        }
    }
    public override void EndNormalSkillAnim()
    {
        base.EndNormalSkillAnim();
        NormalReady = false;
    }

    public IEnumerator UseEX()
    {
        while (indicatorOn)
        {
            StartCoroutine(drawEX.DrawRange());
            //StartCoroutine(drawEX.RotateIndicator());
            //    EX_Skill();
            //    if (CIK != null) CIK.weight = 0;
            yield return null;
        }
    }
    // 직선 범위 내 적 공격력 N% 데미지
    public void EX_Skill()
    {
        
    }

    public override void EndEXSkillAnim()
    {
        base.EndEXSkillAnim();
        ActionEX = false;
    }

    // 30초마다 적 1인에게 공격력 N% 데미지
    public void Normal_Skill()
    {
        StartCoroutine(CoNormalSkill());
    }

    IEnumerator CoNormalSkill()
    {
        float delay = 0.0f;
        while (IsLive)
        {
            delay += Time.deltaTime;
            if (delay >= s_Normal.coolTime)
            {
                NormalReady = true;
                if(myState != STATE.Reload && !ActionEX)
                {
                    Normal();
                }
                delay = 0.0f;
            }
            yield return null;  
        }
    }

    void Normal()
    {
        ChangeState(STATE.Skill);
        myAnim.SetTrigger("Skill_Normal");
        myAnim.SetLayerWeight(myAnim.GetLayerIndex("UpperLayer"), 0);
        if (CIK != null) CIK.weight = 0;
    }

    public IEnumerator ActiveNormalSkill()
    {
        if (scanner.myTarget != null)
        {
            myAnim.SetLayerWeight(myAnim.GetLayerIndex("UpperLayer"), 1);
            if (CIK != null) CIK.weight = 1;
            yield return new WaitForSeconds(1f);
            float stabil = myStat.Stability * 0.5f;
            float skillDamage = Random.Range(myStat.AttackDamage - stabil, myStat.AttackDamage + stabil) * s_Normal.Percentage;
            float rate = myStat.CritRate / (myStat.CritRate + 650.0f);
            float finalDamage = 0;
            if (Random.Range(0.0f, 100.0f) <= rate) finalDamage = skillDamage * (myStat.CritDmg * 0.01f);
            else finalDamage = skillDamage;

            GameObject bullet = Instantiate(myWeapon.Bullet, myWeapon.muzzle.position, myWeapon.muzzle.rotation);
            bullet.GetComponent<Bullet>().OnFire(scanner.myTarget.transform.GetComponent<Character>().HitPos, finalDamage, myWeapon.weapon.weaponData.BulletSpeed * 2.0f);
        }
        EndNormalSkillAnim();
    }

    public void ActionNormal()
    {
        if (coNormal != null) StopCoroutine(coNormal);
        coNormal = StartCoroutine(ActiveNormalSkill());
    }

    // 최대체력 N% 증가
    public void Passive_Skill()
    {
        myStat.MaxHP *= s_Passive.Percentage;
        s_Passive.buff.isBuffOn = true;
    }

    // 이동하지 않으면 공격력 N%증가
    public void Sub_Skill()
    {
        StartCoroutine(SubOnOff());
    }

    IEnumerator SubOnOff()
    {
        while (IsLive)
        {
            if (myState != STATE.Move)
            {
                if (!s_Sub.buff.isBuffOn)
                {
                    myStat.AttackDamage *= s_Sub.Percentage;
                    s_Sub.buff.isBuffOn = true;
                }
            }
            else
            {
                if (s_Sub.buff.isBuffOn)
                {
                    myStat.AttackDamage /= s_Sub.Percentage;
                    s_Sub.buff.isBuffOn = false;
                }
            }
            yield return null;
        }
    }
}

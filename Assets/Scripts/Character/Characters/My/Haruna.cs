using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haruna : Character, ISkill
{
    bool NormalReady;
    bool ActionEX;
    public HarunaEX drawEX;
    public GameObject EX_Bullet;
    Coroutine EX_Range;
    private void Start()
    {

    }

    public override void Setting()
    {
        myStat.Initialize();
        InitializeSkill();
        Passive_Skill();
        myWeapon.weapon.Initialize(this);
        myWeapon.curMagazine = myWeapon.weapon.MaxMagazine;
        myStat.SetHP();
        SetHpBar();
        fire = () => Shooting();
        scanner.FindTarget += () => { if (Changable()) ChangeState(STATE.Battle); };
        scanner.LostTarget += () => { if (Changable()) ChangeState(STATE.Move); };
        StartCoroutine(scanner.CheckEnemyInRange());
        ChangeState(STATE.Wait);
        StartCoroutine(ToMoveState());
    }

    public override void StartSkillCool()
    {
        Normal_Skill();
        Sub_Skill();
    }

    private void Update()
    {
        StateProcess();
    }


    public override void TurnOnIndicator()
    {
        base.TurnOnIndicator();
        drawEX.myRenderer.enabled = true;
        drawEX.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        if (coEX != null) StopCoroutine(coEX);
        coEX = StartCoroutine(UseEX());
        
    }

    public override void TurnOffIndicator()
    {
        base.TurnOffIndicator();
        if (coEX != null)
        {
            StopCoroutine(coEX);
            coEX = null;
            drawEX.myRenderer.enabled = false;
        }
        if(EX_Range != null)
        {
            StopCoroutine(EX_Range);
            EX_Range = null;
        }
        if (UsingEX) UsingEX = false;
        isCanceling = false;    
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
            if(EX_Range != null)
            {
                StopCoroutine(EX_Range);
                EX_Range = null;
            }
            EX_Range = StartCoroutine(drawEX.DrawRange());
            yield return null;
        }
    }
    // 직선 범위 내 적 공격력 N% 데미지
    public void EX_Skill()
    {
        SkillSystem.Inst.curCost -= s_EX.sData.SkillCost;
        SkillSystem.Inst.UseSkillCard(EX_Card);
        ChangeState(STATE.Skill);
        myAnim.SetTrigger("Skill_EX");
        myAnim.SetLayerWeight(myAnim.GetLayerIndex("UpperLayer"), 0);
        if (CIK != null) CIK.weight = 0;

        transform.LookAt(drawEX.EndRange_T);
    }

    public override void Use_EX_Skill()
    {
        EX_Skill();
    }

    public void ShootEXSkill()
    {
        Debug.Log("Shoot");
        float stabil = myStat.Stability * 0.5f;
        float skillDamage = Random.Range(myStat.AttackDamage - stabil, myStat.AttackDamage + stabil) * s_Normal.Percentage;
        float rate = myStat.CritRate / (myStat.CritRate + 650.0f);
        int finalDamage = 0;
        
        GameObject bullet = Instantiate(EX_Bullet, myWeapon.muzzle.position, myWeapon.muzzle.rotation);
        if (Random.Range(0.0f, 100.0f) <= rate)
        {
            finalDamage = (int)(skillDamage * (myStat.CritDmg * 0.01f));
            bullet.GetComponent<Penetrate_Bullet>().isCritical = true;
        }
        else finalDamage = (int)skillDamage;

        bullet.GetComponent<Penetrate_Bullet>().Damage = finalDamage;
        bullet.GetComponent<Penetrate_Bullet>().OnFire(drawEX.EndRange_T, myWeapon.weapon.weaponData.BulletSpeed * 3.0f);
        EndEXSkillAnim();
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
            int finalDamage = 0;
            if (Random.Range(0.0f, 100.0f) <= rate) finalDamage = (int)(skillDamage * (myStat.CritDmg * 0.01f));
            else finalDamage = (int)skillDamage;
            Transform HitPos = scanner.myTarget.transform.GetComponent<IBattle>().hitPos;
            GameObject bullet = Instantiate(myWeapon.Bullet, myWeapon.muzzle.position, myWeapon.muzzle.rotation);
            bullet.GetComponentInChildren<Bullet>().OnFire(HitPos, finalDamage, myWeapon.weapon.weaponData.BulletSpeed * 2.0f, bullet);
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
        s_Passive.isBuffOn = true;
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
                if (!s_Sub.isBuffOn)
                {
                    myStat.AttackDamage *= s_Sub.Percentage;
                    s_Sub.isBuffOn = true;
                }
            }
            else
            {
                if (s_Sub.isBuffOn)
                {
                    myStat.AttackDamage /= s_Sub.Percentage;
                    s_Sub.isBuffOn = false;
                }
            }
            yield return null;
        }
    }
}

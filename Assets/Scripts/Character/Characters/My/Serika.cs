using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serika : Character, ISkill
{
    bool NormalReady = false;
    public AudioClip skillSound;
    Character Target;
    // 일반 공격 3점사
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
    }

    private void Update()
    {
        StateProcess();
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
        if (UsingEX) UsingEX = false;
    }

    public IEnumerator UseEX()
    {
        while (indicatorOn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);
                if (hit.collider != null && UsingEX)
                {
                    Use_EX_Skill();
                }
            }
            yield return null;
        }
    }

    // EX - 스킬 사용시 즉시 재장전, 공격력 N% 증가 (N초간)
    public void EX_Skill()
    {
        SkillSystem.Inst.curCost -= s_EX.sData.SkillCost;
        SkillSystem.Inst.UseSkillCard(EX_Card);
        Reload();
        if (s_EX.isBuffOn)
        {
            ResetEXSkillBuff();
            if (coEXBuff != null)
            {
                StopCoroutine(coEXBuff);
                coEXBuff = null;
            }
        }
        coEXBuff = StartCoroutine(coEXSkill());
        TurnOffIndicator();
    }

    public override void Use_EX_Skill()
    {
        EX_Skill();
    }


    public IEnumerator coEXSkill()
    {
        myStat.AttackDamage *= s_EX.Percentage;
        s_EX.isBuffOn = true;
        yield return new WaitForSeconds(s_EX.BuffTime);
        s_EX.isBuffOn = false;
        ResetEXSkillBuff();
    }
    void ResetEXSkillBuff()
    {
        myStat.AttackDamage /= s_EX.Percentage;
    }


    // Normal - N초마다 적 1인에게 공격력 N% 공격
    public void Normal_Skill()
    {
        StartCoroutine(CoNormalSkill());
    }

    public IEnumerator CoNormalSkill()
    {
        float delay = 0.0f;
        while (IsLive)
        {
            delay += Time.deltaTime;
            if (delay >= s_Normal.coolTime)
            {
                NormalReady = true;
                if (myState != STATE.Reload)
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
        if (CIK != null) CIK.weight = 0;
        ActiveNormalSkill();
    }

    public void ActiveNormalSkill()
    {
        myAnim.SetLayerWeight(myAnim.GetLayerIndex("UpperLayer"), 1);
        if (CIK != null) CIK.weight = 1;
        if (scanner.myTarget != null)
        {
            StartCoroutine(FireNormalSkill());
        }
    }

    public IEnumerator FireNormalSkill()
    {
        float skillDamage = myStat.AttackDamage * s_Normal.Percentage;
        int divideDamage = 5;
        float projectileDamage = skillDamage / divideDamage;
        float stability = myStat.Stability * 0.5f;
        Transform HitPos = scanner.myTarget.transform.GetComponent<IBattle>().hitPos;

        audioSource.PlayOneShot(skillSound);
        for (int i = 0; i < divideDamage; i++)
        {
            float rate = myStat.CritRate / (myStat.CritRate + 650.0f);
            int finalDamage = 0;
            GameObject bullet = Instantiate(myWeapon.Bullet, myWeapon.muzzle.position, myWeapon.muzzle.rotation);
            int Damage = (int)Random.Range(projectileDamage - stability, projectileDamage + stability);
            if (Random.Range(0.0f, 100.0f) <= rate)
            {
                finalDamage = (int)(Damage * (myStat.CritDmg * 0.01f));
                bullet.GetComponent<Bullet>().isCritical = true;
            }
            else finalDamage = Damage;
            bullet.GetComponentInChildren<Bullet>().OnFire(HitPos, finalDamage, 5f, bullet);
            if (i < divideDamage - 1)
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
        yield break;
    }

    // Passive - 공격력 N% 증가
    public void Passive_Skill()
    {
        myStat.AttackDamage *= s_Passive.Percentage;
    }

    // Sub - EX 스킬 사용시 공격속도 N% 증가 (N초간)
    public void Sub_Skill()
    {
        // 버프가 유지중이라면 기존에 있던 버프 삭제 후 갱신
        if (s_Sub.isBuffOn)
        {
            ResetSubSkillBuff();
            if (coSubBuff != null)
            {
                StopCoroutine(coSubBuff);
                coSubBuff = null;
            }
        }
        coSubBuff = StartCoroutine(IncreaseAttackSPD());
    }

    public IEnumerator IncreaseAttackSPD()
    {
        myStat.AttackSpeed *= s_Sub.Percentage;
        s_Sub.isBuffOn = true;
        yield return new WaitForSeconds(s_Sub.BuffTime);
        s_Sub.isBuffOn = false;
        ResetSubSkillBuff();
    }

    void ResetSubSkillBuff()
    {
        myStat.AttackSpeed /= s_Sub.Percentage;
    }

}

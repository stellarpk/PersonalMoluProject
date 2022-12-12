using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serika : Character, ISkill
{
    bool NormalReady = false;
    Character Target;
    // �Ϲ� ���� 3����
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
        StartCoroutine(scanner.CheckEnemyInRange());
        ChangeState(STATE.Wait);
        StartCoroutine(ToMoveState());

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

    // EX - ��ų ���� ��� ������, ���ݷ� N% ���� (N�ʰ�)
    public void EX_Skill()
    {
        SkillSystem.Inst.curCost -= s_EX.sData.SkillCost;
        Reload();
        if (s_EX.buff.isBuffOn)
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
        s_EX.buff.isBuffOn = true;
        yield return new WaitForSeconds(s_EX.buff.buffTime);
        s_EX.buff.isBuffOn = false;
        ResetEXSkillBuff();
    }
    void ResetEXSkillBuff()
    {
        myStat.AttackDamage /= s_EX.Percentage;
    }


    // Normal - N�ʸ��� �� 1�ο��� ���ݷ� N% ����
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
        Transform target = scanner.myTarget.transform.GetComponent<Character>().HitPos;
        for (int i = 0; i < divideDamage; i++)
        {
            float damage = Random.Range(projectileDamage - stability, projectileDamage + stability);
            GameObject bullet = Instantiate(myWeapon.Bullet, myWeapon.muzzle.position, myWeapon.muzzle.rotation);
            bullet.GetComponent<Bullet>().OnFire(target, damage, 5f);
            if (i < divideDamage - 1)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        yield break;
    }

    // Passive - ���ݷ� N% ����
    public void Passive_Skill()
    {
        myStat.AttackDamage *= s_Passive.Percentage;
        s_Passive.buff.isBuffOn = true;
    }

    // Sub - EX ��ų ���� ���ݼӵ� N% ���� (N�ʰ�)
    public void Sub_Skill()
    {
        // ������ �������̶�� ������ �ִ� ���� ���� �� ����
        if (s_Sub.buff.isBuffOn)
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
        s_Sub.buff.isBuffOn = true;
        yield return new WaitForSeconds(s_Sub.buff.buffTime);
        s_Sub.buff.isBuffOn = false;
        ResetSubSkillBuff();
    }

    void ResetSubSkillBuff()
    {
        myStat.AttackSpeed /= s_Sub.Percentage;
    }

}

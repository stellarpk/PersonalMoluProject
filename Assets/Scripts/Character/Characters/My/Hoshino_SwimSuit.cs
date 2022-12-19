using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoshino_SwimSuit : Character, ISkill
{
    bool NormalReady;
    public HoshinoEX EX;
    public LayerMask ally;

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
        fire = () => Shooting();
        scanner.FindTarget += () => { if (Changable()) ChangeState(STATE.Battle); };
        scanner.LostTarget += () => { if (Changable()) ChangeState(STATE.Move); };
        StartCoroutine(scanner.CheckEnemyInRange());
        ChangeState(STATE.Wait);
        StartCoroutine(ToMoveState());
    }
    private void Update()
    {
        StateProcess();
    }

    public override void StartSkillCool()
    {
        Normal_Skill();
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

    public override void Use_EX_Skill()
    {
        EX_Skill();
    }

    // 원형 범위내 아군 공격력 N% 증가 (50초간)
    public void EX_Skill()
    {
        SkillSystem.Inst.curCost -= s_EX.sData.SkillCost;
        SkillSystem.Inst.UseSkillCard(EX_Card);
        ChangeState(STATE.Skill);
        myAnim.SetTrigger("Skill_EX");
        myAnim.SetLayerWeight(myAnim.GetLayerIndex("UpperLayer"), 0);
        if (CIK != null) CIK.weight = 0;
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

    public IEnumerator coEXSkill()
    {
        //myStat.AttackDamage *= s_EX.Percentage;
        //s_EX.buff.isBuffOn = true;
        Collider[] InSkillRange = Physics.OverlapSphere(EX.transform.position, myStat.SkillRange, ally);
        for (int i = 0; i < InSkillRange.Length; i++)
        {
            EX.Allys.Add(InSkillRange[i].GetComponent<Character>());
            InSkillRange[i].GetComponent<Character>().myStat.AttackDamage *= s_EX.Percentage;
        }
        Sub_Skill();
        EX.gameObject.SetActive(true);
        s_EX.buff.isBuffOn = true;
        yield return new WaitForSeconds(s_EX.buff.buffTime);
        EX.gameObject.SetActive(false);
        s_EX.buff.isBuffOn = false;
        ResetEXSkillBuff();
    }
    void ResetEXSkillBuff()
    {
        for (int i = 0; i < EX.Allys.Count; i++)
        {
            EX.Allys[i].myStat.AttackDamage /= s_EX.Percentage;
        }
        EX.Allys.Clear();
        ResetSubSkillBuff();
    }

    // 40초 마다 적 1인에게 N%데미지, 치유력 N%만큼 회복
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
                Debug.Log("Hoshino Normal");
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
        myAnim.SetLayerWeight(myAnim.GetLayerIndex("UpperLayer"), 0);
        if (CIK != null) CIK.weight = 0;
        ActiveNormalSkill();
    }

    public void ActiveNormalSkill()
    {
        if (scanner.myTarget != null)
        {
            float stabil = myStat.Stability * 0.5f;
            float skillDamage = Random.Range(myStat.AttackDamage - stabil, myStat.AttackDamage + stabil) * s_Normal.Percentage;
            float rate = myStat.CritRate / (myStat.CritRate + 650.0f);
            float finalDamage = 0;
            if (Random.Range(0.0f, 100.0f) <= rate) finalDamage = skillDamage * (myStat.CritDmg * 0.01f);
            else finalDamage = skillDamage;
            scanner.myTarget.OnDamage(finalDamage);
            float RecoverHP = myStat.Healing * s_Normal.Percentage_2;
            myStat.UpdateHP(RecoverHP);
        }
    }


    // 방어력 N%, 공격력 N% 증가
    public void Passive_Skill()
    {
        myStat.AttackDamage *= s_Passive.Percentage;
        myStat.DefencePower *= s_Passive.Percentage;
        s_Passive.buff.isBuffOn = true;
    }
    // EX스킬 사용 중 코스트 회복력 증가
    public void Sub_Skill()
    {
        myStat.CostRecover += s_Sub.Percentage;
        s_Sub.buff.isBuffOn = true;
    }

    void ResetSubSkillBuff()
    {
        myStat.CostRecover -= s_Sub.Percentage;
        s_Sub.buff.isBuffOn = false;
    }
}

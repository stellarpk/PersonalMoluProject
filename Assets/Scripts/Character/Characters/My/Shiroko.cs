using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shiroko : Character, ISkill
{
    public GameObject Grenade;
    GameObject Throw;
    public Transform LeftHand;
    bool SubReady = true;
    bool NormalReady;
    void Start()
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
    }
    void Update()
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
        //else coEX = StartCoroutine(UseEX());
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
    // 적 1인에게 N%데미지
    public void EX_Skill()
    {

    }

    // 25초마다 원형범위 내 적에게 공격력 N% 데미지
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
        Debug.Log("Normal");
        //ActiveNormalSkill();
    }

    public void ActiveNormalSkill()
    {
        if (scanner.myTarget != null)
        {
            Debug.Log("Throw Grenade");
            if (Throw != null)
            {
                Throw.transform.SetParent(null);
                StartCoroutine(Throw.GetComponent<Grenade>().Throwing(scanner.myTarget.transform.GetComponent<Character>().HitPos, 3f));
                
            }
            
        }
    }


    public void CatchGrenade()
    {
        Throw = Instantiate(Grenade, LeftHand);
    }



    // 치명 수치 N% 증가
    public void Passive_Skill()
    {
        myStat.CritRate *= s_Passive.Percentage;
        s_Passive.buff.isBuffOn = true;
    }

    // 일반 공격 시 20% 확률로 공격속도 N% 증가(30초간)(쿨타임 25초)
    public void Sub_Skill()
    {
        if (s_Sub.buff.isBuffOn)
        {
            ResetSubSkillBuff();
            if (coSubBuff != null)
            {
                StopCoroutine(coSubBuff);
                coSubBuff = null;
            }
        }
        coSubBuff = StartCoroutine(coSubSkill());
    }

    public IEnumerator coSubSkill()
    {
        SubReady = false;
        myStat.AttackSpeed *= s_Sub.Percentage;
        s_Sub.buff.isBuffOn = true;
        yield return new WaitForSeconds(s_Sub.coolTime);
        SubReady = true;
        yield return new WaitForSeconds(s_Sub.buff.buffTime - s_Sub.coolTime);
        s_Sub.buff.isBuffOn = false;
        ResetSubSkillBuff();
    }

    void ResetSubSkillBuff()
    {
        myStat.AttackSpeed /= s_Sub.Percentage;
    }

    public override void Shooting()
    {
        if (myWeapon.curMagazine > 0)
        {
            myWeapon.Fire(scanner.myTarget.transform.GetComponent<Character>().HitPos);
            if (Random.Range(0.0f, 100.0f) <= 25.0f)
            {
                if (SubReady) Sub_Skill();
            }
        }
        else Reload();
    }
}

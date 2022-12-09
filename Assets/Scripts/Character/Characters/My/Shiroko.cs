using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shiroko : Character, ISkill
{
    public GameObject Grenade;
    public GameObject Drone;
    GameObject coDrone;
    GameObject Throw;
    public Transform LeftHand;
    public Transform DronePos;
    public Transform skillTarget;
    bool SubReady = true;
    bool NormalReady;
    bool EXWorking;
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

    public IEnumerator UseEX()
    {
        while (indicatorOn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);
                if(hit.collider != null)
                {
                    if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        skillTarget = hit.collider.gameObject.transform;
                        float checkDist = (skillTarget.GetComponent<Character>().HitPos.position - HitPos.position).magnitude;
                        if ((myStat.SkillRange*0.1f) + 0.3f >= checkDist)
                        {
                            SkillSystem.Inst.curCost -= s_EX.sData.SkillCost;
                            EXWorking = true;
                            ChangeState(STATE.Skill);
                            myAnim.SetTrigger("Skill_EX");
                            myAnim.SetLayerWeight(myAnim.GetLayerIndex("UpperLayer"), 0);
                            if (CIK != null) CIK.weight = 0;
                        }
                        else
                        {
                            Debug.Log("대상이 사거리보다 먼 곳에 있습니다.");
                        }
                        TurnOffIndicator();
                    }
                    else
                    {
                        Debug.Log(hit.collider.gameObject.name);
                        Debug.Log("적합한 대상이 아닙니다. 다시 지정해주세요");
                    }
                }
            }
            yield return null;
        }
    }

    // 적 1인에게 N%데미지
    public void EX_Skill()
    {
        if(skillTarget != null)
        {
            EX_Skill(skillTarget.GetComponent<Character>().HitPos);
        }
    }

    public void EX_Skill(Transform myTarget)
    {
        coDrone = Instantiate(Drone, DronePos.transform);

        float skillDamage = myStat.AttackDamage * s_EX.Percentage;
        int divideDamage = coDrone.GetComponent<Drone>().shotCount;
        float projectileDamage = skillDamage / divideDamage;
        float stability = myStat.Stability;

        float damage = Random.Range(projectileDamage - stability, projectileDamage + stability);
        float rate = myStat.CritRate / (myStat.CritRate + 650.0f);
        float finalDamage = 0;
        if (Random.Range(0.0f, 100.0f) <= rate) finalDamage = damage * (myStat.CritDmg * 0.01f);
        else finalDamage = damage;
        if(coEX != null) StopCoroutine(coEX);
        coEX = StartCoroutine(coDrone.GetComponent<Drone>().OpenFire(myTarget, finalDamage));
        
    }

    public override void EndEXSkillAnim()
    {
        base.EndEXSkillAnim();
        EXWorking = false;
    }

    // 25초마다 원형범위 내 적에게 공격력 N% 데미지
    public void Normal_Skill()
    {
        if (coNormal != null) StopCoroutine(coNormal);
        coNormal = StartCoroutine(CoNormalSkill());
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
                if (myState != STATE.Reload && !EXWorking)
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
        //ActiveNormalSkill();
    }

    public void ActiveNormalSkill()
    {
        if (scanner.myTarget != null)
        {
            if (Throw != null)
            {
                Throw.transform.SetParent(null);
                float stabil = myStat.Stability * 0.5f;
                float skillDamage = Random.Range(myStat.AttackDamage - stabil, myStat.AttackDamage + stabil) * s_Normal.Percentage;
                float rate = myStat.CritRate / (myStat.CritRate+650.0f);
                float finalDamage = 0;
                if (Random.Range(0.0f, 100.0f) <= rate) finalDamage = skillDamage * (myStat.CritDmg * 0.01f);
                else finalDamage = skillDamage;
                StartCoroutine(Throw.GetComponent<Grenade>().Throwing(scanner.myTarget.transform.GetComponent<Character>().HitPos, 3f, finalDamage));
                
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

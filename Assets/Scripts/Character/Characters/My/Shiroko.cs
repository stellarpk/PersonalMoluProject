using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.GridLayoutGroup;

public class Shiroko : Character, ISkill
{
    public AudioClip DroneFire;
    public GameObject Grenade;
    public GameObject Drone;
    GameObject coDrone;
    GameObject Throw;
    public Transform LeftHand;
    public Transform DronePos;
    public Transform skillTarget;
    public LayerMask myEnemy;
    bool SubReady = true;
    bool NormalReady;
    bool EXWorking;

    void Start()
    {

    }
    void Update()
    {
        StateProcess();
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
        coEX = StartCoroutine(UseEX());
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
        isCanceling = false;
    }

    public IEnumerator UseEX()
    {
        while (indicatorOn)
        {
            Use_EX_Skill();
            yield return null;
        }
    }
    public override void Use_EX_Skill()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButton(0))
            {
                Casting = true;
                isCanceling = true;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);
                if (hit.collider != null)
                {
                    Collider[] col = Physics.OverlapSphere(transform.position, myStat.SkillRange * 0.1f, myEnemy);
                    if (col.Length > 0)
                    {
                        float proximate = 0.0f;
                        int index = 0;
                        for (int i = 0; i < col.Length; i++)
                        {
                            float dis = Mathf.Abs((col[i].transform.position - hit.point).magnitude);
                            if (i == 0)
                            {
                                proximate = dis;
                                index = i;
                            }
                            else if (proximate > dis)
                            {
                                proximate = dis;
                                index = i;
                            }
                        }
                        skillTarget = col[index].transform;
                    }
                    else
                    {
                        TurnOffIndicator();
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (UsingEX)
                {
                    SkillSystem.Inst.curCost -= s_EX.sData.SkillCost;
                    SkillSystem.Inst.UseSkillCard(EX_Card);
                    EXWorking = true;
                    ChangeState(STATE.Skill);
                    myAnim.SetTrigger("Skill_EX");
                    myAnim.SetLayerWeight(myAnim.GetLayerIndex("UpperLayer"), 0);
                    if (CIK != null) CIK.weight = 0;
                    TurnOffIndicator();
                }
                else
                {
                    isCanceling = false;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (!UsingEX && isCanceling)
            {
                TurnOffIndicator();
            }
        }
    }

    // 적 1인에게 N%데미지
    public void EX_Skill()
    {
        if (skillTarget != null)
        {
            Transform HitPos = scanner.myTarget.transform.GetComponent<IBattle>().hitPos;
            EX_Skill(HitPos);
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
        int finalDamage = 0;
        if (Random.Range(0.0f, 100.0f) <= rate)
        {
            finalDamage = (int)(damage * (myStat.CritDmg * 0.01f));
            coDrone.GetComponent<Drone>().isCritical = true;
        }
        else finalDamage = (int)damage;
        if (coEX != null) StopCoroutine(coEX);
        coEX = StartCoroutine(coDrone.GetComponent<Drone>().OpenFire(myTarget, finalDamage, DroneFire));

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
                float rate = myStat.CritRate / (myStat.CritRate + 650.0f);
                int finalDamage = 0;
                if (Random.Range(0.0f, 100.0f) <= rate)
                {
                    finalDamage = (int)(skillDamage * (myStat.CritDmg * 0.01f));
                    Throw.GetComponent<Grenade>().isCritical = true;
                }
                else finalDamage = (int)skillDamage;
                Transform HitPos = scanner.myTarget.transform.GetComponent<IBattle>().hitPos;
                StartCoroutine(Throw.GetComponent<Grenade>().Throwing(HitPos, 3f, finalDamage));

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
        s_Passive.isBuffOn = true;
    }

    // 일반 공격 시 20% 확률로 공격속도 N% 증가(30초간)(쿨타임 25초)
    public void Sub_Skill()
    {
        if (s_Sub.isBuffOn)
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
        s_Sub.isBuffOn = true;
        yield return new WaitForSeconds(s_Sub.coolTime);
        SubReady = true;
        yield return new WaitForSeconds(s_Sub.BuffTime - s_Sub.coolTime);
        s_Sub.isBuffOn = false;
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
            Transform hitPos = scanner.myTarget.transform.GetComponent<IBattle>().hitPos;
            myWeapon.Fire(hitPos);
            if (Random.Range(0.0f, 100.0f) <= 25.0f)
            {
                if (SubReady) Sub_Skill();
            }
        }
        else Reload();
    }
}

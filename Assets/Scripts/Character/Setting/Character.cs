using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Character : CharacterProperty, IBattle
{
    [Header ("Stat")]
    public CharacterStat myStat;
    private float rotSpeed = 360.0f;
    public float AttackDelay;

    [Header("Character Skill")]
    public Skill s_EX;
    public Skill s_Normal;
    public Skill s_Passive;
    public Skill s_Sub;

    public PersonalWeapon myWeapon;

    public Scan scanner = null;

    public Transform HitPos;

    public Transform hitPos => HitPos;
    NavMeshPath myPath = null;
    
    Vector3 Destination = Vector3.zero;
    public Transform targetPos;

    public CharacterIK CIK;
    
    protected UnityAction fire = null;

    public Projector Skill_Indicator;

    protected Coroutine Move;
    protected Coroutine Rot;
    protected Coroutine Moving;
    protected Coroutine Battle;
    
    Coroutine indicator;
    protected Coroutine coEX;
    protected Coroutine coNormal;
    public Coroutine coEXBuff;
    protected Coroutine coSubBuff;

    public bool indicatorOn;
    public bool Casting;
    public bool UsingEX;
    public bool isCanceling;

    public GameObject EX_Card;

    public AudioClip shootSound;
    public AudioSource audioSource;

    public GameObject HPBar;
    public Transform myHeadPos;
    HpBar myHpBar;

    public GameObject DmgTxt;
    //public Transform myHitPos;
    public enum STATE
    {
        Create, Wait, Move, Battle, Skill, Reload, Death, Clear
    }

    public void SetHpBar()
    {
        GameObject bar = Instantiate(HPBar);
        myHpBar = bar.GetComponent<HpBar>();
        myHpBar.myTarget = myHeadPos;
        bar.transform.SetParent(UIManager.Inst.Hpbar);
        myHpBar.available = true;
    }

    public virtual void TurnOnIndicator()
    {
        //Time.timeScale = 0.5f;
        if (SkillSystem.Inst.curCost >= s_EX.sData.SkillCost)
        {
            if (indicator != null) StopCoroutine(indicator);
            indicator = StartCoroutine(FollowIndicator());
            indicatorOn = true;
            Casting = true;
        }
        else
        {
            StartCoroutine(EX_Card.GetComponent<UseEX>().NotEnoughCost());
        }
    }

    public virtual void TurnOffIndicator()
    {
        //Time.timeScale = 1f;
        if (indicator != null) StopCoroutine(indicator);
        Skill_Indicator.gameObject.SetActive(false);
        indicatorOn = false;
        Casting = false;
    }

    public virtual IEnumerator FollowIndicator()
    {
        Skill_Indicator.gameObject.SetActive(true);
        Skill_Indicator.orthographicSize = myStat.SkillRange / 10.0f * 1.05f;
        while (Skill_Indicator.gameObject.activeSelf)
        {
            Skill_Indicator.transform.position = new Vector3(transform.position.x, 5, transform.position.z);
            yield return null;
        }
    }

    public void InitializeSkill()
    {
        s_EX.Initialize(s_EX.sData);
        s_Normal.Initialize(s_Normal.sData);
        s_Passive.Initialize(s_Passive.sData);
        s_Sub.Initialize(s_Sub.sData);
    }

    public STATE myState = STATE.Create;
    public STATE lastState;

    public bool IsLive
    {
        get
        {
            if (Mathf.Approximately(myStat.CurHP, 0.0f))
            {
                return false;
            }
            return true;
        }
    }



    // ?????? ???? - ?????? / (1+(??????+1500))
    public void OnDamage(int damage, bool crit)
    {
        //float finalDamage = damage / (1 + (myStat.DefencePower + 1500));
        GameObject txt = Instantiate(DmgTxt);
        txt.GetComponent<DamageText>().myTarget = myHeadPos;
        txt.GetComponent<DamageText>().damage = damage;
        txt.GetComponent<DamageText>().OnAction();
        if (crit) txt.GetComponent<DamageText>().critImg.SetActive(true);
        txt.transform.SetParent(UIManager.Inst.DmgText);
        myStat.UpdateHP(-damage);
        if (Mathf.Approximately(myStat.CurHP, 0.0f))
        {
            ChangeState(STATE.Death);
            SkillSystem.Inst.OnCharacterDead(this);
            int index = Array.IndexOf(GameManager.Inst.InGameCharacters, this.gameObject);
            GameManager.Inst.InGameCharacters[index] = null;
            GameManager.Inst.CharacterDeathCount++;
            Destroy(EX_Card);
            Destroy(myHpBar.gameObject);
        }
    }

    public virtual void DestroyChar()
    {
        gameObject.SetActive(false);
    }

    public void ChangeState(STATE s)
    {
        if (myState == s) return;
        myState = s;
        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Wait:
                OnWait();
                break;
            case STATE.Move:
                OnMove(targetPos.position);
                lastState = STATE.Move;
                break;
            case STATE.Battle:
                OnBattle();
                lastState = STATE.Battle;
                break;
            case STATE.Skill:
                if (Moving != null) StopCoroutine(Moving);
                if (Move != null) StopCoroutine(Move);
                if (Rot != null) StopCoroutine(Rot);
                if (Battle != null) StopCoroutine(Battle);
                
                break;
            case STATE.Reload:
                if (Moving != null) StopCoroutine(Moving);
                if (Move != null) StopCoroutine(Move);
                if (Rot != null) StopCoroutine(Rot);
                if (Battle != null) StopCoroutine(Battle);
                if (CIK != null) CIK.weight = 0;
                break;
            case STATE.Death:
                StopAllCoroutines();
                myAnim.SetTrigger("Die");
                break;
            case STATE.Clear:
                OnClear();
                StopAllCoroutines();
                break;
        }
    }

    public void StateProcess()
    {
        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Wait:
                break;
            case STATE.Move:
                break;
            case STATE.Battle:
                if (scanner.myTarget != null && !scanner.myTarget.IsLive)
                {
                    scanner.OnLostTarget();
                }
                break;
        }
        if (!IsLive) ChangeState(STATE.Death);
    }

    void OnClear()
    {
        Debug.Log("Stage Clear");
    }

    public bool Changable()
    {
        return myState != STATE.Death;
    }

    void OnWait()
    {
        if (Moving != null) StopCoroutine(Moving);
        if (Move != null) StopCoroutine(Move);
        if (Rot != null) StopCoroutine(Rot);
        if (Battle != null) StopCoroutine(Battle);
        myAnim.SetBool("Move", false);
    }

    protected IEnumerator ToMoveState()
    {
        yield return new WaitForSeconds(2);
        ChangeState(STATE.Move);
        yield break;
    }

    #region STATE.Normal
    void OnMove(Vector3 target)
    {
        myAnim.SetLayerWeight(myAnim.GetLayerIndex("UpperLayer"), 1);
        Destination = target;
        myPath = new NavMeshPath();
        SetPath();
    }
    void SetPath()
    {
        if (Battle != null) StopCoroutine(Battle);

        NavMesh.CalculatePath(transform.position, Destination, NavMesh.AllAreas, myPath);

        Moving = StartCoroutine(MovingByPath(myPath.corners));

        myAnim.SetBool("Move", true);
        if(myAnim.GetBool("Battle")) myAnim.SetBool("Battle", false);
    }

    IEnumerator MovingByPath(Vector3[] poslist)
    {
        if (poslist.Length < 2) yield break;
        int targetPos = 1;
        while (targetPos < poslist.Length)
        {
            yield return Move = StartCoroutine(MovingToPosition(poslist[targetPos++]));
        }
        ChangeState(STATE.Wait);
    }

    IEnumerator MovingToPosition(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();

        Rot = StartCoroutine(RotatingToDirection(dir));

        while (dist > Mathf.Epsilon)
        {
            float delta = myStat.MoveSpeed * Time.deltaTime;
            if (delta > dist)
            {
                delta = dist;
            }
            dist -= delta;
            transform.Translate(dir * delta, Space.World);
            yield return null;
        }

        if (Rot != null) StopCoroutine(Rot);
    }

    IEnumerator RotatingToDirection(Vector3 dir)
    {
        float angle = Vector3.Angle(transform.forward, dir);
        float rotDir = 1.0f;
        if (Vector3.Dot(transform.right, dir) < 0.0f)
        {
            rotDir = -1.0f;
        }

        while (angle > Mathf.Epsilon)
        {
            float delta = rotSpeed * Time.deltaTime;
            if (delta > angle)
            {
                delta = angle;
            }
            angle -= delta;
            transform.Rotate(Vector3.up * delta * rotDir, Space.World);
            yield return null;
        }
    }
    #endregion

    #region STATE.Battle
    void OnBattle()
    {
        if (Moving != null) StopCoroutine(Moving);
        if (Move != null) StopCoroutine(Move);
        if (Rot != null) StopCoroutine(Rot);

        if(myAnim.GetBool("Move")) myAnim.SetBool("Move", false);
        myAnim.SetBool("Battle", true);

        myAnim.SetLayerWeight(myAnim.GetLayerIndex("UpperLayer"), 1);

        Battle = StartCoroutine(CoBattle());
    }

    protected virtual IEnumerator CoBattle()
    {
        float delay = 0.0f;
        AttackDelay = 1 / myStat.AttackSpeed;
        while (scanner.myTarget != null)
        {
            Vector3 dir = (scanner.CurTarget.transform.position - transform.position).normalized; // ???? HitPos?? ????
            Quaternion rot = Quaternion.LookRotation(dir);
            Vector3 rotDeg = rot.eulerAngles;
            rotDeg.x = rotDeg.z = 0.0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotDeg), Time.deltaTime * 5.0f);
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5.0f);

            if (myState == STATE.Battle)
            {
                delay += Time.deltaTime;
                if (delay >= AttackDelay)
                {
                    fire?.Invoke();
                    audioSource.PlayOneShot(shootSound);
                    delay = 0.0f;
                }
            }
            yield return null;
        }
    }
    public void Reload()
    {
        ChangeState(STATE.Reload);
        myAnim.SetTrigger("Reload");
        myWeapon.curMagazine = myWeapon.weapon.MaxMagazine;
    }

    public virtual void EndReload()
    {
        ChangeState(lastState);
        if (CIK != null) CIK.weight = 1;
    }

    public virtual void Shooting()
    {
        if (myWeapon.curMagazine > 0)
        {
            Transform HitPos = scanner.myTarget.transform.GetComponent<IBattle>().hitPos;
            myWeapon.Fire(HitPos);
        }
        else Reload();
    }
    #endregion

    public virtual void EndEXSkillAnim()
    {
        ChangeState(lastState);
        if (CIK != null) CIK.weight = 1;
    }

    public virtual void EndNormalSkillAnim()
    {
        ChangeState(lastState);
        if (CIK != null) CIK.weight = 1;
    }

    public virtual void Use_EX_Skill()
    {

    }

    public virtual void StartSkillCool()
    {

    }

    public virtual void Setting()
    {

    }

    public virtual void EndCoroutine()
    {
        StopAllCoroutines();
    }
}

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


    [Header("Character Skill")]
    public Skill s_Normal;
    public Skill s_Passive;
    public Skill s_Sub;

    [Header("BuffList")]
    public List<Buff> myBuffList = new List<Buff>();

    public Scan scanner = null;

    NavMeshPath myPath = null;
    
    Vector3 Destination = Vector3.zero;
    public Transform targetPos;
    
    protected UnityAction fire = null;

    Coroutine Move;
    Coroutine Rot;
    Coroutine Normal;
    Coroutine Battle;

    //public Transform HitPos;
    //public Transform myHitPos;
    public enum STATE
    {
        Create, Normal, Battle, Skill, Death, Clear
    }

    public void InitializeSkill()
    {
        s_Normal.Initialize(s_Normal.sData);
        s_Passive.Initialize(s_Passive.sData);
        s_Sub.Initialize(s_Sub.sData);
        InitializeBuff();
    }

    public void InitializeBuff()
    {
        if (s_Normal.buff.bData != null)
        {
            s_Normal.buff.Initailize(s_Normal);
        }
        if (s_Passive.buff.bData != null)
        {
            s_Passive.buff.Initailize(s_Passive);
        }
        if (s_Sub.buff.bData != null)
        {
            s_Sub.buff.Initailize(s_Sub);
        }
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



    public void OnDamage(float damage)
    {
        myStat.UpdateHP(-damage);
        if (Mathf.Approximately(myStat.CurHP, 0.0f))
        {
            Destroy(gameObject);
        }
    }

    public void ChangeState(STATE s)
    {
        if (myState == s) return;
        myState = s;
        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Normal:
                OnNormal(targetPos.position);
                lastState = STATE.Normal;
                break;
            case STATE.Battle:
                OnBattle();
                lastState = STATE.Battle;
                break;
            case STATE.Skill:
                if (Normal != null) StopCoroutine(Normal);
                if (Move != null) StopCoroutine(Move);
                if (Rot != null) StopCoroutine(Rot);
                if (Battle != null) StopCoroutine(Battle);
                break;
            case STATE.Death:
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
            case STATE.Normal:
                break;
            case STATE.Battle:
                if (scanner.myTarget != null && !scanner.myTarget.IsLive)
                {
                    Debug.Log("Enemy Dead");
                    scanner.OnLostTarget();
                }
                break;
        }
    }

    void OnClear()
    {
        Debug.Log("Stage Clear");
    }

    public bool Changable()
    {
        return myState != STATE.Death;
    }
    #region STATE.Normal
    void OnNormal(Vector3 target)
    {
        Destination = target;
        myPath = new NavMeshPath();
        SetPath();
    }
    void SetPath()
    {
        if (Battle != null) StopCoroutine(Battle);
        NavMesh.CalculatePath(transform.position, Destination, NavMesh.AllAreas, myPath);
        Normal = StartCoroutine(MovingByPath(myPath.corners));
    }

    IEnumerator MovingByPath(Vector3[] poslist)
    {
        if (poslist.Length < 2) yield break;
        int targetPos = 1;
        myAnim.SetBool("Run", true);
        while (targetPos < poslist.Length)
        {
            yield return Move = StartCoroutine(MovingToPosition(poslist[targetPos++]));
        }
        myAnim.SetBool("Run", false);
        ChangeState(STATE.Clear);
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
        if (Normal != null) StopCoroutine(Normal);
        if (Move != null) StopCoroutine(Move);
        if (Rot != null) StopCoroutine(Rot);
        Battle = StartCoroutine(Attack());
    }

    protected virtual IEnumerator Attack()
    {
        float delay = 0.0f;
        while (scanner.myTarget != null)
        {
            Vector3 dir = (scanner.CurTarget.transform.position - transform.position).normalized; // 추후 HitPos로 변경
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5.0f);

            delay += Time.deltaTime;
            if (delay >= myStat.AttackSpeed)
            {

                fire?.Invoke();
                delay = 0.0f;
            }
            yield return null;
        }
    }

    public virtual void Shooting()
    {
        Debug.Log("Attack");
    }
    #endregion
}

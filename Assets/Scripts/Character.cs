using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Character : CharacterProperty, IBattle
{
    [SerializeField] float rotSpeed = 360.0f;
    [SerializeField] CharacterStat myStat;
    NavMeshPath myPath = null;
    public Transform myHitPos;
    Vector3 Destination = Vector3.zero;
    public Transform targetPos;
    public Scan scanner = null;
    public Transform HitPos;
    public UnityAction fire = null;
    public enum STATE
    {
        Create, Normal, Battle, Death, Clear
    }

    public STATE myState = STATE.Create;

    public bool IsLive
    {
        get
        {
            if(Mathf.Approximately(myStat.CurHP, 0.0f))
            {
                return false;
            }
            return true;
        }
    }

    private void Start()
    {
        fire = () => Shooting();
        scanner.FindTarget += () => { if (Changable()) ChangeState(STATE.Battle); };
        scanner.LostTarget += () => { if (Changable()) ChangeState(STATE.Normal); };
        scanner.Range.radius = myStat.AttackRange / 10.0f;
        ChangeState(STATE.Normal);
    }

    private void Update()
    {
        StateProcess();
    }

    public void OnDamage(float damage)
    {
        myStat.UpdateHP(-damage);
        if (Mathf.Approximately(myStat.CurHP, 0.0f))
        {
            Destroy(gameObject);
        }
    }

    void ChangeState(STATE s)
    {
        if (myState == s) return;
        myState = s;
        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Normal:
                OnNormal(targetPos.position);
                break;
            case STATE.Battle:
                StopAllCoroutines();
                OnBattle();
                break;
            case STATE.Death:
                break;
            case STATE.Clear:
                OnClear();
                StopAllCoroutines();
                break;
        }
    }

    void StateProcess()
    {
        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Normal:
                break;
            case STATE.Battle:
                if(scanner.myTarget != null && !scanner.myTarget.IsLive)
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

    bool Changable()
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
        StopAllCoroutines();
        NavMesh.CalculatePath(transform.position, Destination, NavMesh.AllAreas, myPath);
        StartCoroutine(MovingByPath(myPath.corners));
    }

    IEnumerator MovingByPath(Vector3[] poslist)
    {
        if(poslist.Length<2) yield break;
        int targetPos = 1;
        myAnim.SetBool("Run", true);
        while (targetPos<poslist.Length)
        {
            yield return StartCoroutine(MovingToPosition(poslist[targetPos++]));
        }
        myAnim.SetBool("Run", false);
        ChangeState(STATE.Clear);
    }

    IEnumerator MovingToPosition(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();

        Coroutine rot = StartCoroutine(RotatingToDirection(dir));

        while (dist>Mathf.Epsilon)
        {
            float delta = myStat.MoveSpeed * Time.deltaTime;
            if (delta > dist)
            {
                delta = dist;
            }
            dist -= delta;
            transform.Translate(dir*delta, Space.World);
            yield return null;
        }

        if (rot != null) StopCoroutine(rot);
    }

    IEnumerator RotatingToDirection(Vector3 dir)
    {
        float angle = Vector3.Angle(transform.forward, dir);
        float rotDir = 1.0f;
        if(Vector3.Dot(transform.right, dir) < 0.0f)
        {
            rotDir = -1.0f;
        }

        while (angle>Mathf.Epsilon)
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
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
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

    public void Shooting()
    {
        Debug.Log("Attack");
    }
    #endregion
}

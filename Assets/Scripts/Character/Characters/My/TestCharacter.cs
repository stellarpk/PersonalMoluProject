using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacter : Character, ISkill
{
    int attackCount;
    
    Coroutine Sub_Buff;
    private void Start()
    {
        myStat.Initialize();
        InitializeSkill();
        Passive_Skill();
        myStat.SetHP();
        fire = () => Shooting();
        scanner.FindTarget += () => { if (Changable()) ChangeState(STATE.Battle); };
        scanner.LostTarget += () => { if (Changable()) ChangeState(STATE.Normal); };
        scanner.Range.radius = myStat.AttackRange / 10.0f;
        ChangeState(STATE.Normal);
        Normal_Skill();
    }

    

    private void Update()
    {
        StateProcess();
    }

    public void EX_Skill()
    {

    }


    public void Normal_Skill()
    {
        //ChangeState(STATE.Skill);
        //Sub_Skill();
        //ChangeState(lastState);
        StartCoroutine(AttackNormalSkill());
    }

    public IEnumerator AttackNormalSkill()
    {
        float delay = 0.0f;
        while (true)
        {
            delay += Time.deltaTime;
            if (delay > s_Normal.time)
            {
                ChangeState(STATE.Skill);
                Debug.Log($"NormalSkill Damage: {myStat.AttackDamage*s_Normal.Percentage}");
                delay = 0.0f;
                yield return new WaitForSeconds(1.0f);
                ChangeState(lastState);
            }
            yield return null;
        }
    }

    public void Passive_Skill()
    {
        StartCoroutine(PassiveBuff());
    }

    public IEnumerator PassiveBuff()
    {
        myBuffList.Add(s_Passive.buff);
        myStat.MaxHP *= s_Passive.Percentage;
        s_Passive.buff.isBuffOn = true;
        yield return new WaitForSeconds(s_Passive.buff.buffTime);
        myBuffList.Remove(s_Passive.buff);
        myStat.MaxHP /= s_Passive.Percentage;
        yield break;
    }


    public void Sub_Skill()
    {
        if (s_Sub.buff.isBuffOn)
        {
            ResetSubBuff();
            StopCoroutine(Sub_Buff);
        }
        Sub_Buff = StartCoroutine(SubBuff());
    }

    public IEnumerator SubBuff()
    {
        myBuffList.Add(s_Sub.buff);
        myStat.AttackDamage *= s_Sub.Percentage;
        s_Sub.buff.isBuffOn = true;
        yield return new WaitForSeconds(s_Sub.buff.buffTime);
        ResetSubBuff();
    }

    void ResetSubBuff()
    {
        s_Sub.buff.isBuffOn = false;
        myBuffList.Remove(s_Sub.buff);
        myStat.AttackDamage /= s_Sub.Percentage;
    }


    protected override IEnumerator Attack()
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
                attackCount++;
                //if (attackCount == 6)
                //{
                //    yield return new WaitForSeconds(1.0f);
                //    attackCount = 0;
                //    s_Normal_Skill();
                //}
            }
            yield return null;
        }
    }

    public override void Shooting()
    {
        //Debug.Log("Test Attack");
    }
}

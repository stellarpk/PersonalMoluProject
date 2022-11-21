using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacter : Character, ISkill
{
    public int attackCount;
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
        StartCoroutine(AttackNormalSkill());
    }

    public IEnumerator AttackNormalSkill()
    {
        float delay = 0.0f;
        while (true)
        {
            delay += Time.deltaTime;
            if (delay > s_Normal.coolTime)
            {
                ChangeState(STATE.Skill);
                myAnim.SetTrigger("Skill_Normal");
                //Debug.Log($"NormalSkill Damage: {myStat.AttackDamage*s_Normal.Percentage}");
                delay = 0.0f;
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
            StopCoroutine(SubBuff());
        }
        StartCoroutine(SubBuff());
    }

    public IEnumerator SubBuff()
    {
        ChangeState(STATE.Skill);
        myAnim.SetTrigger("Skill_Sub");
        myBuffList.Add(s_Sub.buff);
        myStat.AttackDamage *= s_Sub.Percentage;
        //Debug.Log($"Attack Damage {s_Sub.Percentage} Increase");
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

    public void EndSkillAnim()
    {
        ChangeState(lastState);
    }

    protected override IEnumerator CoBattle()
    {
        float delay = 0.0f;
        AttackDelay = 1 / myStat.AttackSpeed;
        myAnim.SetBool("Battle", true);

        while (scanner.myTarget != null)
        {
            // 캐릭터 회전
            Vector3 dir = (scanner.CurTarget.transform.position - transform.position).normalized; // 추후 HitPos로 변경
            Quaternion rot = Quaternion.LookRotation(dir);
            Vector3 rotDeg = rot.eulerAngles;
            rotDeg.x = rotDeg.z = 0.0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotDeg), Time.deltaTime * 5.0f);
            
            // 캐릭터 공격
            if (myState == STATE.Battle) {
                delay += Time.deltaTime;
                if (myStat.AttackSpeed > 1) myAnim.SetFloat("AttackSpeed", myStat.AttackSpeed);
                else myAnim.SetFloat("AttackSpeed", 1);
                if (delay >= AttackDelay)
                {
                    fire?.Invoke();
                    if (attackCount == 6)
                    {
                        attackCount = 0;
                        Sub_Skill();
                    }
                    delay = 0.0f;
                }
            }
            yield return null;
        }
    }
    public override void Shooting()
    {
        attackCount++;
        myAnim.SetTrigger("Shoot");
    }
}

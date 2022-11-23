using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serika : Character, ISkill
{
    // �Ϲ� ���� 3����
    MyAction Normal;
    private void Start()
    {
        myStat.Initialize();
        InitializeSkill();
        Passive_Skill();
        myStat.SetHP();
        fire = () => Shooting();
        scanner.FindTarget += () => { if (Changable()) ChangeState(STATE.Battle); };
        scanner.LostTarget += () => { if (Changable()) ChangeState(STATE.Move); };
        scanner.Range.radius = myStat.AttackRange / 10.0f;
    }

    private void Update()
    {
        StateProcess();
    }

    // EX - ��ų ���� ��� ������, ���ݷ� N% ���� (N�ʰ�)
    public void EX_Skill()
    {
        
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
                ChangeState(STATE.Skill);
                myAnim.SetTrigger("Skill_Normal");
                Normal?.Invoke();
                delay = 0.0f;
            }
            yield return null;
        }
    }

    public void ActiveNormalSkill()
    {

    }

    // Passive - ���ݷ� N% ����
    public void Passive_Skill()
    {
        myBuffList.Add(s_Passive.buff);
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
            StopCoroutine(IncreaseAttackSPD());
        }
        StartCoroutine(IncreaseAttackSPD());
    }

    public IEnumerator IncreaseAttackSPD()
    {
        myBuffList.Add(s_Sub.buff);
        myStat.AttackSpeed *= s_Sub.Percentage;
        s_Sub.buff.isBuffOn = true;
        yield return new WaitForSeconds(s_Sub.buff.buffTime);
        ResetSubSkillBuff();
    }

    void ResetSubSkillBuff()
    {
        s_Sub.buff.isBuffOn = false;
        myBuffList.Remove(s_Sub.buff);
        myStat.AttackSpeed /= s_Sub.Percentage;
    }

    public override void Shooting()
    {
        myWeapon.Fire(scanner.myTarget.transform);
    }
}

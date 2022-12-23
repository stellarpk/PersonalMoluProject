using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : CharacterProperty, IBattle
{
    [Header("STAT")]
    public BossStat bossInfo;

    public Transform HitPos;

    public IBattle singularTarget;
    public GameObject single;
    public IBattle[] multipleTarget = new IBattle[4];
    public GameObject[] multi = new GameObject[4];
    public List<GameObject> enemyList = new List<GameObject>();

    public Transform[] Muzzle = new Transform[4];
    public GameObject Skill_1_Projectile;
    public GameObject Skill_1_Flare;

    public Transform ArcMuzzle;
    public GameObject Skill_2_Projectile;

    public Transform[] MissileMuzzle = new Transform[4];
    public GameObject Skill_3_Projectile;
    public GameObject Skill_3_Flare;
    public LayerMask character;
    private void Start()
    {
        bossInfo.Initialize();
        StartCoroutine(Think());
    }

    public bool IsLive
    {
        get
        {
            if (Mathf.Approximately(bossInfo.CurHP, 0.0f))
            {
                return false;
            }
            return true;
        }
    }

    public void OnDamage(float damage)
    {
        bossInfo.UpdateHP(-damage);
        if (Mathf.Approximately(bossInfo.CurHP, 0.0f))
        {
            Destroy(gameObject);
        }
    }


    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int randAction = UnityEngine.Random.Range(0, 5);
        switch (randAction)
        {
            case 0:
            case 1:
                StartCoroutine(Skill_1());
                break;
            case 2:
            case 3:
                //StartCoroutine(Skill_2());
                //break;
            case 4:
                StartCoroutine(Skill_3());
                break;
            default:
                break;
        }
    }

    public void ScanEnemy()
    {
        Collider[] InAttackRange = Physics.OverlapSphere(transform.position, bossInfo.AttackRange, character);
        for (int i = 0; i < InAttackRange.Length; i++)
        {
            if (!enemyList.Contains(InAttackRange[i].gameObject))
            {
                enemyList.Add(InAttackRange[i].gameObject);
                if (singularTarget == null)
                {
                    IBattle ib = InAttackRange[i].transform.GetComponent<IBattle>();
                    if (ib != null)
                    {
                        if (ib.IsLive)
                        {
                            singularTarget = InAttackRange[i].transform.GetComponent<IBattle>();
                            single = InAttackRange[i].gameObject;
                        }
                    }
                }
            }
        }
    }

    public void ScanMultipleEnemy()
    {
        Collider[] InAttackRange = Physics.OverlapSphere(transform.position, bossInfo.AttackRange, character);
        for (int i = 0; i < InAttackRange.Length; i++)
        {
            if (!enemyList.Contains(InAttackRange[i].gameObject))
            {
                enemyList.Add(InAttackRange[i].gameObject);
                if (multipleTarget[i] == null)
                {
                    IBattle ib = InAttackRange[i].transform.GetComponent<IBattle>();
                    if (ib != null)
                    {
                        if (ib.IsLive)
                        {
                            Debug.Log("ib.IsLive");
                            multipleTarget[i] = InAttackRange[i].transform.GetComponent<IBattle>();
                            multi[i] = InAttackRange[i].gameObject;
                        }
                    }
                }
            }
        }
    }

    // �� 1�ο��� ���ݷ� N% ������
    public IEnumerator Skill_1()
    {
        ScanEnemy();
        float skillDamage = bossInfo.AttackDamage * 1.5f;
        int divideDamage = Muzzle.Length;
        float projectileDamage = skillDamage / divideDamage;
        float stabil = bossInfo.Stability * 0.5f;
        Transform target = single.transform.GetComponent<Character>().HitPos;
        transform.LookAt(target);
        for (int i = 0; i < Muzzle.Length; i++)
        {
            float Damage = UnityEngine.Random.Range(projectileDamage - stabil, projectileDamage + stabil);
            GameObject bullet = Instantiate(Skill_1_Projectile, Muzzle[i].position, Muzzle[i].rotation);
            Instantiate(Skill_1_Flare, Muzzle[i].position, Muzzle[i].rotation);
            bullet.GetComponentInChildren<Bullet>().OnFire(target, Damage, 10f, bullet);
        }
        yield return new WaitForSeconds(3f);
        ResetTarget();
        StartCoroutine(Think());
    }

    // ��ä�� ���� �� ������ N% ������
    public IEnumerator Skill_2()
    {
        Debug.Log("Skill_2");
        yield return new WaitForSeconds(3f);
        StartCoroutine(Think());
    }

    // �� 4�ο��� ���ݷ� N% ������(����ü 4�� �߻�)
    public IEnumerator Skill_3()
    {
        ScanMultipleEnemy();
        float skillDamage = bossInfo.AttackDamage * 2.5f;
        int divideDamage = MissileMuzzle.Length;
        float projectileDamage = skillDamage / divideDamage;
        float stabil = bossInfo.Stability * 0.5f;
        
        for (int i = 0; i < MissileMuzzle.Length; i++)
        {
            Transform target = null;
            if (multipleTarget[i] != null) target = multipleTarget[i].transform.GetComponent<Character>().HitPos;
            else
            {
                for (int j = 0; j < i; j++)
                {
                    if (multipleTarget[j]!=null)
                    {
                        target = multipleTarget[j].transform.GetComponent<Character>().HitPos;
                        break;
                    }
                }
            }
            
            float Damage = UnityEngine.Random.Range(projectileDamage - stabil, projectileDamage + stabil);
            GameObject bullet = Instantiate(Skill_3_Projectile, MissileMuzzle[i].position, MissileMuzzle[i].rotation);
            Instantiate(Skill_3_Flare, MissileMuzzle[i].position, MissileMuzzle[i].rotation);
            bullet.GetComponent<Rigidbody>().AddForce(Vector3.up*10f, ForceMode.Impulse);
            bullet.GetComponentInChildren<Bullet>().OnFire(target, Damage, 20f, bullet);
        }
        ResetTarget();
        yield return new WaitForSeconds(3f);
        StartCoroutine(Think());
    }

    void ResetTarget()
    {
        enemyList.Clear();
        single = null;
        singularTarget = null;
        Array.Clear(multi, 0, multi.Length);
        Array.Clear(multipleTarget, 0, multipleTarget.Length);
    }
}

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

    public Transform TextPos;
    public GameObject DmgTxt;

    public AudioSource audioSource;
    public AudioClip skill_1_Sound;
    public AudioClip skill_2_Sound;
    public AudioClip skill_3_Sound;
    public AudioClip DeadSound;

    public BossHpBar hpbar;

    public float CurDamaged;
    private void Start()
    {
        //StartPattern();

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

    public Transform hitPos => HitPos;

    public void OnDamage(int damage, bool crit)
    {
        if (!myAnim.GetBool("Attacked"))
        {
            myAnim.SetBool("Attacked", true);
            StartPattern();
        }
        GameObject txt = Instantiate(DmgTxt);
        
        txt.GetComponent<DamageText>().myTarget = TextPos;
        txt.GetComponent<DamageText>().damage = damage;
        txt.GetComponent<DamageText>().OnAction();
        if (crit) txt.GetComponent<DamageText>().critImg.SetActive(true);
        txt.transform.SetParent(UIManager.Inst.DmgText);
        bossInfo.UpdateHP(-damage);
        hpbar.Damaged();
        //CurDamaged += damage;
        if (Mathf.Approximately(bossInfo.CurHP, 0.0f))
        {
            StopAllCoroutines();
            audioSource.PlayOneShot(DeadSound);
            GameManager.Inst.CheckBossDead();
            myAnim.SetTrigger("Die");
        }
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }

    public void Setting()
    {
        bossInfo.Initialize(DataManager.Inst.difficulty);
    }

    public void StartPattern()
    {
        StartCoroutine(Think());
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
                StartCoroutine(Skill_2());
                break;
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
                            multipleTarget[i] = InAttackRange[i].transform.GetComponent<IBattle>();
                            multi[i] = InAttackRange[i].gameObject;
                        }
                    }
                }
            }
        }
    }

    public int Damage(float skillPercentage)
    {
        float skillDamage = bossInfo.AttackDamage * skillPercentage;
        int divideDamage = MissileMuzzle.Length;
        float projectileDamage = skillDamage / divideDamage;
        float stabil = bossInfo.Stability * 0.5f;

        return (int)UnityEngine.Random.Range(projectileDamage - stabil, projectileDamage + stabil);
    }

    // 적 1인에게 공격력 N% 데미지
    public IEnumerator Skill_1()
    {
        ScanEnemy();
        if (single!=null)
        {
            myAnim.SetTrigger("Skill_1");

            Transform target = single.transform.GetComponent<Character>().HitPos;
            transform.LookAt(target);
            for (int i = 0; i < Muzzle.Length; i++)
            {
                GameObject bullet = Instantiate(Skill_1_Projectile, Muzzle[i].position, Muzzle[i].rotation);
                Instantiate(Skill_1_Flare, Muzzle[i].position, Muzzle[i].rotation);
                audioSource.PlayOneShot(skill_1_Sound);
                bullet.GetComponentInChildren<Bullet>().OnFire(target, Damage(5f), 10f, bullet);
            }
            yield return new WaitForSeconds(3f);
            ResetTarget();
            StartCoroutine(Think());
        }
    }

    // Robot 5 Land
    // 부채꼴 범위 내 적에게 N% 데미지를 가하는 투사체 N개 발사
    public IEnumerator Skill_2()
    {
        myAnim.SetTrigger("Skill_2");
        int startAngle = 45;
        int endAngle = 135;
        int angleInterval = 15;

        for (int i = startAngle; i <= endAngle; i += angleInterval)
        {
            GameObject bullet = Instantiate(Skill_2_Projectile, ArcMuzzle.position, ArcMuzzle.rotation);
            Vector3 dir = new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), Mathf.Sin(i * Mathf.Deg2Rad));
            Vector3 turn = Quaternion.Euler(270, 0, 0) * dir;
            bullet.transform.forward = turn;
            bullet.GetComponentInChildren<NoneTargetBullet>().OnFire(Damage(3.5f), 10f, bullet);
            Destroy(bullet, 5f);
        }
        audioSource.PlayOneShot(skill_2_Sound);
        yield return new WaitForSeconds(3f);
        StartCoroutine(Think());
    }

    // 적 4인에게 공격력 N% 데미지(투사체 4개 발사)
    public IEnumerator Skill_3()
    {
        ScanMultipleEnemy();
        bool AllEnemyLive = false;
        for (int i = 0; i < multi.Length; i++)
        {
            if (multi[i] != null) AllEnemyLive = true;
        }
        if (AllEnemyLive)
        {
            myAnim.SetTrigger("Skill_3");
            for (int i = 0; i < MissileMuzzle.Length; i++)
            {
                Transform target = null;
                if (multipleTarget[i] != null) target = multipleTarget[i].transform.GetComponent<IBattle>().hitPos;
                else
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (multipleTarget[j] != null)
                        {
                            target = multipleTarget[j].transform.GetComponent<IBattle>().hitPos;
                            break;
                        }
                    }
                }

                GameObject bullet = Instantiate(Skill_3_Projectile, MissileMuzzle[i].position, MissileMuzzle[i].rotation);
                Instantiate(Skill_3_Flare, MissileMuzzle[i].position, MissileMuzzle[i].rotation);
                bullet.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
                bullet.GetComponentInChildren<Bullet>().OnFire(target, Damage(7.0f), 20f, bullet);
            }
            audioSource.PlayOneShot(skill_3_Sound);
            ResetTarget();
        }
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

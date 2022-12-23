using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PersonalWeapon : MonoBehaviour
{
    public Weapon weapon;
    public GameObject muzzleEffect;
    public int curMagazine;
    public Transform muzzle;
    public GameObject Bullet;
    public Character Owner;

    private void OnEnable()
    {
        curMagazine = weapon.MaxMagazine;
    }

    public void Fire(Transform myTarget)
    {
        StartCoroutine(Shoot(myTarget));
    }
    public IEnumerator Shoot(Transform targetPos)
    {
        int divideDmg = weapon.weaponData.BulletPerAttack;
        for (int i = 0; i < weapon.weaponData.BulletPerAttack; i++)
        {
            float stabil = Owner.myStat.Stability * 0.5f;
            float bulletDamage = Random.Range(Owner.myStat.AttackDamage - stabil, Owner.myStat.AttackDamage + stabil) / divideDmg;
            float rate = Owner.myStat.CritRate / (Owner.myStat.CritRate + 650.0f);
            float finalDamage = 0;
            if (Random.Range(0.0f, 100.0f) <= rate)
            {
                finalDamage = bulletDamage * (Owner.myStat.CritDmg * 0.01f);
                Debug.Log("Critical");
            }
            else finalDamage = bulletDamage;
            GameObject bullet = Instantiate(Bullet, muzzle.position, muzzle.rotation);
            Instantiate(muzzleEffect, muzzle.position, muzzle.rotation);
            bullet.GetComponentInChildren<Bullet>().OnFire(targetPos, finalDamage, weapon.weaponData.BulletSpeed, bullet);
            if (i < weapon.weaponData.BulletPerAttack - 1)
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
        curMagazine--;
        yield break;
    }
}

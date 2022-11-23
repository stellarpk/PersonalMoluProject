using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PersonalWeapon : MonoBehaviour
{
    public Weapon weapon;
    public int curMagazine;
    public Transform muzzle;
    public Character owner;
    public GameObject Bullet;
    public bool isReload;
    public UnityEvent reload;

    private void OnEnable()
    {
        curMagazine = weapon.MaxMagazine;
    }

    public void Fire(Transform myTarget)
    {
        if (curMagazine > 0) StartCoroutine(Shoot(myTarget));
        else reload?.Invoke();
    }
    public IEnumerator Shoot(Transform targetPos)
    {
        int divideDmg = weapon.weaponData.BulletPerAttack;
        for (int i = 0; i < weapon.weaponData.BulletPerAttack; i++)
        {
            float stabil = weapon.Staibility * 0.5f;
            float bulletDamage = Random.Range(weapon.BulletDamage-stabil, weapon.BulletDamage + stabil)/divideDmg;
            GameObject bullet =  Instantiate(Bullet,muzzle.position,muzzle.rotation);
            bullet.GetComponent<Bullet>().OnFire(targetPos, bulletDamage, weapon.weaponData.BulletSpeed);
            if (i < weapon.weaponData.BulletPerAttack - 1)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        curMagazine--;
        Debug.Log($"Mag: {curMagazine}");
        yield break;
    }
}

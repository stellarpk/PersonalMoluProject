using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Sciptable Object/Weapon Data", order = -4)]
public class WeaponData : ScriptableObject
{
    [SerializeField] float bulletSpeed;
    [SerializeField] int maxMagazine;
    [SerializeField] int bulletPerAttack;

    public float BulletSpeed { get { return bulletSpeed; } set { bulletSpeed = value; } }
    public int MaxMagazine { get { return maxMagazine; } set { maxMagazine = value; } }
    public int BulletPerAttack { get { return bulletPerAttack; } set { bulletPerAttack = value; } }
}

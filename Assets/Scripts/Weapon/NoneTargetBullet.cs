using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneTargetBullet : MonoBehaviour
{
    public GameObject Impact;
    public GameObject Bullet;
    public LayerMask character;
    public LayerMask structure;
    public int Damage;
    public void OnFire(int dmg, float moveSpeed, GameObject bullet)
    {
        Damage = dmg;
        Bullet = bullet;
        StartCoroutine(Move(moveSpeed));
    }

    IEnumerator Move(float moveSpeed)
    {
        while (true)
        {
            transform.Translate(transform.forward*moveSpeed*Time.deltaTime, Space.World);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(Impact, transform.position, Quaternion.Euler(0, 180, 0));
        if (other.GetComponent<IBattle>() != null)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ally"))
            {
                other.GetComponent<IBattle>().OnDamage(Damage, false);
            }
        }
        Destroy(Bullet);
    }
}

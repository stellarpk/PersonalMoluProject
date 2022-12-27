using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject Impact;
    public void OnFire(Transform target, float dmg, float moveSpeed, GameObject bullet)
    {
        StartCoroutine(Move(target, dmg, moveSpeed, bullet));
    }

    IEnumerator Move(Transform target, float dmg, float moveSpeed,GameObject bullet)
    {
        while (target != null)
        {
            Vector3 dir = target.position - transform.position;
            float dist = dir.magnitude;
            dir.Normalize();
            float delta = Time.deltaTime * moveSpeed;
            if (delta >= dist)
            {
                delta = dist;
                break;
            }
            transform.Translate(dir * delta, Space.World);
            transform.LookAt(target);
            yield return null;
        }
        if (target != null)
        {
            transform.position = target.position;
            Instantiate(Impact, target.position, Quaternion.Euler(0,180,0));
            if (target.GetComponent<IBattle>() != null)
            {
                if (target.GetComponent<IBattle>().IsLive) target.GetComponent<IBattle>().OnDamage(dmg);
            }
            else
            {
                if (target.parent.GetComponent<IBattle>().IsLive) target.parent.GetComponent<IBattle>().OnDamage(dmg);
            }
        }
        Destroy(bullet);
    }
}

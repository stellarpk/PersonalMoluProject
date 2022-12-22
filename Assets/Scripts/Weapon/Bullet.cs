using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject Impact;
    public void OnFire(Transform target, float dmg, float moveSpeed)
    {
        StartCoroutine(Move(target, dmg, moveSpeed));
    }

    IEnumerator Move(Transform target, float dmg, float moveSpeed)
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
            target.parent.GetComponent<IBattle>().OnDamage(dmg);
            // Add hitEffect 
        }
        Destroy(gameObject);
    }
}

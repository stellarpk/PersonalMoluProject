using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penetrate_Bullet : MonoBehaviour
{
    public float DecreaseDamage;
    public float DecreaseCount;
    public float Damage;
    public bool isCritical;
    float decreasing = 1.0f;
    public void OnFire(Transform target,  float moveSpeed)
    {
        StartCoroutine(Move(target, moveSpeed));
    }

    IEnumerator Move(Transform target, float moveSpeed)
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
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (other.gameObject.GetComponent<IBattle>() != null)
            {
                other.gameObject.GetComponent<IBattle>().OnDamage((int)Damage, isCritical);
                if (DecreaseCount > 0)
                {
                    DecreaseCount--;
                    decreasing -= DecreaseDamage;
                    Damage *= decreasing;
                }
                
            }
        }
    }
}

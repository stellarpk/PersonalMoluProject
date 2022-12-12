using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public LayerMask myEnemy;
    public IEnumerator Throwing(Transform target, float MoveSpeed, float Damage)
    {
        Vector3 sdir = target.position - transform.position;
        float sdist, dist = sdir.magnitude;
        sdist = dist;
        sdir.Normalize();
        Vector3 pos = transform.position;
        while (target!=null)
        {
            float delta = Time.deltaTime * MoveSpeed;
            float y = Mathf.Sin((Mathf.PI * (sdist - dist)) / sdist)* (sdist/Mathf.PI);
            
            if (delta >= dist)
            {
                delta = dist;
                break;
            }
            dist -= delta;
            pos += sdir * delta;
            transform.position = pos + new Vector3(0,y,0);
            yield return null;
        }
        if (target != null)
        {
            transform.position = target.position;
            Collider[] col = Physics.OverlapSphere(transform.position, 2.0f,myEnemy);
            for (int i = 0; i < col.Length; i++)
            {
                col[i].gameObject.GetComponent<IBattle>().OnDamage(Damage);
            }
        }
        Destroy(gameObject);
    }
}

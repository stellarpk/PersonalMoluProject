using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public IEnumerator Throwing(Transform target, float MoveSpeed)
    {
        float movedis = 0;
        Vector3 sdir = target.position - transform.position;
        float sdist = sdir.magnitude;
        while (target!=null)
        {
            Vector3 dir = target.position - transform.position;
            float dist = dir.magnitude;
            float delta = Time.deltaTime * MoveSpeed;
            float y = Mathf.Sin((Mathf.PI * movedis) / sdist);
            Debug.Log(y);
            dir = new Vector3(dir.x, dir.y + y, dir.z);
            dir.Normalize();
            if (delta >= dist)
            {
                delta = dist;
                break;
            }
            transform.Translate(dir * delta, Space.World);
            movedis += (dir * delta).magnitude;
            yield return null;
        }
        if (target != null)
        {
            transform.position = target.position;
            Collider[] col = Physics.OverlapSphere(transform.position, 2.0f);

        }
        Destroy(gameObject);
    }
}

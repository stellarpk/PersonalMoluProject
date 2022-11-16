using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scan : MonoBehaviour
{
    public LayerMask myEnemy = default;
    public IBattle myTarget = null;
    public List<GameObject> enemyList = new List<GameObject>();
    public event MyAction FindTarget = null;
    public event MyAction LostTarget = null;
    public SphereCollider Range;
    public GameObject CurTarget;
    public void OnLostTarget()
    {
        myTarget = null;
        enemyList.Remove(CurTarget);
        CurTarget = null;
        if (enemyList.Count > 0)
        {
            myTarget = enemyList[0].GetComponent<IBattle>();
            CurTarget = enemyList[0];
        }
        else
        {
            LostTarget?.Invoke();
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if((myEnemy & 1 << other.gameObject.layer) != 0)
        {
            enemyList.Add(other.gameObject);
            if(myTarget == null)
            {
                IBattle ib = other.transform.GetComponent<IBattle>();
                if (ib != null)
                {
                    if (ib.IsLive)
                    {
                        myTarget = other.transform.GetComponent<IBattle>();
                        CurTarget = other.gameObject;
                        FindTarget?.Invoke();
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (myTarget != null && other.transform == myTarget.transform) OnLostTarget();
        enemyList.Remove(other.gameObject);
    }
}

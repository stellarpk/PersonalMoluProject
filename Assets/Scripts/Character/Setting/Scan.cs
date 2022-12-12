using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scan : MonoBehaviour
{
    public Character Owner;
    public LayerMask myEnemy;
    public IBattle myTarget = null;
    public List<GameObject> enemyList = new List<GameObject>();
    public event MyAction FindTarget = null;
    public event MyAction LostTarget = null;
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

    public IEnumerator CheckEnemyInRange()
    {
        float range = Owner.myStat.AttackRange * 0.1f;
        while (Owner.IsLive)
        {
            Collider[] InAttackRange = Physics.OverlapSphere(transform.position, range, myEnemy);
            for (int i = 0; i < InAttackRange.Length; i++)
            {
                if (!enemyList.Contains(InAttackRange[i].gameObject))
                {
                    enemyList.Add(InAttackRange[i].gameObject);
                    if (myTarget == null)
                    {
                        IBattle ib = InAttackRange[i].transform.GetComponent<IBattle>();
                        if (ib != null)
                        {
                            if (ib.IsLive)
                            {
                                myTarget = InAttackRange[i].transform.GetComponent<IBattle>();
                                CurTarget = InAttackRange[i].gameObject;
                                FindTarget?.Invoke();
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < enemyList.Count; i++)
            {
                float distance = (enemyList[i].transform.position - transform.position).magnitude;
                float colDistance = (Owner.myStat.AttackRange * 0.1f + 0.2f);
                if (distance > colDistance)
                {
                    if (myTarget != null && enemyList[i].transform == myTarget.transform)
                    {
                        OnLostTarget();
                        continue;
                    }
                    enemyList.Remove(enemyList[i].gameObject);
                }
            }
            yield return null;
        }
    }

}

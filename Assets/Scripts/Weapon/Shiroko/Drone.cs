using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform FirePos;
    float m_speed = 1.0f;
    float m_disFromStart = 4.0f;
    float m_disFromEnd = 2.0f;
    public int shotCount = 12;
    float m_interval = 0.15f;
    int shotCountPerInterval = 2;
    public IEnumerator OpenFire(Transform myTarget, float Damage)
    {
        int _shotCount = shotCount;
        while (myTarget!=null && _shotCount>0)
        {
            for (int i = 0; i < shotCountPerInterval; i++)
            {
                if (_shotCount>0)
                {
                    GameObject missile = Instantiate(missilePrefab);
                    missile.GetComponent<DroneMissile>().target = myTarget;
                    missile.GetComponent<DroneMissile>().damage = Damage;
                    missile.GetComponent<DroneMissile>().Init(FirePos,myTarget,m_speed,m_disFromStart,m_disFromEnd);
                    _shotCount--;
                }
            }
            yield return new WaitForSeconds(m_interval);
        }
        Destroy(gameObject);
    }
}

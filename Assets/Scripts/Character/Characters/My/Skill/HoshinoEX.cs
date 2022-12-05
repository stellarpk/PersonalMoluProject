using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoshinoEX : MonoBehaviour
{
    public Character Owner;
    public List<Character> Allys;
    public LayerMask ally;
    Coroutine checkAlly;
    private void Start()
    {

    }

    private void OnEnable()
    {
        if (checkAlly != null)
        {
            StopCoroutine(checkAlly);
            checkAlly = null;
        }
        checkAlly = StartCoroutine(CheckAlly());
    }

    private void OnDisable()
    {
        if (checkAlly != null)
        {
            StopCoroutine(checkAlly);
            checkAlly = null;
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Ally"))
    //    {
    //        if (!Allys.Contains(other.GetComponent<Character>()))
    //        {
    //            Allys.Add(other.GetComponent<Character>());
    //            other.GetComponent<Character>().myStat.AttackDamage *= Owner.s_EX.Percentage;
    //        }
    //    }

    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Ally"))
    //    {
    //        Allys.Remove(other.GetComponent<Character>());
    //        other.GetComponent<Character>().myStat.AttackDamage /= Owner.s_EX.Percentage;
    //    }
    //}

    IEnumerator CheckAlly()
    {
        float range = Owner.myStat.SkillRange * 0.1f;
        while (gameObject.activeSelf)
        {
            Collider[] InSkillRange = Physics.OverlapSphere(transform.position, range, ally);
            for (int i = 0; i < InSkillRange.Length; i++)
            {
                if (!Allys.Contains(InSkillRange[i].GetComponent<Character>()))
                {
                    Allys.Add(InSkillRange[i].GetComponent<Character>());
                    InSkillRange[i].GetComponent<Character>().myStat.AttackDamage *= Owner.s_EX.Percentage;
                }
            }
            for (int i = 0; i < Allys.Count; i++)
            {
                float distance = (Allys[i].transform.position - transform.position).magnitude;
                if(distance > range)
                {
                    Allys[i].myStat.AttackDamage /= Owner.s_EX.Percentage;
                    Allys.Remove(Allys[i]);
                }
            }
            yield return null;
        }
    }
}

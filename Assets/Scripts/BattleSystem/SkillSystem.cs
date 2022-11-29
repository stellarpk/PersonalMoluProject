using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : MonoBehaviour
{
    public static SkillSystem Inst = null;
    public List<Character> characters;
    const int MaxCost = 10;
    public int max;
    public float curCost = 0;
    public int usableCost;

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }
        Inst = this;
    }
    private void Start()
    {
        max = MaxCost;
        RecoverCost();
    }

    public void RecoverCost()
    {
        StartCoroutine(Recovering());
    }

    // ĳ���ʹ� �⺻ �ڽ�Ʈ ȸ���� 700, �ʴ� 0.07ȸ��. ȸ���� = ȸ����/10000
    IEnumerator Recovering()
    {
        while (true)
        {
            if (curCost <= MaxCost && usableCost <= MaxCost)
            {
                float totalCostRecover = 0;
                for (int i = 0; i < characters.Count; i++)
                {
                    totalCostRecover += characters[i].myStat.CostRecover;
                }
                curCost += totalCostRecover * 0.0001f * Time.deltaTime;
                usableCost = Mathf.FloorToInt(curCost);
            }
            yield return null;
        }
    }

    public void EXSkillSetting()
    {

    }
}

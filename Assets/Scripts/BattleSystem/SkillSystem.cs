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

    // 캐릭터당 기본 코스트 회복력 700, 초당 0.07회복. 회복량 = 회복력/10000
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

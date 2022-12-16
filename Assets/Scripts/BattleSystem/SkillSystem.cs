using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillSystem : MonoBehaviour
{
    public static SkillSystem Inst = null;
    public List<Character> characters;
    [Header("Skill Card Position")]
    public Transform[] ArrangedPos;
    public Transform NotArrangedPos;
    public Transform SkillCardReadyPos;

    
    [Header("Skill Card")]
    public GameObject[] Arranged;
    public List<GameObject> NotArranged;
    public GameObject SkillCard;
    public List<GameObject> SkillCardReady;

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

    

    public void ArrangeSkillCard()
    {
        int NotArrangedIndex = 0;
        for (int i = 0; i < ArrangedPos.Length; i++)
        {
            if (i < characters.Count)
            {
                GameObject card = SkillCardReady[i];
                card.transform.SetParent(ArrangedPos[i]);
                card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                Arranged[i] = card;
                NotArrangedIndex++;
            }
        }
        int NotArrangedLength = characters.Count - ArrangedPos.Length + NotArrangedIndex;
        for (int i = NotArrangedIndex; i < NotArrangedLength; i++)
        {
            GameObject card = SkillCardReady[i];
            card.transform.SetParent(NotArrangedPos);
            card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            NotArranged.Add(card);
        }
    }

    // 카드 사용시 다른 카드 재배치
    public void UseSkillCard(GameObject used)
    {
        // 사용한 카드 제거
        int index = Array.FindIndex(Arranged, i => i.GetComponent<UseEX>().character == used.GetComponent<UseEX>().character);
        NotArranged.Add(Arranged[index]);
        Arranged[index].transform.SetParent(NotArrangedPos);
        Arranged[index].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        Arranged[index] = null;
        // 배치 안된 카드 배치
        NotArranged[0].transform.SetParent(ArrangedPos[index]);
        NotArranged[0].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        Arranged[index] = NotArranged[0];
        NotArranged.RemoveAt(0);
    }

    public void SkillCardSetting()
    {
        for(int i = 0; i < characters.Count; i++)
        {
            GameObject card = Instantiate(SkillCard, SkillCardReadyPos);
            card.GetComponent<UseEX>().character = characters[i];
            card.GetComponent<UseEX>().Setting();
            characters[i].EX_Card = card;
            SkillCardReady.Add(card);
        }
    }
}

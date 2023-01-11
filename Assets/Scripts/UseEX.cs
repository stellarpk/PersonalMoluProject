using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class UseEX : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image CharImage;
    public TMP_Text cost;
    public Character character;
    bool CheckAllySkill;

    public void OnEnable()
    {
        
    }

    public void Setting()
    {
        cost.text = character.s_EX.sData.SkillCost.ToString();
        CharImage.sprite = Resources.Load<Sprite>("Sprites/CharIcon/" + character.myStat.myData.IconSpriteName); 
    }

    public void Test()
    {
        if (character.s_EX.sData.SkillCost <= SkillSystem.Inst.curCost)
        {
            if (!character.Skill_Indicator.gameObject.activeSelf)
            {
                character.TurnOnIndicator();
            }
            else
            {
                character.TurnOffIndicator();
            }
        }
        else
        {
            StartCoroutine(NotEnoughCost());
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!character.Skill_Indicator.gameObject.activeSelf) character.TurnOnIndicator();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (character.UsingEX)
        {
            character.Use_EX_Skill();
        }
        character.TurnOffIndicator();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        for (int i = 0; i < SkillSystem.Inst.characters.Count; i++)
        {
            if (SkillSystem.Inst.characters[i].Casting)
            {
                SkillSystem.Inst.characters[i].UsingEX = false;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        for (int i = 0; i < SkillSystem.Inst.characters.Count; i++)
        {
            if (SkillSystem.Inst.characters[i].Casting)
            {
                SkillSystem.Inst.characters[i].UsingEX = true;
            }
        }
    }

    public IEnumerator NotEnoughCost()
    {
        GameManager.Inst.alert.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        GameManager.Inst.alert.SetActive(false);
    }
}


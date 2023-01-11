using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCard : MonoBehaviour
{
    public GameObject character;
    public GameObject CharImg;
    public TMP_Text charNameTest;
    private void Start()
    {
        charNameTest.text = character.GetComponent<Character>().myStat.myData.CharName;
        CharImg.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CharIcon/" + character.GetComponent<Character>().myStat.myData.IconSpriteName);
    }
    public void AddToFormation()
    {
        FormationSystem.Inst.FindEmptyIndex();
        if (!FormationSystem.Inst.InSetting.Contains(character))
        {
            FormationSystem.Inst.FormationCard[FormationSystem.Inst.index].charImg.SetActive(true);
            FormationSystem.Inst.FormationCard[FormationSystem.Inst.index].character = character;
            FormationSystem.Inst.FormationCard[FormationSystem.Inst.index].Icon.sprite = Resources.Load<Sprite>("Sprites/CharIcon/" + character.GetComponent<Character>().myStat.myData.IconSpriteName);
            FormationSystem.Inst.FormationCard[FormationSystem.Inst.index].test.text = charNameTest.text;
            FormationSystem.Inst.InSetting[FormationSystem.Inst.index] = character;
            FormationSystem.Inst.index++;
        }
        else
        {
            int idx = Array.FindIndex(FormationSystem.Inst.InSetting, i => i == character);
            FormationSystem.Inst.InSetting[idx] = null;
            FormationSystem.Inst.FormationCard[idx].character = null;
            FormationSystem.Inst.FormationCard[idx].charImg.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UseEX : MonoBehaviour
{
    public Image CharImage;
    public TMP_Text cost;
    public Character character;

    public void Setting()
    {
        cost.text = character.s_EX.sData.SkillCost.ToString();
    }

    public void Test()
    {
        if (!character.Skill_Indicator.gameObject.activeSelf) character.TurnOnIndicator();
        else character.TurnOffIndicator();

    }
}

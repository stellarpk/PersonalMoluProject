using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[Serializable]
public struct UpgradeData
{
    public int[] EXSkillUpgradeGold;
    public int[] SkillUpgradeGold;
    public int[] LevelUpgradeGold;

    public int GetEXSkillUpgradeGold(int lv)
    {
        if (lv < 1 || lv > EXSkillUpgradeGold.Length) return 0;
        return EXSkillUpgradeGold[lv - 1];
    }

    public int GetSkillUpgradeGold(int lv)
    {
        if (lv < 1 || lv > SkillUpgradeGold.Length) return 0;
        return SkillUpgradeGold[lv - 1];
    }

    public int GetLevelUpgradeGold(int lv)
    {
        if (lv < 1 || lv > LevelUpgradeGold.Length) return 0;
        return LevelUpgradeGold[lv - 1];
    }
}

public class UpgradeCharacter : MonoBehaviour
{
    public TMP_Text LV;
    public TMP_Text Name;
    public TMP_Text[] Stats;
    public GameObject[] Skills;
    public GameObject curCharacter;
    public UpgradeData UData;

    public void SetUI()
    {
        LV.text = curCharacter.GetComponent<Character>().myStat.myData.Level.ToString();
        Name.text = curCharacter.GetComponent<Character>().myStat.myData.CharName;
        Stats[0].text = "HP  " + curCharacter.GetComponent<Character>().myStat.myData.MaxHP.ToString();
        Stats[1].text = "ATK  " + curCharacter.GetComponent<Character>().myStat.myData.AttackDamage.ToString();
        Stats[2].text = "DEF  " + curCharacter.GetComponent<Character>().myStat.myData.DefencePower.ToString();
        Stats[3].text = "Healing  " + curCharacter.GetComponent<Character>().myStat.myData.Healing.ToString();
        Skills[0].GetComponent<SkillUI>().Setting(curCharacter.GetComponent<Character>().s_EX.sData);
        Skills[1].GetComponent<SkillUI>().Setting(curCharacter.GetComponent<Character>().s_Normal.sData);
        Skills[2].GetComponent<SkillUI>().Setting(curCharacter.GetComponent<Character>().s_Passive.sData);
        Skills[3].GetComponent<SkillUI>().Setting(curCharacter.GetComponent<Character>().s_Sub.sData);
    }

    public void UpgradeSkill()
    {

    }

    public void LevelUp()
    {
        DataManager.Inst.RInfo.Gold -= UData.GetLevelUpgradeGold(curCharacter.GetComponent<Character>().myStat.myData.Level);
        curCharacter.GetComponent<Character>().myStat.myData.Level += 1;
        DataManager.Inst.SaveCharacterData(curCharacter.GetComponent<Character>().myStat.myData);
        DataManager.Inst.GetJsonCharacterData(curCharacter.GetComponent<Character>(), curCharacter.GetComponent<Character>().myStat.myData);
        DataManager.Inst.SaveGoldData();
        DataManager.Inst.GetJsonGoldData();
    }
}

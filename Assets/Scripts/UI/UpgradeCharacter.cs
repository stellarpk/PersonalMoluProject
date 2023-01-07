using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[Serializable]
public struct UpgradeData
{
    public int[] SkillUpgradeGold;
    public int[] LevelUpgradeGold;
    public SkillLVUP[] Materials;

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

[Serializable]
public struct SkillLVUP
{
    public int ItemID;
    public int Count;
    public int Gold;
}

public enum SkillType
{
    EX, Normal, Passive, Sub
}

public enum Explain
{
    Skill_1, Skill_2, CoolTime, Duration
}

public class UpgradeCharacter : MonoBehaviour
{
    public TMP_Text LV;
    public TMP_Text Name;
    public TMP_Text[] Stats;
    public GameObject[] Skills;
    public GameObject curCharacter;
    public CharacterListCard curCard;
    public UpgradeData UData;
    public GameObject StatDetails;
    public TMP_Text[] Details = new TMP_Text[9];

    [Header("Skill UI")]
    public Image Skill_Icon;
    public TMP_Text Skill_Name;
    public TMP_Text Skill_Type;
    public TMP_Text[] Skill_Level;
    public TMP_Text CostText;
    public GameObject MaxLevel;
    public TMP_Text[] Skill_Explain;
    public TMP_Text Gold;
    public GameObject[] UpgradeSkillIcon;
    public GameObject Selected;
    public GameObject MaterialPos;
    public SkillData curSkillData;
    public SkillType curType;
    public string[] change = { "[Skill_1]", "[Skill_2]", "[CoolTime]", "[Duration]" };

    public GameObject Slot;
    public void SetUI()
    {
        DataManager.Inst.GetJsonCharacterSkillData(curCharacter.GetComponent<Character>().myStat.myData, curCharacter.GetComponent<Character>().s_EX.sData);
        DataManager.Inst.GetJsonCharacterSkillData(curCharacter.GetComponent<Character>().myStat.myData, curCharacter.GetComponent<Character>().s_Normal.sData);
        DataManager.Inst.GetJsonCharacterSkillData(curCharacter.GetComponent<Character>().myStat.myData, curCharacter.GetComponent<Character>().s_Passive.sData);
        DataManager.Inst.GetJsonCharacterSkillData(curCharacter.GetComponent<Character>().myStat.myData, curCharacter.GetComponent<Character>().s_Sub.sData);
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

    public void SetDetailsUI()
    {
        Details[0].text = "HP  " + curCharacter.GetComponent<Character>().myStat.myData.MaxHP.ToString();
        Details[1].text = "ATK  " + curCharacter.GetComponent<Character>().myStat.myData.AttackDamage.ToString();
        Details[2].text = "DEF  " + curCharacter.GetComponent<Character>().myStat.myData.DefencePower.ToString();
        Details[3].text = "Healing  " + curCharacter.GetComponent<Character>().myStat.myData.Healing.ToString();
        Details[4].text = "CritRate  " + curCharacter.GetComponent<Character>().myStat.myData.CritPer.ToString();
        Details[5].text = "CritDamage  " + curCharacter.GetComponent<Character>().myStat.myData.CritDamage.ToString();
        Details[6].text = "Range  " + curCharacter.GetComponent<Character>().myStat.myData.AttackRange.ToString();
        Details[7].text = "Stability  " + curCharacter.GetComponent<Character>().myStat.myData.Stability.ToString();
        Details[8].text = "CostRecover  " + curCharacter.GetComponent<Character>().myStat.myData.CostRecover.ToString();
    }

    public void SetUpgradeSkillUI()
    {
        UpgradeSkillIcon[0].GetComponent<SkillUI>().Setting(curCharacter.GetComponent<Character>().s_EX.sData);
        UpgradeSkillIcon[1].GetComponent<SkillUI>().Setting(curCharacter.GetComponent<Character>().s_Normal.sData);
        UpgradeSkillIcon[2].GetComponent<SkillUI>().Setting(curCharacter.GetComponent<Character>().s_Passive.sData);
        UpgradeSkillIcon[3].GetComponent<SkillUI>().Setting(curCharacter.GetComponent<Character>().s_Sub.sData);

        Character Cur = curCharacter.GetComponent<Character>();
        SkillWindow(Cur.s_EX.sData, SkillType.EX);
    }

    public void EXWindow()
    {
        SkillWindow(curCharacter.GetComponent<Character>().s_EX.sData, SkillType.EX);
    }

    public void NormalWindow()
    {
        SkillWindow(curCharacter.GetComponent<Character>().s_Normal.sData, SkillType.Normal);
    }

    public void PassiveWindow()
    {
        SkillWindow(curCharacter.GetComponent<Character>().s_Passive.sData, SkillType.Passive);
    }

    public void SubWindow()
    {
        SkillWindow(curCharacter.GetComponent<Character>().s_Sub.sData, SkillType.Sub);
    }

    public void SkillWindow(SkillData data, SkillType type)
    {
        DataManager.Inst.GetJsonSkillUPData();
        curSkillData = data;
        SkillUI Set = UpgradeSkillIcon[(int)type].GetComponent<SkillUI>();
        Selected.transform.position = UpgradeSkillIcon[(int)type].transform.position;
        Skill_Icon.sprite = Set.Skill_Icon.sprite;
        Skill_Name.text = data.SkillName;
        Skill_Type.text = $"{data.name} Skill";
        Skill_Level[0].text = $"Lv.{data.SkillLevel}";
        Skill_Level[1].text = $"Lv.{data.SkillLevel + 1}";
        if (type == SkillType.EX)
        {
            if (!CostText.gameObject.activeSelf) CostText.gameObject.SetActive(true);
            CostText.text = $"Cost: {data.SkillCost}";
        }
        else CostText.gameObject.SetActive(false);

        SkillExplain(data);
        curType = type;
        Gold.text = UData.Materials[data.SkillLevel].Gold.ToString();
        ShowMaterial(data);

        MaxLevel.SetActive(false);
    }

    public void SkillExplain(SkillData data)
    {
        string ExplainTxt1 = data.SkillExplain;
        string ExplainTxt2 = data.SkillExplain;
        if (ExplainTxt1.Contains(change[(int)Explain.Skill_1]))
        {
            ExplainTxt1 = ExplainTxt1.Replace(change[(int)Explain.Skill_1], CheckPercentage(data, data.GetPercentage(data.SkillLevel)).ToString());
            ExplainTxt2 = ExplainTxt2.Replace(change[(int)Explain.Skill_1], CheckPercentage(data, data.GetPercentage(data.SkillLevel + 1)).ToString());
        }
        if (ExplainTxt1.Contains(change[(int)Explain.Skill_2]))
        {
            ExplainTxt1 = ExplainTxt1.Replace(change[(int)Explain.Skill_2], CheckPercentage_2(data, data.GetPercentage_2(data.SkillLevel)).ToString());
            ExplainTxt2 = ExplainTxt2.Replace(change[(int)Explain.Skill_2], CheckPercentage_2(data, data.GetPercentage_2(data.SkillLevel + 1)).ToString());
        }
        if (ExplainTxt1.Contains(change[(int)Explain.CoolTime]))
        {
            ExplainTxt1 = ExplainTxt1.Replace(change[(int)Explain.CoolTime], data.CoolTime.ToString());
            ExplainTxt2 = ExplainTxt2.Replace(change[(int)Explain.CoolTime], data.CoolTime.ToString());
        }
        if (ExplainTxt1.Contains(change[(int)Explain.Duration]))
        {
            ExplainTxt1 = ExplainTxt1.Replace(change[(int)Explain.Duration], data.BuffTime.ToString());
            ExplainTxt2 = ExplainTxt2.Replace(change[(int)Explain.Duration], data.BuffTime.ToString());
        }
        Skill_Explain[0].text = ExplainTxt1;
        Skill_Explain[1].text = ExplainTxt2;
    }

    public float CheckPercentage(SkillData data, float per)
    {
        bool isDecimal = false;
        for (int i = 0; i < data.percentage.Length; i++)
        {
            float check = data.GetPercentage(i);
            if (Mathf.Round(check) != data.GetPercentage(i))
            {
                isDecimal = true;
                break;
            }
        }
        if (isDecimal)
        {
            if (data.isBuff) return (per - 1) * 100;
            else return per * 100;
        }

        return per;
    }

    public float CheckPercentage_2(SkillData data, float per)
    {
        bool isDecimal = false;
        for (int i = 0; i < data.percentage.Length; i++)
        {
            float check = data.GetPercentage_2(i);
            if (Mathf.Round(check) != data.GetPercentage_2(i))
            {
                isDecimal = true;
                break;
            }
        }
        if (isDecimal)
        {
            if (data.isBuff) return (per - 1) * 100;
            else return per * 100;
        }

        return per;
    }

    public void SkillLevelUP(SkillData data)
    {


    }

    public void ShowDetails()
    {
        SetDetailsUI();
        StatDetails.SetActive(true);
    }

    public void ShowMaterial(SkillData data)
    {
        GameObject material = Slot;
        material.GetComponent<Item>().itemValue = DataManager.Inst.GetItemByID(UData.Materials[data.SkillLevel-1].ItemID);
        material.GetComponent<Item>().SetItem(UData.Materials[data.SkillLevel-1].Count);
        material.GetComponent<Item>().Setting();
        material.GetComponent<Item>().SetUI();
        material.transform.SetParent(MaterialPos.transform);
    }

    public void UpgradeSkill()
    {
        DataManager.Inst.RInfo.Gold -= UData.Materials[curSkillData.SkillLevel].Gold;

        int index = InventoryManager.Inst.Copy.FindIndex(x => x.itemValue.ID == UData.Materials[curSkillData.SkillLevel-1].ItemID);
        InventoryManager.Inst.Copy[index].SetItem(InventoryManager.Inst.Copy[index].ItemCount - UData.Materials[curSkillData.SkillLevel-1].Count);
        InventoryManager.Inst.Copy[index].Setting();
        DataManager.Inst.SaveItemData();
        DataManager.Inst.GetJsonItemData();
        curSkillData.SkillLevel += 1;
        DataManager.Inst.SaveCharacterSkillData(curCharacter.GetComponent<Character>().myStat.myData, curSkillData);
        DataManager.Inst.GetJsonCharacterSkillData(curCharacter.GetComponent<Character>().myStat.myData, curSkillData);
        SkillWindow(curSkillData, curType);
        SetUI();
        SetUpgradeSkillUI();
        curCharacter.GetComponent<Character>().InitializeSkill();
        DataManager.Inst.SaveGoldData();
        DataManager.Inst.GetJsonGoldData();
    }

    public void LevelUp()
    {
        DataManager.Inst.RInfo.Gold -= UData.GetLevelUpgradeGold(curCharacter.GetComponent<Character>().myStat.myData.Level);
        curCharacter.GetComponent<Character>().myStat.myData.Level += 1;
        DataManager.Inst.SaveCharacterData(curCharacter.GetComponent<Character>().myStat.myData);
        DataManager.Inst.GetJsonCharacterData(curCharacter.GetComponent<Character>(), curCharacter.GetComponent<Character>().myStat.myData);
        curCard.SetUI();
        curCharacter.GetComponent<Character>().myStat.Initialize();
        DataManager.Inst.SaveGoldData();
        DataManager.Inst.GetJsonGoldData();
    }
}

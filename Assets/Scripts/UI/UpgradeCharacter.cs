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
    public int MaxLevel;
    public int MaxSkillLevel;

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
    public Button LevelUpgrade;
    public Image CharSprite;

    [Header("Skill UI")]
    public Image Skill_Icon;
    public TMP_Text Skill_Name;
    public TMP_Text Skill_Type;
    public TMP_Text[] Skill_Level;
    public TMP_Text[] CostText;
    public GameObject[] MaxLevel;
    public TMP_Text[] Skill_Explain;
    public TMP_Text Gold;
    public GameObject[] UpgradeSkillIcon;
    public GameObject Selected;
    public GameObject MaterialPos;
    public GameObject Alert;
    public Button SkillUpgradeButton;
    public SkillData curSkillData;
    public SkillType curType;
    public string[] change = { "[Skill_1]", "[Skill_2]", "[CoolTime]", "[Duration]" };
    public GameObject LevelUpEffect;
    public GameObject SkillUpEffect;

    public AudioSource audioSource;
    public AudioClip effectSound;

    bool MaterialEnough;
    bool GoldEnough;
    bool isMaxSkillLevel;

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
        CharSprite.sprite = Resources.Load<Sprite>("Sprites/CharSprite/" + curCharacter.GetComponent<Character>().myStat.myData.SpriteName);
        if (curCharacter.GetComponent<Character>().myStat.myData.Level == UData.MaxLevel)
        {
            LevelUpgrade.interactable = false;
        }
        else
        {
            LevelUpgrade.interactable = true;
        }
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

    public void SetDefaultUpgradeUI()
    {
        SetUpgradeSkillUI(curType);
    }

    public void SetUpgradeSkillUI(SkillType skillType)
    {
        UpgradeSkillIcon[0].GetComponent<SkillUI>().Setting(curCharacter.GetComponent<Character>().s_EX.sData);
        UpgradeSkillIcon[1].GetComponent<SkillUI>().Setting(curCharacter.GetComponent<Character>().s_Normal.sData);
        UpgradeSkillIcon[2].GetComponent<SkillUI>().Setting(curCharacter.GetComponent<Character>().s_Passive.sData);
        UpgradeSkillIcon[3].GetComponent<SkillUI>().Setting(curCharacter.GetComponent<Character>().s_Sub.sData);

        Character Cur = curCharacter.GetComponent<Character>();
        switch (skillType)
        {
            case SkillType.EX:
                SkillWindow(Cur.s_EX.sData, skillType);
                break;
            case SkillType.Normal:
                SkillWindow(Cur.s_Normal.sData, skillType);
                break;
            case SkillType.Passive:
                SkillWindow(Cur.s_Passive.sData, skillType);
                break;
            case SkillType.Sub:
                SkillWindow(Cur.s_Sub.sData, skillType);
                break;
            default:
                break;
        }
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


        if (type == SkillType.EX)
        {
            if (!CostText[0].gameObject.activeSelf) CostText[0].gameObject.SetActive(true);
            CostText[0].text = $"Cost: {data.SkillCost}";
            if (!CostText[1].gameObject.activeSelf) CostText[0].gameObject.SetActive(true);
            CostText[1].text = $"Cost: {data.SkillCost}";
        }
        else
        {
            CostText[0].gameObject.SetActive(false);
            CostText[1].gameObject.SetActive(false);
        }

        Skill_Level[0].text = $"Lv.{data.SkillLevel}";
        SkillExplainT(data, data.GetPercentage(data.SkillLevel), 0);
        
        if (data.SkillLevel < UData.MaxSkillLevel)
        {
            if (data.SkillLevel == (UData.MaxSkillLevel - 1))
            {
                Skill_Level[1].gameObject.SetActive(true);
                Skill_Level[1].text = $"Lv.{data.SkillLevel + 1}";
                MaxLevel[0].SetActive(false);
                MaxLevel[1].SetActive(true);
                SkillExplainT(data, data.GetPercentage(data.SkillLevel + 1), 1);
            }
            else
            {
                Skill_Level[1].gameObject.SetActive(true);
                Skill_Level[1].text = $"Lv.{data.SkillLevel + 1}";
                MaxLevel[0].SetActive(false);
                MaxLevel[1].SetActive(false);
                SkillExplainT(data, data.GetPercentage(data.SkillLevel + 1), 1);
            }
            isMaxSkillLevel = false;
        }
        else if (data.SkillLevel == UData.MaxSkillLevel)
        {
            MaxLevel[0].SetActive(true);
            MaxLevel[1].SetActive(false);
            Skill_Level[1].gameObject.SetActive(false);
            Skill_Explain[1].text = "skill level max";
            isMaxSkillLevel = true;
        }
        curType = type;
        
        
        

        if (isMaxSkillLevel)
        {
            Slot.SetActive(false);
            SkillUpgradeButton.interactable = false;
            Gold.text = "0";
        }
        else
        {
            ShowMaterial(data);
            SkillUpgradeButton.interactable = true;

            if (DataManager.Inst.RInfo.Gold >= UData.Materials[curSkillData.SkillLevel].Gold)
            {
                GoldEnough = true;
                Gold.text = $"<color=white>{UData.Materials[data.SkillLevel].Gold}</color>";
            }
            else
            {
                GoldEnough = false;
                Gold.text = $"<color=red>{UData.Materials[data.SkillLevel].Gold}</color>";
            }

            if (!GoldEnough || !MaterialEnough)
            {
                Alert.SetActive(true);
                SkillUpgradeButton.interactable = false;
            }
            else
            {
                Alert.SetActive(false);
                SkillUpgradeButton.interactable = true;
            }
        }
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
    public void SkillExplainT(SkillData data, float skillLevel, int cur)
    {
        string ExplainTxt = data.SkillExplain;
        if (ExplainTxt.Contains(change[(int)Explain.Skill_1]))
        {
            ExplainTxt = ExplainTxt.Replace(change[(int)Explain.Skill_1], CheckPercentage(data, skillLevel).ToString());
        }
        if (ExplainTxt.Contains(change[(int)Explain.Skill_2]))
        {
            ExplainTxt = ExplainTxt.Replace(change[(int)Explain.Skill_2], CheckPercentage_2(data, skillLevel).ToString());
        }
        if (ExplainTxt.Contains(change[(int)Explain.CoolTime]))
        {
            ExplainTxt = ExplainTxt.Replace(change[(int)Explain.CoolTime], data.CoolTime.ToString());
        }
        if (ExplainTxt.Contains(change[(int)Explain.Duration]))
        {
            ExplainTxt = ExplainTxt.Replace(change[(int)Explain.Duration], data.BuffTime.ToString());
        }
        Skill_Explain[cur].text = ExplainTxt;
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

    public void ShowDetails()
    {
        SetDetailsUI();
        StatDetails.SetActive(true);
    }

    public void ShowMaterial(SkillData data)
    {
        Slot.SetActive(true);
        Slot.GetComponent<Item>().itemValue = DataManager.Inst.GetItemByID(UData.Materials[data.SkillLevel - 1].ItemID);
        int invenIndex = InventoryManager.Inst.Copy.FindIndex(x => x.itemValue.ID == UData.Materials[curSkillData.SkillLevel - 1].ItemID);
        int curInven = 0;
        if (invenIndex >= 0)
        {
            curInven = InventoryManager.Inst.Copy[invenIndex].ItemCount;
        }
        

        Slot.GetComponent<Item>().SetItem(UData.Materials[data.SkillLevel - 1].Count);
        Slot.GetComponent<Item>().Setting();

        if (curInven >= Slot.GetComponent<Item>().ItemCount)
        {
            MaterialEnough = true;
            Slot.GetComponent<Item>().countText.text = $"<color=white>{curInven}</color>/{Slot.GetComponent<Item>().ItemCount}";
        }
        else
        {
            MaterialEnough = false;
            Slot.GetComponent<Item>().countText.text = $"<color=red>{curInven}</color>/{Slot.GetComponent<Item>().ItemCount}";
        }
        Slot.transform.SetParent(MaterialPos.transform);
    }

    public void UpgradeSkill()
    {
        DataManager.Inst.RInfo.Gold -= UData.Materials[curSkillData.SkillLevel].Gold;
        StartCoroutine(SUpEffect());
        int index = InventoryManager.Inst.Copy.FindIndex(x => x.itemValue.ID == UData.Materials[curSkillData.SkillLevel - 1].ItemID);
        InventoryManager.Inst.Copy[index].SetItem(InventoryManager.Inst.Copy[index].ItemCount - UData.Materials[curSkillData.SkillLevel - 1].Count);
        InventoryManager.Inst.Copy[index].Setting();
        if (InventoryManager.Inst.Copy[index].ItemCount == 0)
        {
            Destroy(InventoryManager.Inst.Copy[index]);
            InventoryManager.Inst.Copy.RemoveAt(index);
        }
        DataManager.Inst.SaveItemData();
        DataManager.Inst.GetJsonItemData();
        curSkillData.SkillLevel += 1;
        DataManager.Inst.SaveCharacterSkillData(curCharacter.GetComponent<Character>().myStat.myData, curSkillData);
        DataManager.Inst.GetJsonCharacterSkillData(curCharacter.GetComponent<Character>().myStat.myData, curSkillData);
        SkillWindow(curSkillData, curType);
        SetUI();
        SetUpgradeSkillUI(curType);
        curCharacter.GetComponent<Character>().InitializeSkill();
        DataManager.Inst.SaveGoldData();
        DataManager.Inst.GetJsonGoldData();
    }

    IEnumerator LVUPEffect()
    {
        audioSource.PlayOneShot(effectSound);
        LevelUpEffect.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1.0f);
        LevelUpEffect.GetComponent<ParticleSystem>().Stop();
    }

    IEnumerator SUpEffect()
    {
        audioSource.PlayOneShot(effectSound);
        SkillUpEffect.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1.0f);
        SkillUpEffect.GetComponent<ParticleSystem>().Stop();
    }

    public void LevelUp()
    {
        DataManager.Inst.RInfo.Gold -= UData.GetLevelUpgradeGold(curCharacter.GetComponent<Character>().myStat.myData.Level);
        StartCoroutine(LVUPEffect());
        StatIncrease(curCharacter.GetComponent<Character>().myStat.myData);
        DataManager.Inst.SaveCharacterData(curCharacter.GetComponent<Character>().myStat.myData);
        DataManager.Inst.GetJsonCharacterData(curCharacter.GetComponent<Character>(), curCharacter.GetComponent<Character>().myStat.myData);
        curCard.SetUI();
        SetUI();
        curCharacter.GetComponent<Character>().myStat.Initialize();
        DataManager.Inst.SaveGoldData();
        DataManager.Inst.GetJsonGoldData();
    }

    public void StatIncrease(CharacterData data)
    {
        data.Level += 1;
        data.MaxHP += 200;
        data.DefencePower += 5;
        data.AttackDamage += 100;
        data.Healing += 50;
    }
}

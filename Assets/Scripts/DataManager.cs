using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Globalization;
using static UnityEditor.Progress;

public class DataManager : MonoBehaviour
{
    public static DataManager Inst = null;
    public GameObject[] Formation = new GameObject[4];

    public const int MaxBossTicket = 3;
    public int CurBossTicket;

    public int TotalGold;
    public ResourceInfo RInfo;
    public SkillLVUP SLVInfo;
    private Dictionary<int, ItemValue> IID = new Dictionary<int, ItemValue>();
    public List<ItemValue> IVs;
    public List<CharacterData> CVs;
    public static string itemData;
    public static string goldData;
    public static string charData;
    public static string skillupData;
    public static string skillData;
    public List<ItemInfo> Sorting;
    public ItemValue ForGetJson;
    public CharacterData Dummy;
    public GameObject Slot;
    public GameObject curCharacter;
    public BossDifficulty difficulty;
    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }
        Inst = this;
        IVs = Resources.LoadAll<ItemValue>("SO").ToList();
        CVs = Resources.LoadAll<CharacterData>("SO").ToList();
        IID = IVs.ToDictionary(x => x.ID);
        DontDestroyOnLoad(gameObject);
    }

    public string LoadText(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    public void GetJsonItemData()
    {
        string fileName = @"Player_Item.Json";
        string filePath = Application.dataPath + "/" + fileName;
        if (File.Exists(filePath))
        {
            itemData = LoadText(filePath);
            if (itemData != string.Empty)
            {
                string[] items = itemData.Split("=");
                GetItemJson(items, InventoryManager.Inst.items);
                GetItemJson(items, InventoryManager.Inst.Copy);

                SortingItem();

                MainScene.Inst.expain.SetExplain(InventoryManager.Inst.items, 0);
                InventoryManager.Inst.SetInteractable();
                for (int i = 0; i < InventoryManager.Inst.Copy.Count; i++)
                {
                    InventoryManager.Inst.Copy[i].transform.SetParent(InventoryManager.Inst.gameObject.transform);
                }
            }

            ClearSO();
        }
    }

    public void GetJsonGoldData()
    {
        string fileName = @"Player_Resource.Json";
        string filePath = Application.dataPath + "/" + fileName;
        if (File.Exists(filePath))
        {
            goldData = LoadText(filePath);
            if (goldData != string.Empty)
            {
                RInfo = JsonUtility.FromJson<ResourceInfo>(goldData);
                MainScene.Inst.GoldText.text = InputCommaToText(RInfo.Gold).ToString();
                MainScene.Inst.BossTicket.text = RInfo.CurBossTicket.ToString() + " / " + RInfo.MaxBossTicket.ToString();
            }
        }
    }

    public void GetJsonCharacterData(Character toSave,CharacterData toGet)
    {
        string fileName = @$"{toGet.CharName}_Data.Json";
        string filePath = Application.dataPath + "/Jsons/" + fileName;
        if (File.Exists(filePath))
        {
            charData = LoadText(filePath);
            if (charData != null)
            {
                JsonUtility.FromJsonOverwrite(charData, toGet);
                toSave.myStat.myData = toGet;
                MainScene.Inst.UpChar.LV.text = toSave.myStat.myData.Level.ToString();
            }
        }
    }

    public void GetJsonSkillUPData()
    {
        string fileName = "SkillLVUP_Data.Json";
        string filePath = Application.dataPath + "/Jsons/" + fileName;
        if (File.Exists(filePath))
        {
            skillupData = LoadText(filePath);
            if (skillupData != null)
            {
                string[] data = skillupData.Split("=");
                for (int i = 0; i < data.Length; i++)
                {
                    MainScene.Inst.UpChar.UData.Materials[i] = JsonUtility.FromJson<SkillLVUP>(data[i]);
                }
            }
        }
    }

    public void GetJsonCharacterSkillData(CharacterData owner, SkillData toGet)
    {
        string fileName = $"{owner.CharName}_{toGet.name}_Data.Json";
        string filepath = Application.dataPath + $"/Jsons/{owner.CharName}/" + fileName;
        if (File.Exists(filepath))
        {
            skillData = LoadText(filepath);
            if (skillData != null)
            {
                JsonUtility.FromJsonOverwrite(skillData, toGet);
            }
        }
    }

    public ItemValue GetItemByID(int ID)
    {
        return IID[ID];
    }

    public void SetJsonCharacterData()
    {
        for (int i = 0; i < CVs.Count; i++)
        {
            string fileName = @$"{CVs[i].CharName}_Data.Json";
            string filePath = Application.dataPath + "/Jsons/" + fileName;
            if (File.Exists(filePath))
            {
                charData = LoadText(filePath);
                if (charData != null)
                {
                    JsonUtility.FromJsonOverwrite(charData, CVs[i]);
                }
            }
        }
    }

    string InputCommaToText(double Data)
    {
        return string.Format("{0:#,###}", Data);
    }

    public void SortingItem()
    {
        InventoryManager.Inst.items = InventoryManager.Inst.items.OrderBy(x => x.IInfo.ID).ToList();
        for (int i = 0; i < InventoryManager.Inst.items.Count; i++)
        {
            InventoryManager.Inst.items[i].gameObject.transform.SetSiblingIndex(i);
        }
    }

    public void GetItemJson(string[] items, List<Item> list)
    {
        for (int i = 0; i < items.Length; i++)
        {
            JsonUtility.FromJsonOverwrite(items[i], ForGetJson);
            if (ForGetJson.ItemCount != 0)
            {
                GameObject item = Instantiate(Slot);
                Item itemInfo = item.GetComponent<Item>();
                int index = IVs.FindIndex(x => x.ID == ForGetJson.ID);
                itemInfo.itemValue = IVs[index];

                itemInfo.SetItem(ForGetJson.ItemCount);
                itemInfo.Setting();
                itemInfo.SetUI();
                item.transform.SetParent(MainScene.Inst.ItemContainer);
                item.transform.localScale = new Vector3(1, 1, 1);
                list.Add(itemInfo);
            }
        }
    }

    void ClearSO()
    {
        ForGetJson.ID = 100000;
        ForGetJson.ItemCount = 0;
        ForGetJson.ItemName = string.Empty;
        ForGetJson.Droprate = 0;
    }

    public void AddItem(Item item)
    {
        bool Contain = false;
        int count = 0;
        int idx = 0;
        for (int i = 0; i < InventoryManager.Inst.Copy.Count; i++)
        {
            if (item.itemValue.ID == InventoryManager.Inst.Copy[i].ID)
            {
                Contain = true;
                count = InventoryManager.Inst.Copy[i].ItemCount + 1;
                idx = i;
            }
        }
        if (Contain)
        {
            InventoryManager.Inst.Copy[idx].ItemCount = count;
        }
        else
        {
            int index = IVs.FindIndex(x => x.ID == item.itemValue.ID);
            GameObject added = Instantiate(Slot);
            added.GetComponent<Item>().itemValue = IVs[index];
            added.GetComponent<Item>().SetItem(IVs[index].ItemCount);
            added.GetComponent<Item>().Setting();
            InventoryManager.Inst.Copy.Add(added.GetComponent<Item>());
        }
    }
    public void SaveItemData()
    {
        string item = string.Empty;
        for (int i = 0; i < InventoryManager.Inst.Copy.Count; i++)
        {
            string itemValueToJson = string.Empty;
            InventoryManager.Inst.Copy[i].SavingData();
            itemValueToJson = JsonUtility.ToJson(InventoryManager.Inst.Copy[i].IInfo, true);
            item += itemValueToJson + "=";
        }
        if (item.Length != 0)
        {
            item = item.Substring(0, item.Length - 1);
        }
        string fileName = "Player_Item";
        string path = Application.dataPath + "/" + fileName + ".Json";
        File.WriteAllText(path, item);

        InventoryManager.Inst.ClearCopy();
        InventoryManager.Inst.ClearItem();
    }

    public void SaveGoldData()
    {
        string resource = string.Empty;
        string resourceValueToJson = string.Empty;
        resourceValueToJson = JsonUtility.ToJson(RInfo, true);
        resource += resourceValueToJson;

        string fileName = "Player_Resource";
        string path = Application.dataPath + "/" + fileName + ".Json";
        File.WriteAllText(path, resource);
    }

    public void SaveCharacterData(CharacterData toSave)
    {
        string character = string.Empty;
        character = JsonUtility.ToJson(toSave, true);

        string fileName = $"{toSave.CharName}_Data";
        string path = Application.dataPath + "/Jsons/" + fileName + ".Json";
        File.WriteAllText(path, character);
    }

    public void SaveCharacterSkillData(CharacterData owner,SkillData toSave)
    {
        string skillLv = string.Empty;
        skillLv = JsonUtility.ToJson(toSave, true);

        string fileName = $"{owner.CharName}_{toSave.name}_Data";
        string path = Application.dataPath + $"/Jsons/{owner.CharName}/" + fileName + ".Json";
        File.WriteAllText(path, skillLv);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Globalization;

public class DataManager : MonoBehaviour
{
    public static DataManager Inst = null;
    public GameObject[] Formation = new GameObject[4];

    public const int MaxBossTicket = 3;
    public int CurBossTicket;

    public int TotalGold;
    public ResourceInfo RInfo;
    private Dictionary<int, ItemValue> IID = new Dictionary<int, ItemValue>();
    public List<ItemValue> IVs;
    public List<CharacterData> CVs;
    public static string itemData;
    public static string goldData;
    public static string charData;
    public ItemValue ForGetJson;
    public GameObject Slot;
    public GameObject curCharacter;
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
                toGet = JsonUtility.FromJson<CharacterData>(charData);
                toSave.myStat.myData = toGet;
            }
        }
    }

    string InputCommaToText(double Data)
    {
        return string.Format("{0:#,###}", Data);
    }

    public void GetItemJson(string[] items, List<Item> list)
    {
        for (int i = 0; i < items.Length; i++)
        {
            GameObject item = Instantiate(Slot);
            Item itemInfo = item.GetComponent<Item>();
            JsonUtility.FromJsonOverwrite(items[i], ForGetJson);
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
}

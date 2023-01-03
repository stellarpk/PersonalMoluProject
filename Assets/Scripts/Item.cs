using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public struct ItemInfo
{
    public int ID;
    public string ItemName;
    public float Droprate;
    public int ItemCount;
    public string SpriteName;
    public string IconName;
}

public class Item : UIProperty
{
    public ItemValue itemValue;
    public Image Frame;
    public Image Icon;
    public ItemInfo IInfo;
    public int ID = 0;
    public int ItemCount = 1;
    public float Droprate;
    public string ItemName = "";
    public string SpriteName;
    public string IconName;
    public TMP_Text countText;
    public void SetItem(int count)
    {
        IInfo.ID = itemValue.ID;
        IInfo.ItemName = itemValue.ItemName;
        IInfo.Droprate = itemValue.Droprate;
        IInfo.SpriteName = itemValue.SpriteName;
        IInfo.IconName = itemValue.IconName;
        Frame.sprite = Resources.Load<Sprite>("Sprites/" + itemValue.SpriteName);
        Icon.sprite = Resources.Load<Sprite>("Sprites/" + itemValue.IconName);
        IInfo.ItemCount = count;
    }

    public void Setting()
    {
        ID = IInfo.ID;
        ItemName = IInfo.ItemName;
        ItemCount = IInfo.ItemCount;
        Droprate = IInfo.Droprate;
        SpriteName = IInfo.SpriteName;
        IconName = IInfo.IconName;
    }

    public void SavingData()
    {
        IInfo.ID = ID;
        IInfo.ItemName = ItemName;
        IInfo.ItemCount = ItemCount;
        IInfo.Droprate = Droprate;
        IInfo.SpriteName = SpriteName;
        IInfo.IconName = IconName;
    }

    public void SetUI()
    {
        countText.text = "x"+ ItemCount.ToString();
    }

    public void useItem(int mount, int index)
    {
        ItemCount -= mount;
        if (ItemCount < 1)
        {
            Destroy(this.gameObject);
        }
    }
}

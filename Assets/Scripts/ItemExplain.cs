using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemExplain : MonoBehaviour
{
    public TMP_Text ItemName;
    public Item curExplain;
    public TMP_Text ExplainText;

    public void SetExplain(List<Item> list, int index)
    {
        curExplain.itemValue = list[index].itemValue;
        ItemName.text = list[index].ItemName;
        ExplainText.text = list[index].itemValue.Explain;
        
        curExplain.countText.gameObject.SetActive(false);
        curExplain.Frame.sprite = Resources.Load<Sprite>("Sprites/" + curExplain.itemValue.SpriteName);
        curExplain.Icon.sprite = Resources.Load<Sprite>("Sprites/" + curExplain.itemValue.IconName);
    }
}

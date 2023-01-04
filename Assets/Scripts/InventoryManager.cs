using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Inst = null;
    public List<Item> items = new List<Item>();

    public List<Item> Copy;
    public GameObject slot;
    public Transform itemContainer;
    public int Gold;
    public TMP_Text GoldText;
    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }
        Inst = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        
    }

    public void ClearCopy()
    {
        for (int i = 0; i < Copy.Count; i++)
        {
            Destroy(Copy[i].gameObject);
        }
        Copy.Clear();
    }

    public void ChangeGoldText()
    {
        GoldText.text = Gold.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BossHpBar : MonoBehaviour
{
    public Slider MainBar;
    public Slider BackBar;
    public TMP_Text CurHP;
    public Boss boss;
    public bool backHP = false;
    
    private void Start()
    {

    }

    private void Update()
    {
        MainBar.value = boss.bossInfo.CurHP;
        CurHP.text = ((int)boss.bossInfo.CurHP).ToString() + "/" + ((int)boss.bossInfo.MaxHP).ToString();
        if (backHP)
        {
            BackBar.value = boss.bossInfo.CurHP;
            if (Mathf.Approximately(MainBar.value, BackBar.value))
            {
                backHP = false;
                BackBar.value = MainBar.value;
            }
        }
    }

    public void Setting()
    {
        MainBar.maxValue = boss.bossInfo.MaxHP;
        BackBar.maxValue = boss.bossInfo.MaxHP;
    }

    public void Damaged()
    {
        Invoke("BackDecrease", 0.5f);
    }

    void BackDecrease()
    {
        backHP = true;
    }
}

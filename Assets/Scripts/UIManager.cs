using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header ("Cost")]
    public Slider CostSlider;
    public TMP_Text CostValue;
    public Button DoubleSpeed;
    public SkillSystem SKill_Sys;


    void SetSKillUI()
    {
        CostValue.text = SKill_Sys.usableCost.ToString();
        CostSlider.maxValue = SKill_Sys.max;
        CostSlider.value = SKill_Sys.curCost;
    }

    private void Update()
    {
        SetSKillUI();
    }
}

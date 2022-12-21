using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Cost")]
    public Slider CostSlider;
    public TMP_Text CostValue;
    public Button DoubleSpeed;
    public SkillSystem SKill_Sys;
    public GameObject[] doubleImage = new GameObject[3];
    public Sprite[] doubleSprite = new Sprite[3];
    int speedMag = 0;
    float[] timeSpeed = { 1.0f, 1.5f, 2.0f };
    void SetSKillUI()
    {
        CostValue.text = SKill_Sys.usableCost.ToString();
        CostSlider.maxValue = SKill_Sys.max;
        CostSlider.value = SKill_Sys.curCost;
    }

    private void Start()
    {
        SetGameSpeed();
    }

    private void Update()
    {
        SetSKillUI();
    }

    public void AddGameSpeed()
    {
        speedMag++;
        SetGameSpeed();
    }

    public void SetGameSpeed()
    {
        if (speedMag > timeSpeed.Length-1)
        {
            speedMag = 0;
        }
        switch (speedMag)
        {
            case 0:
                doubleImage[0].SetActive(true);
                doubleImage[0].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                if (doubleImage[1].activeSelf) doubleImage[1].SetActive(false);
                if (doubleImage[2].activeSelf) doubleImage[2].SetActive(false);
                break;
            case 1:
                Vector2[] ImagePos_2 = { new Vector2(-18f, 0), new Vector2(18f, 0) };
                for (int i = 0; i < 2; i++)
                {
                    if (!doubleImage[i].activeSelf) doubleImage[i].SetActive(true);
                    doubleImage[i].GetComponent<RectTransform>().anchoredPosition = ImagePos_2[i];
                }
                break;
            case 2:
                Vector2[] ImagePos_3 = { new Vector2(-33f, 0), new Vector2(0, 0), new Vector2(33f, 0) };
                for (int i = 0; i < 3; i++)
                {
                    if (!doubleImage[i].activeSelf) doubleImage[i].SetActive(true);
                    doubleImage[i].GetComponent<RectTransform>().anchoredPosition = ImagePos_3[i];
                }
                break;
            default:
                break;
        }
        DoubleSpeed.GetComponent<Image>().sprite = doubleSprite[speedMag];
        Time.timeScale = timeSpeed[speedMag];
    }

    public void ChangeSpeedWhenUseSkill()
    {

    }
}

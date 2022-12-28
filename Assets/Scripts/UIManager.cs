using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Inst = null;
    [Header("Cost")]
    public Slider CostSlider;
    public TMP_Text CostValue;
    public Button DoubleSpeed;
    public SkillSystem SKill_Sys;
    public GameObject[] doubleImage = new GameObject[3];
    public Sprite[] doubleSprite = new Sprite[3];
    public Transform Hpbar;
    public TMP_Text time;
    public Transform DmgText;
    public GameObject Over;
    public GameObject Clear;
    int speedMag = 0;
    bool pause;
    float[] timeSpeed = { 1.0f, 1.5f, 2.0f };

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }
        Inst = this;
    }

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
        Timer();
    }

    public void AddGameSpeed()
    {
        speedMag++;
        SetGameSpeed();
    }

    void UpdateTime(float value)
    {
        GameManager.Inst.playTime = Mathf.Clamp(GameManager.Inst.playTime - value, 0.0f, GameManager.Inst.maxPlayTime);
    }

    void Timer()
    {
        UpdateTime(Time.deltaTime);
        int Min = (int)(GameManager.Inst.playTime / 60);
        float sec = GameManager.Inst.playTime % 60;
        time.text = string.Format("{0:D2}:{1:D2}", Min, (int)sec);
    }

    public void PauseGame()
    {
        if (!pause)
        {
            Time.timeScale = 0;
            pause = true;
        }
        else
        {
            Time.timeScale = timeSpeed[speedMag];
            pause = false;
        }

    }

    public void SetGameSpeed()
    {
        if (speedMag > timeSpeed.Length - 1)
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

    public void GameOver()
    {
        Over.SetActive(true);
    }

    public void GameClear()
    {
        Clear.SetActive(true);
    }

    public void RestartGame()
    {
        MoveScene.Inst.Move(1);
    }

    public void ExitGame()
    {
        MoveScene.Inst.Move(0);
    }

    public void ChangeSpeedWhenUseSkill()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterListCard : MonoBehaviour
{
    public GameObject myCharacter;
    public Image CharImg;
    public TMP_Text Level;
    public TMP_Text Name;

    private void Start()
    {
        SetUI();
    }

    public void SetUI()
    {
        Level.text = "LV."+ myCharacter.GetComponent<Character>().myStat.myData.Level.ToString();
        Name.text = myCharacter.GetComponent<Character>().myStat.myData.CharName;
    }

    public void UpgradingCharacter()
    {
        DataManager.Inst.curCharacter = myCharacter;
        MainScene.Inst.UpChar.curCard = this;
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FormationCard : MonoBehaviour
{
    public GameObject charImg;
    public GameObject character;
    public TMP_Text test;

    public void UnorganizeCharacter()
    {
        int idx = Array.FindIndex(FormationSystem.Inst.InSetting, i => i == character);
        FormationSystem.Inst.InSetting[idx] = null;
        character = null;
        charImg.SetActive(false);
    }

}

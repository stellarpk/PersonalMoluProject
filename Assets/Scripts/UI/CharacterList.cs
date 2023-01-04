using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterList : MonoBehaviour
{
    public Transform container;
    public GameObject UpgradeUI;

    public void SetUpgradeUI()
    {
        UpgradeUI.GetComponent<UpgradeCharacter>().curCharacter = DataManager.Inst.curCharacter;
        UpgradeUI.GetComponent<UpgradeCharacter>().SetUI();
        UpgradeUI.SetActive(true);
    }
}

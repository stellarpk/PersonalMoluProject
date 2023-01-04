using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
public class FormationSystem : MonoBehaviour
{
    public static FormationSystem Inst = null;
    public FormationCard[] FormationCard;
    public GameObject[] InSetting;
    public GameObject[] InMain;
    public GameObject[] FormationBtn;
    public Transform[] ShowFormation;
    public GameObject[] CharArrangedPanel;
    public TMP_Text[] CharacterName;
    public int index = 0;
    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }
        Inst = this;
    }

    public void DecisionFormation()
    {
        InMain = InSetting.Clone() as GameObject[];
        for (int i = 0; i < InMain.Length; i++)
        {
            if (ShowFormation[i].childCount > 0)
            {
                Destroy(ShowFormation[i].GetChild(0).gameObject);
            }
            if (InMain[i] != null)
            {
                GameObject form = Instantiate(FormationCard[i].character, ShowFormation[i]);
                CharArrangedPanel[i].SetActive(true);
                form.GetComponent<Character>().myStat.Initialize();
                form.GetComponent<Character>().myStat.SetHP();
                CharacterName[i].text = form.GetComponent<Character>().myStat.myData.CharName;
                FormationBtn[i].SetActive(false);
            }
            else
            {
                FormationBtn[i].SetActive(true);
                CharArrangedPanel[i].SetActive(false);
            }
        }
    }

    public void CancelFormation()
    {
        InSetting = InMain.Clone() as GameObject[];
        for (int i = 0; i < InSetting.Length; i++)
        {
            if (InSetting[i] != null) FormationCard[i].charImg.SetActive(true);
            else FormationCard[i].charImg.SetActive(false);
        }
    }

    public void FindEmptyIndex()
    {
        for (int i = 0; i < InSetting.Length; i++)
        {
            if (InSetting[i] == null)
            {
                index = i;
                break;
            }
        }
    }

    public void ConveyFormation()
    {
        DataManager.Inst.Formation = InMain.Clone() as GameObject[];
        DataManager.Inst.RInfo.CurBossTicket--;
        MoveScene.Inst.Move(2);
    }

    public void BackToMain()
    {
        MoveScene.Inst.Move(0);
    }
}

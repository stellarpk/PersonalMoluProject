using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FormationSystem : MonoBehaviour
{
    public static FormationSystem Inst = null;
    public FormationCard[] FormationCard;
    public GameObject[] InSetting;
    public GameObject[] InMain;
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
        MoveScene.Inst.Move(1);
    }
}

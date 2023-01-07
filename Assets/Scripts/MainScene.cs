using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MainScene : MonoBehaviour
{
    public static MainScene Inst = null;
    public UpgradeCharacter UpChar;
    public Transform ItemContainer;
    public TMP_Text GoldText;
    public TMP_Text BossTicket;

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }
        Inst = this;
    }

    private void Start()
    {
        DataManager.Inst.SetJsonCharacterData();
    }

    public void MoveToBoss()
    {
        if (DataManager.Inst.RInfo.CurBossTicket > 0)
        {
            MoveScene.Inst.Move(1);
        }
        else Debug.Log("보스 입장에 필요한 티켓이 없습니다.");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MainScene : MonoBehaviour
{
    public static MainScene Inst = null;
    public Transform ItemContainer;
    public TMP_Text GoldText;

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }
        Inst = this;
    }

    public void MoveToBoss()
    {
        if (DataManager.Inst.CurBossTicket > 0)
        {
            DataManager.Inst.CurBossTicket--;
            MoveScene.Inst.Move(1);
        }
        else Debug.Log("보스 입장에 필요한 티켓이 없습니다.");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MainScene : MonoBehaviour
{
    public static MainScene Inst = null;
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

    public void MoveToBoss()
    {
        if (DataManager.Inst.RInfo.CurBossTicket > 0)
        {
            MoveScene.Inst.Move(1);
        }
        else Debug.Log("���� ���忡 �ʿ��� Ƽ���� �����ϴ�.");
    }
}

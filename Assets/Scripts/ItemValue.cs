using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemValue", menuName = "Sciptable Object/Item Data", order = -4)]
public class ItemValue : ScriptableObject
{
    [SerializeField]
    public int ID; // ������ ID
    [SerializeField]
    public string ItemName; // ������ �̸�
    [SerializeField]
    public float Droprate; // �����
    [SerializeField]
    public int ItemCount; // ������ ĭ�� ���� ���� <- ������ �����
    [SerializeField]
    public string SpriteName;
    [SerializeField]
    public string IconName;
}

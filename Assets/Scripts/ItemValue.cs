using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemValue", menuName = "Sciptable Object/Item Data", order = -4)]
public class ItemValue : ScriptableObject
{
    [SerializeField]
    public int ID; // 아이템 ID
    [SerializeField]
    public string ItemName; // 아이템 이름
    [SerializeField]
    public float Droprate; // 드롭율
    [SerializeField]
    public int ItemCount; // 아이템 칸당 현재 개수 <- 데이터 저장용
    [SerializeField]
    public string SpriteName;
    [SerializeField]
    public string IconName;
}

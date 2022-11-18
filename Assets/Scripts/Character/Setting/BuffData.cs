using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff Data", menuName = "Sciptable Object/Buff Data", order = -2)]
public class BuffData : ScriptableObject
{
    [SerializeField] float buffTime;
    [SerializeField] bool buffType;

    
    public float BuffTime { get { return buffTime; } set { buffTime = value; } }
    public bool BuffType { get { return buffType; } set { buffType = value; } }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Data", menuName = "Sciptable Object/Boss Data", order = -1)]
public class BossData : ScriptableObject
{
    [SerializeField] float stunGauge;
    [SerializeField] float timeLimit;

    public float StunGauge { get { return stunGauge; } set { stunGauge = value; } }
    public float TimeLimit { get { return timeLimit; } set { timeLimit = value; } }
}

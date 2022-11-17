using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStat : MonoBehaviour
{
    [field:SerializeField] public float MaxHP { get;  set; }
    [field: SerializeField] public float CurHP { get;  set; }
    [field: SerializeField] public float MoveSpeed { get;  set; }
    [field: SerializeField] public float AttackRange { get;  set; }
    [field: SerializeField] public float AttackSpeed { get;  set; }
    private void Start()
    {
        
    }

    public void Initialize()
    {
        CurHP = MaxHP;
    }

    public void UpdateHP(float value)
    {
        CurHP = Mathf.Clamp(CurHP + value, 0.0f, MaxHP);
    }
}

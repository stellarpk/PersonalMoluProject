using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStat : MonoBehaviour
{
    [field:SerializeField] public float MaxHP { get; private set; }
    [field: SerializeField] public float CurHP { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public float AttackSpeed { get; private set; }
    private void Start()
    {
        CurHP = MaxHP;
    }

    public void UpdateHP(float value)
    {
        CurHP = Mathf.Clamp(CurHP + value, 0.0f, MaxHP);
    }
}

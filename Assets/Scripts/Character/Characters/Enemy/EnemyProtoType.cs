using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProtoType : Character
{
    private void Start()
    {
        myStat.Initialize();
        myStat.SetHP();
        fire = () => Shooting();
        scanner.FindTarget += () => { if (Changable()) ChangeState(STATE.Battle); };
        scanner.LostTarget += () => { if (Changable()) ChangeState(STATE.Normal); };
        scanner.Range.radius = myStat.AttackRange / 10.0f;
        ChangeState(STATE.Normal);
    }

    private void Update()
    {
        StateProcess();
    }
}

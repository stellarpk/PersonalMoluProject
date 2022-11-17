using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacter : Skill
{
    public Character myChar;
    public override void EX_Skill()
    {
        
    }

    public override void Normal_Skill()
    {
        
    }

    public override void Passive_Skill()
    {
        myChar.myStat.MaxHP *= 1.2f;
    }

    public override void Sub_Skill()
    {
        
    }

    public override void SkillProcess()
    {
        
    }
}

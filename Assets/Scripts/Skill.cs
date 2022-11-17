using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class Skill : MonoBehaviour
{
    public abstract void EX_Skill();
    public abstract void Normal_Skill();
    public abstract void Passive_Skill();
    public abstract void Sub_Skill();

    public abstract void SkillProcess();

}

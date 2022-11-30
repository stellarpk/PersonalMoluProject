using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    public UnityEvent Shoot;
    public UnityEvent EXSkillAnim;
    public UnityEvent NormalSkillAnim;
    public UnityEvent ActiveNormalSkill;
    public UnityEvent Instantiating;
    public UnityEvent Reload;

    public void EndEXSkillAnim()
    {
        EXSkillAnim?.Invoke();
    }
    
    public void ActionNormalSkill()
    {
        ActiveNormalSkill?.Invoke();
    }

    public void EndNormalAnim()
    {
        NormalSkillAnim?.Invoke();
    }

    public void InstantiateObject()
    {
        Instantiating?.Invoke();
    }

    public void OnShoot()
    {
        Shoot?.Invoke();
    }

    public void EndReload()
    {
        Reload?.Invoke();
    }
}

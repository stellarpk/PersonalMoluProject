using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    public UnityEvent Shoot;
    public UnityEvent EXSkillAnim;
    public UnityEvent NormalSkillAnim;
    public UnityEvent Reload;

    public void EndEXSkillAnim()
    {
        EXSkillAnim?.Invoke();
    }
    
    public void EndNormalAnim()
    {
        NormalSkillAnim?.Invoke();
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

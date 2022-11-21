using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    public UnityEvent Shoot;
    public UnityEvent EXSkillAnim;
    public UnityEvent NormalSkillAnim;
    public UnityEvent SubSkillAnim;

    
    public void EndNormalAnim()
    {
        NormalSkillAnim?.Invoke();
    }

    public void EndSubAnim()
    {
        SubSkillAnim?.Invoke(); 
    }

    public void OnShoot()
    {
        Shoot?.Invoke();
    }
}

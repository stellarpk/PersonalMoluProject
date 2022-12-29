using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HpBar : MonoBehaviour
{
    public Transform myTarget;
    public Slider slider;
    public bool available = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(myTarget.position);
        if (available) slider.value = myTarget.GetComponentInParent<Character>().myStat.CurHP / myTarget.GetComponentInParent<Character>().myStat.MaxHP;
        transform.position = pos;
    }
}

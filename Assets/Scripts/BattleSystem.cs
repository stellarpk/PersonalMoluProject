using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattle
{
    void OnDamage(float damage);
    Transform transform { get; }
    bool IsLive
    {
        get;
    }
}

public class BattleSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

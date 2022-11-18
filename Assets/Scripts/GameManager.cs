using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst = null;

    public float playTime = 3.0f;

    private void Awake()
    {
        if( Inst != null)
        {
            Destroy(gameObject);
            return;
        }
        Inst = this;
    }
}

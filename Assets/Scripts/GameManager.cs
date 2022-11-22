using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst = null;

    public float playTime = 180.0f;
    public Transform[] Path;
    public int Wave;
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

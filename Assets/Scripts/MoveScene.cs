using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour
{
    public static MoveScene Inst = null;
    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }
        Inst = this;
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void Move(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InventoryManager.Inst.items.Clear();
        if (scene.name == "MainScene")
        {
            InventoryManager.Inst.ClearCopy();

            DataManager.Inst.GetJsonItemData();
            DataManager.Inst.GetJsonGoldData();
        }
        if(scene.buildIndex == 2)
        {
            GameManager.Inst.InGameSetting(DataManager.Inst.Formation);
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

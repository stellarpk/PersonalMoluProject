using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst = null;

    public Projector SkillIndicator;
    public GameObject[] InGameCharacters = new GameObject[4];
    public Transform[] CharacterFormation = new Transform[4];

    public float playTime = 180.0f;
    public Transform[] Path;
    public Transform TargetPos;

    public List<Transform> EnemyPos;
    public GameObject[] TestEnemy;
    public Transform[] EnemySpawnPos;
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

    public void InGameSetting(GameObject[] Characters)
    {
        for (int i = 0; i < Characters.Length; i++)
        {
            if(Characters[i] != null)
            {
                InGameCharacters[i] = Instantiate(Characters[i], CharacterFormation[i]);
                InGameCharacters[i].GetComponent<Character>().Setting();
                InGameCharacters[i].GetComponent<Character>().targetPos = TargetPos;
                InGameCharacters[i].GetComponent<Character>().Skill_Indicator = SkillIndicator;
                InGameCharacters[i].GetComponent<Character>().StartSkillCool();
                SkillSystem.Inst.characters.Add(InGameCharacters[i].GetComponent<Character>());
            }
        }
        for (int i = 0; i < TestEnemy.Length; i++)
        {
            GameObject enemyTest = Instantiate(TestEnemy[i], EnemySpawnPos[i]);
            EnemyPos.Add(enemyTest.transform);
        }
        SkillSystem.Inst.SkillCardSetting();
        SkillSystem.Inst.ArrangeSkillCard();
    }
}

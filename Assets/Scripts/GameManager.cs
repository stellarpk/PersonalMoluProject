using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst = null;

    public Projector SkillIndicator;
    public GameObject[] InGameCharacters = new GameObject[4];
    public Transform[] CharacterFormation = new Transform[4];

    public float playTime;
    public float maxPlayTime;
    public Transform[] Path;
    public Transform TargetPos;

    public List<Transform> EnemyPos;
    public Transform[] EnemySpawnPos;

    public GameObject BossCharacter;
    public Transform BossSpawn;

    public bool GameOver;

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
        GameObject boss = Instantiate(BossCharacter, BossSpawn.position, BossSpawn.rotation);
        boss.transform.rotation = Quaternion.Euler(0, 180, 0);
        boss.GetComponent<Boss>().Setting();
        boss.GetComponent<Boss>().bossInfo.SetHP();
        playTime = boss.GetComponent<Boss>().bossInfo.TimeLimit;
        maxPlayTime = boss.GetComponent<Boss>().bossInfo.TimeLimit;
        SkillSystem.Inst.SkillCardSetting();
        SkillSystem.Inst.ArrangeSkillCard();
    }

    public void CheckCharacterDead()
    {
        int count = 0;
        for (int i = 0; i < InGameCharacters.Length; i++)
        {
            if (InGameCharacters[i] == null)
            {
                count++;
            }
        }
        if (count == InGameCharacters.Length) GameOver = true;
    }

    private void Update()
    {
        if (GameOver || playTime <= 0) Debug.Log("Game Over");
    }
}

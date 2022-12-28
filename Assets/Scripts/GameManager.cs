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
    GameObject curBoss;
    public Transform BossSpawn;

    public int CharacterDeathCount;

    public bool GameOver;
    public bool GameClear;
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
        curBoss = boss;
        playTime = boss.GetComponent<Boss>().bossInfo.TimeLimit;
        maxPlayTime = boss.GetComponent<Boss>().bossInfo.TimeLimit;
        SkillSystem.Inst.SkillCardSetting();
        SkillSystem.Inst.ArrangeSkillCard();
    }

    public void CheckCharacterDead()
    {
        if (CharacterDeathCount == InGameCharacters.Length || playTime <= 0) GameOver = true;
    }

    public void CheckBossDead()
    {
        if (!curBoss.GetComponent<Boss>().IsLive && CharacterDeathCount != InGameCharacters.Length && playTime>0) GameClear = true;
    }

    private void Update()
    {
        CheckCharacterDead();
        if (GameClear)
        {
            for (int i = 0; i < InGameCharacters.Length; i++)
            {
                //InGameCharacters[i].GetComponent<Character>().scanner.OnLostTarget();
                InGameCharacters[i].GetComponent<Character>().ChangeState(Character.STATE.Wait);
                InGameCharacters[i].GetComponent<Character>().EndCoroutine();
            }
            UIManager.Inst.GameClear();
        }
        if (GameOver) UIManager.Inst.GameOver();
    }
}

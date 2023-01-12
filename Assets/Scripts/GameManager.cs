using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class RewardPerDiff
{
    public Item[] Rewards;
}

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
    public Transform CamPos;
    public int CharacterDeathCount;

    public GameObject alert;

    public TMPro.TMP_Text diff;

    public bool GameEnd;
    public bool GameOver;
    public bool GameClear;

    public int[] RewardGold;
    public RewardPerDiff[] Items;
    //public List<Item> RewardItem = new List<Item>();
    public GameObject GoldGO;
    public Transform RewardPos;

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }
        Inst = this;
    }

    public void Start()
    {
        StartCoroutine(CheckGameSet());
    }

    public void InGameSetting(GameObject[] Characters)
    {
        for (int i = 0; i < Characters.Length; i++)
        {
            if (Characters[i] != null)
            {
                InGameCharacters[i] = Instantiate(Characters[i], CharacterFormation[i]);
                InGameCharacters[i].GetComponent<Character>().Setting();
                InGameCharacters[i].GetComponent<Character>().targetPos = TargetPos;
                InGameCharacters[i].GetComponent<Character>().Skill_Indicator = SkillIndicator;
                InGameCharacters[i].GetComponent<Character>().StartSkillCool();
                InGameCharacters[i].AddComponent<NavMeshAgent>();
                SkillSystem.Inst.characters.Add(InGameCharacters[i].GetComponent<Character>());
            }
        }
        diff.text = DataManager.Inst.difficulty.ToString();
        GameObject boss = Instantiate(BossCharacter, BossSpawn.position, BossSpawn.rotation);
        boss.transform.rotation = Quaternion.Euler(0, 180, 0);
        boss.GetComponent<Boss>().Setting();
        boss.GetComponent<Boss>().bossInfo.SetHP();
        UIManager.Inst.BossIcon.sprite = Resources.Load<Sprite>("Sprites/Boss/" + boss.GetComponent<Boss>().bossInfo.bossData.BossIcon);
        UIManager.Inst.BHB.boss = boss.GetComponent<Boss>();
        UIManager.Inst.BHB.Setting();
        boss.GetComponent<Boss>().hpbar = UIManager.Inst.BHB;
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
        if (!curBoss.GetComponent<Boss>().IsLive && CharacterDeathCount != InGameCharacters.Length && playTime > 0) GameClear = true;
    }

    public IEnumerator GameCleared()
    {
        yield return new WaitForSeconds(2f);
        GetReward(RewardGold[(int)DataManager.Inst.difficulty],Items[(int)DataManager.Inst.difficulty].Rewards);
        UIManager.Inst.GameEnd(UIManager.Inst.Clear);
    }

    public void UpdateCameraPos()
    {
        List<float> Zpos = new List<float>();
        for (int i = 0; i < InGameCharacters.Length; i++)
        {
            if (InGameCharacters[i] != null)
            {
                Zpos.Add(InGameCharacters[i].transform.position.z);
            }
        }
        float MaxZ = Mathf.Max(Zpos.ToArray());
        float MinZ = Mathf.Min(Zpos.ToArray());

        Vector3 camPos = new Vector3(0, 0, (MaxZ + MinZ) / 2);
        CamPos.position = camPos;
    }

    public void GetReward(int Gold, Item[] rewardItems)
    {
        for (int i = 0; i < rewardItems.Length; i++)
        {
            int DropRate = Random.Range(0, 100);
            if (rewardItems[i].itemValue.Droprate >= DropRate)
            {
                DataManager.Inst.AddItem(rewardItems[i]);
                GameObject showReward = Instantiate(rewardItems[i].gameObject);
                showReward.transform.SetParent(RewardPos);
                showReward.GetComponent<Item>().SetItem(rewardItems[i].itemValue.ItemCount);
                showReward.GetComponent<Item>().Setting();
                showReward.GetComponent<Item>().SetUI();

            }
        }
        DataManager.Inst.SaveItemData();
        DataManager.Inst.RInfo.Gold += Gold;
        GameObject goldIcon = Instantiate(GoldGO);
        goldIcon.transform.SetParent(RewardPos);
        goldIcon.GetComponent<Item>().countText.text = Gold.ToString();
        DataManager.Inst.SaveGoldData();
    }

    public IEnumerator GameSet()
    {
        yield return new WaitForSeconds(2f);
        UIManager.Inst.GameEnd(UIManager.Inst.Over);
    }

    public IEnumerator CheckGameSet()
    {
        while (!GameClear && !GameOver)
        {
            CheckCharacterDead();
            UpdateCameraPos();
            yield return null;
        }
        if (GameClear)
        {
            for (int i = 0; i < InGameCharacters.Length; i++)
            {
                if (InGameCharacters[i] != null)
                {
                    InGameCharacters[i].GetComponent<Character>().ChangeState(Character.STATE.Wait);
                    InGameCharacters[i].GetComponent<Character>().EndCoroutine();
                }
            }
            StartCoroutine(GameCleared());
        }
        if (GameOver) StartCoroutine(GameSet());
    }

    private void Update()
    {
    }
}

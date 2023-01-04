using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class SkillUI : MonoBehaviour
{
    public TMP_Text Skill_LV;
    public Image Skill_Icon;

    public void Setting(SkillData data)
    {
        Skill_Icon.sprite = Resources.Load<Sprite>("Sprites/" + data.SkillIcon);
        Skill_LV.text = data.SkillLevel.ToString();
    }
}

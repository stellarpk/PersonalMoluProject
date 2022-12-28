using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamageText : MonoBehaviour
{
    private float moveSpeed;
    private float alphaSpeed;
    private float destroyTime;
    TMP_Text dmgText;
    public RectTransform rect;
    public GameObject critImg;
    public Transform myTarget;
    Color alpha;
    public int damage;
    bool isCritical;

    private void Start()
    {
        moveSpeed = 2.0f;
        alphaSpeed = 2.0f;
        destroyTime = 2.0f;
        dmgText = GetComponentInChildren<TMP_Text>();
        alpha = dmgText.color;
        dmgText.text = damage.ToString();
        if (isCritical) critImg.SetActive(true);
        var rectSize = rect.sizeDelta;
        rectSize.x = dmgText.preferredWidth;
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(myTarget.position);
        transform.position = pos;
        transform.Translate(pos*moveSpeed*Time.deltaTime);
        //transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        dmgText.color = alpha;
    }
}

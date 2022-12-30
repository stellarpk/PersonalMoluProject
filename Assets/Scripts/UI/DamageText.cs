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
        moveSpeed = 150.0f;
        alphaSpeed = 2.0f;
        destroyTime = 1.0f;
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
        
    }

    public void OnAction()
    {
        StartCoroutine(Actioning(Camera.main.WorldToScreenPoint(myTarget.position)));
    }

    IEnumerator Actioning(Vector3 pos)
    {
        transform.position = pos;

        yield return new WaitForSeconds(0.25f);
        float t = 1.0f;
        while(t > 0.0f)
        {
            t -= Time.deltaTime;
            transform.Translate(Vector3.up*moveSpeed * Time.deltaTime);
            alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
            dmgText.color = alpha;
            yield return null;
        }
    }
}

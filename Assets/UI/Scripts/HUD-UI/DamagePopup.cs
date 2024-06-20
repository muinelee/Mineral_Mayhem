using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private TextMeshProUGUI text;

    [SerializeField] Color damageColor;
    [SerializeField] Color healColor;

    [Header("Lerp Settings")]
    [SerializeField] private float lerpDuration = 0.7f;
    [SerializeField] float moveYSpeed = 3f;
    private float startAlpha = 1f;
    private float endAlpha = 0f;
    private float curAlpha;
    [SerializeField] float startScale = 1.5f;
    [SerializeField] float endScale = 1f;
    private float curScale;

    //----------------------------------//

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        text = GetComponentInChildren<TextMeshProUGUI>();

        canvas.worldCamera = Camera.main;
    }
    private void Start()
    {
        StartCoroutine(iDisappear());
    }

    public static DamagePopup Create(Transform tr, float damage)
    {
        Transform damageTransform = Instantiate(PrefabManager.instance.P_DamagePopup, tr.position, tr.rotation);

        DamagePopup damagePopup = damageTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damage);

        return damagePopup;
    }

    public void Setup(float damage)
    {
        if (damage > 0)
        {
            text.text = "-" + Mathf.Abs(damage).ToString("0");
            text.color = damageColor;
        }
        else if (damage < 0)
        {
            text.text = "+" + Mathf.Abs(damage).ToString("0");
            text.color = healColor;
        }
    }

    private IEnumerator iDisappear()
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {

            curAlpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / lerpDuration);
            canvasGroup.alpha = curAlpha;
            curScale = Mathf.Lerp(startScale, endScale, timeElapsed / lerpDuration);
            transform.localScale = new Vector3(curScale, curScale, curScale);
            transform.position += new Vector3(moveYSpeed / 4, moveYSpeed) * Time.deltaTime;

            timeElapsed += Time.unscaledDeltaTime;

            yield return new WaitForSecondsRealtime(0.005f);
        }
        curAlpha = endAlpha;
        canvasGroup.alpha = curAlpha;
        curScale = endScale;
        transform.localScale = new Vector3(curScale, curScale, curScale);

        Destroy(gameObject);
    }
}

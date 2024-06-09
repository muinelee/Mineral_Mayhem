using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoundUI : MonoBehaviour
{
    public static RoundUI instance;

    int blueWins = 0;
    int redWins = 0;

    [Header("UI Element References")]
    [SerializeField] Image gemLeft1;
    [SerializeField] Image gemLeftOutline1;
    [SerializeField] Image gemLeft2;
    [SerializeField] Image gemLeftOutline2;
    [SerializeField] Image gemRight1;
    [SerializeField] Image gemRightOutline1;
    [SerializeField] Image gemRight2;
    [SerializeField] Image gemRightOutline2;

    [Header("Asset References")]
    [SerializeField] Sprite gemGrey;
    [SerializeField] Sprite gemGreyOutline;
    [SerializeField] Sprite gemBlue;
    [SerializeField] Sprite gemBlueOutline;
    [SerializeField] Sprite gemRed;
    [SerializeField] Sprite gemRedOutline;

    [SerializeField] CG_Fade roundUIBar;

    [Header("Lerp Settings Expand/Collapse")]
    [SerializeField] private float lerpDuration = 0.5f;
    [SerializeField] private float startSizeX = 0;
    [SerializeField] private float endSizeX = 1f;

    private float curValue;

    [Header("Lerp Settings Grow/Shrink")]
    [SerializeField] private float lerpDuration2 = 0.5f;
    [SerializeField] private float startScaleX = 1;
    [SerializeField] private float endScaleX = 0.55f;

    private float curValue2;

    //------------------------------//

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void BlueWin()
    {
        if (blueWins == 0)
        {
            blueWins = 1;

            gemLeft1.sprite = gemBlue;
            gemLeftOutline1.sprite = gemBlueOutline;
        }
        else if (blueWins == 1)
        {
            blueWins = 2;

            gemLeft2.sprite = gemBlue;
            gemLeftOutline2.sprite = gemBlueOutline;
        }
    }
    public void RedWin()
    {
        if (redWins == 0)
        {
            redWins = 1;

            gemRight1.sprite = gemRed;
            gemRightOutline1.sprite = gemRedOutline;
        }
        else if (redWins == 1)
        {
            redWins = 2;

            gemRight2.sprite = gemRed;
            gemRightOutline2.sprite = gemRedOutline;
        }
    }

    [ContextMenu("Collapse Round UI")]
    public void CollapseRoundUI()
    {
        StartCoroutine(iCollapseRoundUI());
    }
    private IEnumerator iCollapseRoundUI()
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {

            curValue = Mathf.Lerp(startSizeX, endSizeX, timeElapsed / lerpDuration);
            roundUIBar.GetComponent<RectTransform>().sizeDelta = new Vector2(curValue, roundUIBar.GetComponent<RectTransform>().sizeDelta.y);

            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        curValue = endSizeX;
        roundUIBar.GetComponent<RectTransform>().sizeDelta = new Vector2(curValue, roundUIBar.GetComponent<RectTransform>().sizeDelta.y);
    }

    [ContextMenu("Expand Round UI")]
    public void ExpandRoundUI()
    {
        StartCoroutine(iExpandRoundUI());
    }
    private IEnumerator iExpandRoundUI()
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {

            curValue = Mathf.Lerp(endSizeX, startSizeX, timeElapsed / lerpDuration);
            roundUIBar.GetComponent<RectTransform>().sizeDelta = new Vector2(curValue, roundUIBar.GetComponent<RectTransform>().sizeDelta.y);

            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        curValue = startSizeX;
        roundUIBar.GetComponent<RectTransform>().sizeDelta = new Vector2(curValue, roundUIBar.GetComponent<RectTransform>().sizeDelta.y);
    }

    [ContextMenu("Shrink Round UI")]
    public void ShrinkRoundUI()
    {
        StartCoroutine(iShrinkRoundUI());
    }
    private IEnumerator iShrinkRoundUI()
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration2)
        {

            curValue2 = Mathf.Lerp(startScaleX, endScaleX, timeElapsed / lerpDuration2);
            roundUIBar.GetComponent<RectTransform>().localScale = new Vector3(curValue2, curValue2, curValue2);

            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        curValue2 = endScaleX;
        roundUIBar.GetComponent<RectTransform>().localScale = new Vector3(curValue2, curValue2, curValue2);
    }

    [ContextMenu("Grow Round UI")]
    public void GrowRoundUI()
    {
        StartCoroutine(iGrowRoundUI());
    }
    private IEnumerator iGrowRoundUI()
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration2)
        {

            curValue2 = Mathf.Lerp(endScaleX, startScaleX, timeElapsed / lerpDuration2);
            roundUIBar.GetComponent<RectTransform>().localScale = new Vector3(curValue2, curValue2, curValue2);

            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        curValue2 = startScaleX;
        roundUIBar.GetComponent<RectTransform>().localScale = new Vector3(curValue2, curValue2, curValue2);
    }

    public void ShowRoundUI()
    {
        roundUIBar.gameObject.SetActive(true);
        roundUIBar.FadeIn();
    }
    public void HideRoundUI()
    {
        roundUIBar.gameObject.SetActive(true);
        roundUIBar.FadeOut();
    }
}
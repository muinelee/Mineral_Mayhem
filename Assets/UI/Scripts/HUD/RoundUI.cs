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
    [SerializeField] Image gemLeft2;
    [SerializeField] Image gemLeft3;
    [SerializeField] Image gemRight1;
    [SerializeField] Image gemRight2;
    [SerializeField] Image gemRight3;

    [Header("Asset References")]
    [SerializeField] Sprite gemGrey;
    [SerializeField] Sprite gemGreyOutline;
    [SerializeField] Sprite gemBlue;
    [SerializeField] Sprite gemBlueOutline;
    [SerializeField] Sprite gemRed;
    [SerializeField] Sprite gemRedOutline;

    [SerializeField] CG_Fade roundUIBar;

    [Header("Lerp Settings")]
    [SerializeField] private float lerpDuration = 0.5f;
    [SerializeField] private float startScaleX = 0;
    [SerializeField] private float endScaleX = 1f;

    private float curValue;

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
            gemLeft1.transform.GetComponentInChildren<Image>().sprite = gemBlueOutline;
        }
        else if (blueWins == 1)
        {
            blueWins = 2;

            gemLeft2.sprite = gemBlue;
            gemLeft2.transform.GetComponentInChildren<Image>().sprite = gemBlueOutline;
        }
        else if (blueWins == 2)
        {
            blueWins = 3;

            gemLeft3.sprite = gemBlue;
            gemLeft3.transform.GetComponentInChildren<Image>().sprite = gemBlueOutline;
        }
    }
    public void RedWin()
    {
        if (redWins == 0)
        {
            redWins = 1;

            gemLeft1.sprite = gemRed;
            gemLeft1.transform.GetComponentInChildren<Image>().sprite = gemRedOutline;
        }
        else if (redWins == 1)
        {
            redWins = 2;

            gemLeft2.sprite = gemRed;
            gemLeft2.transform.GetComponentInChildren<Image>().sprite = gemRedOutline;
        }
        else if (redWins == 2)
        {
            redWins = 3;

            gemLeft3.sprite = gemRed;
            gemLeft3.transform.GetComponentInChildren<Image>().sprite = gemRedOutline;
        }
    }

    public void CollapseRoundUI()
    {
        StartCoroutine(iCollapseRoundUI());
    }
    private IEnumerator iCollapseRoundUI()
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {

            curValue = Mathf.Lerp(endScaleX, startScaleX, timeElapsed / lerpDuration);
            roundUIBar.transform.localScale = new Vector3(curValue, roundUIBar.transform.localScale.y, roundUIBar.transform.localScale.z);

            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        curValue = startScaleX;
        roundUIBar.transform.localScale = new Vector3(startScaleX, roundUIBar.transform.localScale.y, roundUIBar.transform.localScale.z);
    }

    public void ExpandRoundUI()
    {
        StartCoroutine(iExpandRoundUI());
    }
    private IEnumerator iExpandRoundUI()
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {

            curValue = Mathf.Lerp(startScaleX, endScaleX, timeElapsed / lerpDuration);
            roundUIBar.transform.localScale = new Vector3(curValue, roundUIBar.transform.localScale.y, roundUIBar.transform.localScale.z);

            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        curValue = endScaleX;
        roundUIBar.transform.localScale = new Vector3(endScaleX, roundUIBar.transform.localScale.y, roundUIBar.transform.localScale.z);
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
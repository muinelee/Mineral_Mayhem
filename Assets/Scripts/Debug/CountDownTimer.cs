using System.Collections;
using UnityEngine;
using Fusion;
using TMPro;

public class CountDownTimer : MonoBehaviour
{
    public static CountDownTimer instance;
    [SerializeField] private float countDownDelay = 1.5f;

    [SerializeField] private CG_Fade text1;
    [SerializeField] private CG_Fade text2;
    [SerializeField] private CG_Fade text3;
    [SerializeField] private CG_Fade textGo;

    [SerializeField] private AudioClip[] countdownAudio;

    public int countDownNumber = 3;
    private bool firstTime = true;

    private void Start()
    {
        instance = this;
    }

    public void StartCountDown()
    {
        StartCoroutine(StartCountDownHelper());
    }

    IEnumerator StartCountDownHelper()
    {
        if (RoundUI.instance.roundUIBar.GetComponent<RectTransform>().localScale.x == RoundUI.instance.endScaleX)
            RoundUI.instance.GrowRoundUI();
        TimerManager.instance.StopTimer(true);

        int index = 0;
        countDownNumber = 3;
        AudioManager.Instance.PlayAudioSFX(countdownAudio[index], transform.position);
        text3.gameObject.SetActive(true);
        text3.FadeIn();

        yield return new WaitForSeconds(countDownDelay / 1.5f);
        text3.FadeOut();

        yield return new WaitForSeconds(countDownDelay);


        countDownNumber--;
        index++;
        AudioManager.Instance.PlayAudioSFX(countdownAudio[index], transform.position);
        text2.gameObject.SetActive(true);
        text2.FadeIn();

        yield return new WaitForSeconds(countDownDelay / 1.5f);
        text2.FadeOut();

        yield return new WaitForSeconds(countDownDelay);

        countDownNumber--;
        index++;
        AudioManager.Instance.PlayAudioSFX(countdownAudio[index], transform.position);
        text1.gameObject.SetActive(true);
        text1.FadeIn();

        yield return new WaitForSeconds(countDownDelay / 1.5f);
        text1.FadeOut();

        yield return new WaitForSeconds(countDownDelay);

        index++;
        AudioManager.Instance.PlayAudioSFX(countdownAudio[index], transform.position);
        if (NetworkPlayer.Local.HasStateAuthority) RoundManager.Instance.RPC_DisableControls(false);
        textGo.gameObject.SetActive(true);
        textGo.FadeIn();

        TimerManager.instance.ResetTimer(0);
        NetworkPlayer_InGameUI.instance.ShowPlayerUI();

        yield return new WaitForSeconds(countDownDelay);
        textGo.FadeOut();

        yield return new WaitForSeconds(countDownDelay / 1.5f);

        RoundUI.instance.ShrinkRoundUI();
    }
}
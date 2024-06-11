using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CountDownTimer : MonoBehaviour
{
    public static CountDownTimer instance;
    [SerializeField] private float countDownDelay = 1.5f;


    [SerializeField] private AudioClip[] countdownAudio;

    public int countDownNumber = 3;

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
        int index = 0;
        countDownNumber = 3;
        AudioManager.Instance.PlayAudioSFX(countdownAudio[index],Camera.main.gameObject.transform.position);
        yield return new WaitForSeconds(countDownDelay);

        countDownNumber--;
        index++;
        AudioManager.Instance.PlayAudioSFX(countdownAudio[index], Camera.main.gameObject.transform.position);
        yield return new WaitForSeconds(countDownDelay);

        countDownNumber--;
        index++;
        AudioManager.Instance.PlayAudioSFX(countdownAudio[index], Camera.main.gameObject.transform.position);
        yield return new WaitForSeconds(countDownDelay);

        index++;
        AudioManager.Instance.PlayAudioSFX(countdownAudio[index], Camera.main.gameObject.transform.position);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private Queue<AudioSource> audioPool;
    [SerializeField] private AudioSource audioTemplate;
    [SerializeField] private int poolSize = 15;
    [SerializeField] private AudioMixer mixer;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        audioPool = new Queue<AudioSource>();

        for (int i = 0; i < poolSize; i++)
        {
            AudioSource sourceAudio = Instantiate(audioTemplate, transform);
            audioPool.Enqueue(sourceAudio);
        }
    }

    public void PlayAudioSFX(AudioClip clip, Vector3 origin)
    {
        AudioSource source = GetThreeDimensionalSource(origin);
        source.clip = clip;
        source.Play();
        StartCoroutine(ReturnToPoolAfterPlaying(source, clip.length));
    }

    private IEnumerator ReturnToPoolAfterPlaying(AudioSource source, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        ReturnAudioSourceToPool(source);
    }

    public AudioSource GetThreeDimensionalSource(Vector3 origin)
    {
        if (audioPool.Count <= 0)
        {
            AudioSource sourceAudio = Instantiate(audioTemplate, transform);
            audioPool.Enqueue(sourceAudio);
        }

        AudioSource source = audioPool.Dequeue();
        //threeDAudioPool.Enqueue(source);
        source.transform.position = origin;
        return source;
    }

    public void ReturnAudioSourceToPool(AudioSource source)
    {
        source.transform.SetParent(transform);

        source.pitch = 1;
        source.volume = 1;

        audioPool.Enqueue(source);
    }

    public void SetMixerVolume(string name, float value)
    {
        PlayerPrefs.SetFloat(name, value);
        mixer.SetFloat(name, Mathf.Log10(value) * 20);
    }
}

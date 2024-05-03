using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private Queue<AudioSource> threeDAudioPool;
/*    private Queue<AudioSource> twoDAudioPool;
*/
/*    [SerializeField] private AudioSource twoDTemplate;
*/    [SerializeField] private AudioSource threeDTemplate;
    [SerializeField] private int numberOfPool = 15;
    [SerializeField] private AudioMixer mixer;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        threeDAudioPool = new Queue<AudioSource>();
/*        twoDAudioPool = new Queue<AudioSource>();
*/
        for (int i = 0; i < numberOfPool; i++)
        {
            AudioSource threeD = Instantiate(threeDTemplate, transform);
            threeDAudioPool.Enqueue(threeD);

/*            AudioSource twoD = Instantiate(twoDTemplate, transform);
            twoDAudioPool.Enqueue(twoD);*/
        }
    }

/*    public AudioSource GetTwoDimensionalSource()
    {
        AudioSource source = twoDAudioPool.Dequeue();
        twoDAudioPool.Enqueue(source);
        return source;
    }*/

    public void PlayAudioSFX(AudioClip clip, Vector3 origin)
    {
        //AudioSource source = GetTwoDimensionalSource();
        AudioSource source = GetThreeDimensionalSource(origin);
        source.clip = clip;
        source.Play();
    }

    public AudioSource GetThreeDimensionalSource(Vector3 origin)
    {
        if (threeDAudioPool.Count <= 0)
        {
            AudioSource threeD = Instantiate(threeDTemplate, transform);
            threeDAudioPool.Enqueue(threeD);
        }

        AudioSource source = threeDAudioPool.Dequeue();
        //threeDAudioPool.Enqueue(source);
        source.transform.position = origin;
        return source;
    }

    public void ReturnAudioSourceToPool(AudioSource source)
    {
        source.transform.SetParent(transform);

        source.pitch = 1;
        source.volume = 1;

        threeDAudioPool.Enqueue(source);
    }

    public void SetMixerVolume(string name, float value)
    {
        PlayerPrefs.SetFloat(name, value);
        mixer.SetFloat(name, Mathf.Log10(value) * 20);
    }
}

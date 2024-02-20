using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BTN_Audio : MonoBehaviour
{
    private AudioSource source;
    [SerializeField] AudioClip sfxMouseEnter;
    [SerializeField] AudioClip sfxMouseExit;
    [SerializeField] AudioClip sfxMouseDown;
    [SerializeField] AudioClip sfxMouseUp;

    //--------------------------------//

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void OnEnter()
    {
        if (sfxMouseEnter != null)
            source.PlayOneShot(sfxMouseEnter, 1);
    }
    public void OnExit()
    {
        if (sfxMouseExit != null)
            source.PlayOneShot(sfxMouseExit, 1);
    }
    public void OnDown()
    {
        if (sfxMouseDown != null)
            source.PlayOneShot(sfxMouseDown, 1);
    }
    public void OnUp()
    {
        if (sfxMouseUp != null)
            source.PlayOneShot(sfxMouseUp, 1);
    }
}

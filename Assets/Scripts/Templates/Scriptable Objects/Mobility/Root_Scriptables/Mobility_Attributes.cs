using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mobility", menuName = "New Mobility Option")]
public class Mobility_Attributes : ScriptableObject
{
    public string abilityName;
    public string description;

    public float spdIncrease;
    public float duration;
    public float coolDown;
    public bool canSteer;
    public bool canPhase;

    [SerializeField] private GameObject extraEffectPrefab;
    [SerializeField] private AudioClip mobilitySFX;

    public void Activate(Transform spawnPoint, Quaternion rotation)
    {
        AudioManager.Instance.PlayAudioSFX(mobilitySFX);
        if (extraEffectPrefab) Instantiate(extraEffectPrefab,spawnPoint.position, rotation);
    }
}

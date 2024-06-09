using UnityEngine;
using Fusion;
using System.Collections;
using Unity.VisualScripting;

public class CharacterVisualHandler : CharacterComponent
{
    public enum FlashEffect
    {
        Hit,
        Heal,
        Energy,
        Speed,
    }

    private SkinnedMeshRenderer[] meshRenderers;

    [Header("On Hit Flash Effects")]
    [SerializeField] public AnimationCurve onHitFlashCurve;
    public float onHitFlashDuration = 0.5f;
    public float onHitFlashIntensity = 2f;
    public Color onHitFlashColor = Color.black;

    [Header("On Heal Flash Effects")]
    [SerializeField] public AnimationCurve onHealFlashCurve;
    public float onHealFlashDuration = 0.5f;
    public float onHealFlashIntensity = 2f;
    public Color onHealFlashColor = Color.black;

    [Header("On Energy Pickup Flash Effects")]
    [SerializeField] public AnimationCurve onEnergizeFlashCurve;
    public float onEnergizeFlashDuration = 0.5f;
    public float onEnergizeFlashIntensity = 2f;
    public Color onEnergizeFlashColor = Color.black;

    [Header("On Speed Pickup Flash Effects")]
    [SerializeField] public AnimationCurve onSpeedFlashCurve;
    public float onSpeedFlashDuration = 0.5f;
    public float onSpeedFlashIntensity = 2f;
    public Color onSpeedFlashColor = Color.black;

    private Material[] materials;
    private Color[] originalEmissiveColors;
    [HideInInspector] public bool effectActive = false;
    private float flashTimer = 0f;
    private string emissionID = "_EmissionColor";
    private Color targetColor = Color.black;
    private AnimationCurve currentFlashCurve;
    private float currentFlashDuration;
    private float currentEmissionIntensity;

    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetVisualHandler(this);
        PrimeMaterials();
    }

    public void FixedUpdate()
    {
        if (!effectActive) return;

        flashTimer += Time.fixedDeltaTime;
        float t = flashTimer / currentFlashDuration;
        float curveValue = currentFlashCurve.Evaluate(t);

        if (flashTimer >= currentFlashDuration)
        {
            effectActive = false;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetColor(emissionID, originalEmissiveColors[i]);
            }
            return;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetColor(emissionID, Color.Lerp(originalEmissiveColors[i], targetColor * currentEmissionIntensity, curveValue));
        }
    }

    public override void OnHit(float x, bool hitReact)
    {
        RPC_StartFlashEffect(FlashEffect.Hit);
    }

    public override void OnHeal(float x)
    {
        RPC_StartFlashEffect(FlashEffect.Heal);
    }

    public override void OnPickup(bool isSpeed)
    {
        if (isSpeed) RPC_StartFlashEffect(FlashEffect.Speed);
        else RPC_StartFlashEffect(FlashEffect.Energy);
    }

    private void PrimeMaterials()
    {
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        Assert.Check(meshRenderers);
        int matCount = 0;
        foreach(SkinnedMeshRenderer mesh in  meshRenderers)
        {
            matCount += mesh.materials.Length;
        }
        materials = new Material[matCount];
        for(int i = 0; i < meshRenderers.Length; i++)
        {
            for(int j = 0; j < meshRenderers[i].materials.Length; j++)
            {
                materials[i + j] = meshRenderers[i].materials[j];
                materials[i + j].EnableKeyword("_EMISSION");
            }
        }
        originalEmissiveColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalEmissiveColors[i] = materials[i].GetColor(emissionID);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_StartFlashEffect(FlashEffect FX)
    {
        StartFlashEffect(FX);
    }

    private void StartFlashEffect(FlashEffect FX)
    {
        switch(FX)
        {
            case FlashEffect.Hit:
                targetColor = onHitFlashColor;
                currentFlashDuration = onHitFlashDuration;
                currentFlashCurve = onHitFlashCurve;
                currentEmissionIntensity = onHitFlashIntensity;
                break;
            case FlashEffect.Heal:
                targetColor = onHealFlashColor;
                currentFlashDuration = onHealFlashDuration;
                currentFlashCurve = onHealFlashCurve;
                currentEmissionIntensity = onHealFlashIntensity;
                break;
            case FlashEffect.Energy:
                targetColor = onEnergizeFlashColor;
                currentFlashDuration = onEnergizeFlashDuration;
                currentFlashCurve = onEnergizeFlashCurve;
                currentEmissionIntensity = onEnergizeFlashIntensity;
                break;
            case FlashEffect.Speed:
                targetColor = onSpeedFlashColor;
                currentFlashDuration = onSpeedFlashDuration;
                currentFlashCurve = onSpeedFlashCurve;
                currentEmissionIntensity = onSpeedFlashIntensity;
                break;
        }
        /*targetColor = target;
        currentFlashDuration = duration;
        if (target == onHitFlashColor && duration == onHitFlashDuration)
        {
            currentFlashCurve = onHitFlashCurve;
            currentEmissionIntensity = onHitFlashIntensity;
        }
        else
        {
            currentFlashCurve = onHealFlashCurve;
            currentEmissionIntensity = onHealFlashIntensity;
        }*/
        effectActive = true;
        flashTimer = 0f;
    }
}
using UnityEngine;
using Fusion;
using System.Collections;
using Unity.VisualScripting;

public class CharacterVisualHandler : CharacterComponent
{
    private SkinnedMeshRenderer meshRenderer;
    [SerializeField] public AnimationCurve flashCurve;
    public float flashDuration = 0.5f;
    public Color flashColor = Color.black;

    private Material[] materials;
    private Color[] originalColors;
    public bool effectActive = false;
    private float flashTimer = 0f;

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
        float t = flashTimer / flashDuration;
        float curveValue = flashCurve.Evaluate(t);

        if (flashTimer >= flashDuration)
        {
            effectActive = false;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetColor("_BaseColor", originalColors[i]);
            }
            return;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetColor("_BaseColor", Color.Lerp(originalColors[i], flashColor, curveValue));
        }
    }

    public override void OnHit(float x, bool hitReact)
    {
        if (effectActive) return;
        RPC_StartFlashEffect();
    }

    private void PrimeMaterials()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        Assert.Check(meshRenderer);
        materials = meshRenderer.materials;
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].GetColor("_BaseColor");
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_StartFlashEffect()
    {
        StartFlashEffect();
    }

    private void StartFlashEffect()
    {
        effectActive = true;
        flashTimer = 0f;
    }
}
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
    [Networked(OnChanged = nameof(OnBoolChanged))]
    public bool effectActive { get; set; }
    private float flashTimer = 0;

    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetVisualHandler(this);
        PrimeMaterials();
    }

    public override void Spawned()
    {
        if (NetworkPlayer.Local.HasStateAuthority) effectActive = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (!effectActive) return;
        
        Debug.Log("Doin the thing");

        flashTimer += Time.fixedDeltaTime;
        float t = flashTimer / flashDuration;
        float curveValue = flashCurve.Evaluate(t);

        Debug.Log($"Curve Value: {curveValue}");
        Debug.Log($"T Value: {t}");

        if (t >= 1.0f)
        {
            t = 1.0f;
            if (Object.HasStateAuthority) effectActive = false;
            return;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetColor("_BaseColor", Color.Lerp(originalColors[i], flashColor, curveValue));
            Debug.Log("Updating Colour");
        }
    }

    public override void OnHit(float x)
    {
        if (effectActive) return;
        if (NetworkPlayer.Local.HasStateAuthority) effectActive = true;
    }

    private void PrimeMaterials()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        Assert.Check(meshRenderer);
        materials = meshRenderer.materials;
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    static void OnBoolChanged(Changed<CharacterVisualHandler> changed)
    {
        Debug.Log($"Effect Active on {changed.Behaviour.Character.Team} is set to: " + changed.Behaviour.effectActive.ToString());
        changed.Behaviour.DetermineFlashState(changed.Behaviour.effectActive);
    }

    private void DetermineFlashState(bool isActive)
    {
        if (isActive) flashTimer = 0f;
        else
        {
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetColor("_BaseColor", originalColors[i]);
            }
        }
    }
}
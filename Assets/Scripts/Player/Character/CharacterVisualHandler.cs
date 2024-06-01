using UnityEngine;
using Fusion;
using System.Collections;
using Unity.VisualScripting;

public class CharacterVisualHandler : CharacterComponent
{
    private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private AnimationCurve flashCurve;
    public float flashDuration = 0.5f;
    public Color flashColor = Color.white;

    private Material[] materials;
    private Color[] originalColors;
    [Networked(OnChanged = nameof(OnBoolChanged))] public bool effectActive { get; set; } = false;
    private float flashTimer = 0;

    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetVisualHandler(this);
    }

    public override void Spawned()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        Assert.Check(meshRenderer);

        materials = meshRenderer.materials;
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
        Debug.Log($"Found {materials.Length} materials on {Character.name}");
    }

    private void FixedUpdate()
    {
        if (!effectActive) return;

        flashTimer += Time.fixedDeltaTime;
        float t = flashTimer / flashDuration;
        float curveValue = flashCurve.Evaluate(t);

        if (t >= 1.0f)
        {
            t = 1.0f;
            effectActive = false;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetColor("_BaseColor", originalColors[i]);
            }
        }

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetColor("_BaseColor", Color.Lerp(originalColors[i], flashColor, curveValue));
        }
    }

    public override void OnHit(float x)
    {
        if (effectActive) return;
        flashTimer = 0f;
        effectActive = true;
    }

    static void OnBoolChanged(Changed<NetworkPlayer_Health> changed)
    {
    }

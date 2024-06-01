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
    private bool effectActive = false;
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
    }

    private void FixedUpdate()
    {
        if (!effectActive) return;

        Debug.Log("Flashing!!!!");
        flashTimer += Time.fixedDeltaTime;
        float t = flashTimer / flashDuration;
        float curveValue = flashCurve.Evaluate(t);

        if (t >= 1.0f)
        {
            t = 1.0f;
            effectActive = false;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetColor("_Color", originalColors[i]);
            }
        }

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetColor("_Color", Color.Lerp(originalColors[i], flashColor, curveValue));
        }
    }

    public override void OnHit(float x)
    {
        if (effectActive) return;
        flashTimer = 0f;
        effectActive = true;
        Debug.Log("Flash!");
    }
}

using UnityEngine;
using UnityEngine.UI;

public class DummyHealthBar : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private Transform target;

    [SerializeField]
    private NPC_Core npcStats;

    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Image fillImage;

    private void Start()
    {
        npcStats = GetComponentInParent<NPC_Core>();
        mainCamera = Camera.main;
        slider.maxValue = npcStats.currentHP;
        UpdateHealthBar(slider.value);
    }

    public void UpdateHealthBar(float currentValue)
    {
        slider.value = npcStats.currentHP;
    }


    // Update is called once per frame
    void Update()
    {
        transform.rotation = mainCamera.transform.rotation;

        if (slider.value != npcStats.currentHP && slider.value > 0)
        {
            slider.value = npcStats.currentHP;
        }

        transform.position = target.position + offset;
    }
}
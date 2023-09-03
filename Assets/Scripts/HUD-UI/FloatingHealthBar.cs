using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Player_StatsSO playerStats;

    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Image fillImage;

    private void Start()
    {
       mainCamera = Camera.main;
       slider.maxValue = playerStats.maxHP;
       UpdateHealthBar(slider.value);
    }

    public void UpdateHealthBar(float currentValue)
    {
        slider.value = playerStats.currentHP;
    }


    // Update is called once per frame
    void Update()
    {
        transform.rotation = mainCamera.transform.rotation;

        if (slider.value != playerStats.currentHP && slider.value > 0)
        {
            slider.value = playerStats.currentHP;
        }

        transform.position = target.position + offset;

        /*        // Calculate the distance between the camera and the target
                float distanceToCamera = Vector3.Distance(mainCamera.transform.position, transform.position);

                // Scale the health bar based on the distance to the camera
                float scale = Mathf.Clamp(distanceToCamera / 10.0f, 0.5f, 2.0f);
                slider.transform.localScale = new Vector3(scale, scale, 1.0f);*/
    }
}

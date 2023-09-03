using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField]
    private Transform target;
/*    [SerializeField]
    private Vector3 offset;*/

    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Image fillImage;

    public FloatReference playerCurrentHealth;
    public FloatReference playerMaxHealth;

    private void Start()
    {
       mainCamera = Camera.main;
       UpdateHealthBar(slider.value, slider.maxValue);
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = currentValue;
    }


    // Update is called once per frame
    void Update()
    {
        transform.rotation = mainCamera.transform.rotation;
/*        transform.position = target.position + offset;*/

/*        // Calculate the distance between the camera and the target
        float distanceToCamera = Vector3.Distance(mainCamera.transform.position, transform.position);

        // Scale the health bar based on the distance to the camera
        float scale = Mathf.Clamp(distanceToCamera / 10.0f, 0.5f, 2.0f);
        slider.transform.localScale = new Vector3(scale, scale, 1.0f);*/
    }
}

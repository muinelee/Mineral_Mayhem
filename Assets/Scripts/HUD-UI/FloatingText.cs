using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private Camera mainCamera;
    public float destroyTime = 2.5f;
    public Vector3 RandomizeIntensity = new Vector3(0.5f, 0, 0);
    [SerializeField] public Vector3 offset = new Vector3(0, 5f, 0);

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        Destroy(gameObject, destroyTime);
        transform.localPosition += offset;
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
                                Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y),
                                Random.Range(-RandomizeIntensity.z, RandomizeIntensity.z));
    }

    private void Update()
    {
        transform.rotation = mainCamera.transform.rotation;
    }
}

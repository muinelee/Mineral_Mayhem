using UnityEngine;

public class ShakeAndDestroy : MonoBehaviour
{
    public float delayBeforeShake = 3.0f; // delay in seconds before shaking starts
    public float shakeDuration = 2.0f;     // duration of the shake in seconds
    public float shakeIntensity = 0.1f;    // intensity of the shake
    public float dropSpeed = 100.0f;       // speed at which the object drops
    public float destroyDelay = 0.1f;      // delay before destroying the object after it drops

    private float shakeStartTime;
    private Vector3 initialPosition;
    private bool isShaking = false;
    private bool isDropping = false;

    void Start()
    {
        shakeStartTime = Time.time + delayBeforeShake;
    }

    void Update()
    {
        if (!isShaking && Time.time >= shakeStartTime)
        {
            initialPosition = transform.position; // get the current position before shaking starts (for spawn)
            StartShake();
        }

        if (isShaking && Time.time < shakeStartTime + shakeDuration)
        {
            Shake();
        }
        else if (isShaking && !isDropping) //  check to make sure the object drops only once
        {
            EndShake();
        }

        if (isDropping)
        {
            Drop();
        }
    }

    void StartShake()
    {
        isShaking = true;
    }

    void Shake()
    {
        Vector3 randomOffset = Random.insideUnitSphere * shakeIntensity;
        transform.position = initialPosition + randomOffset; // apply the shake effect to the initial position (used for the spawn)

    }

    void EndShake()
    {
        transform.position = initialPosition;
        isShaking = false;
        isDropping = true;
    }

    void Drop()
    {
        transform.Translate(Vector3.down * dropSpeed * Time.deltaTime);

        // destroy the object after a delay 
        Destroy(gameObject, destroyDelay);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_FrostCloud : MonoBehaviour
{
    [Header("Movement Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float maxTravelDistance;

    private float lifeTimer = 0;
    private float distanceTravelled = 0;

    private void Start()
    {
        transform.position += transform.up * offset.y + transform.forward * offset.z;
    }

    void FixedUpdate()
    {
        float moveDistance = speed * Time.deltaTime;

        if (distanceTravelled + moveDistance > maxTravelDistance)
        {
            moveDistance = maxTravelDistance - distanceTravelled;
        }

        transform.Translate(Vector3.forward * moveDistance);
        distanceTravelled += moveDistance;

        lifeTimer += Time.deltaTime;
        if (lifeTimer > lifetime) Destroy(gameObject);
    }
}

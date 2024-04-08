using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Test_RapidIceShot_IceSpike : NetworkBehaviour
{
    [Header("Movement Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    private float lifeTimer = 0;
    [SerializeField] private float offset;

    private void Start()
    {
        float offsetX = Random.Range(-offset, offset);
        float offsetY = Random.Range(0, offset);

        Vector3 offsetVector = new Vector3(offsetX, offsetY, 0);

        transform.Translate(offsetVector);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // move
        transform.Translate(Vector3.forward * speed);

        // manage lifetime
        lifeTimer += Time.deltaTime;
        if (lifeTimer > lifetime) Destroy(gameObject);
    }
}

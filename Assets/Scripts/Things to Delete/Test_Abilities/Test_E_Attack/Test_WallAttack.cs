using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_WallAttack : Attack
{

    [SerializeField] private float duration;
    private float timer  = 0;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration) Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log(other.name);
        if (other.transform.tag == "Projectile") Destroy(other.gameObject);
    }
}

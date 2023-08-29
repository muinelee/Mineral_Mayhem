using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE_Attack : MonoBehaviour
{
    [SerializeField] private float endSize;
    private Vector3 startSizeVector = new Vector3(1,1,1);
    private Vector3 endSizeVector;
    [SerializeField] private float growTime;
    [SerializeField] private float duration;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        endSizeVector = transform.localScale * endSize;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(startSizeVector, endSizeVector, timer / growTime);

        timer += Time.deltaTime;
        if (timer > duration) Destroy(this.gameObject);
    }
}

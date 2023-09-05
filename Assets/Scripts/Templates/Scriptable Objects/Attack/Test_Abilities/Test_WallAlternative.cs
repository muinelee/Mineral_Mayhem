using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_WallAlternative : MonoBehaviour
{
    [SerializeField] private float duration;
    private float timer = 0;

    void Update()
    {
        if (timer < duration) timer += Time.deltaTime;
        else Destroy(this.gameObject); 
    }
}
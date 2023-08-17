using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    private float _hp = 50;
    public float hp
    {
        get 
        {
            return _hp;
        }
        
        set 
        {
            _hp = value;
            if (_hp <= 0) pc.enabled = false;
        }
    }

    [SerializeField] PlayControl pc;

    // Start is called before the first frame update
    void Awake()
    {
        if  (!pc) pc = gameObject.GetComponent<PlayControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

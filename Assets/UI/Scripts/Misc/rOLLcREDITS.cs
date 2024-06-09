using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rOLLcREDITS : MonoBehaviour
{
    private Animator anim;

    [SerializeField] float normalSpeed = 10f;
    [SerializeField] float fastSpeed = 25f;

    //----------------------------------//

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        anim.Play("Roll");
        anim.speed = normalSpeed;
    }

    public void ReverseDown()
    {
        anim.speed = fastSpeed;
        anim.SetFloat("Direction", -1);
    }
    public void ReverseUp()
    {
        anim.speed = normalSpeed;
        anim.SetFloat("Direction", 1);
    }

    public void FastDown()
    {
        anim.speed = fastSpeed;
    }
    public void FastUp()
    {
        anim.speed = normalSpeed;
    }
}

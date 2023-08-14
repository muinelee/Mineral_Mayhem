using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayControl : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Movement mov;

    void Start()
    {
        if (!cam) cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (!mov) mov = gameObject.GetComponent<Movement>();
    }

    // Use update for input and calculations
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit)) mov.SetLookDirection(raycastHit.point);    
    }

    // Use fixed update for affecting the game
    void FixedUpdate()
    {
        mov.SetDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized);
    }
}
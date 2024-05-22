using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraTiltByMousePos : MonoBehaviour
{
    Camera cam;

    Vector3 startCamPos;
    public Vector2 mousePos;
    public float idleCamRotY = 0;

    //-------------------------------------------//

    private void Awake()
    {
        cam = GetComponent<Camera>();
        startCamPos = cam.transform.position;
    }

    private void FixedUpdate()
    {
        if (Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height)
        {
            mousePos.x = Input.mousePosition.x / Screen.width - 0.5f;
            mousePos.y = Input.mousePosition.y / Screen.height - 0.5f;

            cam.transform.position = new Vector3(startCamPos.x + (mousePos.x / 2), startCamPos.y + (mousePos.y / 3), startCamPos.z);
            cam.transform.localEulerAngles = new Vector3(Mathf.Clamp(0 + (mousePos.y * -2f), -5, 5), Mathf.Clamp(idleCamRotY + (mousePos.x * 4), idleCamRotY -5, idleCamRotY + 5), 0);
        }
    }
}

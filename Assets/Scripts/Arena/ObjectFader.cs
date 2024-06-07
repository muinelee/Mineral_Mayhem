using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFader : MonoBehaviour
{
    [SerializeField] private Renderer[] objectToFade;
    [SerializeField] private GameObject[] opaqueObject;
    private Color[] objectColor;

    [Header("Fading Properties")]
    [SerializeField] private float timeToFade = 2f;
    private float fadeTimer;
    private bool isFading = false;

    private int playerCount;

    // Start is called before the first frame update
    void Start()
    {
        objectColor = new Color[objectToFade.Length];

        for (int i = 0; i < objectToFade.Length; i++)
        {
            objectColor[i] = objectToFade[i].material.color;
            objectToFade[i].material.color = new Color(objectColor[i].r, objectColor[i].g, objectColor[i].b, 1);
        }

        for (int i = 0; i < opaqueObject.Length; i++)
        {
            opaqueObject[i].SetActive(true);
        }

        fadeTimer = timeToFade;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFading && fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
            for (int i = 0; i < objectToFade.Length; i++)
            {
                objectToFade[i].material.color = new Color(objectColor[i].r, objectColor[i].g, objectColor[i].b, fadeTimer / timeToFade);
            }
        }

        else if (fadeTimer < timeToFade)
        {
            fadeTimer += Time.deltaTime;

            for (int i = 0; i < objectToFade.Length; i++)
            {
                objectToFade[i].material.color = new Color(objectColor[i].r, objectColor[i].g, objectColor[i].b, fadeTimer / timeToFade);
            }
        }
        else if (!isFading && fadeTimer > timeToFade)
        {
            for (int i = 0; i <  opaqueObject.Length; i++)
            {
                opaqueObject[i].SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerCount == 0) isFading = true;
            for (int i = 0; i  < opaqueObject.Length; i++)
            {
                opaqueObject[i].SetActive(false);
            }
            playerCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount--;

            if (playerCount == 0) isFading = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFader : MonoBehaviour
{
    [SerializeField] private Renderer objectToFade;
    private Color objectColor;

    [Header("Fading Properties")]
    [SerializeField] private float timeToFade = 2f;
    private float fadeTimer;
    private bool isFading = false;

    private int playerCount;

    // Start is called before the first frame update
    void Start()
    {
        objectColor = objectToFade.material.color;
        objectToFade.material.color = new Color(objectColor.r, objectColor.g, objectColor.b, 0.4f);

        fadeTimer = timeToFade;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFading && fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
            objectToFade.material.color = new Color(objectColor.r, objectColor.g, objectColor.b, fadeTimer / timeToFade);
        }

        else if (fadeTimer < timeToFade)
        {
            fadeTimer += Time.deltaTime;

            objectToFade.material.color = new Color(objectColor.r, objectColor.g, objectColor.b, fadeTimer / timeToFade);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerCount == 0) isFading = true;
            
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

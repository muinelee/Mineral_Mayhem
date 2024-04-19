using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IMG_ChangeColor : MonoBehaviour
{
    private Image image;
    private Color startColor;
    [SerializeField] Color endColor;

    //--------------------------------------//

    private void Awake()
    {
        image = GetComponent<Image>();
        startColor = image.color;
    }

    public void ChangeColor()
    {
        image.color = endColor;
    }
    public void RevertColor()
    {
        image.color = startColor;
    }
}

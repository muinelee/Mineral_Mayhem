using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SelectionCentering : MonoBehaviour
{
    RectTransform tr;
    [SerializeField] float[] xPosition;
    [SerializeField] BTN_Animation[] selections;
    public int page;

    [SerializeField] private float lerpDuration = 0.5f;

    public UnityEvent purchase;

    //-----------------------------------------//

    private void Awake()
    {
        tr = GetComponent<RectTransform>();
    }
    private void Start()
    {
        page = 1;

        CentreItem(0);
    }

    public void CentreItem(int x)
    {
        StartCoroutine(iCentreItem());
        if (x == -1 && page > 0)
        {
            page--;
        }
        else if (x == 1 && page < 2)
        {
            page++;
        }
    }
    private IEnumerator iCentreItem()
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            tr.localPosition = Vector2.Lerp(tr.localPosition, new Vector2(xPosition[page], tr.localPosition.y), Time.fixedDeltaTime * lerpDuration);

            timeElapsed += Time.fixedDeltaTime;
            yield return null;
        }

        tr.localPosition = new Vector2(xPosition[page], tr.localPosition.y);
        purchase?.Invoke();
    }
}

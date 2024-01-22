using UnityEngine;
using UnityEngine.Events;

public class SelectionAnim : MonoBehaviour
{
    RectTransform tr;
    [SerializeField] float[] xPosition;
    [SerializeField] BTN_Animation[] selections;
    [SerializeField] int startIndex;

    public UnityEvent purchase;

    //-----------------------------------------//

    private void Awake()
    {
        tr = GetComponent<RectTransform>();
    }
    private void Start()
    {
        CentreItem(startIndex);
    }

    public void CentreItem(int x)
    {
         tr.localPosition = new Vector2(xPosition[x], tr.localPosition.y);

         for (int i = 0; i < selections.Length; i++)
         {
             if (i == x)
             {
                 selections[i].MouseUp();
             }
             else
             {
                 selections[i].MouseDown();
             }
         }

         purchase?.Invoke();
    }
}

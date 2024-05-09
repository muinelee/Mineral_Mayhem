using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BTN_OpenClose : MonoBehaviour
{
    [SerializeField] float delayTime = 0.5f;

    [Header("OpenCloseMenu Options")]
    [SerializeField] CG_Fade[] groupToClose;
    [SerializeField] CG_Fade[] groupToOpen;

    [SerializeField] string nextSceneName;
    [SerializeField] bool restart;
    [SerializeField] bool quit;

    public bool disabled = false;

    public UnityEvent onPress;
    public UnityEvent onPressDelayed;

    //------------------------------------//

    public void OnPress()
    {
        if (!disabled)
        {
            onPress?.Invoke();
            StartCoroutine(iOnPress());
        }
    }
    private IEnumerator iOnPress()
    {
        yield return new WaitForSecondsRealtime(delayTime);

        onPressDelayed?.Invoke();

        if (quit)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        if (restart)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (nextSceneName != "")
        {
            SceneManager.LoadScene(nextSceneName);
        }

        if (groupToOpen.Length > 0)
        {
            for (int i = 0; i < groupToOpen.Length; i++)
            {
                groupToOpen[i].gameObject.SetActive(true);
                groupToOpen[i].FadeIn();
            }
        }

        if (groupToClose.Length > 0)
        {
            for (int i = 0; i < groupToClose.Length; i++)
            {
                groupToClose[i].gameObject.SetActive(true);
                groupToClose[i].FadeOut();
            }
        }
    }
}

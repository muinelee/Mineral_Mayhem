using UnityEngine;

public class UIScreen : MonoBehaviour
{
    [SerializeField] private UIScreen previousScreen = null;
    public static UIScreen activeScreen;

    public static void Focus(UIScreen screen)
    {
        if (screen == activeScreen) return;

        if (activeScreen) activeScreen.Defocus();

        screen.previousScreen = activeScreen;
        activeScreen = screen;
        screen.Focus();
    }

    public static void BackToInitial()
    {
        activeScreen?.BackTo(null);
    }

    public void FocusScreen(UIScreen screen)
    {
        Focus(screen);
    }

    private void Focus()
    {
        if (gameObject) gameObject.SetActive(true);
    }

    private void Defocus()
    {
        if (gameObject) gameObject.SetActive(false);
    }

    public void Back()
    {
        if (previousScreen)
        {
            Defocus();
            activeScreen = previousScreen;
            activeScreen.Focus();
            previousScreen = null;
        }
    }

    public void BackTo(UIScreen screen)
    {
        while (activeScreen != null && activeScreen.previousScreen != null && activeScreen != screen)
        {
            activeScreen.Back();
        }
    }
}

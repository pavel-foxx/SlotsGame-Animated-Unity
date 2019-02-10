using UnityEngine;

public class window_switcher : MonoBehaviour {

    public button_handler b_start, b_exit;

    public GameObject WelcomeScreen, GameScreen;
    public bool isOpen;

    private void Update()
    {
        if (isOpen)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isOpen = false;
                InAppBrowser.CloseBrowser();
            }
    }
    private void Awake()
    {
        b_start.OnClick += OpenGame;
        b_exit.OnClick += Exit;
    }

    private void OpenGame(button_handler button)
    {
        WelcomeScreen.SetActive(false);
        GameScreen.SetActive(true);
    }
    private void Exit(button_handler button)
    {
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Full-screen main menu that lets the player choose a board size.
/// Set up the panel, buttons, and references in the Unity inspector.
/// </summary>
public class MainMenu : MonoBehaviour
{
    public GameManager gameManager;
    public UIManager   uiManager;
    public GameObject  hudBar;
    public GameObject menuPanel;

    public Button easyButton;     // 2×2
    public Button mediumButton;   // 4×3
    public Button hardButton;     // 4×4
    public Button expertButton;   // 6×4
    public Button masterButton;   // 6×5

    void Start()
    {
        if (easyButton   != null) easyButton.onClick.AddListener(()   => StartGame(2, 2));
        if (mediumButton != null) mediumButton.onClick.AddListener(() => StartGame(4, 3));
        if (hardButton   != null) hardButton.onClick.AddListener(()   => StartGame(4, 4));
        if (expertButton != null) expertButton.onClick.AddListener(() => StartGame(6, 4));
        if (masterButton != null) masterButton.onClick.AddListener(() => StartGame(6, 5));

        if (SaveManager.HasSave())
        {
            if (gameManager.LoadGame())
            {
                if (menuPanel != null) menuPanel.SetActive(false);
                if (hudBar    != null) hudBar.SetActive(true);


                Camera cam = Camera.main;
                if (cam != null)
                    cam.orthographicSize = Mathf.Max(gameManager.rows, gameManager.columns) * 0.85f + 1f;

                return;
            }
        }

        Show();
    }


    public void Show()
    {

        SaveManager.DeleteSave();

        if (menuPanel != null) menuPanel.SetActive(true);
        if (hudBar    != null) hudBar.SetActive(false);
    }


    public void StartGame(int cols, int rowCount)
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (hudBar    != null) hudBar.SetActive(true);


        Camera cam = Camera.main;
        if (cam != null)
            cam.orthographicSize = Mathf.Max(rowCount, cols) * 0.85f + 1f;


    }
}

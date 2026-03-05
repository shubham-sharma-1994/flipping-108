using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Full-screen main menu that lets the player choose a board size.
/// Set up the panel, buttons, and references in the Unity inspector.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    public UIManager   uiManager;
    public GameObject  hudBar;

    [Header("Menu Panel")]
    [Tooltip("The root panel GameObject that holds all menu UI. Toggled on/off.")]
    public GameObject menuPanel;

    [Header("Difficulty Buttons")]
    public Button easyButton;     // 2×2
    public Button mediumButton;   // 4×3
    public Button hardButton;     // 4×4
    public Button expertButton;   // 6×4
    public Button masterButton;   // 6×5

    void Start()
    {
        // Wire up button listeners
        if (easyButton   != null) easyButton.onClick.AddListener(()   => StartGame(2, 2));
        if (mediumButton != null) mediumButton.onClick.AddListener(() => StartGame(4, 3));
        if (hardButton   != null) hardButton.onClick.AddListener(()   => StartGame(4, 4));
        if (expertButton != null) expertButton.onClick.AddListener(() => StartGame(6, 4));
        if (masterButton != null) masterButton.onClick.AddListener(() => StartGame(6, 5));

        // If there is an unfinished game, resume it instead of showing the menu
        if (SaveManager.HasSave())
        {
            if (gameManager.LoadGame())
            {
                if (menuPanel != null) menuPanel.SetActive(false);
                if (hudBar    != null) hudBar.SetActive(true);

                // Resize camera for the loaded board
                Camera cam = Camera.main;
                if (cam != null)
                    cam.orthographicSize = Mathf.Max(gameManager.rows, gameManager.columns) * 0.85f + 1f;

                return; // skip showing the menu
            }
        }

        // No save found – show menu
        Show();
    }

    /// <summary>Show the menu (e.g. after winning or returning).</summary>
    public void Show()
    {
        // Clear any saved game when returning to menu
        SaveManager.DeleteSave();

        if (menuPanel != null) menuPanel.SetActive(true);
        if (hudBar    != null) hudBar.SetActive(false);
    }

    /// <summary>Hide the menu and start a game with the chosen size.</summary>
    public void StartGame(int cols, int rowCount)
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (hudBar    != null) hudBar.SetActive(true);

        // Resize camera for new board
        Camera cam = Camera.main;
        if (cam != null)
            cam.orthographicSize = Mathf.Max(rowCount, cols) * 0.85f + 1f;

        // Configure and launch
        gameManager.columns = cols;
        gameManager.rows    = rowCount;
        gameManager.InitGame();
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    public GameManager gameManager;

    public TextMeshProUGUI matchesText;
    public TextMeshProUGUI turnsText;

    public GameObject winPanel;
    public TextMeshProUGUI winMessageText;
    public Button restartButton;
    public Button mainMenuButton;

    public Button abortButton;

    public MainMenu mainMenu;

    void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);

        if (gameManager != null)
            Init(gameManager);
    }

    public void Init(GameManager gm)
    {
        gameManager = gm;

        gm.OnScoreChanged.AddListener(UpdateScore);
        gm.OnGameWon.AddListener(ShowWinScreen);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        if (abortButton != null)
            abortButton.onClick.AddListener(OnAbortClicked);

        UpdateScore(0, 0);
    }

    public void UpdateScore(int matches, int turns)
    {
        if (matchesText != null) matchesText.text = $"Matches: {matches}";
        if (turnsText   != null) turnsText.text   = $"Turns: {turns}";
    }

    public void ShowWinScreen()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (winMessageText != null)
                winMessageText.text = $"You Win!\n{gameManager.Matches} pairs in {gameManager.Turns} turns";
        }
    }

    public void OnRestartClicked()
    {
        if (winPanel != null)
            winPanel.SetActive(false);

        gameManager.InitGame();
    }

    public void OnMainMenuClicked()
    {
        if (winPanel != null)
            winPanel.SetActive(false);

        gameManager.CleanUpBoard();

        if (mainMenu != null)
            mainMenu.Show();
    }

    public void OnAbortClicked()
    {
        gameManager.ClearSave();
        gameManager.CleanUpBoard();

        if (mainMenu != null)
            mainMenu.Show();
    }
}

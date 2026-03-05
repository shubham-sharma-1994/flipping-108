using UnityEngine;
using UnityEngine.UI;
using TMPro;

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


        Show();
    }

    public void Show()
    {

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

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DeathPopupManager : MonoBehaviour
{
    [SerializeField] private GameObject deathPopupPanel;
    [SerializeField] private Button soundToggleButton;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private bool isSoundOn = true;
    private SecureScoreManager scoreManager;

    void Start()
    {
        if (deathPopupPanel != null)
        {
            deathPopupPanel.SetActive(false);
        }

        if (soundToggleButton != null)
        {
            isSoundOn = AudioListener.volume > 0;
            UpdateSoundButtonImage();
            soundToggleButton.onClick.AddListener(ToggleSound);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        // ScoreManager 찾기
        scoreManager = FindObjectOfType<SecureScoreManager>();
    }

    private void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        AudioListener.volume = isSoundOn ? 1f : 0f;
        UpdateSoundButtonImage();
    }

    private void UpdateSoundButtonImage()
    {
        if (soundToggleButton != null)
        {
            soundToggleButton.image.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        }
    }

    public void ShowDeathPopup()
    {
        if (deathPopupPanel != null && scoreManager != null)
        {
            Time.timeScale = 0f;
            deathPopupPanel.SetActive(true);

            int currentScore = scoreManager.CurrentScore;
            finalScoreText.text = "최종 점수: " + currentScore;
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }
}

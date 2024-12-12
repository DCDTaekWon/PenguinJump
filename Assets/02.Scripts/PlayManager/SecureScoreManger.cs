using UnityEngine;
using TMPro;
using System;

public class SecureScoreManager : MonoBehaviour
{
    public static SecureScoreManager Instance { get; private set; }

    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    public float score = 0f;
    public float scoreIncreaseRate = 10f;

    public int displayedScore = 0;
    public int highScore = 0;

   public PenguinController controller;

    public int CurrentScore => displayedScore;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (SecureScoreManager.Instance != null)
        {
            TMP_Text newScoreText = FindObjectOfType<TMP_Text>(); // 실제 UI Text 오브젝트 참조
            TMP_Text newHighScoreText = FindObjectOfType<TMP_Text>(); // 하이스코어 Text 참조
            SecureScoreManager.Instance.RebindUI(newScoreText, newHighScoreText);
        }
    }


    void Update()
    {
        if (controller.isGameFlag)
        {
            score = 0f;
            return;
        }
            

        score += scoreIncreaseRate * Time.deltaTime;
        int newScore = Mathf.FloorToInt(score / 10) * 10;

        if (newScore != displayedScore)
        {
            displayedScore = newScore;
            if (scoreText != null) scoreText.text = displayedScore.ToString();

            if (displayedScore > highScore)
            {
                highScore = displayedScore;
                UpdateHighScoreText();
                SaveHighScore(highScore);
            }
        }
    }

    private void UpdateHighScoreText()
    {
        if (highScoreText != null) highScoreText.text = highScore.ToString();
    }

    void Start()
    {
        if (SecureScoreManager.Instance != null)
        {
            TMP_Text newScoreText = FindObjectOfType<TMP_Text>(); // 실제 UI Text 오브젝트 참조
            TMP_Text newHighScoreText = FindObjectOfType<TMP_Text>(); // 하이스코어 Text 참조
            SecureScoreManager.Instance.RebindUI(newScoreText, newHighScoreText);
        }
    }

    public void ResetScore()
    {
        score = 0f;
        displayedScore = 0;

        if (scoreText != null)
            scoreText.text = "0"; // UI 초기화
    }

    public void SetFinalScore(int finalScore)
    {
        score = finalScore;
    }

    private void SaveHighScore(int score)
    {
        string encryptedScore = Convert.ToBase64String(BitConverter.GetBytes(score));
        PlayerPrefs.SetString("HighScore", encryptedScore);
        PlayerPrefs.Save();
    }

    private int LoadHighScore()
    {
        string encryptedScore = PlayerPrefs.GetString("HighScore");
        if (!string.IsNullOrEmpty(encryptedScore))
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(encryptedScore);
                return BitConverter.ToInt32(bytes, 0);
            }
            catch
            {
                Debug.LogWarning("Failed to decode high score.");
            }
        }
        return 0;
    }
}

using UnityEngine;
using TMPro;
using System;

public class SecureScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    private float score = 0f;
    public float scoreIncreaseRate = 10f;

    private int displayedScore = 0;
    private int highScore = 0;

    public int CurrentScore => displayedScore; // 현재 점수를 반환하는 프로퍼티 추가

    void Start()
    {
        highScore = LoadHighScore();
        UpdateHighScoreText();
    }

    void Update()
    {
        score += scoreIncreaseRate * Time.deltaTime;
        int newScore = Mathf.FloorToInt(score / 10) * 10;

        if (newScore != displayedScore)
        {
            displayedScore = newScore;
            scoreText.text = displayedScore.ToString();

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
        highScoreText.text = highScore.ToString();
    }

    private void SaveHighScore(int score)
    {
        string encryptedScore = Convert.ToBase64String(BitConverter.GetBytes(score));
        PlayerPrefs.SetString("", encryptedScore);
        PlayerPrefs.Save();
    }

    private int LoadHighScore()
    {
        string encryptedScore = PlayerPrefs.GetString("");
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
                return 0;
            }
        }
        return 0;
    }
}

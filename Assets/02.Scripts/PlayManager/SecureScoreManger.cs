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

    public void AddScore(int amount)
    {
        score += amount; // 점수 추가
        displayedScore = Mathf.FloorToInt(score);
        scoreText.text = displayedScore.ToString();

        if (displayedScore > highScore)
        {
            highScore = displayedScore;
            UpdateHighScoreText();
            SaveHighScore(highScore);
        }
    }
    public void OnGameClear()
    {
        PlayerPrefs.SetInt("FinalScore", CurrentScore); // 점수 저장
        PlayerPrefs.Save();
        Debug.Log($"FinalScore 저장됨: {CurrentScore}");
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

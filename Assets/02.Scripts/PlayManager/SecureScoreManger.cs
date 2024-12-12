using System; // �߰��� �κ�
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SecureScoreManager : MonoBehaviour
{
    public static SecureScoreManager Instance { get; private set; }
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    private float score = 0f;
    public float scoreIncreaseRate = 10f;

    private int displayedScore = 0;
    private int highScore = 0;

    public int CurrentScore => displayedScore;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // SecureScoreManager�� root GameObject���� Ȯ��
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        highScore = LoadHighScore();
        UpdateHighScoreText();
        ResetScore(); // ���� �ʱ�ȭ
    }

    void Update()
    {
        score += scoreIncreaseRate * Time.deltaTime;
        int newScore = Mathf.FloorToInt(score / 10) * 10;

        if (newScore != displayedScore)
        {
            displayedScore = newScore;

            if (scoreText != null)
                scoreText.text = displayedScore.ToString();

            if (displayedScore > highScore)
            {
                highScore = displayedScore;
                if (highScoreText != null)
                    highScoreText.text = highScore.ToString();
                SaveHighScore(highScore);
            }
        }
    }

    private void UpdateHighScoreText()
    {
        if (highScoreText != null)
            highScoreText.text = highScore.ToString();
    }

    public void RebindUI(TMP_Text newScoreText, TMP_Text newHighScoreText)
    {
        scoreText = newScoreText;
        highScoreText = newHighScoreText;
        UpdateHighScoreText(); // ���̽��ھ� UI ������Ʈ
        ResetScore(); // ���� ���� UI �ʱ�ȭ
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
                return 0;
            }
        }
        return 0;
    }

    public void SetCurrentScore(int newScore)
    {
        displayedScore = newScore;
        score = newScore;

        if (scoreText != null)
            scoreText.text = displayedScore.ToString();
    }

    // ���� �ʱ�ȭ �޼���
    public void ResetScore()
    {
        score = 0f;
        displayedScore = 0;

        if (scoreText != null)
            scoreText.text = displayedScore.ToString();
    }
}

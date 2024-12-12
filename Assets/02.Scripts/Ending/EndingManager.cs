using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    public CanvasGroup blackScreen;
    public CanvasGroup endingScene;
    public CanvasGroup scoreScreen;
    public TextMeshProUGUI scoreText;
    public Button restartButton;

    public AudioSource effectAudioSource;
    public AudioSource musicAudioSource;
    public AudioClip soundEffect;
    public AudioClip backgroundMusic;

    private int finalScore = 0;

    void Start()
    {
        // SecureScoreManager�κ��� ���� ���� ��������
        if (SecureScoreManager.Instance != null)
        {
            finalScore = SecureScoreManager.Instance.CurrentScore;
        }
        else
        {
            Debug.LogWarning("SecureScoreManager �ν��Ͻ��� ã�� �� �����ϴ�.");
        }

        // �ʱ�ȭ: ��� ȭ���� ��Ȱ��ȭ
        SetCanvasAlpha(blackScreen, 1);
        SetCanvasAlpha(endingScene, 0);
        SetCanvasAlpha(scoreScreen, 0);

        // ��ư Ŭ�� �̺�Ʈ ���
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        // �ܰ��� ���� ����
        StartCoroutine(PlayEndingSequence());
    }

    private void OnRestartButtonClicked()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �̺�Ʈ ���

        if (SecureScoreManager.Instance != null)
        {
            SecureScoreManager.Instance.ResetScore(); // ���� �ʱ�ȭ
        }

        SceneManager.LoadScene("Title"); // Ÿ��Ʋ �� �ε�
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Ÿ��Ʋ �������� UI�� �翬������ ����
        if (scene.name == "Title")
        {
            Debug.Log("Title scene loaded, no UI to rebind.");
            return;
        }

        // ���� �� �Ǵ� Ŭ���� ������ UI �翬��
        TMP_Text newScoreText = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();
        TMP_Text newHighScoreText = GameObject.Find("HighScoreText")?.GetComponent<TMP_Text>();

        if (SecureScoreManager.Instance != null && newScoreText != null && newHighScoreText != null)
        {
            SecureScoreManager.Instance.RebindUI(newScoreText, newHighScoreText);
        }
        else
        {
            Debug.LogWarning("Failed to rebind SecureScoreManager UI in scene: " + scene.name);
        }
    }


    IEnumerator PlayEndingSequence()
    {
        if (effectAudioSource != null && soundEffect != null)
        {
            effectAudioSource.PlayOneShot(soundEffect);
        }
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(FadeCanvas(blackScreen, false));

        if (musicAudioSource != null && backgroundMusic != null)
        {
            musicAudioSource.clip = backgroundMusic;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }
        yield return StartCoroutine(FadeCanvas(endingScene, true));
        yield return new WaitForSeconds(5f);

        StartCoroutine(FadeCanvas(endingScene, false));
        yield return StartCoroutine(FadeCanvas(scoreScreen, true));

        yield return StartCoroutine(AnimateScore(0, finalScore));
    }

    IEnumerator AnimateScore(int startScore, int endScore)
    {
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int currentScore = Mathf.FloorToInt(Mathf.Lerp(startScore, endScore, elapsed / duration));
            if (scoreText != null)
                scoreText.text = currentScore.ToString();
            yield return null;
        }

        if (scoreText != null)
            scoreText.text = endScore.ToString();
    }

    IEnumerator FadeCanvas(CanvasGroup canvasGroup, bool fadeIn)
    {
        float duration = 1f;
        float elapsed = 0f;
        float startAlpha = fadeIn ? 0 : 1;
        float endAlpha = fadeIn ? 1 : 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = endAlpha;
    }

    void SetCanvasAlpha(CanvasGroup canvasGroup, float alpha)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
    }
}

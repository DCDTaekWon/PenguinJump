using System.Collections;
using UnityEngine;
using TMPro; // TextMeshPro ���ӽ����̽� �߰�
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    public CanvasGroup blackScreen; // ���� ȭ��
    public CanvasGroup endingScene; // ���� �ƽ� ȭ��
    public CanvasGroup scoreScreen; // ���� ǥ�� ȭ��
    public TextMeshProUGUI scoreText; // TextMeshPro�� ���� ǥ��
    public Button restartButton; // "ó������" ��ư

    public AudioSource effectAudioSource; // ȿ������ ���� AudioSource
    public AudioSource musicAudioSource; // ��������� ���� AudioSource
    public AudioClip soundEffect; // ���� ȭ�鿡�� ���� ȿ����
    public AudioClip backgroundMusic; // ��� �ƽ��� �������

    private int finalScore = 12345; // ���� ����. ���� ������ �ҷ����� ���� ����.
                                    // EndingManager.cs
    void Start()
    {
        // PlayerPrefs���� ������ ������
        finalScore = PlayerPrefs.GetInt("FinalScore", 0); // ���� �⺻���� 0

        // �ʱ�ȭ �� �ڷ�ƾ ����
        SetCanvasAlpha(blackScreen, 1);
        SetCanvasAlpha(endingScene, 0);
        SetCanvasAlpha(scoreScreen, 0);

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        StartCoroutine(PlayEndingSequence());
    }


    IEnumerator PlayEndingSequence()
    {
        // 1�ܰ�: ���� ȭ�� ���� �� ȿ���� ��� �� ���̵� �ƿ�
        if (effectAudioSource != null && soundEffect != null)
        {
            effectAudioSource.PlayOneShot(soundEffect); // ȿ���� ���
        }
        yield return new WaitForSeconds(5f); // 5�� ���
        yield return StartCoroutine(FadeCanvas(blackScreen, false)); // ���� ȭ�� ���̵� �ƿ�

        // 2�ܰ�: ���� �ƽ� ȭ�� ǥ�� �� ������� ���
        if (musicAudioSource != null && backgroundMusic != null)
        {
            musicAudioSource.clip = backgroundMusic;
            musicAudioSource.loop = true; // ������� �ݺ� ����
            musicAudioSource.Play(); // ������� ���
        }
        yield return StartCoroutine(FadeCanvas(endingScene, true)); // ���� �ƽ� ���̵� ��
        yield return new WaitForSeconds(5f); // 4�� ���� ���� �ƽ� ����

        // 3�ܰ�: ���� ȭ�� ǥ��
        StartCoroutine(FadeCanvas(endingScene, false)); // ���� �ƽ� ���̵� �ƿ�
        yield return StartCoroutine(FadeCanvas(scoreScreen, true)); // ���� ȭ�� ���̵� ��

        // ���� �ִϸ��̼� ����
        yield return StartCoroutine(AnimateScore(0, finalScore));
    }

    // ���� �ִϸ��̼� (���� ǥ��)
    IEnumerator AnimateScore(int startScore, int endScore)
    {
        float duration = 2f; // �ִϸ��̼� ���� �ð�
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int currentScore = Mathf.FloorToInt(Mathf.Lerp(startScore, endScore, elapsed / duration));
            if (scoreText != null)
                scoreText.text = currentScore.ToString();
            yield return null;
        }

        // ���� ���� ����
        if (scoreText != null)
            scoreText.text = endScore.ToString();
    }

    // ���̵� ó�� �Լ�
    IEnumerator FadeCanvas(CanvasGroup canvasGroup, bool fadeIn)
    {
        float duration = 1f; // ���̵� �ð�
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

    // ĵ���� ���İ� ����
    void SetCanvasAlpha(CanvasGroup canvasGroup, float alpha)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
    }

    // "ó������" ��ư Ŭ�� �� ����
    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene("Title");
    }
}

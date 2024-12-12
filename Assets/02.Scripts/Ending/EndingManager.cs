using System.Collections;
using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스 추가
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    public CanvasGroup blackScreen; // 검정 화면
    public CanvasGroup endingScene; // 엔딩 컷신 화면
    public CanvasGroup scoreScreen; // 점수 표시 화면
    public TextMeshProUGUI scoreText; // TextMeshPro로 점수 표시
    public Button restartButton; // "처음으로" 버튼

    public AudioSource effectAudioSource; // 효과음을 위한 AudioSource
    public AudioSource musicAudioSource; // 배경음악을 위한 AudioSource
    public AudioClip soundEffect; // 검정 화면에서 나올 효과음
    public AudioClip backgroundMusic; // 펭귄 컷신의 배경음악

    private int finalScore = 0;

    void Start()
    {
        if (SecureScoreManager.Instance != null)
        {
            finalScore = SecureScoreManager.Instance.CurrentScore;
            Debug.Log($"EndingManager: Retrieved final score: {finalScore}");

            if (scoreText != null)
                scoreText.text = finalScore.ToString(); // UI에 최종 점수 반영
        }
        else
        {
            Debug.LogError("SecureScoreManager.Instance가 null입니다. 확인 필요.");
        }

        // UI 초기화
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
        // 1단계: 검정 화면 유지 후 효과음 재생 및 페이드 아웃
        if (effectAudioSource != null && soundEffect != null)
        {
            effectAudioSource.PlayOneShot(soundEffect); // 효과음 재생
        }
        yield return new WaitForSeconds(5f); // 5초 대기
        yield return StartCoroutine(FadeCanvas(blackScreen, false)); // 검정 화면 페이드 아웃

        // 2단계: 엔딩 컷신 화면 표시 및 배경음악 재생
        if (musicAudioSource != null && backgroundMusic != null)
        {
            musicAudioSource.clip = backgroundMusic;
            musicAudioSource.loop = true; // 배경음악 반복 설정
            musicAudioSource.Play(); // 배경음악 재생
        }
        yield return StartCoroutine(FadeCanvas(endingScene, true)); // 엔딩 컷신 페이드 인
        yield return new WaitForSeconds(5f); // 5초 대기

        // 3단계: 점수 화면 표시
        StartCoroutine(FadeCanvas(endingScene, false)); // 엔딩 컷신 페이드 아웃
        yield return StartCoroutine(FadeCanvas(scoreScreen, true)); // 점수 화면 페이드 인

        // 최종 점수 애니메이션 시작
        yield return StartCoroutine(AnimateScore(0, finalScore));
    }

    // 점수 애니메이션 (점수 표시)
    IEnumerator AnimateScore(int startScore, int endScore)
    {
        float duration = 2f; // 애니메이션 지속 시간
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int currentScore = Mathf.FloorToInt(Mathf.Lerp(startScore, endScore, elapsed / duration));
            if (scoreText != null)
                scoreText.text = currentScore.ToString();
            yield return null;
        }

        // 최종 점수 설정
        if (scoreText != null)
            scoreText.text = endScore.ToString();
    }

    // 페이드 처리 함수
    IEnumerator FadeCanvas(CanvasGroup canvasGroup, bool fadeIn)
    {
        float duration = 1f; // 페이드 시간
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

    // 캔버스 알파값 설정
    void SetCanvasAlpha(CanvasGroup canvasGroup, float alpha)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
    }

    public void OnRestartButtonClicked()
    {
        if (SecureScoreManager.Instance != null)
        {
            SecureScoreManager.Instance.ResetScore();
            Debug.Log("EndingManager: Score reset to 0.");
        }

        SceneManager.LoadScene("Title");
    }
}

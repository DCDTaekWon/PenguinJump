using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager 사용

public class ProgressBarManager : MonoBehaviour
{
    public Transform progressBarTransform; // ProgressBar의 Transform (기준)
    public RectTransform spriteTransform; // 이동할 스프라이트 (UI RectTransform)
    public float startX = 0f; // ProgressBar를 기준으로 시작 X좌표
    public float targetX = 500f; // ProgressBar를 기준으로 목표 X좌표
    public float duration = 240f; // 4분 (240초)

    private float elapsedTime = 0f;

    void Start()
    {
        // 스프라이트의 초기 위치를 ProgressBar 기준으로 설정
        Vector3 startPosition = spriteTransform.localPosition; // 로컬 좌표 사용
        startPosition.x = startX;
        spriteTransform.localPosition = startPosition;

        // 이동 시작
        StartCoroutine(MoveSprite());
    }

    private System.Collections.IEnumerator MoveSprite()
    {
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            float newX = Mathf.Lerp(startX, targetX, progress);
            Vector3 currentPosition = spriteTransform.localPosition;
            currentPosition.x = newX;
            spriteTransform.localPosition = currentPosition;

            yield return null; // 다음 프레임까지 대기
        }

        Vector3 finalPosition = spriteTransform.localPosition;
        finalPosition.x = targetX;
        spriteTransform.localPosition = finalPosition;

        // SecureScoreManager의 OnGameClear 호출
        SecureScoreManager scoreManager = FindObjectOfType<SecureScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.OnGameClear(); // 인스턴스를 통해 호출
        }
        else
        {
            Debug.LogError("SecureScoreManager를 찾을 수 없습니다!");
        }

        // Clear 씬으로 이동
        SceneManager.LoadScene("Clear");
    }
}

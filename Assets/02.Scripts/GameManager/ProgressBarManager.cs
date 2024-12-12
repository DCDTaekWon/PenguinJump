using UnityEngine;
using UnityEngine.SceneManagement; // 씬 로드를 위해 추가

public class ProgressBarManager : MonoBehaviour
{
    public Transform progressBarTransform; // ProgressBar의 Transform (기준)
    public RectTransform spriteTransform; // 이동할 스프라이트 (UI RectTransform)
    public float startX = 0f; // ProgressBar를 기준으로 시작 X좌표
    public float targetX = 500f; // ProgressBar를 기준으로 목표 X좌표
    public float duration = 5f; // 기본 클리어 시간 (초)

    private float elapsedTime = 0f;

    void Start()
    {
        ResetProgressBar(); // ProgressBar 초기화
        StartCoroutine(MoveSprite());
    }

    void Update()
    {
        // 클리어 시간이 변경되었을 때 UI를 최신화
        if (Input.GetKeyDown(KeyCode.R)) // 예: R 키로 클리어 시간을 변경
        {
            duration = 180f; // 예: 새로운 클리어 시간 (180초)
            ResetProgressBar();
            StopAllCoroutines();
            StartCoroutine(MoveSprite());
        }
    }

    private void ResetProgressBar()
    {
        // 스프라이트의 초기 위치를 ProgressBar 기준으로 설정
        elapsedTime = 0f;
        Vector3 startPosition = spriteTransform.localPosition; // 로컬 좌표 사용
        startPosition.x = startX;
        spriteTransform.localPosition = startPosition;
    }

    private System.Collections.IEnumerator MoveSprite()
    {
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // 진행 비율 계산
            float progress = elapsedTime / duration;

            // 로컬 X좌표를 선형 보간
            float newX = Mathf.Lerp(startX, targetX, progress);

            // 스프라이트 위치 갱신 (로컬 좌표 기준)
            Vector3 currentPosition = spriteTransform.localPosition;
            currentPosition.x = newX;
            spriteTransform.localPosition = currentPosition;

            yield return null; // 다음 프레임까지 대기
        }

        // 마지막 위치 보정
        Vector3 finalPosition = spriteTransform.localPosition;
        finalPosition.x = targetX;
        spriteTransform.localPosition = finalPosition;

        // 클리어 신 로드
        LoadClearScene();
    }

    private void LoadClearScene()
    {
        // SecureScoreManager의 점수를 EndingManager가 읽을 수 있도록 최신화
        if (SecureScoreManager.Instance != null)
        {
            int finalScore = Mathf.FloorToInt(elapsedTime * SecureScoreManager.Instance.scoreIncreaseRate);
            SecureScoreManager.Instance.SetCurrentScore(finalScore);
        }

        SceneManager.LoadScene("Clear"); // 클리어 씬 로드
    }


}

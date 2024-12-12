using UnityEngine;

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
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MemoryPanelController : MonoBehaviour
{
    public GameObject memoryPanel; // Memory 패널
    public Image[] memoryImages;   // 01, 02, 03, 04 이미지를 설정
    public float displayTime = 1.0f; // 각 이미지가 고정된 시간
    public float fadeDuration = 0.5f; // 페이드 효과 시간

    private Coroutine currentCoroutine; // 현재 실행 중인 코루틴을 관리

    private void Start()
    {
        // 처음에는 패널과 이미지를 모두 비활성화
        memoryPanel.SetActive(false);
        ResetImages();
    }

    public void OnMemoryButtonClicked()
    {
        // 기존 실행 중인 코루틴을 중지
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 코루틴을 실행하여 페이드 효과와 함께 패널과 이미지를 순차적으로 표시
        currentCoroutine = StartCoroutine(ShowMemoryPanelAndImagesWithFade());
    }

    public void OnBackButtonClicked()
    {
        // 기존 실행 중인 코루틴 중지
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        // 패널과 모든 이미지를 비활성화하고 초기화
        memoryPanel.SetActive(false);
        ResetImages();
    }

    private void ResetImages()
    {
        // 모든 이미지를 초기 상태로 되돌림
        foreach (var image in memoryImages)
        {
            image.gameObject.SetActive(false);
            var color = image.color;
            color.a = 0; // 알파값 초기화
            image.color = color;
        }
    }

    private IEnumerator ShowMemoryPanelAndImagesWithFade()
    {
        // 메모리 패널 활성화
        memoryPanel.SetActive(true);

        // 이미지를 순차적으로 페이드 인
        for (int i = 0; i < memoryImages.Length; i++)
        {
            memoryImages[i].gameObject.SetActive(true); // 이미지 활성화
            yield return StartCoroutine(FadeIn(memoryImages[i])); // 페이드 인

            // 이전 이미지 유지하면서 일정 시간 대기
            yield return new WaitForSeconds(displayTime);
        }

        // 마지막 이미지가 활성화된 상태에서 모든 이미지가 그대로 보여짐
        yield return null; // 모든 동작 완료 후 대기
    }

    private IEnumerator FadeIn(Image image)
    {
        float elapsedTime = 0f;
        Color color = image.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            image.color = color;
            yield return null;
        }

        color.a = 1f; // 최종 알파값 설정
        image.color = color;
    }
}

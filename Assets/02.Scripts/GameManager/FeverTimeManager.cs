using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FeverTimeManager : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform feverImageRect; // FeverTime 이미지 RectTransform
    public Vector3 offScreenStartPosition; // 시작 위치 (화면 밖)
    public Vector3 centerPosition; // 중앙 위치
    public Vector3 offScreenEndPosition; // 사라질 위치 (화면 밖)
    public float moveDuration = 0.5f; // 이미지 이동 시간
    public float stayDuration = 1.5f; // 중앙에서 머무르는 시간

    [Header("Fever Time Settings")]
    public float feverDuration = 5f; // 피버타임 지속 시간
    public float initialDelay = 144f; // 피버타임 시작 대기 시간

    [Header("Audio Settings")]
    public AudioSource audioSource; // 게임 오브젝트의 AudioSource
    public AudioClip feverBGM; // 피버타임 브금

    private bool isFeverActive = false;

    public bool IsFeverActive => isFeverActive; // 피버타임 활성 상태 반환

    private void Start()
    {
        // FeverTime 이미지 비활성화
        if (feverImageRect != null)
        {
            feverImageRect.gameObject.SetActive(false);
        }

        // 피버타임 시작 코루틴
        Invoke(nameof(StartFeverTime), initialDelay);
    }

    private void StartFeverTime()
    {
        if (isFeverActive) return;

        isFeverActive = true;
        Debug.Log("피버타임 활성화!");

        feverImageRect.gameObject.SetActive(true);
        feverImageRect.anchoredPosition = offScreenStartPosition;

        Sequence feverSequence = DOTween.Sequence();

        feverSequence.Append(feverImageRect.DOAnchorPos(centerPosition, moveDuration).SetEase(Ease.OutQuad));
        feverSequence.AppendInterval(stayDuration);
        feverSequence.Append(feverImageRect.DOAnchorPos(offScreenEndPosition, moveDuration).SetEase(Ease.InQuad));

        feverSequence.OnComplete(() =>
        {
            feverImageRect.gameObject.SetActive(false);
            Debug.Log("피버타임 종료!");
            //isFeverActive = false;
        });

        if (audioSource != null && feverBGM != null)
        {
            audioSource.clip = feverBGM;
            audioSource.Play();
        }

        feverSequence.Play();
    }

}

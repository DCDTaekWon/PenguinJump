using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FeverTimeManager : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform feverImageRect; // FeverTime �̹��� RectTransform
    public Vector3 offScreenStartPosition; // ���� ��ġ (ȭ�� ��)
    public Vector3 centerPosition; // �߾� ��ġ
    public Vector3 offScreenEndPosition; // ����� ��ġ (ȭ�� ��)
    public float moveDuration = 0.5f; // �̹��� �̵� �ð�
    public float stayDuration = 1.5f; // �߾ӿ��� �ӹ����� �ð�

    [Header("Fever Time Settings")]
    public float feverDuration = 5f; // �ǹ�Ÿ�� ���� �ð�
    public float initialDelay = 144f; // �ǹ�Ÿ�� ���� ��� �ð�

    [Header("Audio Settings")]
    public AudioSource audioSource; // ���� ������Ʈ�� AudioSource
    public AudioClip feverBGM; // �ǹ�Ÿ�� ���

    private bool isFeverActive = false;

    public bool IsFeverActive => isFeverActive; // �ǹ�Ÿ�� Ȱ�� ���� ��ȯ

    private void Start()
    {
        // FeverTime �̹��� ��Ȱ��ȭ
        if (feverImageRect != null)
        {
            feverImageRect.gameObject.SetActive(false);
        }

        // �ǹ�Ÿ�� ���� �ڷ�ƾ
        Invoke(nameof(StartFeverTime), initialDelay);
    }

    private void StartFeverTime()
    {
        if (isFeverActive) return;

        isFeverActive = true;
        Debug.Log("�ǹ�Ÿ�� Ȱ��ȭ!");

        feverImageRect.gameObject.SetActive(true);
        feverImageRect.anchoredPosition = offScreenStartPosition;

        Sequence feverSequence = DOTween.Sequence();

        feverSequence.Append(feverImageRect.DOAnchorPos(centerPosition, moveDuration).SetEase(Ease.OutQuad));
        feverSequence.AppendInterval(stayDuration);
        feverSequence.Append(feverImageRect.DOAnchorPos(offScreenEndPosition, moveDuration).SetEase(Ease.InQuad));

        feverSequence.OnComplete(() =>
        {
            feverImageRect.gameObject.SetActive(false);
            Debug.Log("�ǹ�Ÿ�� ����!");
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

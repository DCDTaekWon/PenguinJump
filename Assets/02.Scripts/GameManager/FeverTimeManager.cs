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
    public AudioClip defaultBGM; // �⺻ �������� ���

    private bool isFeverActive = false;

    public bool IsFeverActive => isFeverActive; // �ǹ�Ÿ�� Ȱ�� ���� ��ȯ

    private void Start()
    {
        // FeverTime �̹��� ��Ȱ��ȭ
        if (feverImageRect != null)
        {
            feverImageRect.gameObject.SetActive(false);
        }

        // �⺻ ��� ���
        if (audioSource != null && defaultBGM != null)
        {
            audioSource.clip = defaultBGM; // �⺻ ����� ����
            audioSource.volume = 0.8f; // �ʱ� ���� ����
            audioSource.Play(); // ��� ����
        }

        // �ǹ�Ÿ�� ���� �ڷ�ƾ
        Invoke(nameof(StartFeverTime), initialDelay);
    }

    private void StartFeverTime()
    {
        if (isFeverActive) return;

        isFeverActive = true;
        Debug.Log("�ǹ�Ÿ�� Ȱ��ȭ!");

        // FeverTime UI Ȱ��ȭ �� �ִϸ��̼� ����
        feverImageRect.gameObject.SetActive(true);
        feverImageRect.anchoredPosition = offScreenStartPosition;

        Sequence feverSequence = DOTween.Sequence();

        feverSequence.Append(feverImageRect.DOAnchorPos(centerPosition, moveDuration).SetEase(Ease.OutQuad));
        feverSequence.AppendInterval(stayDuration);
        feverSequence.Append(feverImageRect.DOAnchorPos(offScreenEndPosition, moveDuration).SetEase(Ease.InQuad));

        feverSequence.OnComplete(() =>
        {
            feverImageRect.gameObject.SetActive(false);
        });

        // FeverTime ��� ���
        if (audioSource != null && feverBGM != null)
        {
            audioSource.clip = feverBGM; // FeverTime ��� ����
            audioSource.volume = 0.8f; // ���� ����
            audioSource.Play(); // ���
        }

        // �ǹ�Ÿ�� ���� ���� (feverDuration ����)
        Invoke(nameof(EndFeverTime), feverDuration);

        feverSequence.Play();
    }

    private void EndFeverTime()
    {
        isFeverActive = false;

        Debug.Log("�ǹ�Ÿ�� ����!");

        // �⺻ ��� ����
        if (audioSource != null && defaultBGM != null)
        {
            audioSource.clip = defaultBGM; // �⺻ ������� ����
            audioSource.volume = 1f; // ������ ���� ����
            audioSource.Play(); // ��� ����
        }
    }
}

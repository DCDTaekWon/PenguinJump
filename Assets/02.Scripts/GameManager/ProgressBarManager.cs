using UnityEngine;

public class ProgressBarManager : MonoBehaviour
{
    public Transform progressBarTransform; // ProgressBar�� Transform (����)
    public RectTransform spriteTransform; // �̵��� ��������Ʈ (UI RectTransform)
    public float startX = 0f; // ProgressBar�� �������� ���� X��ǥ
    public float targetX = 500f; // ProgressBar�� �������� ��ǥ X��ǥ
    public float duration = 240f; // 4�� (240��)

    private float elapsedTime = 0f;

    void Start()
    {
        // ��������Ʈ�� �ʱ� ��ġ�� ProgressBar �������� ����
        Vector3 startPosition = spriteTransform.localPosition; // ���� ��ǥ ���
        startPosition.x = startX;
        spriteTransform.localPosition = startPosition;

        // �̵� ����
        StartCoroutine(MoveSprite());
    }

    private System.Collections.IEnumerator MoveSprite()
    {
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // ���� ���� ���
            float progress = elapsedTime / duration;

            // ���� X��ǥ�� ���� ����
            float newX = Mathf.Lerp(startX, targetX, progress);

            // ��������Ʈ ��ġ ���� (���� ��ǥ ����)
            Vector3 currentPosition = spriteTransform.localPosition;
            currentPosition.x = newX;
            spriteTransform.localPosition = currentPosition;

            yield return null; // ���� �����ӱ��� ���
        }

        // ������ ��ġ ����
        Vector3 finalPosition = spriteTransform.localPosition;
        finalPosition.x = targetX;
        spriteTransform.localPosition = finalPosition;
    }
}

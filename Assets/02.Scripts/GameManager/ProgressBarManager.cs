using UnityEngine;
using UnityEngine.SceneManagement; // �� �ε带 ���� �߰�

public class ProgressBarManager : MonoBehaviour
{
    public Transform progressBarTransform; // ProgressBar�� Transform (����)
    public RectTransform spriteTransform; // �̵��� ��������Ʈ (UI RectTransform)
    public float startX = 0f; // ProgressBar�� �������� ���� X��ǥ
    public float targetX = 500f; // ProgressBar�� �������� ��ǥ X��ǥ
    public float duration = 5f; // �⺻ Ŭ���� �ð� (��)

    private float elapsedTime = 0f;

    void Start()
    {
        ResetProgressBar(); // ProgressBar �ʱ�ȭ
        StartCoroutine(MoveSprite());
    }

    void Update()
    {
        // Ŭ���� �ð��� ����Ǿ��� �� UI�� �ֽ�ȭ
        if (Input.GetKeyDown(KeyCode.R)) // ��: R Ű�� Ŭ���� �ð��� ����
        {
            duration = 180f; // ��: ���ο� Ŭ���� �ð� (180��)
            ResetProgressBar();
            StopAllCoroutines();
            StartCoroutine(MoveSprite());
        }
    }

    private void ResetProgressBar()
    {
        // ��������Ʈ�� �ʱ� ��ġ�� ProgressBar �������� ����
        elapsedTime = 0f;
        Vector3 startPosition = spriteTransform.localPosition; // ���� ��ǥ ���
        startPosition.x = startX;
        spriteTransform.localPosition = startPosition;
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

        // Ŭ���� �� �ε�
        LoadClearScene();
    }

    private void LoadClearScene()
    {
        // SecureScoreManager�� ������ EndingManager�� ���� �� �ֵ��� �ֽ�ȭ
        if (SecureScoreManager.Instance != null)
        {
            int finalScore = Mathf.FloorToInt(elapsedTime * SecureScoreManager.Instance.scoreIncreaseRate);
            SecureScoreManager.Instance.SetCurrentScore(finalScore);
        }

        SceneManager.LoadScene("Clear"); // Ŭ���� �� �ε�
    }


}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MemoryPanelController : MonoBehaviour
{
    public GameObject memoryPanel; // Memory �г�
    public Image[] memoryImages;   // 01, 02, 03, 04 �̹����� ����
    public float displayTime = 1.0f; // �� �̹����� ������ �ð�
    public float fadeDuration = 0.5f; // ���̵� ȿ�� �ð�

    private Coroutine currentCoroutine; // ���� ���� ���� �ڷ�ƾ�� ����

    private void Start()
    {
        // ó������ �гΰ� �̹����� ��� ��Ȱ��ȭ
        memoryPanel.SetActive(false);
        ResetImages();
    }

    public void OnMemoryButtonClicked()
    {
        // ���� ���� ���� �ڷ�ƾ�� ����
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // �ڷ�ƾ�� �����Ͽ� ���̵� ȿ���� �Բ� �гΰ� �̹����� ���������� ǥ��
        currentCoroutine = StartCoroutine(ShowMemoryPanelAndImagesWithFade());
    }

    public void OnBackButtonClicked()
    {
        // ���� ���� ���� �ڷ�ƾ ����
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        // �гΰ� ��� �̹����� ��Ȱ��ȭ�ϰ� �ʱ�ȭ
        memoryPanel.SetActive(false);
        ResetImages();
    }

    private void ResetImages()
    {
        // ��� �̹����� �ʱ� ���·� �ǵ���
        foreach (var image in memoryImages)
        {
            image.gameObject.SetActive(false);
            var color = image.color;
            color.a = 0; // ���İ� �ʱ�ȭ
            image.color = color;
        }
    }

    private IEnumerator ShowMemoryPanelAndImagesWithFade()
    {
        // �޸� �г� Ȱ��ȭ
        memoryPanel.SetActive(true);

        // �̹����� ���������� ���̵� ��
        for (int i = 0; i < memoryImages.Length; i++)
        {
            memoryImages[i].gameObject.SetActive(true); // �̹��� Ȱ��ȭ
            yield return StartCoroutine(FadeIn(memoryImages[i])); // ���̵� ��

            // ���� �̹��� �����ϸ鼭 ���� �ð� ���
            yield return new WaitForSeconds(displayTime);
        }

        // ������ �̹����� Ȱ��ȭ�� ���¿��� ��� �̹����� �״�� ������
        yield return null; // ��� ���� �Ϸ� �� ���
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

        color.a = 1f; // ���� ���İ� ����
        image.color = color;
    }
}

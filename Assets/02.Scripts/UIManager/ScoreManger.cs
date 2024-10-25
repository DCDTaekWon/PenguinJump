using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;  // ������ ǥ���� UI �ؽ�Ʈ
    private float score = 0f;  // ���ھ� ���� float���� ����
    public float scoreIncreaseRate = 10f;  // �ʴ� ���ھ ������ ��

    private int displayedScore = 0;  // ȭ�鿡 ǥ���� ���ھ� ��

    void Update()
    {
        // �ð��� ���� ���ھ� ����
        score += scoreIncreaseRate * Time.deltaTime;

        // ���� ���ھ�� ȭ�鿡 ǥ�õ� ���ھ� ��
        int newScore = Mathf.FloorToInt(score / 10) * 10;

        if (newScore != displayedScore)
        {
            // ���ھ� ����
            displayedScore = newScore;
            scoreText.text = "Score: " + displayedScore.ToString();
        }
    }
}


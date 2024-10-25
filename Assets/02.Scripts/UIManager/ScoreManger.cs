using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;  // 점수를 표시할 UI 텍스트
    private float score = 0f;  // 스코어 값을 float으로 설정
    public float scoreIncreaseRate = 10f;  // 초당 스코어가 오르는 양

    private int displayedScore = 0;  // 화면에 표시할 스코어 값

    void Update()
    {
        // 시간에 따라 스코어 증가
        score += scoreIncreaseRate * Time.deltaTime;

        // 현재 스코어와 화면에 표시된 스코어 비교
        int newScore = Mathf.FloorToInt(score / 10) * 10;

        if (newScore != displayedScore)
        {
            // 스코어 갱신
            displayedScore = newScore;
            scoreText.text = "Score: " + displayedScore.ToString();
        }
    }
}


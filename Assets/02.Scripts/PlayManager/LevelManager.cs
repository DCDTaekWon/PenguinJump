using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelPhase
{
    public string phaseName;          // 구간 이름
    public int maxActivePlatforms;    // 최대 활성 발판 수
    public int largeRate;             // 큰 발판 비율
    public int mediumRate;            // 중간 발판 비율
    public int smallRate;             // 작은 발판 비율
    public float destroyTime;         // 발판 유지 시간
    public int maxFishCount;          // 최대 물고기 수
    public float duration;            // 구간 지속 시간
    public int columns;               // 발판 열 개수
    public int rows;                  // 발판 행 개수
}

public class LevelManager : MonoBehaviour
{
    [Header("Phase Settings")]
    public List<LevelPhase> levelPhases = new List<LevelPhase>();  // 구간 데이터 리스트
    private LevelPhase currentPhase;                              // 현재 구간 데이터
    private int currentPhaseIndex = 0;                            // 현재 구간 인덱스
    private float elapsedTime = 0f;                               // 게임 진행 시간

    [Header("Managers")]
    public PlatformManager platformManager;                       // 발판 관리 스크립트

    private void Start()
    {
        // 레벨 초기화
        InitializePhases();
        StartGame();
    }

    private void InitializePhases()
    {
        // 구간 데이터 추가
        levelPhases.Add(new LevelPhase
        {
            phaseName = "Easy",
            maxActivePlatforms = 10,
            largeRate = 10,
            mediumRate = 0,
            smallRate = 0,
            destroyTime = 7f,
            maxFishCount = 3,
            duration = 30f,
            columns = 5,
            rows = 5
        });

        levelPhases.Add(new LevelPhase
        {
            phaseName = "Normal",
            maxActivePlatforms = 15,
            largeRate = 7,
            mediumRate = 3,
            smallRate = 0,
            destroyTime = 6f,
            maxFishCount = 5,
            duration = 40f,
            columns = 6,
            rows = 6
        });

        levelPhases.Add(new LevelPhase
        {
            phaseName = "Hard",
            maxActivePlatforms = 20,
            largeRate = 5,
            mediumRate = 5,
            smallRate = 0,
            destroyTime = 5f,
            maxFishCount = 7,
            duration = 50f,
            columns = 7,
            rows = 7
        });

        levelPhases.Add(new LevelPhase
        {
            phaseName = "Extreme",
            maxActivePlatforms = 25,
            largeRate = 3,
            mediumRate = 4,
            smallRate = 3,
            destroyTime = 4f,
            maxFishCount = 10,
            duration = 60f,
            columns = 8,
            rows = 8
        });
    }

    public void StartGame()
    {
        Debug.Log("게임 시작");
        currentPhaseIndex = 0; // 첫 구간 설정
        currentPhase = levelPhases[currentPhaseIndex];

        // 첫 구간 발판 생성
        platformManager.UpdatePhase(currentPhase);
        Debug.Log($"Phase {currentPhaseIndex + 1} 시작: {currentPhase.phaseName}");

        StartCoroutine(GameFlow());
    }

    private IEnumerator GameFlow()
    {
        elapsedTime = 0f;

        while (elapsedTime < 240f) // 4분 동안 진행
        {
            // 현재 시간이 다음 Phase의 시작 시간 이상이면 Phase 전환
            if (currentPhaseIndex + 1 < levelPhases.Count &&
                elapsedTime > GetPhaseStartTime(currentPhaseIndex + 1))
            {
                currentPhaseIndex++;
                currentPhase = levelPhases[currentPhaseIndex];
                platformManager.UpdatePhase(currentPhase);
                Debug.Log($"Phase {currentPhaseIndex + 1}로 전환: {currentPhase.phaseName}");
            }

            // 매초마다 업데이트
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        // 4분 후 클리어 신으로 이동
        Debug.Log("게임 클리어!");
        LoadClearScene();
    }

    private float GetPhaseStartTime(int phaseIndex)
    {
        float startTime = 0f;
        for (int i = 0; i < phaseIndex; i++)
        {
            startTime += levelPhases[i].duration;
        }
        return startTime;
    }

    private void LoadClearScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("ClearScene");
    }
}

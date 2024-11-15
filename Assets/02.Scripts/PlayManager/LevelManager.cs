using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelPhase
{
    public string phaseName;
    public int maxActivePlatforms;
    public int minActivePlatforms;
    public int largeRate;
    public int mediumRate;
    public int smallRate;
    public float destroyTime;
    public int maxFishCount;
    public float duration;

    // 추가
    public int columns; // 발판의 열 개수
    public int rows;    // 발판의 행 개수
}


public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    public List<LevelPhase> levelPhases; // 구간 데이터 리스트
    private int currentPhaseIndex = 0;   // 현재 구간 인덱스
    private LevelPhase currentPhase;     // 현재 구간 데이터

    [Header("Managers")]
    public PlatformManager platformManager; // 발판 관리 스크립트     

    private bool isTransitioning = false;   // 구간 전환 중인지 여부

    public delegate void PhaseChanged(LevelPhase newPhase);
    public event PhaseChanged OnPhaseChanged; // 구간 변경 이벤트

    private void Start()
    {
        InitializeLevelPhases();   // 구간 데이터 초기화
        StartGame();               // 첫 구간 시작
    }

    private void InitializeLevelPhases()
    {
        levelPhases = new List<LevelPhase>
    {
        new LevelPhase
        {
            phaseName = "1Phase",
            maxActivePlatforms = 10,
            minActivePlatforms = 5,
            largeRate = 10,
            mediumRate = 0,
            smallRate = 0,
            destroyTime = 7f,
            maxFishCount = 5,
            duration = 30f,
            columns = 5, // 열 개수
            rows = 5     // 행 개수
        },
        new LevelPhase
        {
            phaseName = "2Phase",
            maxActivePlatforms = 15,
            minActivePlatforms = 10,
            largeRate = 5,
            mediumRate = 5,
            smallRate = 0,
            destroyTime = 6f,
            maxFishCount = 6,
            duration = 40f,
            columns = 6,
            rows = 6
        }
        // 추가 구간...
    };
    }


    public void StartGame()
    {
        SetCurrentPhase(0); // 첫 구간 설정
        StartCoroutine(PhaseTransitionTimer()); // 구간 전환 시작
    }

    private void SetCurrentPhase(int phaseIndex)
    {
        if (!IsValidPhaseIndex(phaseIndex))
        {
            Debug.LogError("Invalid phase index!");
            return;
        }

        currentPhaseIndex = phaseIndex;
        currentPhase = levelPhases[currentPhaseIndex];

        Debug.Log($"구간 시작: {currentPhase.phaseName}");

        // 발판 관리 호출
        platformManager.SpawnPlatforms(currentPhase);

        // 구간 변경 이벤트 호출
        OnPhaseChanged?.Invoke(currentPhase);
    }

    private IEnumerator PhaseTransitionTimer()
    {
        while (currentPhaseIndex < levelPhases.Count)
        {
            if (isTransitioning) yield break; // 이미 전환 중이면 종료
            isTransitioning = true;

            yield return new WaitForSeconds(currentPhase.duration); // 구간 지속 시간 대기

            if (currentPhaseIndex < levelPhases.Count - 1)
            {
                SetCurrentPhase(currentPhaseIndex + 1); // 다음 구간으로 전환
            }
            else
            {
                Debug.Log("모든 구간 완료!");
                break;
            }

            isTransitioning = false; // 전환 완료
        }
    }

    private bool IsValidPhaseIndex(int index)
    {
        return index >= 0 && index < levelPhases.Count;
    }
}

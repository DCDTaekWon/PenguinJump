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

    // �߰�
    public int columns; // ������ �� ����
    public int rows;    // ������ �� ����
}


public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    public List<LevelPhase> levelPhases; // ���� ������ ����Ʈ
    private int currentPhaseIndex = 0;   // ���� ���� �ε���
    private LevelPhase currentPhase;     // ���� ���� ������

    [Header("Managers")]
    public PlatformManager platformManager; // ���� ���� ��ũ��Ʈ     

    private bool isTransitioning = false;   // ���� ��ȯ ������ ����

    public delegate void PhaseChanged(LevelPhase newPhase);
    public event PhaseChanged OnPhaseChanged; // ���� ���� �̺�Ʈ

    private void Start()
    {
        InitializeLevelPhases();   // ���� ������ �ʱ�ȭ
        StartGame();               // ù ���� ����
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
            columns = 5, // �� ����
            rows = 5     // �� ����
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
        // �߰� ����...
    };
    }


    public void StartGame()
    {
        SetCurrentPhase(0); // ù ���� ����
        StartCoroutine(PhaseTransitionTimer()); // ���� ��ȯ ����
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

        Debug.Log($"���� ����: {currentPhase.phaseName}");

        // ���� ���� ȣ��
        platformManager.SpawnPlatforms(currentPhase);

        // ���� ���� �̺�Ʈ ȣ��
        OnPhaseChanged?.Invoke(currentPhase);
    }

    private IEnumerator PhaseTransitionTimer()
    {
        while (currentPhaseIndex < levelPhases.Count)
        {
            if (isTransitioning) yield break; // �̹� ��ȯ ���̸� ����
            isTransitioning = true;

            yield return new WaitForSeconds(currentPhase.duration); // ���� ���� �ð� ���

            if (currentPhaseIndex < levelPhases.Count - 1)
            {
                SetCurrentPhase(currentPhaseIndex + 1); // ���� �������� ��ȯ
            }
            else
            {
                Debug.Log("��� ���� �Ϸ�!");
                break;
            }

            isTransitioning = false; // ��ȯ �Ϸ�
        }
    }

    private bool IsValidPhaseIndex(int index)
    {
        return index >= 0 && index < levelPhases.Count;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelPhase
{
    public string phaseName;          // ���� �̸�
    public int maxActivePlatforms;    // �ִ� Ȱ�� ���� ��
    public int largeRate;             // ū ���� ����
    public int mediumRate;            // �߰� ���� ����
    public int smallRate;             // ���� ���� ����
    public float destroyTime;         // ���� ���� �ð�
    public int maxFishCount;          // �ִ� ����� ��
    public float duration;            // ���� ���� �ð�
    public int columns;               // ���� �� ����
    public int rows;                  // ���� �� ����
}

public class LevelManager : MonoBehaviour
{
    [Header("Phase Settings")]
    public List<LevelPhase> levelPhases = new List<LevelPhase>();  // ���� ������ ����Ʈ
    private LevelPhase currentPhase;                              // ���� ���� ������
    private int currentPhaseIndex = 0;                            // ���� ���� �ε���
    private float elapsedTime = 0f;                               // ���� ���� �ð�

    [Header("Managers")]
    public PlatformManager platformManager;                       // ���� ���� ��ũ��Ʈ

    private void Start()
    {
        // ���� �ʱ�ȭ
        InitializePhases();
        StartGame();
    }

    private void InitializePhases()
    {
        // ���� ������ �߰�
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
        Debug.Log("���� ����");
        currentPhaseIndex = 0; // ù ���� ����
        currentPhase = levelPhases[currentPhaseIndex];

        // ù ���� ���� ����
        platformManager.UpdatePhase(currentPhase);
        Debug.Log($"Phase {currentPhaseIndex + 1} ����: {currentPhase.phaseName}");

        StartCoroutine(GameFlow());
    }

    private IEnumerator GameFlow()
    {
        elapsedTime = 0f;

        while (elapsedTime < 240f) // 4�� ���� ����
        {
            // ���� �ð��� ���� Phase�� ���� �ð� �̻��̸� Phase ��ȯ
            if (currentPhaseIndex + 1 < levelPhases.Count &&
                elapsedTime > GetPhaseStartTime(currentPhaseIndex + 1))
            {
                currentPhaseIndex++;
                currentPhase = levelPhases[currentPhaseIndex];
                platformManager.UpdatePhase(currentPhase);
                Debug.Log($"Phase {currentPhaseIndex + 1}�� ��ȯ: {currentPhase.phaseName}");
            }

            // ���ʸ��� ������Ʈ
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        // 4�� �� Ŭ���� ������ �̵�
        Debug.Log("���� Ŭ����!");
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlatformManager : MonoBehaviour
{
    [Header("Tile Settings")]
    public GameObject[] platformPrefabs; // 여러 프리팹을 받을 수 있도록 배열로 수정
    public int rows = 5;
    public int columns = 5;
    public float hexSize = 1.0f;
    public float hexSpacing = 0.1f;
    public Vector3 platformRotation = Vector3.zero;
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;

    [Header("Difficulty and Timing")]
    public int initialActivePlatforms = 5;
    public int minActivePlatforms = 1;
    public float initialPlatformLifetime = 3f;
    public float respawnDelay = 3f;
    public float warningDuration = 1f;
    public float activationInterval = 2f;
    public float reduceInterval = 10f;
    public float minPlatformLifetime = 1f;
    public int maxPlatformsToDestroy = 1;
    public int maxPlatformDestroyLimit = 12;

    [Header("Player Settings")]
    public Transform player;

    private List<PlatformInfo> platformInfos = new List<PlatformInfo>();
    private float currentPlatformLifetime;
    private int currentDifficultyLevel = 0; // 난이도 단계 (0 = Easy, 1 = Medium, 2 = Hard)

    private enum PlatformState
    {
        Normal,  // 기본 활성화 상태
        Hit,     // 플레이어가 밟았을 때 상태
        Warning, // 파괴될 예정인 상태
        Break    // 비활성화 상태
    }

    private class PlatformInfo
    {
        public GameObject platform;
        public PlatformState state;
        public Renderer renderer;
        public Vector3 initialPosition; // 초기 위치 추가

        public PlatformInfo(GameObject platform, Renderer renderer, Vector3 initialPosition)
        {
            this.platform = platform;
            this.renderer = renderer;
            this.initialPosition = initialPosition;
            this.state = PlatformState.Break; // 처음에는 Break 상태로 설정
        }
    }

    private void Start()
    {
        SpawnHexagonalPlatforms();
        currentPlatformLifetime = initialPlatformLifetime;

        Debug.Log("코루틴 시작!");
        StartCoroutine(UpdateDifficultyAndParameters());
        ActivateInitialPlatforms();
        StartCoroutine(ActivateRandomPlatforms());
        StartCoroutine(DecreaseActivePlatforms());
        StartCoroutine(PlatformLifecycle());
    }

    private void SpawnHexagonalPlatforms()
    {
        float width = (hexSize + hexSpacing) * 2;
        float height = Mathf.Sqrt(3) * (hexSize + hexSpacing);

        if (platformPrefabs == null || platformPrefabs.Length == 0)
        {
            Debug.LogError("Error: No platform prefabs are assigned in the inspector.");
            return;
        }

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 spawnPosition = HexToWorldPosition(col, row, width, height);

                GameObject prefabToSpawn = platformPrefabs[Random.Range(0, platformPrefabs.Length)];
                if (prefabToSpawn == null)
                {
                    Debug.LogWarning("Warning: Selected prefab is null.");
                    continue;
                }

                GameObject platform = Instantiate(prefabToSpawn, spawnPosition, Quaternion.Euler(platformRotation));
                platform.layer = LayerMask.NameToLayer("GroundLayer");
                MeshRenderer renderer = platform.GetComponent<MeshRenderer>();
                platformInfos.Add(new PlatformInfo(platform, renderer, spawnPosition));

                platform.SetActive(false);
            }
        }

        Debug.Log("발판 생성 완료: 총 발판 수 = " + platformInfos.Count);
    }

    private void ActivateInitialPlatforms()
    {
        List<PlatformInfo> breakPlatforms = platformInfos.FindAll(p => p.state == PlatformState.Break);
        int platformsToActivate = Mathf.Min(initialActivePlatforms, breakPlatforms.Count);
        List<PlatformInfo> activatedPlatforms = new List<PlatformInfo>();

        for (int i = 0; i < platformsToActivate; i++)
        {
            PlatformInfo platformInfo = breakPlatforms[Random.Range(0, breakPlatforms.Count)];
            ActivatePlatform(platformInfo);
            breakPlatforms.Remove(platformInfo);
            activatedPlatforms.Add(platformInfo);
        }

        if (activatedPlatforms.Count > 0)
        {
            PlacePenguinOnPlatform(activatedPlatforms[Random.Range(0, activatedPlatforms.Count)]);
        }
    }

    private IEnumerator UpdateDifficultyAndParameters()
    {
        for (int i = 0; i < 5; i++) // 총 5회 업데이트
        {
            Debug.Log($"루프 {i} 실행 대기 중");
            yield return new WaitForSeconds(36f); // 36초 간격

            Debug.Log($"루프 {i} 실행 완료");

            switch (i)
            {
                case 0:
                    currentDifficultyLevel = 0;
                    currentPlatformLifetime = 3.0f;
                    activationInterval = 2.0f;
                    reduceInterval = 7.0f;
                    
                    break;

                case 1:
                    currentDifficultyLevel = 1;
                    currentPlatformLifetime = 2.5f;
                    activationInterval = 1.8f;
                    reduceInterval = 6.0f;
                    break;

                case 2:
                    currentDifficultyLevel = 2;
                    currentPlatformLifetime = 2.0f;
                    activationInterval = 1.6f;
                    reduceInterval = 6.0f;
                    break;

                case 3:
                    currentDifficultyLevel = 3;
                    currentPlatformLifetime = 1.5f;
                    activationInterval = 1.4f;
                    reduceInterval = 5.0f;
                    maxPlatformsToDestroy = 2;
                    break;
                
                // case 4는 실제 인게임에선 작동하지 않음. 3가 마지막!
                case 4: 
                    currentDifficultyLevel = 3;
                    currentPlatformLifetime = 1.0f;
                    activationInterval = 1.2f;
                    reduceInterval = 4.0f;
                    break;
            }

            Debug.Log($"난이도 업데이트: 단계 {currentDifficultyLevel}, 생존 시간 {currentPlatformLifetime}, 활성화 주기 {activationInterval}, 파괴 간격 {reduceInterval}");
        }
    }

    private GameObject GetPlatformPrefab()
    {
        switch (currentDifficultyLevel)
        {
            case 0:
                return platformPrefabs[0];
            case 1:
                return platformPrefabs[Random.Range(0, 2)];
            case 2:
                return platformPrefabs[Random.Range(1, 2)];
            case 3:
                return platformPrefabs[2];
            default:
                return platformPrefabs[0];
        }
    }

    private void ActivatePlatform(PlatformInfo platformInfo)
    {
        GameObject prefabToSpawn = GetPlatformPrefab();
        platformInfo.platform = Instantiate(prefabToSpawn, platformInfo.initialPosition, Quaternion.Euler(platformRotation));
        platformInfo.renderer = platformInfo.platform.GetComponent<Renderer>();

        platformInfo.platform.SetActive(true);
        platformInfo.renderer.material.color = normalColor;
        platformInfo.state = PlatformState.Normal;

        platformInfo.platform.transform.DOMoveY(platformInfo.initialPosition.y, 3f)
            .SetEase(Ease.OutBounce);

        Debug.Log($"활성화된 발판 (난이도 {currentDifficultyLevel}): {platformInfo.platform.name}");
    }

    private void PlacePenguinOnPlatform(PlatformInfo platformInfo)
    {
        Vector3 penguinPosition = platformInfo.platform.transform.position;
        player.position = penguinPosition + new Vector3(0, 1f, 0);
        Debug.Log("펭귄 초기 위치 설정: " + penguinPosition);
    }

    private Vector3 HexToWorldPosition(int col, int row, float width, float height)
    {
        float x = col * width * 0.75f;
        float z = row * height + (col % 2 == 0 ? 0 : height / 2);
        return new Vector3(x, 0, z);
    }

    private void SetPlatformWarning(PlatformInfo platformInfo)
    {
        platformInfo.renderer.material.color = warningColor;
        platformInfo.state = PlatformState.Warning;
        Debug.Log("발판 Warning 상태로 변경: " + platformInfo.platform.name);
    }

    private IEnumerator ActivateRandomPlatforms()
    {
        while (true)
        {
            yield return new WaitForSeconds(activationInterval);

            int currentlyActiveCount = platformInfos.FindAll(p => p.state == PlatformState.Normal).Count;
            if (currentlyActiveCount < minActivePlatforms) continue;

            int platformsToActivate = Mathf.Min(initialActivePlatforms - currentlyActiveCount, platformInfos.Count);
            List<PlatformInfo> breakPlatforms = platformInfos.FindAll(p => p.state == PlatformState.Break);
            platformsToActivate = Mathf.Min(breakPlatforms.Count, platformsToActivate);

            for (int i = 0; i < platformsToActivate; i++)
            {
                PlatformInfo platformInfo = breakPlatforms[Random.Range(0, breakPlatforms.Count)];
                ActivatePlatform(platformInfo);
                breakPlatforms.Remove(platformInfo);
            }
        }
    }

    private IEnumerator DecreaseActivePlatforms()
    {
        while (true)
        {
            yield return new WaitForSeconds(reduceInterval);

            int currentlyActiveCount = platformInfos.FindAll(p => p.state == PlatformState.Normal).Count;
            if (currentlyActiveCount > minActivePlatforms)
            {
                PlatformInfo platformInfo = platformInfos.FindAll(p => p.state == PlatformState.Normal)[Random.Range(0, currentlyActiveCount)];
                SetPlatformWarning(platformInfo);
                StartCoroutine(DisableAndRespawnPlatform(platformInfo));
            }
        }
    }

    private IEnumerator PlatformLifecycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentPlatformLifetime);

            List<PlatformInfo> activePlatforms = platformInfos.FindAll(p => p.state == PlatformState.Normal);
            int platformsToDestroy = Mathf.Min(maxPlatformsToDestroy, activePlatforms.Count);

            for (int i = 0; i < platformsToDestroy; i++)
            {
                PlatformInfo platformInfo = activePlatforms[Random.Range(0, activePlatforms.Count)];
                SetPlatformWarning(platformInfo);
                activePlatforms.Remove(platformInfo);

                StartCoroutine(DisableAndRespawnPlatform(platformInfo));
            }
        }
    }

    private IEnumerator DisableAndRespawnPlatform(PlatformInfo platformInfo)
    {
        yield return new WaitForSeconds(warningDuration);
        platformInfo.platform.transform.DOMoveY(platformInfo.initialPosition.y - 2f, 1f)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                platformInfo.platform.SetActive(false);
                platformInfo.state = PlatformState.Break;

                platformInfo.platform.transform.position = platformInfo.initialPosition;
                Debug.Log("발판 비활성화 및 초기화 완료: " + platformInfo.platform.name);
            });
    }
}

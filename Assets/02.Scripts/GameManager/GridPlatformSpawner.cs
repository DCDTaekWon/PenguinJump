using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexGridPlatformSpawner : MonoBehaviour
{
    [Header("Tile Settings")]
    public GameObject platformPrefab;
    public int rows = 5;
    public int columns = 5;
    public float hexSize = 1.0f;
    public float platformThickness = 3.0f;
    public float hexSpacing = 0.1f;
    public float platformScale = 1.0f;
    public Color normalColor = Color.white;
    public Color hitColor = Color.yellow;
    public Color warningColor = Color.red;
    public Material IceMaterial;
    public Vector3 platformRotation = Vector3.zero;

    [Header("Difficulty and Timing")]
    public float initialPlatformLifetime = 3f;
    public float respawnDelay = 3f;
    public float warningDuration = 1f;
    public float activationInterval = 2f;  // 랜덤 발판 활성화 주기
    public float difficultyIncreaseInterval = 10f;
    public float minPlatformLifetime = 1f;
    public int maxPlatformsToDestroy = 1;
    public int maxPlatformDestroyLimit = 12;
    public int maxActivePlatforms = 5; // 동시에 활성화될 최대 발판 수

    [Header("Player Settings")]
    public Transform player;

    private List<PlatformInfo> platformInfos = new List<PlatformInfo>();
    private float currentPlatformLifetime;

    private enum PlatformState
    {
        Normal,
        Hit,
        Warning,
        Break
    }

    private class PlatformInfo
    {
        public GameObject platform;
        public PlatformState state;
        public Renderer renderer;

        public PlatformInfo(GameObject platform, Renderer renderer)
        {
            this.platform = platform;
            this.renderer = renderer;
            this.state = PlatformState.Break; // 처음엔 Break 상태로 설정
        }
    }

    private void Start()
    {
        SpawnHexagonalPlatforms();
        currentPlatformLifetime = initialPlatformLifetime;

        ActivateInitialPlatforms(); // 게임 시작 시 즉시 발판 활성화
        StartCoroutine(ActivateRandomPlatforms()); // 일정 주기로 발판 활성화
        StartCoroutine(IncreaseDifficulty());
        StartCoroutine(PlatformLifecycle());
    }

    private void SpawnHexagonalPlatforms()
    {
        float width = (hexSize + hexSpacing) * 2;
        float height = Mathf.Sqrt(3) * (hexSize + hexSpacing);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 spawnPosition = HexToWorldPosition(col, row, width, height);
                GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.Euler(platformRotation));
                platform.transform.localScale = new Vector3(platformScale, platformThickness, platformScale);

                // IceMaterial 적용
                MeshRenderer renderer = platform.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material = IceMaterial;
                }

                platform.layer = LayerMask.NameToLayer("GroundLayer");
                platformInfos.Add(new PlatformInfo(platform, renderer));

                // 처음 생성된 발판을 Break 상태로 비활성화
                platform.SetActive(false);
            }
        }

        Debug.Log("발판 생성 완료: 총 발판 수 = " + platformInfos.Count);
    }

    private void ActivateInitialPlatforms()
    {
        List<PlatformInfo> breakPlatforms = platformInfos.FindAll(p => p.state == PlatformState.Break);
        int platformsToActivate = Mathf.Min(maxActivePlatforms, breakPlatforms.Count);

        for (int i = 0; i < platformsToActivate; i++)
        {
            PlatformInfo platformInfo = breakPlatforms[Random.Range(0, breakPlatforms.Count)];
            platformInfo.platform.SetActive(true);
            platformInfo.renderer.material.color = normalColor;
            platformInfo.state = PlatformState.Normal;
            breakPlatforms.Remove(platformInfo);

            Debug.Log("초기 활성화된 발판: " + platformInfo.platform.name);
        }
    }

    private IEnumerator ActivateRandomPlatforms()
    {
        while (true)
        {
            yield return new WaitForSeconds(activationInterval);

            // 현재 활성화된 발판 수 체크
            int currentlyActiveCount = platformInfos.FindAll(p => p.state == PlatformState.Normal).Count;
            int platformsToActivate = Mathf.Min(maxActivePlatforms - currentlyActiveCount, platformInfos.Count);

            // Break 상태의 발판 중 일부를 활성화
            List<PlatformInfo> breakPlatforms = platformInfos.FindAll(p => p.state == PlatformState.Break);
            platformsToActivate = Mathf.Min(breakPlatforms.Count, platformsToActivate);

            for (int i = 0; i < platformsToActivate; i++)
            {
                PlatformInfo platformInfo = breakPlatforms[Random.Range(0, breakPlatforms.Count)];
                platformInfo.platform.SetActive(true);
                platformInfo.renderer.material.color = normalColor;
                platformInfo.state = PlatformState.Normal;
                breakPlatforms.Remove(platformInfo);

                Debug.Log("랜덤 활성화된 발판: " + platformInfo.platform.name);
            }
        }
    }

    private IEnumerator PlatformLifecycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentPlatformLifetime);

            // 활성화된 발판을 대상으로 Warning 상태로 변경 후 파괴
            List<PlatformInfo> activePlatforms = platformInfos.FindAll(p => p.state == PlatformState.Normal);
            int platformsToDestroy = Mathf.Min(maxPlatformsToDestroy, activePlatforms.Count);

            for (int i = 0; i < platformsToDestroy; i++)
            {
                PlatformInfo platformInfo = activePlatforms[Random.Range(0, activePlatforms.Count)];
                platformInfo.renderer.material.color = warningColor;
                platformInfo.state = PlatformState.Warning;
                Debug.Log("발판 Warning 상태로 변경: " + platformInfo.platform.name);
                activePlatforms.Remove(platformInfo);

                StartCoroutine(DisableAndRespawnPlatform(platformInfo));
            }
        }
    }

    private IEnumerator DisableAndRespawnPlatform(PlatformInfo platformInfo)
    {
        yield return new WaitForSeconds(warningDuration);
        platformInfo.platform.SetActive(false);
        platformInfo.state = PlatformState.Break;
        Debug.Log("발판 비활성화 및 Break 상태로 전환: " + platformInfo.platform.name);
    }

    private Vector3 HexToWorldPosition(int col, int row, float width, float height)
    {
        float x = col * width * 0.75f;
        float z = row * height + (col % 2 == 0 ? 0 : height / 2);
        return new Vector3(x, 0, z);
    }

    private IEnumerator IncreaseDifficulty()
    {
        while (true)
        {
            yield return new WaitForSeconds(difficultyIncreaseInterval);

            if (currentPlatformLifetime > minPlatformLifetime)
            {
                currentPlatformLifetime = Mathf.Max(minPlatformLifetime, currentPlatformLifetime - 0.5f);
            }

            if (maxPlatformsToDestroy < maxPlatformDestroyLimit)
            {
                maxPlatformsToDestroy++;
            }

            Debug.Log("난이도 증가: 발판 지속 시간 = " + currentPlatformLifetime + ", 동시에 파괴되는 발판 수 = " + maxPlatformsToDestroy);
        }
    }
}
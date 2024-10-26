using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HexGridPlatformSpawner : MonoBehaviour
{
    [Header("Tile Settings")]
    [Tooltip("생성할 발판의 프리팹들")]
    public GameObject[] platformPrefabs; // 여러 프리팹을 받을 수 있도록 배열로 수정

    [Tooltip("발판의 행(row) 개수")]
    public int rows = 5;

    [Tooltip("발판의 열(column) 개수")]
    public int columns = 5;

    [Tooltip("발판의 기본 크기 (반지름)")]
    public float hexSize = 1.0f;

    [Tooltip("발판 간의 간격 (hexSize를 기준으로)")]
    public float hexSpacing = 0.1f;

    [Tooltip("발판의 회전 각도")]
    public Vector3 platformRotation = Vector3.zero;

    [Tooltip("발판의 기본 색상")]
    public Color normalColor = Color.white;

    [Tooltip("발판이 경고 상태일 때 색상")]
    public Color warningColor = Color.red;

    [Header("Difficulty and Timing")]
    [Tooltip("초기 활성화할 발판 수")]
    public int initialActivePlatforms = 5;

    [Tooltip("최소 활성화할 발판 수 (모든 발판이 사라지지 않도록 유지)")]
    public int minActivePlatforms = 1;

    [Tooltip("초기 발판의 생존 시간")]
    public float initialPlatformLifetime = 3f;

    [Tooltip("발판이 사라진 후 재생성까지의 대기 시간")]
    public float respawnDelay = 3f;

    [Tooltip("발판이 경고 상태를 유지하는 시간")]
    public float warningDuration = 1f;

    [Tooltip("랜덤 발판 활성화 주기 (초 단위)")]
    public float activationInterval = 2f;

    [Tooltip("발판 수 줄이는 간격 (초 단위)")]
    public float reduceInterval = 10f;

    [Tooltip("발판의 최소 생존 시간")]
    public float minPlatformLifetime = 1f;

    [Tooltip("한 번에 파괴될 수 있는 최대 발판 수")]
    public int maxPlatformsToDestroy = 1;

    [Tooltip("난이도가 올라감에 따라 파괴될 수 있는 최대 발판 수 제한")]
    public int maxPlatformDestroyLimit = 12;

    [Header("Player Settings")]
    [Tooltip("플레이어 캐릭터의 Transform")]
    public Transform player;

    private List<PlatformInfo> platformInfos = new List<PlatformInfo>();
    private float currentPlatformLifetime;

    private enum PlatformState
    {
        Normal,  // 기본 활성화 상태
        Hit,     // 플레이어가 밟았을 때 상태
        Warning, // 파괴될 예정인 상태
        Break    // 비활성화 상태
    }

    // 발판 정보 구조체
    private class PlatformInfo
    {
        public GameObject platform;
        public PlatformState state;
        public Renderer renderer;

        public PlatformInfo(GameObject platform, Renderer renderer)
        {
            this.platform = platform;
            this.renderer = renderer;
            this.state = PlatformState.Break; // 처음에는 Break 상태로 설정
        }
    }

    private void Start()
    {
        // 발판 생성 및 초기 설정
        SpawnHexagonalPlatforms();
        currentPlatformLifetime = initialPlatformLifetime;

        // 초기 발판 활성화 및 코루틴 시작
        ActivateInitialPlatforms();
        StartCoroutine(ActivateRandomPlatforms());
        StartCoroutine(DecreaseActivePlatforms());
        StartCoroutine(PlatformLifecycle());
    }

    // 발판 그리드 생성
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

                // 랜덤으로 프리팹 선택
                GameObject prefabToSpawn = platformPrefabs[Random.Range(0, platformPrefabs.Length)];

                if (prefabToSpawn == null)
                {
                    Debug.LogWarning("Warning: Selected prefab is null.");
                    continue;
                }

                // 선택된 프리팹을 생성
                GameObject platform = Instantiate(prefabToSpawn, spawnPosition, Quaternion.Euler(platformRotation));
                platform.layer = LayerMask.NameToLayer("GroundLayer");
                MeshRenderer renderer = platform.GetComponent<MeshRenderer>();
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
        int platformsToActivate = Mathf.Min(initialActivePlatforms, breakPlatforms.Count);
        List<PlatformInfo> activatedPlatforms = new List<PlatformInfo>();

        for (int i = 0; i < platformsToActivate; i++)
        {
            PlatformInfo platformInfo = breakPlatforms[Random.Range(0, breakPlatforms.Count)];
            ActivatePlatform(platformInfo);
            breakPlatforms.Remove(platformInfo);
            activatedPlatforms.Add(platformInfo);
        }

        // 활성화된 발판 중 하나를 랜덤으로 선택하여 펭귄 배치
        if (activatedPlatforms.Count > 0)
        {
            PlacePenguinOnPlatform(activatedPlatforms[Random.Range(0, activatedPlatforms.Count)]);
        }
    }

    private IEnumerator ActivateRandomPlatforms()
    {
        while (true)
        {
            yield return new WaitForSeconds(activationInterval);

            int currentlyActiveCount = platformInfos.FindAll(p => p.state == PlatformState.Normal).Count;
            if (currentlyActiveCount < minActivePlatforms) continue; // 최소 개수 유지

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

    private void ActivatePlatform(PlatformInfo platformInfo)
    {
        // 발판을 처음 비활성화된 상태로 -2 아래에 위치시킵니다.
        platformInfo.platform.SetActive(true);
        platformInfo.platform.transform.position += new Vector3(0, -2f, 0);  // y축 -2로 이동
        platformInfo.renderer.material.color = normalColor;
        platformInfo.state = PlatformState.Normal;

        // DOTween을 사용하여 -2에서 원래 위치로 애니메이션
        platformInfo.platform.transform.DOMoveY(platformInfo.platform.transform.position.y + 2f, 3f) // 3초에 걸쳐 위로 이동
            .SetEase(Ease.OutBounce);  // 자연스럽게 위로 튀는 느낌

        Debug.Log($"활성화된 발판: {platformInfo.platform.name}, 스케일: {platformInfo.platform.transform.localScale}, 위치: {platformInfo.platform.transform.position}");
    }


    private void PlacePenguinOnPlatform(PlatformInfo platformInfo)
    {
        Vector3 penguinPosition = platformInfo.platform.transform.position;
        player.position = penguinPosition + new Vector3(0, 1f, 0); // 약간 위로 이동
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
        platformInfo.renderer.material.color = warningColor; // 발판 색상을 경고 색상으로 변경
        platformInfo.state = PlatformState.Warning;
        Debug.Log("발판 Warning 상태로 변경: " + platformInfo.platform.name);
    }

    private IEnumerator DisableAndRespawnPlatform(PlatformInfo platformInfo)
    {
        yield return new WaitForSeconds(warningDuration); // 경고 시간이 지나면
        // 발판을 아래로 하강시키고 비활성화
        platformInfo.platform.transform.DOMoveY(platformInfo.platform.transform.position.y - 2f, 1f) // 1초에 걸쳐 아래로 이동
            .SetEase(Ease.InQuad)  // 부드럽게 하강
            .OnComplete(() => {
                platformInfo.platform.SetActive(false);  // 하강 후 발판을 비활성화
                platformInfo.state = PlatformState.Break;
                Debug.Log("발판 비활성화 및 Break 상태로 전환: " + platformInfo.platform.name);
            });
    }
}

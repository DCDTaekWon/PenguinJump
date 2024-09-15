using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexGridPlatformSpawner : MonoBehaviour
{
    // 타일 설정
    [Header("Tile Settings")]
    public GameObject platformPrefab;
    public int rows = 5;
    public int columns = 5;
    public float hexSize = 1.0f;
    public float hexSpacing = 0.1f;
    public float platformScale = 1.0f;
    public Color normalColor = Color.white;
    public Color hitColor = Color.yellow; // 밟았을 때 색상
    public Color warningColor = Color.red;

    // 난이도 및 시간 설정
    [Header("Difficulty and Timing")]
    public float initialPlatformLifetime = 3f;
    public float respawnDelay = 3f;  // 발판이 사라진 후 재생성되는 시간
    public float warningDuration = 1f;  // 경고 상태 유지 시간
    public float difficultyIncreaseInterval = 10f;
    public float minPlatformLifetime = 1f;
    public int maxPlatformsToDestroy = 1;
    public int maxPlatformDestroyLimit = 12;
    public int initialTilesToDestroy = 3;

    // 플레이어 설정
    [Header("Player Settings")]
    public Transform player;

    private List<GameObject> platforms = new List<GameObject>();
    private float currentPlatformLifetime;

    private enum PlatformState
    {
        Normal,
        Hit,        // 밟힌 상태
        Warning     // 경고 상태
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
            this.state = PlatformState.Normal; // 초기 상태는 Normal
        }
    }

    private List<PlatformInfo> platformInfos = new List<PlatformInfo>();

    private void Start()
    {
        Debug.Log("게임 시작 - 발판 생성 및 초기 설정");
        SpawnHexagonalPlatforms();
        PlacePlayerAtCenter();
        currentPlatformLifetime = initialPlatformLifetime;
        DestroyInitialTiles();
        StartCoroutine(IncreaseDifficulty());
        StartCoroutine(PlatformLifecycle());
    }

    private void Update()
    {
        CheckPlatformCollision();
    }

    // 발판 충돌 감지를 Update에서 처리
    private void CheckPlatformCollision()
    {
        Collider playerCollider = player.GetComponent<Collider>();
        
        foreach (PlatformInfo platformInfo in platformInfos)
        {
            if (platformInfo.platform.activeSelf && platformInfo.state == PlatformState.Normal)
            {
                Collider platformCollider = platformInfo.platform.GetComponent<Collider>();

                if (playerCollider.bounds.Intersects(platformCollider.bounds))
                {
                    OnPlatformHit(platformInfo.platform);
                }
            }
        }
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
                GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
                platform.transform.localScale = new Vector3(platformScale, 0.1f, platformScale);

                Renderer renderer = platform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = normalColor;
                }

                platform.layer = LayerMask.NameToLayer("GroundLayer"); // 발판 레이어 설정
                platformInfos.Add(new PlatformInfo(platform, renderer));
            }
        }

        Debug.Log("발판 생성 완료: 총 발판 수 = " + platformInfos.Count);
    }

    private void DestroyInitialTiles()
    {
        List<PlatformInfo> activePlatforms = new List<PlatformInfo>(platformInfos);
        for (int i = 0; i < initialTilesToDestroy && activePlatforms.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, activePlatforms.Count);
            PlatformInfo selectedPlatformInfo = activePlatforms[randomIndex];
            activePlatforms.RemoveAt(randomIndex);

            if (selectedPlatformInfo.renderer != null)
            {
                selectedPlatformInfo.renderer.material.color = warningColor;
            }

            selectedPlatformInfo.state = PlatformState.Warning; // 경고 상태로 설정
            StartCoroutine(DisablePlatformAfterDelay(selectedPlatformInfo.platform, warningDuration));

            Debug.Log("초기 파괴 발판 선택 및 경고 색상 변경: " + selectedPlatformInfo.platform.name);
        }
    }

    private IEnumerator DisablePlatformAfterDelay(GameObject platform, float delay)
    {
        yield return new WaitForSeconds(delay);
        platform.SetActive(false);
        Debug.Log("발판 파괴: " + platform.name);
    }

    private Vector3 HexToWorldPosition(int col, int row, float width, float height)
    {
        float x = col * width * 0.75f;
        float z = row * height + (col % 2 == 0 ? 0 : height / 2);
        return new Vector3(x, 0, z);
    }

    private void PlacePlayerAtCenter()
    {
        Vector3 centerPosition = HexToWorldPosition(columns / 2, rows / 2, (hexSize + hexSpacing) * 2, Mathf.Sqrt(3) * (hexSize + hexSpacing));
        player.position = centerPosition;
        Debug.Log("플레이어 중앙 배치 완료");
    }

    public void OnPlatformHit(GameObject platform)
    {
        PlatformInfo platformInfo = platformInfos.Find(p => p.platform == platform);

        if (platformInfo != null)
        {
            if (platformInfo.state == PlatformState.Warning)
            {
                Debug.Log("경고 상태 발판: 기존 로직 유지 (" + platform.name + ")");
                return;
            }

            if (platformInfo.state == PlatformState.Normal)
            {
                platformInfo.state = PlatformState.Hit; // 상태를 Hit로 변경
                if (platformInfo.renderer != null)
                {
                    platformInfo.renderer.material.color = hitColor; // 색상 변경
                    Debug.Log("플랫폼 밟힘 및 색상 변경: " + platform.name);
                }
                StartCoroutine(DestroyPlatformAfterDelay(platformInfo.platform, 2f));
                StartCoroutine(RespawnPlatform(platformInfo)); // 발판 재생성
            }
        }
    }

    private IEnumerator DestroyPlatformAfterDelay(GameObject platform, float delay)
    {
        yield return new WaitForSeconds(delay);
        platform.SetActive(false);
        Debug.Log("밟힌 발판 파괴: " + platform.name);
    }

    private IEnumerator PlatformLifecycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentPlatformLifetime);
            int platformsToDestroy = Mathf.Min(maxPlatformsToDestroy, platformInfos.Count);

            List<PlatformInfo> activePlatforms = new List<PlatformInfo>();
            foreach (PlatformInfo platformInfo in platformInfos)
            {
                if (platformInfo.platform.activeSelf && platformInfo.state != PlatformState.Hit)
                {
                    activePlatforms.Add(platformInfo);
                }
            }

            platformsToDestroy = Mathf.Min(platformsToDestroy, activePlatforms.Count);
            List<PlatformInfo> platformsToChangeColor = new List<PlatformInfo>();
            for (int i = 0; i < platformsToDestroy; i++)
            {
                int randomIndex;
                PlatformInfo selectedPlatform;

                do
                {
                    randomIndex = Random.Range(0, activePlatforms.Count);
                    selectedPlatform = activePlatforms[randomIndex];
                } while (platformsToChangeColor.Contains(selectedPlatform));

                platformsToChangeColor.Add(selectedPlatform);

                if (selectedPlatform.renderer != null)
                {
                    selectedPlatform.renderer.material.color = warningColor;
                }

                selectedPlatform.state = PlatformState.Warning; // 상태를 Warning으로 변경
                Debug.Log("플랫폼 경고 상태 변경: " + selectedPlatform.platform.name);
            }

            yield return new WaitForSeconds(warningDuration);

            foreach (PlatformInfo platformInfo in platformsToChangeColor)
            {
                platformInfo.platform.SetActive(false);
                Debug.Log("경고 발판 파괴: " + platformInfo.platform.name);
                StartCoroutine(RespawnPlatform(platformInfo)); // 발판 재생성 로직 추가
            }
        }
    }

    private IEnumerator RespawnPlatform(PlatformInfo platformInfo)
    {
        yield return new WaitForSeconds(respawnDelay);
        platformInfo.platform.SetActive(true);
        platformInfo.renderer.material.color = normalColor;
        platformInfo.state = PlatformState.Normal; // 상태 복구
        Debug.Log("발판 재생성: " + platformInfo.platform.name);
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









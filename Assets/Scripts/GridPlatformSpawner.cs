using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexGridPlatformSpawner : MonoBehaviour
{
    // 타일 설정
    [Header("Tile Settings")]
    public GameObject platformPrefab;   // 발판 프리팹
    public int rows = 5;                // 타일 행의 수
    public int columns = 5;             // 타일 열의 수
    public float hexSize = 1.0f;        // 육각형 타일의 크기
    public float hexSpacing = 0.1f;     // 육각형 타일 사이의 간격
    public float platformScale = 1.0f;  // 발판 크기
    public Color normalColor = Color.white; // 발판의 기본 색상

    // 난이도 및 시간 설정
    [Header("Difficulty and Timing")]
    public float initialPlatformLifetime = 3f; // 발판이 사라지기 전 초기 시간
    public float respawnDelay = 3f;            // 발판이 사라진 후 재생성 지연 시간
    public float difficultyIncreaseInterval = 10f; // 난이도 증가 주기
    public float minPlatformLifetime = 1f;     // 발판의 최소 생명 시간
    public int maxPlatformsToDestroy = 1;      // 한 번에 사라지는 발판의 수 (초기값)
    public int maxPlatformDestroyLimit = 12;   // 동시에 사라지는 발판의 최대 수
    public int initialTilesToDestroy = 3;      // 처음에 부숴지는 타일 수
    public Color warningColor = Color.red;     // 발판이 사라지기 전 경고 색상

    // 플레이어 설정
    [Header("Player Settings")]
    public Transform player;                   // 플레이어의 Transform

    // 내부 상태 관리 변수 (private)
    private List<GameObject> platforms = new List<GameObject>(); // 발판 리스트
    private float currentPlatformLifetime;

    private void Start()
    {
        // 초기 발판 생성
        SpawnHexagonalPlatforms();

        // 플레이어를 중앙 발판에 배치
        PlacePlayerAtCenter();

        // 초기 발판 생명 시간 설정
        currentPlatformLifetime = initialPlatformLifetime;

        // 초기 타일 파괴
        DestroyInitialTiles();

        // 난이도 조정 코루틴 시작
        StartCoroutine(IncreaseDifficulty());

        // 발판 파괴 및 재생성 루틴 시작
        StartCoroutine(PlatformLifecycle());
    }

    // 육각형 배열로 발판 생성 (열과 행을 통해 배열)
    private void SpawnHexagonalPlatforms()
    {
        float width = (hexSize + hexSpacing) * 2;
        float height = Mathf.Sqrt(3) * (hexSize + hexSpacing);

        // 행과 열의 수에 따라 발판 생성
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 spawnPosition = HexToWorldPosition(col, row, width, height);
                GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);

                // 발판 크기 조정
                platform.transform.localScale = new Vector3(platformScale, 0.1f, platformScale);

                // 발판 기본 색상 설정
                Renderer renderer = platform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = normalColor;
                }

                platforms.Add(platform);
            }
        }
    }

    // 초기 타일 파괴 로직
    private void DestroyInitialTiles()
    {
        List<GameObject> activePlatforms = new List<GameObject>(platforms);
        for (int i = 0; i < initialTilesToDestroy && activePlatforms.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, activePlatforms.Count);
            GameObject selectedPlatform = activePlatforms[randomIndex];
            activePlatforms.RemoveAt(randomIndex);

            // 선택된 타일을 빨간색으로 변경 후 비활성화
            Renderer renderer = selectedPlatform.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = warningColor;
            }

            // 1초 후 발판 비활성화
            StartCoroutine(DisablePlatformAfterDelay(selectedPlatform, 1f));
        }
    }

    // 발판을 지연 후 비활성화하는 코루틴
    private IEnumerator DisablePlatformAfterDelay(GameObject platform, float delay)
    {
        yield return new WaitForSeconds(delay);
        platform.SetActive(false);
    }

    // 헥사 그리드 좌표를 월드 좌표로 변환 (열과 행에 따른 좌표 계산)
    private Vector3 HexToWorldPosition(int col, int row, float width, float height)
    {
        // 홀수 열의 경우 Z축을 조금 내리기
        float x = col * width * 0.75f;
        float z = row * height + (col % 2 == 0 ? 0 : height / 2);

        return new Vector3(x, 0, z);
    }

    // 플레이어를 중앙 발판에 배치
    private void PlacePlayerAtCenter()
    {
        Vector3 centerPosition = HexToWorldPosition(columns / 2, rows / 2, (hexSize + hexSpacing) * 2, Mathf.Sqrt(3) * (hexSize + hexSpacing)); // 중앙 발판의 좌표 계산
        player.position = centerPosition;
    }

    // 발판 파괴 및 재생성 루틴
    private IEnumerator PlatformLifecycle()
    {
        while (true)
        {
            // 일정 시간마다 발판을 랜덤하게 파괴
            yield return new WaitForSeconds(currentPlatformLifetime);

            // 파괴할 발판 선택 (여러 발판을 한 번에 파괴)
            int platformsToDestroy = Mathf.Min(maxPlatformsToDestroy, platforms.Count);

            // 활성화된 발판을 추려내기
            List<GameObject> activePlatforms = new List<GameObject>();
            foreach (GameObject platform in platforms)
            {
                if (platform.activeSelf)
                {
                    activePlatforms.Add(platform);
                }
            }

            // 실제 파괴 가능한 발판 수를 조정
            platformsToDestroy = Mathf.Min(platformsToDestroy, activePlatforms.Count);

            // 발판을 선택하여 빨간색으로 변경 (경고 표시)
            List<GameObject> platformsToChangeColor = new List<GameObject>();
            for (int i = 0; i < platformsToDestroy; i++)
            {
                int randomIndex;
                GameObject selectedPlatform;

                // 중복 발판 또는 비활성화된 발판 방지
                do
                {
                    randomIndex = Random.Range(0, activePlatforms.Count);
                    selectedPlatform = activePlatforms[randomIndex];
                }
                while (platformsToChangeColor.Contains(selectedPlatform));

                platformsToChangeColor.Add(selectedPlatform);

                // 발판이 사라지기 1초 전에 빨간색으로 변경
                Renderer renderer = selectedPlatform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = warningColor;
                }
            }

            // 1초 후 발판 비활성화
            yield return new WaitForSeconds(1f);

            foreach (GameObject platform in platformsToChangeColor)
            {
                platform.SetActive(false);

                // 일정 시간 후 발판 재생성
                StartCoroutine(RespawnPlatform(platform, platform.GetComponent<Renderer>()));
            }
        }
    }

    // 발판 재생성 루틴
    private IEnumerator RespawnPlatform(GameObject platform, Renderer renderer)
    {
        // 발판이 사라진 후 일정 시간 대기
        yield return new WaitForSeconds(respawnDelay);

        // 발판 다시 활성화
        platform.SetActive(true);

        // 발판 색상 복구
        if (renderer != null)
        {
            renderer.material.color = normalColor;
        }
    }

    // 난이도 증가 (발판 생명 시간이 1로 고정된 후에도 파괴되는 발판 수가 증가)
    private IEnumerator IncreaseDifficulty()
    {
        while (true)
        {
            yield return new WaitForSeconds(difficultyIncreaseInterval);

            // 발판 생명 시간 감소, 최소값이 1이 되면 고정
            if (currentPlatformLifetime > minPlatformLifetime)
            {
                currentPlatformLifetime = Mathf.Max(minPlatformLifetime, currentPlatformLifetime - 0.5f);
            }

            // 파괴되는 발판 수는 계속 증가, 최대 12개까지
            if (maxPlatformsToDestroy < maxPlatformDestroyLimit)
            {
                maxPlatformsToDestroy++;
            }

            Debug.Log("난이도 증가: 발판 지속 시간 = " + currentPlatformLifetime + ", 동시에 파괴되는 발판 수 = " + maxPlatformsToDestroy);
        }
    }
}



   



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridPlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;   // 발판 프리팹
    public Transform player;            // 플레이어의 Transform
    public float initialPlatformLifetime = 3f; // 발판이 사라지기 전 초기 시간
    public float respawnDelay = 3f;     // 발판이 사라진 후 재생성 지연 시간
    public Color warningColor = Color.red; // 발판이 사라지기 전 경고 색상
    public Color normalColor = Color.white; // 발판의 기본 색상
    public int gridSize = 5;            // 발판 그리드 크기
    public float platformScale = 1.0f;  // 발판 크기
    public float difficultyIncreaseInterval = 10f; // 난이도 증가 주기
    public float minPlatformLifetime = 1f; // 발판의 최소 생명 시간
    public int maxPlatformsToDestroy = 1;    // 한 번에 사라지는 발판의 수 (처음 1개)
    public int maxPlatformDestroyLimit = 12; // 동시에 사라지는 발판의 최대 수

    private List<GameObject> platforms = new List<GameObject>(); // 발판 리스트
    private float currentPlatformLifetime;
    private float adjustedSpacing = 1.5f;  // 발판 간격을 겹치지 않도록 조정

    private void Start()
    {
        // 초기 발판 생성
        SpawnInitialPlatforms();

        // 플레이어를 중앙 발판에 배치
        PlacePlayerAtCenter();

        // 초기 발판 생명 시간 설정
        currentPlatformLifetime = initialPlatformLifetime;

        // 난이도 조정 코루틴 시작
        StartCoroutine(IncreaseDifficulty());

        // 발판 파괴 및 재생성 루틴 시작
        StartCoroutine(PlatformLifecycle());
    }

    // 초기 발판 생성
    private void SpawnInitialPlatforms()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // 발판 생성, 45도 회전된 상태에서 간격 조정
                Vector3 spawnPosition = new Vector3(x * adjustedSpacing, 0, z * adjustedSpacing);
                GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.Euler(0, 45, 0));

                // 발판 크기 조정
                platform.transform.localScale = new Vector3(platformScale, 0.1f, platformScale);

                // 발판 기본 색상 설정
                Renderer renderer = platform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = normalColor;
                }

                platforms.Add(platform); // 발판을 리스트에 추가
            }
        }
    }

    // 플레이어를 중앙 발판에 배치
    private void PlacePlayerAtCenter()
    {
        // 그리드 중앙 위치 계산
        float centerX = (gridSize - 1) * adjustedSpacing / 2;
        float centerZ = (gridSize - 1) * adjustedSpacing / 2;

        Vector3 centerPosition = new Vector3(centerX, player.position.y, centerZ);
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

            Debug.Log("난이도 증가: 발판 생명 시간 = " + currentPlatformLifetime + ", 동시에 파괴되는 발판 수 = " + maxPlatformsToDestroy);
        }
    }
}












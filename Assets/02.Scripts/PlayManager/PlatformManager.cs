using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlatformManager : MonoBehaviour
{
    [Header("Tile Settings")]
    public GameObject largePlatformPrefab;  // 큰 발판 프리팹
    public GameObject mediumPlatformPrefab; // 중간 발판 프리팹
    public GameObject smallPlatformPrefab;  // 작은 발판 프리팹

    public GameObject goldFishPrefab;   // 금물고기 프리팹
    public GameObject silverFishPrefab; // 은물고기 프리팹
    public GameObject bronzeFishPrefab; // 동물고기 프리팹

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

    [Header("Player Settings")]
    public Transform player; // 플레이어 위치

    private List<PlatformInfo> platformInfos = new List<PlatformInfo>();

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
        public GameObject fish;        // 물고기 오브젝트
        public string size;            // 발판 크기 (Large, Medium, Small)

        public PlatformInfo(GameObject platform, Renderer renderer, string size)
        {
            this.platform = platform;
            this.renderer = renderer;
            this.state = PlatformState.Break; // 초기 상태는 비활성화
            this.size = size;
            this.fish = null; // 물고기 없음
        }
    }

    public void SpawnPlatforms(LevelPhase phase)
    {
        ClearPlatforms(); // 기존 데이터 초기화

        int totalPlatforms = phase.maxActivePlatforms;

        int largeCount = Mathf.RoundToInt(totalPlatforms * (phase.largeRate / 10f));
        int mediumCount = Mathf.RoundToInt(totalPlatforms * (phase.mediumRate / 10f));
        int smallCount = totalPlatforms - largeCount - mediumCount;

        SpawnPlatformBySize(largePlatformPrefab, "Large", largeCount, phase, bronzeFishPrefab);
        SpawnPlatformBySize(mediumPlatformPrefab, "Medium", mediumCount, phase, silverFishPrefab);
        SpawnPlatformBySize(smallPlatformPrefab, "Small", smallCount, phase, goldFishPrefab);

        Debug.Log($"발판 생성 완료: 큰 {largeCount}, 중간 {mediumCount}, 작은 {smallCount}");

        ActivateInitialPlatforms(phase.minActivePlatforms);
    }

    private void ActivateInitialPlatforms(int count)
    {
        List<PlatformInfo> breakPlatforms = platformInfos.FindAll(p => p.state == PlatformState.Break);
        int platformsToActivate = Mathf.Min(count, breakPlatforms.Count);

        for (int i = 0; i < platformsToActivate; i++)
        {
            ActivatePlatform(breakPlatforms[i]);
        }
    }

    private void SpawnPlatformBySize(GameObject prefab, string size, int count, LevelPhase phase, GameObject fishPrefab)
    {
        float width = (hexSize + hexSpacing) * 2;
        float height = Mathf.Sqrt(3) * (hexSize + hexSpacing);

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = HexToWorldPosition(i % phase.columns, i / phase.columns, width, height);

            GameObject platform = Instantiate(prefab, spawnPosition, Quaternion.Euler(platformRotation));
            platform.layer = LayerMask.NameToLayer("GroundLayer");
            MeshRenderer renderer = platform.GetComponent<MeshRenderer>();
            PlatformInfo platformInfo = new PlatformInfo(platform, renderer, size);
            platformInfos.Add(platformInfo);

            if (Random.Range(0, 10) < 5 && platformInfos.Count <= phase.maxFishCount)
            {
                GameObject fish = Instantiate(fishPrefab,
                    platform.transform.position + new Vector3(0, 0.5f, 0),
                    Quaternion.identity);

                fish.transform.SetParent(platform.transform); // 물고기를 발판의 자식으로 설정
                platformInfo.fish = fish;
            }

            platform.SetActive(false);
        }
    }

    private Vector3 HexToWorldPosition(int col, int row, float width, float height)
    {
        float x = col * width * 0.75f;
        float z = row * height + (col % 2 == 0 ? 0 : height / 2);
        return new Vector3(x, -1, z); // y 위치 낮춤
    }

    private void ActivatePlatform(PlatformInfo platformInfo)
    {
        platformInfo.platform.SetActive(true);
        platformInfo.renderer.material.color = normalColor;
        platformInfo.state = PlatformState.Normal;

        platformInfo.platform.transform.DOMoveY(platformInfo.platform.transform.position.y + 2f, 3f)
            .SetEase(Ease.OutBounce);

        if (platformInfo.fish != null)
            platformInfo.fish.SetActive(true);

        Debug.Log($"활성화된 발판: {platformInfo.platform.name}");
    }

    private IEnumerator DisableAndRespawnPlatform(PlatformInfo platformInfo)
    {
        yield return new WaitForSeconds(1f);

        platformInfo.platform.transform.DOMoveY(platformInfo.platform.transform.position.y - 2f, 1f)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                platformInfo.platform.SetActive(false);
                platformInfo.state = PlatformState.Break;

                if (platformInfo.fish != null)
                {
                    Destroy(platformInfo.fish);
                    platformInfo.fish = null;
                }

                Debug.Log($"발판 비활성화 완료: {platformInfo.platform.name}");
            });
    }

    private void ClearPlatforms()
    {
        foreach (var platformInfo in platformInfos)
        {
            if (platformInfo.platform != null)
                Destroy(platformInfo.platform);
            if (platformInfo.fish != null)
                Destroy(platformInfo.fish);
        }
    }
}

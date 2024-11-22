using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlatformManager : MonoBehaviour
{
    [Header("Platform Prefabs")]
    public GameObject largePlatformPrefab;
    public GameObject mediumPlatformPrefab;
    public GameObject smallPlatformPrefab;

    [Header("Fish Prefabs")]
    public GameObject goldFishPrefab;
    public GameObject silverFishPrefab;
    public GameObject bronzeFishPrefab;

    [Header("Settings")]
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public Vector3 platformRotation = Vector3.zero;
    public float hexSize = 1.0f;
    public float hexSpacing = 0.1f;

    private List<PlatformInfo> platformInfos = new List<PlatformInfo>();

    private enum PlatformState
    {
        Normal,  // Ȱ�� ����
        Disable, // ��Ȱ�� ����
        Break    // �ϰ� ����
    }

    private class PlatformInfo
    {
        public GameObject platform;
        public PlatformState state;
        public Renderer renderer;
        public GameObject fish;
        public string size;

        public PlatformInfo(GameObject platform, Renderer renderer, string size)
        {
            this.platform = platform;
            this.renderer = renderer;
            this.state = PlatformState.Disable; // �ʱ� ����
            this.size = size;
            this.fish = null;
        }
    }

    /// <summary>
    /// ���� ���� ������ �°� ������ ������Ʈ�մϴ�.
    /// </summary>
    public void UpdatePhase(LevelPhase phase)
    {
        // ���� Ȱ��ȭ�� ���� ���͸�
        List<PlatformInfo> activePlatforms = platformInfos.FindAll(p => p.state == PlatformState.Normal);

        // �ʰ��� ���� ��Ȱ��ȭ
        int currentActiveCount = activePlatforms.Count;
        if (currentActiveCount > phase.maxActivePlatforms)
        {
            int platformsToDisable = currentActiveCount - phase.maxActivePlatforms;
            DisableExcessPlatforms(activePlatforms, platformsToDisable);
        }

        // ������ ���� �߰�
        int neededPlatforms = phase.maxActivePlatforms - currentActiveCount;
        if (neededPlatforms > 0)
        {
            SpawnAdditionalPlatforms(neededPlatforms, phase);
        }
    }

    private void DisableExcessPlatforms(List<PlatformInfo> activePlatforms, int count)
    {
        List<PlatformInfo> platformsToDisable = new List<PlatformInfo>();
        while (platformsToDisable.Count < count)
        {
            PlatformInfo randomPlatform = activePlatforms[Random.Range(0, activePlatforms.Count)];
            if (!platformsToDisable.Contains(randomPlatform))
            {
                platformsToDisable.Add(randomPlatform);
            }
        }

        foreach (var platform in platformsToDisable)
        {
            StartCoroutine(DisableAndSinkPlatform(platform));
        }
    }

    private void SpawnAdditionalPlatforms(int count, LevelPhase phase)
    {
        // ���� ũ�� ���� ���
        int largeCount = Mathf.RoundToInt(count * (phase.largeRate / 10f));
        int mediumCount = Mathf.RoundToInt(count * (phase.mediumRate / 10f));
        int smallCount = count - largeCount - mediumCount;

        // ���� ����
        SpawnPlatformBySize(largePlatformPrefab, "Large", largeCount, phase);
        SpawnPlatformBySize(mediumPlatformPrefab, "Medium", mediumCount, phase);
        SpawnPlatformBySize(smallPlatformPrefab, "Small", smallCount, phase);
    }

    private void SpawnPlatformBySize(GameObject prefab, string size, int count, LevelPhase phase)
    {
        List<Vector3> gridPositions = GenerateHexGridPositions(phase.columns, phase.rows);

        for (int i = 0; i < count; i++)
        {
            if (gridPositions.Count == 0)
            {
                Debug.LogWarning("�׸��� ��ġ ����!");
                break;
            }

            int randomIndex = Random.Range(0, gridPositions.Count);
            Vector3 spawnPosition = gridPositions[randomIndex];
            gridPositions.RemoveAt(randomIndex);

            // ���� ����
            GameObject platform = Instantiate(prefab, spawnPosition, Quaternion.Euler(platformRotation));
            platform.layer = LayerMask.NameToLayer("GroundLayer");
            Renderer renderer = platform.GetComponent<Renderer>();
            PlatformInfo platformInfo = new PlatformInfo(platform, renderer, size);

            platformInfos.Add(platformInfo);

            // ����� �߰�
            if (Random.Range(0, 10) < 5 && platformInfos.Count <= phase.maxFishCount)
            {
                GameObject fishPrefab = GetFishPrefabBySize(size);
                GameObject fish = Instantiate(fishPrefab, platform.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                fish.transform.SetParent(platform.transform);
                platformInfo.fish = fish;
            }
        }
    }

    private GameObject GetFishPrefabBySize(string size)
    {
        switch (size)
        {
            case "Large": return bronzeFishPrefab;
            case "Medium": return silverFishPrefab;
            case "Small": return goldFishPrefab;
            default: return null;
        }
    }

    private IEnumerator DisableAndSinkPlatform(PlatformInfo platformInfo)
    {
        platformInfo.state = PlatformState.Break;
        platformInfo.renderer.material.color = warningColor;

        platformInfo.platform.transform.DOMoveY(-4f, 2f)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                platformInfo.state = PlatformState.Disable;
                platformInfo.platform.SetActive(false);

                if (platformInfo.fish != null)
                {
                    Destroy(platformInfo.fish);
                    platformInfo.fish = null;
                }
            });

        yield return new WaitForSeconds(2f);
    }

    private List<Vector3> GenerateHexGridPositions(int columns, int rows)
    {
        List<Vector3> positions = new List<Vector3>();
        float width = (hexSize + hexSpacing) * 2f;
        float height = Mathf.Sqrt(3f) * (hexSize + hexSpacing);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float x = col * width * 0.75f;
                float z = row * height + (col % 2 == 0 ? 0 : height / 2);
                positions.Add(new Vector3(x, 0, z));
            }
        }

        return positions;
    }
}

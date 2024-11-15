using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlatformManager : MonoBehaviour
{
    [Header("Tile Settings")]
    public GameObject largePlatformPrefab;  // ū ���� ������
    public GameObject mediumPlatformPrefab; // �߰� ���� ������
    public GameObject smallPlatformPrefab;  // ���� ���� ������

    public GameObject goldFishPrefab;   // �ݹ���� ������
    public GameObject silverFishPrefab; // ������� ������
    public GameObject bronzeFishPrefab; // ������� ������

    [Tooltip("������ �⺻ ũ�� (������)")]
    public float hexSize = 1.0f;

    [Tooltip("���� ���� ���� (hexSize�� ��������)")]
    public float hexSpacing = 0.1f;

    [Tooltip("������ ȸ�� ����")]
    public Vector3 platformRotation = Vector3.zero;

    [Tooltip("������ �⺻ ����")]
    public Color normalColor = Color.white;

    [Tooltip("������ ��� ������ �� ����")]
    public Color warningColor = Color.red;

    [Header("Player Settings")]
    public Transform player; // �÷��̾� ��ġ

    private List<PlatformInfo> platformInfos = new List<PlatformInfo>();

    private enum PlatformState
    {
        Normal,  // �⺻ Ȱ��ȭ ����
        Hit,     // �÷��̾ ����� �� ����
        Warning, // �ı��� ������ ����
        Break    // ��Ȱ��ȭ ����
    }

    private class PlatformInfo
    {
        public GameObject platform;
        public PlatformState state;
        public Renderer renderer;
        public GameObject fish;        // ����� ������Ʈ
        public string size;            // ���� ũ�� (Large, Medium, Small)

        public PlatformInfo(GameObject platform, Renderer renderer, string size)
        {
            this.platform = platform;
            this.renderer = renderer;
            this.state = PlatformState.Break; // �ʱ� ���´� ��Ȱ��ȭ
            this.size = size;
            this.fish = null; // ����� ����
        }
    }

    public void SpawnPlatforms(LevelPhase phase)
    {
        ClearPlatforms(); // ���� ������ �ʱ�ȭ

        int totalPlatforms = phase.maxActivePlatforms;

        int largeCount = Mathf.RoundToInt(totalPlatforms * (phase.largeRate / 10f));
        int mediumCount = Mathf.RoundToInt(totalPlatforms * (phase.mediumRate / 10f));
        int smallCount = totalPlatforms - largeCount - mediumCount;

        SpawnPlatformBySize(largePlatformPrefab, "Large", largeCount, phase, bronzeFishPrefab);
        SpawnPlatformBySize(mediumPlatformPrefab, "Medium", mediumCount, phase, silverFishPrefab);
        SpawnPlatformBySize(smallPlatformPrefab, "Small", smallCount, phase, goldFishPrefab);

        Debug.Log($"���� ���� �Ϸ�: ū {largeCount}, �߰� {mediumCount}, ���� {smallCount}");

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

                fish.transform.SetParent(platform.transform); // ����⸦ ������ �ڽ����� ����
                platformInfo.fish = fish;
            }

            platform.SetActive(false);
        }
    }

    private Vector3 HexToWorldPosition(int col, int row, float width, float height)
    {
        float x = col * width * 0.75f;
        float z = row * height + (col % 2 == 0 ? 0 : height / 2);
        return new Vector3(x, -1, z); // y ��ġ ����
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

        Debug.Log($"Ȱ��ȭ�� ����: {platformInfo.platform.name}");
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

                Debug.Log($"���� ��Ȱ��ȭ �Ϸ�: {platformInfo.platform.name}");
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

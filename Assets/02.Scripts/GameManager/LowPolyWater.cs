using UnityEngine;
using System.Collections.Generic;

namespace LowPolyWater
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Wave Settings")]
        public float baseWaveHeight = 0.5f;
        public float maxWaveHeight = 1.5f; // 물결의 최대 높이
        public float waveHeightChangeRate = 0.1f; // 물결 높이 변화 속도
        public float baseWaveFrequency = 0.5f;
        public float maxWaveFrequency = 2.0f; // 물결의 최대 빈도
        public float waveFrequencyChangeRate = 0.05f; // 물결 빈도 변화 속도
        public float waveLength = 0.75f;

        public Vector3 waveOriginPosition = new Vector3(0.0f, 0.0f, 0.0f);

        private float currentWaveHeight;
        private float currentWaveFrequency;
        private MeshFilter meshFilter;
        private Mesh mesh;
        private Vector3[] vertices;

        private List<GameObject> waveFollowers = new List<GameObject>();

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        void Start()
        {
            CreateMeshLowPoly(meshFilter);
            currentWaveHeight = baseWaveHeight;
            currentWaveFrequency = baseWaveFrequency;
        }

        void Update()
        {
            UpdateWaveParameters();
            GenerateWaves();
            UpdateFollowers();
        }

        MeshFilter CreateMeshLowPoly(MeshFilter mf)
        {
            mesh = mf.sharedMesh;
            Vector3[] originalVertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            Vector3[] vertices = new Vector3[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                vertices[i] = originalVertices[triangles[i]];
                triangles[i] = i;
            }

            mesh.vertices = vertices;
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            this.vertices = mesh.vertices;

            return mf;
        }

        // 시간에 따라 물결의 높이와 빈도 변경
        void UpdateWaveParameters()
        {
            // 예시: 특정 시간 간격으로 물결 강도가 증가하고 감소
            float timeFactor = Mathf.PingPong(Time.time * waveHeightChangeRate, 1);
            currentWaveHeight = Mathf.Lerp(baseWaveHeight, maxWaveHeight, timeFactor);
            currentWaveFrequency = Mathf.Lerp(baseWaveFrequency, maxWaveFrequency, timeFactor);
        }

        void GenerateWaves()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];
                v.y = 0.0f;

                float distance = Vector3.Distance(v, waveOriginPosition);
                distance = (distance % waveLength) / waveLength;

                // 현재 물결 높이와 빈도에 따라 물결 생성
                v.y = currentWaveHeight * Mathf.Sin(Time.time * Mathf.PI * 2.0f * currentWaveFrequency
                + (Mathf.PI * 2.0f * distance));

                vertices[i] = v;
            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.MarkDynamic();
            meshFilter.mesh = mesh;
        }

        // 물결에 따라 프리팹의 움직임 업데이트
        void UpdateFollowers()
        {
            foreach (GameObject follower in waveFollowers)
            {
                Vector3 pos = follower.transform.position;
                float waveHeight = GetWaveHeightAtPosition(pos);
                pos.y = waveHeight;
                follower.transform.position = pos;
            }
        }

        // 특정 위치에서 물결 높이 계산
        private float GetWaveHeightAtPosition(Vector3 position)
        {
            float distance = Vector3.Distance(new Vector3(position.x, 0, position.z), waveOriginPosition);
            distance = (distance % waveLength) / waveLength;
            return currentWaveHeight * Mathf.Sin(Time.time * Mathf.PI * 2.0f * currentWaveFrequency + (Mathf.PI * 2.0f * distance));
        }

        // 프리팹을 물결에 등록
        public void RegisterFollower(GameObject follower)
        {
            if (!waveFollowers.Contains(follower))
            {
                waveFollowers.Add(follower);
            }
        }

        // 프리팹을 물결에서 제거
        public void UnregisterFollower(GameObject follower)
        {
            if (waveFollowers.Contains(follower))
            {
                waveFollowers.Remove(follower);
            }
        }
    }
}

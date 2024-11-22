using UnityEngine;
using System.Collections.Generic;

namespace LowPolyWater
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Wave Settings")]
        public float baseWaveHeight = 0.5f;
        public float waveHeightIncreaseRate = 0.1f;
        public float waveFrequencyIncreaseRate = 0.05f;
        public float waveLength = 0.75f;
        public Vector3 waveOriginPosition = new Vector3(0f, 0f, 0f);

        private float currentWaveHeight;
        private float currentWaveFrequency;
        private float timePassed;

        private MeshFilter meshFilter;
        private Mesh mesh;
        private Vector3[] vertices;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        void Start()
        {
            CreateMeshLowPoly(meshFilter);
            currentWaveHeight = baseWaveHeight;
            currentWaveFrequency = 0.5f;
        }

        void Update()
        {
            UpdateWaveParameters();
            GenerateWaves();
        }

        MeshFilter CreateMeshLowPoly(MeshFilter mf)
        {
            if (mf == null || mf.sharedMesh == null)
            {
                Debug.LogError("Error: MeshFilter or sharedMesh is null. Cannot create low poly mesh.");
                return mf;
            }

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

        void UpdateWaveParameters()
        {
            timePassed += Time.deltaTime;

            if (timePassed >= 60f)
            {
                currentWaveHeight += waveHeightIncreaseRate;
                currentWaveFrequency += waveFrequencyIncreaseRate;
                timePassed = 0f;
            }
        }

        void GenerateWaves()
        {
            if (vertices == null || mesh == null) return;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];
                v.y = 0.0f;

                float distance = Vector3.Distance(v, waveOriginPosition);
                distance = (distance % waveLength) / waveLength;

                v.y = currentWaveHeight * Mathf.Sin(Time.time * Mathf.PI * 2.0f * currentWaveFrequency
                + (Mathf.PI * 2.0f * distance));

                vertices[i] = v;
            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.MarkDynamic();
            meshFilter.mesh = mesh;
        }
    }
}

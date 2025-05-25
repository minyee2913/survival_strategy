using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using minyee2913.Utils;

public class ObjectGenerator : Singleton<ObjectGenerator>
{
    [System.Serializable]
    public class ObjectSpawnData
    {
        public GameObject prefab;
        public int count = 1;
        public float interval = 1f;
        public int weight = 1;
    }

    [Header("생성 설정")]
    public List<ObjectSpawnData> spawnObjects = new List<ObjectSpawnData>();
    public Vector3 areaSize = new Vector3(10, 0, 10);
    public Vector3 areaCenter = Vector3.zero;

    [Header("디버그")]
    public bool autoGenerateOnStart = false;

    private void Start()
    {
        if (autoGenerateOnStart)
        {
            Generate();
        }
    }

    public void Generate()
    {
        StartCoroutine(GenerateCoroutine());
    }

    private IEnumerator GenerateCoroutine()
    {
        List<ObjectSpawnData> weightedList = new List<ObjectSpawnData>();

        foreach (var obj in spawnObjects)
        {
            for (int i = 0; i < obj.weight; i++)
            {
                weightedList.Add(obj);
            }
        }

        while (weightedList.Count > 0)
        {
            ObjectSpawnData selected = weightedList[Random.Range(0, weightedList.Count)];

            for (int i = 0; i < selected.count; i++)
            {
                Vector3 spawnPos = GetRandomPosition();
                var obj = Instantiate(selected.prefab, spawnPos, Quaternion.identity);

                obj.transform.rotation = Quaternion.Euler(obj.transform.rotation.eulerAngles.x, Random.Range(0f, 360f), obj.transform.rotation.eulerAngles.z);
                yield return new WaitForSeconds(selected.interval);
            }

            weightedList.RemoveAll(x => x.prefab == selected.prefab);
        }
    }


    private Vector3 GetRandomPosition()
    {
        Vector3 offset = new Vector3(
            Random.Range(-areaSize.x / 2, areaSize.x / 2),
            Random.Range(-areaSize.y / 2, areaSize.y / 2),
            Random.Range(-areaSize.z / 2, areaSize.z / 2)
        );

        return transform.position + areaCenter + offset;
    }

    // Gizmo로 생성 범위 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawCube(transform.position + areaCenter, areaSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + areaCenter, areaSize);
    }
}

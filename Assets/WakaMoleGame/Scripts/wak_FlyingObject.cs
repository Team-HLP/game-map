using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wak_FlyingObject : MonoBehaviour
{
    public GameObject[] meteoritePrefabs;  // 운석 Prefab 여러 개 배열
    private float spawnInterval = 1.5f;     // 생성 간격 

    public RectTransform spawnArea;
    private Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform;
        InvokeRepeating(nameof(SpawnObject), 1f, spawnInterval);
    }

    void SpawnObject()
    {
        if (spawnArea == null || meteoritePrefabs.Length == 0) return;

        float widthRange = spawnArea.rect.width * 0.3f;
        float heightRange = spawnArea.rect.height * 0.3f;

        Vector2 spawnPosition2D = new Vector2(
            Random.Range(-spawnArea.rect.width / 2, spawnArea.rect.width / 2),
            Random.Range(-spawnArea.rect.height / 2, spawnArea.rect.height / 2)
        );

        Vector3 spawnPosition = spawnArea.TransformPoint(spawnPosition2D);
        spawnPosition.y -= 3f;

        // 랜덤 프리팹 선택 및 생성
        GameObject prefabToSpawn = meteoritePrefabs[Random.Range(0, meteoritePrefabs.Length)];
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        // 생성 후 0.3~0.7초 내에 사라지게
        float destroyTime = Random.Range(0.7f, 1.0f);
        Destroy(spawnedObject, destroyTime);
    }
}

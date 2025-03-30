using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject ufoPrefab;
    public GameObject fuelPrefab;
    public float spawnDistance = 20f; // 플레이어로부터 떨어진 거리
    public float spawnRadius = 3f; // 랜덤 생성 반경
    public float spawnInterval = 3f; // 생성 간격

    private Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform;
        InvokeRepeating(nameof(SpawnObject), 1f, spawnInterval);
    }

    void SpawnObject()
    {
        if (playerCamera == null) return;

        Vector3 spawnCenter = playerCamera.position + playerCamera.forward * spawnDistance;
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 randomOffset = new Vector3(randomCircle.x, randomCircle.y, 0);
        Vector3 spawnPosition = spawnCenter + playerCamera.right * randomOffset.x + playerCamera.up * randomOffset.y;

        // UFO와 연료 중 하나를 랜덤으로 선택
        GameObject prefabToSpawn = Random.value > 0.5f ? ufoPrefab : fuelPrefab;

        // 오브젝트 생성 후 변수에 저장
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        ClickableObject clickableObject = spawnedObject.GetComponent<ClickableObject>();
        if (clickableObject != null)
        {
            clickableObject.autoDestroyTime = 2f;
        }
    }
}
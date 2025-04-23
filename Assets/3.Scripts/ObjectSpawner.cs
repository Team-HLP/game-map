using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject meteoritePrefab;
    public GameObject fuelPrefab;
    public float spawnDistance = 20f; // 플레이어로부터 떨어진 거리
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

        float randomX = Random.Range(-5f, 5f);
        Vector3 spawnPosition = playerCamera.position + playerCamera.forward * spawnDistance 
                                + playerCamera.right * randomX + Vector3.up * 1.5f;

        // 운석과 연료 중 하나를 랜덤으로 선택
        GameObject prefabToSpawn = Random.value > 0.5f ? meteoritePrefab : fuelPrefab;

        // 오브젝트 생성 후 변수에 저장
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        ClickableObject clickableObject = spawnedObject.GetComponent<ClickableObject>();
        if (clickableObject != null)
        {
            clickableObject.autoDestroyTime = 2f;
        }
    }
}
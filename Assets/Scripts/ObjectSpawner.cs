using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject ufoPrefab;
    public GameObject fuelPrefab;
    public float spawnDistance = 20f; // �÷��̾�κ��� ������ �Ÿ�
    public float spawnRadius = 3f; // ���� ���� �ݰ�
    public float spawnInterval = 3f; // ���� ����

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

        // UFO�� ���� �� �ϳ��� �������� ����
        GameObject prefabToSpawn = Random.value > 0.5f ? ufoPrefab : fuelPrefab;

        // ������Ʈ ���� �� ������ ����
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        ClickableObject clickableObject = spawnedObject.GetComponent<ClickableObject>();
        if (clickableObject != null)
        {
            clickableObject.autoDestroyTime = 2f;
        }
    }
}
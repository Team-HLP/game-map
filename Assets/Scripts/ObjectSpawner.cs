using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject meteoritePrefab;
    public GameObject fuelPrefab;
    public float spawnDistance = 32f; // �÷��̾�κ��� ������ �Ÿ�
    public float spawnInterval = 4f;  // ���� ����

    private Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform;
        InvokeRepeating(nameof(SpawnObject), 1f, spawnInterval);
    }
    void SpawnObject()
    {
        if (playerCamera == null) return;

        float randomX = Random.Range(-2f, 2f);
        Vector3 spawnPosition = playerCamera.position + playerCamera.forward * spawnDistance 
                                + playerCamera.right * randomX + Vector3.up;

        // ��� ���� �� �ϳ��� �������� ����
        GameObject prefabToSpawn = Random.value > 0.5f ? meteoritePrefab : fuelPrefab;

        // ������Ʈ ���� �� ������ ����
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        ClickableObject clickableObject = spawnedObject.GetComponent<ClickableObject>();
        if (clickableObject != null)
        {
            clickableObject.autoDestroyTime = 4f;
        }
    }
}
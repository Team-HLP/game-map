using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject meteoritePrefab;
    public GameObject fuelPrefab;
    public float spawnDistance = 20f; // �÷��̾�κ��� ������ �Ÿ�
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

        float randomX = Random.Range(-5f, 5f);
        Vector3 spawnPosition = playerCamera.position + playerCamera.forward * spawnDistance 
                                + playerCamera.right * randomX + Vector3.up * 1.5f;

        // ��� ���� �� �ϳ��� �������� ����
        GameObject prefabToSpawn = Random.value > 0.5f ? meteoritePrefab : fuelPrefab;

        // ������Ʈ ���� �� ������ ����
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        ClickableObject clickableObject = spawnedObject.GetComponent<ClickableObject>();
        if (clickableObject != null)
        {
            clickableObject.autoDestroyTime = 2f;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingObject : MonoBehaviour
{
    public GameObject meteoritePrefab;
    public GameObject fuelPrefab;
    public float spawnInterval = 5f;  // ���� ����

    public RectTransform spawnArea;
    private Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform;
        InvokeRepeating(nameof(SpawnObject), 1f, spawnInterval);
    }

    void SpawnObject()
    {
        if (spawnArea == null) return; // spawnArea�� �������� ������ ����

        // ĵ���� ������ ��� ���� ��ġ�� �����ϰ� ����
        Vector2 spawnPosition2D = new Vector2(
            Random.Range(-spawnArea.rect.width / 2, spawnArea.rect.width / 2),
            Random.Range(-spawnArea.rect.height / 2, spawnArea.rect.height / 2)
        );

        // ĵ���� ���� ���� ��ǥ�� ���� ��ǥ�� ��ȯ
        Vector3 spawnPosition = spawnArea.TransformPoint(spawnPosition2D);

        // ��� ���� �� �ϳ��� �������� ����
        GameObject prefabToSpawn = Random.value > 0.7f ? meteoritePrefab : fuelPrefab;

        // ������Ʈ ���� �� ������ ����
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        ClickableObject clickableObject = spawnedObject.GetComponent<ClickableObject>();
        if (clickableObject != null)
        {
            clickableObject.autoDestroyTime = 5f;
        }
    }
}
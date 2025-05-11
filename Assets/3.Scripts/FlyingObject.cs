using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingObject : MonoBehaviour
{
    public GameObject meteoritePrefab;
    public GameObject fuelPrefab;
    private float spawnInterval = 4f;  // 생성 간격

    public RectTransform spawnArea;
    private Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform;
        InvokeRepeating(nameof(SpawnObject), 1f, spawnInterval);
    }

    void SpawnObject()
    {
        if (spawnArea == null) return; // spawnArea가 설정되지 않으면 리턴

        float widthRange = spawnArea.rect.width * 0.3f;   // 좌우 폭 30%
        float heightRange = spawnArea.rect.height * 0.3f; // 상하 높이 30%

        // 캔버스 내에서 운석의 생성 위치를 랜덤하게 설정
        Vector2 spawnPosition2D = new Vector2(
            Random.Range(-spawnArea.rect.width / 2, spawnArea.rect.width / 2),
            Random.Range(-spawnArea.rect.height / 2, spawnArea.rect.height / 2)
        );


        // 캔버스 공간 내의 좌표를 월드 좌표로 변환
        Vector3 spawnPosition = spawnArea.TransformPoint(spawnPosition2D);
        spawnPosition.y -= 3f;

        // 운석과 연료 중 하나를 랜덤으로 선택
        GameObject prefabToSpawn = Random.value < 0.7f ? meteoritePrefab : fuelPrefab;

        // 오브젝트 생성 후 변수에 저장
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        ClickableObject clickableObject = spawnedObject.GetComponent<ClickableObject>();
        if (clickableObject != null)
        {
            clickableObject.autoDestroyTime = 8f;
        }
    }
}
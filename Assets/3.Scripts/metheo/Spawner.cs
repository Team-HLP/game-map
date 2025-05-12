using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject meteoritePrefab;
    public GameObject fuelPrefab;
    public float spawnDistance = 20f; // 플레이어(카메라)로부터 생성될 거리
    public float spawnInterval = 3f; // 오브젝트 생성 간격(초)

    private Transform playerCamera;
    private Vector3 initialForward;
    private Vector3 initialRight;

    void Start()
    {
        playerCamera = Camera.main.transform;
        // 게임 시작 시 카메라 방향 저장
        initialForward = playerCamera.forward;
        initialRight = playerCamera.right;
        // spawnInterval마다 SpawnObject를 반복 호출
        InvokeRepeating(nameof(SpawnObject), 1f, spawnInterval);
    }
    void SpawnObject()
    {
        if (playerCamera == null) return;

        // X축으로 랜덤 위치 오프셋
        float randomX = Random.Range(-5f, 5f);
        // 저장된 방향만 사용
        Vector3 spawnPosition = playerCamera.position + initialForward * spawnDistance 
                                + initialRight * randomX + Vector3.up * 1.5f;

        // 운석 또는 연료 중 하나를 랜덤으로 선택
        GameObject prefabToSpawn = Random.value > 0.5f ? meteoritePrefab : fuelPrefab;

        // 오브젝트 생성
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        // ClickableObject가 있으면 자동 파괴 시간 설정
        // ClickableObject clickableObject = spawnedObject.GetComponent<ClickableObject>();
        // if (clickableObject != null)
        // {
        //     clickableObject.autoDestroyTime = 2f;
        // }
    }
}
using UnityEngine;

public class FlyingObject2 : MonoBehaviour
{
    public GameObject meteoritePrefab; // 운석 프리팹
    public GameObject fuelPrefab;      // 연료 프리팹
   
    [SerializeField]
    private float spawnInterval = 3f;  // 오브젝트 생성 간격 (초)

    public RectTransform spawnArea;    // UI 캔버스 상의 스폰 영역 (RectTransform 기준)
    private Transform playerCamera;    // 플레이어의 카메라 Transform 참조

    private static int meteoriteSpawnCount; // 생성된 운석 개수
    private static int fuelSpawnCount;      // 생성된 연료 개수

    void Start()
    {
        // 생성 카운터 초기화
        meteoriteSpawnCount = 0;
        fuelSpawnCount = 0;

        // 플레이어 카메라 캐싱
        playerCamera = Camera.main.transform;

        // 일정 간격으로 오브젝트를 생성
        InvokeRepeating(nameof(SpawnObject), 1f, spawnInterval);
    }

    void SpawnObject()
    {
        // 스폰 영역이 없으면 리턴
        if (spawnArea == null) return;

        // 스폰 영역 범위 계산 (가로/세로 30%)
        float widthRange = spawnArea.rect.width * 0.3f;
        float heightRange = spawnArea.rect.height * 0.3f;

        // 스폰 위치를 2D 랜덤으로 지정
        Vector2 spawnPosition2D = new Vector2(
            Random.Range(-spawnArea.rect.width / 2, spawnArea.rect.width / 2),
            Random.Range(-spawnArea.rect.height / 2, spawnArea.rect.height / 2)
        );

        // 2D 위치를 실제 월드 좌표로 변환
        Vector3 spawnPosition = spawnArea.TransformPoint(spawnPosition2D);
        spawnPosition.y -= 3f; // 살짝 아래로 조정

        // 어떤 프리팹을 생성할지 결정
        GameObject prefabToSpawn;

        if (Random.value < 0.7f) {
            prefabToSpawn = meteoritePrefab;
            meteoriteSpawnCount++;
        }
        else {
            prefabToSpawn = fuelPrefab;
            fuelSpawnCount++;
        }

        // ✅ meteoritePrefab만 Y축 180도 회전해서 생성
        Quaternion spawnRotation = prefabToSpawn == meteoritePrefab
            ? Quaternion.Euler(0f, 180f, 0f)    // 운석일 경우 회전 적용
            : Quaternion.identity;             // 연료는 회전 없이 그대로

        // 오브젝트를 생성
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);

        // ClickableObject2 컴포넌트가 있다면 자동 파괴 시간 설정
        ClickableObject2 clickableObject = spawnedObject.GetComponent<ClickableObject2>();
        if (clickableObject != null)
        {
            clickableObject.autoDestroyTime = 3f;
        }
    }

    // 생성된 오브젝트 수 저장
    public static void SavePrefabSpawnCount() {
        PlayerPrefs.SetInt("meteorite_prefab_count", meteoriteSpawnCount);
        PlayerPrefs.SetInt("fuel_prefab_count", fuelSpawnCount);
    }
}

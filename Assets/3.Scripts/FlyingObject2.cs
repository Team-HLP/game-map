using UnityEngine;

public class FlyingObject2 : MonoBehaviour
{
    public GameObject meteoritePrefab;
    public GameObject fuelPrefab;
   
    [SerializeField]
    private float spawnInterval = 1.5f;  // ���� ����

    public RectTransform spawnArea;
    private Transform playerCamera;

    private static int meteoriteSpawnCount;
    private static int fuelSpawnCount;

    void Start()
    {
        meteoriteSpawnCount = 0;
        fuelSpawnCount = 0;
        playerCamera = Camera.main.transform;
        InvokeRepeating(nameof(SpawnObject), 1f, spawnInterval);
    }

    void SpawnObject()
    {
        if (spawnArea == null) return; // spawnArea�� �������� ������ ����

        float widthRange = spawnArea.rect.width * 0.3f;   // �¿� �� 30%
        float heightRange = spawnArea.rect.height * 0.3f; // ���� ���� 30%

        // ĵ���� ������ ��� ���� ��ġ�� �����ϰ� ����
        Vector2 spawnPosition2D = new Vector2(
            Random.Range(-spawnArea.rect.width / 2, spawnArea.rect.width / 2),
            Random.Range(-spawnArea.rect.height / 2, spawnArea.rect.height / 2)
        );


        // ĵ���� ���� ���� ��ǥ�� ���� ��ǥ�� ��ȯ
        Vector3 spawnPosition = spawnArea.TransformPoint(spawnPosition2D);
        spawnPosition.y -= 3f;

        // ��� ���� �� �ϳ��� �������� ����
        GameObject prefabToSpawn;

        if (Random.value < 0.7f) {
            prefabToSpawn = meteoritePrefab;
            meteoriteSpawnCount++;
        }
        else {
            prefabToSpawn = fuelPrefab;
            fuelSpawnCount++;
        }

        // ������Ʈ ���� �� ������ ����
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        ClickableObject clickableObject = spawnedObject.GetComponent<ClickableObject>();
        if (clickableObject != null)
        {
            clickableObject.autoDestroyTime = 3f;
        }
    }

    public static void SavePrefabSpawnCount() {
        PlayerPrefs.SetInt("meteorite_prefab_count", meteoriteSpawnCount);
        PlayerPrefs.SetInt("fuel_prefab_count", fuelSpawnCount);
    }
}
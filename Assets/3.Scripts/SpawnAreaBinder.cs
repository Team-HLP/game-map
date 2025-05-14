using UnityEngine;

public class SpawnAreaBinder : MonoBehaviour
{
    public FlyingObject flyingObjectManager;
    public string playerTag = "Player";

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            Debug.LogWarning("Player 오브젝트를 찾을 수 없습니다.");
            return;
        }

        RectTransform spawnCanvas = player.GetComponentInChildren<RectTransform>(true);
        if (spawnCanvas != null && spawnCanvas.name == "CreateObjectCanvas")
        {
            flyingObjectManager.spawnArea = spawnCanvas;
            Debug.Log("[SpawnAreaBinder] SpawnArea 연결 성공: " + spawnCanvas.name);
        }
        else
        {
            Debug.LogWarning("CreateObjectCanvas를 RectTransform으로 찾지 못했습니다.");
        }
    }
}

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
            return;
        }

        RectTransform spawnCanvas = player.GetComponentInChildren<RectTransform>(true);
        if (spawnCanvas != null && spawnCanvas.name == "CreateObjectCanvas")
        {
            flyingObjectManager.spawnArea = spawnCanvas;
        }
    }
}

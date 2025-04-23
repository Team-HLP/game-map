using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClickableObject : MonoBehaviour
{
    public static ClickableObject Instance { get; private set; }

    public enum ObjectType { Meteorite, Fuel }
    public ObjectType objectType;
    public float gazeDuration = 3f;
    public float autoDestroyTime = 5f;

    private float gazeTimer = 0f;
    private bool isGazedAt = false;
    private bool hasInteracted = false;

    public GameObject explosionEffectPrefab;
    public GameObject fuelCollectEffectPrefab;

    void Start()
    {
        Invoke(nameof(AutoDestroy), autoDestroyTime);
    }

    void Update()
    {
        if (hasInteracted) return;

        if (isGazedAt)
        {
            gazeTimer += Time.deltaTime;
            if (gazeTimer >= gazeDuration)
            {
                OnGazeComplete();
            }
        }
        else
        {
            gazeTimer = 0f;
        }

        isGazedAt = false;
    }

    public void OnGazeEnter()
    {
        isGazedAt = true;
    }

    void OnGazeComplete()
    {
        hasInteracted = true;
        CancelInvoke(nameof(AutoDestroy));

        if (objectType == ObjectType.Meteorite)
        {
            GameManager.Instance.destroyedMeteo++;
            SpawnEffect(explosionEffectPrefab); // ¿î¼® ÆÄ±« ¼º°ø
        }
        else if (objectType == ObjectType.Fuel)
        {
            // ¿¬·á ¹Ù¶óºÃÀ¸¹Ç·Î È¹µæ ½ÇÆÐ
        }

        Destroy(gameObject);
    }

    void AutoDestroy()
    {
        hasInteracted = true;

        if (objectType == ObjectType.Meteorite)
        {
            GameManager.Instance.AddHp(-10); // ¿î¼®ÀÌ ºÎµúÈû
        }
        else if (objectType == ObjectType.Fuel)
        {
            SpawnEffect(fuelCollectEffectPrefab);
            GameManager.Instance.AddHp(10); // ¿¬·á È¹µæ ¼º°ø
        }

        Destroy(gameObject);
    }

    void SpawnEffect(GameObject effectPrefab)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f); // 2ÃÊ ÈÄ Á¦°Å
        }
    }
}
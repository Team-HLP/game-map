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
            SpawnEffect(explosionEffectPrefab); // � �ı� ����
        }
        else if (objectType == ObjectType.Fuel)
        {
            // ���� �ٶ�����Ƿ� ȹ�� ����
        }

        Destroy(gameObject);
    }

    void AutoDestroy()
    {
        hasInteracted = true;

        if (objectType == ObjectType.Meteorite)
        {
            GameManager.Instance.AddHp(-10); // ��� �ε���
        }
        else if (objectType == ObjectType.Fuel)
        {
            SpawnEffect(fuelCollectEffectPrefab);
            GameManager.Instance.AddHp(10); // ���� ȹ�� ����
        }

        Destroy(gameObject);
    }

    void SpawnEffect(GameObject effectPrefab)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f); // 2�� �� ����
        }
    }
}
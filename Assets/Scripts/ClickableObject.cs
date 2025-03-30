using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    public enum ObjectType { Ufo, Fuel }
    public ObjectType objectType;
    public float autoDestroyTime = 2f;

    private void Start()
    {
        Invoke(nameof(AutoDestroy), autoDestroyTime);
    }
    private void OnMouseDown()
    {
        if (objectType == ObjectType.Fuel)
        {
            GameManager.Instance.AddHp(20);
        }

        Destroy(gameObject);
        CancelInvoke(nameof(AutoDestroy));
    }
    private void AutoDestroy()
    {
        if (objectType == ObjectType.Ufo)
        {
            GameManager.Instance.AddHp(-10);
        }

        Destroy(gameObject);
    }
}
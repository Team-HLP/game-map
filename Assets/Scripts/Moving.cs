using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
    public float speed;           // ���� �ӵ�
    public float turnSpeed = 30f; // ȸ�� �ӵ�

    private float horizontalInput;

    void Start()
    {
        speed = 10f;
    }
    void Update()
    {
        // �ڵ��� ����
        transform.Translate(speed * Time.deltaTime * Vector3.forward);

        // �¿� ȸ��
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);
    }
}

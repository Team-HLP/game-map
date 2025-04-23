using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Moving : MonoBehaviour
{
    public float speed;     // ���� �ӵ�
    public float turnSpeed = 30f; // ȸ�� �ӵ�

    // SteamVR Input �׼�
    public SteamVR_Action_Boolean MoveLeft;
    public SteamVR_Action_Boolean MoveRight;

    public SteamVR_Input_Sources handType = SteamVR_Input_Sources.Any; // ���� �� ��� �Է� ���

    void Start()
    {
        speed = 10f;
    }
    void Update()
    {
        // ���ּ� ����
        transform.Translate(speed * Time.deltaTime * Vector3.forward);

        // �¿� ȸ��
        if (MoveLeft != null && MoveLeft.GetState(handType))
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
        }

        if (MoveRight != null && MoveRight.GetState(handType))
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
        }
    }
}
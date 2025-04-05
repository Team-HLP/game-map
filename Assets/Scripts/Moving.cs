using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Moving : MonoBehaviour
{
    public float speed;     // 전진 속도
    public float turnSpeed = 30f; // 회전 속도

    // SteamVR Input 액션
    public SteamVR_Action_Boolean MoveLeft;
    public SteamVR_Action_Boolean MoveRight;

    public SteamVR_Input_Sources handType = SteamVR_Input_Sources.Any; // 양쪽 손 모두 입력 허용

    void Start()
    {
        speed = 10f;
    }
    void Update()
    {
        // 우주선 전진
        transform.Translate(speed * Time.deltaTime * Vector3.forward);

        // 좌우 회전
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
    public float speed;           // 전진 속도
    public float turnSpeed = 30f; // 회전 속도

    private float horizontalInput;

    void Start()
    {
        speed = 10f;
    }
    void Update()
    {
        // 자동차 전진
        transform.Translate(speed * Time.deltaTime * Vector3.forward);

        // 좌우 회전
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);
    }
}

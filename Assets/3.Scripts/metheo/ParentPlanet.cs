using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentPlanet : MonoBehaviour
{
    // 중심이 될 오브젝트(a)
    public Transform a;
    // 공전할 오브젝트(b)
    public Transform b;
    // 공전 반지름
    public float radius = 10f;
    // 궤도 평면의 방향 (오일러 각)
    public Vector3 orbitEuler = Vector3.zero;
    // 공전 속도 (도/초)
    public float orbitSpeed = 10f;
    // 현재 각도(라디안)
    private float angle = 0f;

    void Start()
    {
        if (a != null && b != null)
        {
            // 시작 위치를 0도 방향으로 배치
            Quaternion rot = Quaternion.Euler(orbitEuler);
            Vector3 localPos = rot * (Vector3.right * radius);
            b.position = a.position + localPos;
        }
    }

    void Update()
    {
        if (a != null && b != null)
        {
            // 각도 증가
            angle += orbitSpeed * Mathf.Deg2Rad * Time.deltaTime;
            // 궤도 평면 회전
            Quaternion rot = Quaternion.Euler(orbitEuler);
            // 원 위의 위치 계산 (0도 기준에서 angle만큼 회전)
            Vector3 localPos = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward) * (Vector3.right * radius);
            b.position = a.position + rot * localPos;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotate : MonoBehaviour
{
    // 자전 속도 (초당 각도)
    public float rotationSpeed = 10f;
    // 자전축 기울기 (오일러 각도)
    public Vector3 tilt = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        // 시작 시 기울기 적용
        transform.rotation = Quaternion.Euler(tilt);
    }

    // Update is called once per frame
    void Update()
    {
        // 자전축을 기준으로 자전
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}

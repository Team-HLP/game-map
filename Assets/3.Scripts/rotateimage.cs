using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateimage : MonoBehaviour
{
    public float rotationSpeed = 100f; // 회전 속도 (초당 100도)
    public Space rotationSpace = Space.Self; // 회전 좌표계 설정 (Self: 로컬, World: 전역)
    
    // 각 축별 회전 여부를 설정하는 변수
    public bool rotateX = false;
    public bool rotateY = false;
    public bool rotateZ = true;  // Z축은 기본적으로 활성화

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 선택된 축에 따라 회전 벡터 생성
        Vector3 rotationVector = new Vector3(
            rotateX ? rotationSpeed : 0f,
            rotateY ? rotationSpeed : 0f,
            rotateZ ? rotationSpeed : 0f
        );
        
        // 설정된 좌표계를 기준으로 회전
        transform.Rotate(rotationVector * Time.deltaTime, rotationSpace);
    }
}

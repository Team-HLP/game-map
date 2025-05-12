using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRock : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 180, 0); // Inspector에서 회전 속도 조절 가능 (기본값: Y축 180도/초)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}

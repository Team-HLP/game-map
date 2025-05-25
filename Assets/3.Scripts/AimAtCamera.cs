using UnityEngine;

public class AimAtCamera : MonoBehaviour
{
    [Tooltip("비워 두면 Run 시에 Camera.main 자동 검색")]
    public Transform targetCamera;

    [Header("옵션")]
    public bool updateContinuously = true;
    public float verticalOffset = -0.3f;   // 화면 아래로 살짝
    public float forwardOffset = 1.0f;    // 카메라 앞 거리

    void Awake()
    {
        // 스폰 시점에 카메라가 아직 못 켜졌으면 다음 프레임에 다시 찾는다
        if (targetCamera == null)
            StartCoroutine(FindCameraNextFrame());
    }

    System.Collections.IEnumerator FindCameraNextFrame()
    {
        yield return null;                    // 1프레임 대기
        if (targetCamera == null)
            targetCamera = Camera.main?.transform;
    }

    void Start() { if (!updateContinuously) FaceToCam(); }
    void LateUpdate() { if (updateContinuously) FaceToCam(); }

    void FaceToCam()
    {
        if (targetCamera == null) return;

        Vector3 tgt = targetCamera.position +
                      targetCamera.forward * forwardOffset +
                      targetCamera.up * verticalOffset;

        transform.rotation = Quaternion.LookRotation(tgt - transform.position,
                                                     Vector3.up);
    }
}

// AimAtCamera.cs
using UnityEngine;

public class AimAtCamera : MonoBehaviour
{
    public bool updateContinuously = true;  // 매 프레임 추적 여부
    [Tooltip("카메라 정면 기준 아래(−)로 얼마나 내릴지, 단위: 미터")]
    public float verticalOffset = -0.3f; // -0.3m = 화면 중앙보다 아래

    void Start()
    {
        if (!updateContinuously) FaceToCam();
    }

    void Update()
    {
        if (updateContinuously) FaceToCam();
    }

    void FaceToCam()
    {
        if (Camera.main == null) return;

        Transform cam = Camera.main.transform;

        // 카메라 앞 0.5m 지점을 먼저 잡고, 거기에 아래쪽 오프셋 추가
        Vector3 targetPos =
            cam.position + cam.forward * 1.0f + cam.up * verticalOffset;

        transform.rotation =
            Quaternion.LookRotation(targetPos - transform.position, Vector3.up);
    }
}

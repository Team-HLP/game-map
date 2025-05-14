using UnityEngine;
using VSX.Engines3D;

[RequireComponent(typeof(VehicleEngines3D))]
public class AutoThrottleByHMD : MonoBehaviour
{
    [Range(0f, 1f)] public float throttleInput = 0.15f;
    public bool alignYaw = true;

    VehicleEngines3D engines;
    Transform hmd;

    void Awake()
    {
        engines = GetComponent<VehicleEngines3D>();
        hmd     = Camera.main.transform;
    }

    void LateUpdate()
    {
        // 1. 지속적인 전진 입력
        engines.SetMovementInputs(new Vector3(0, 0, throttleInput));

        // 2. 우주선 앞머리를 HMD 수평 전방으로 살짝 따라가게
        if (alignYaw)
        {
            Vector3 fwd = new Vector3(hmd.forward.x, 0, hmd.forward.z).normalized;
            if (fwd.sqrMagnitude > 0.001f)
            {
                Quaternion target = Quaternion.LookRotation(fwd, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, 0.05f);
            }
        }
    }
}

using UnityEngine;
using VSX.Engines3D;

public class ForceStraightMotion : MonoBehaviour
{
    public float forwardSpeed = 0.3f;

    private VehicleEngines3D engines;

    void Start()
    {
        engines = GetComponent<VehicleEngines3D>();
    }

    void FixedUpdate()
    {
        if (engines != null)
        {
            // 앞으로만 이동 (local Z축)
            engines.SetMovementInputs(new Vector3(0f, 0f, forwardSpeed));

            // 회전은 완전히 차단
            engines.SetSteeringInputs(Vector3.zero);
        }
    }
}

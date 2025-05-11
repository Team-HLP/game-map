using UnityEngine;
using System.Collections;
using VSX.Vehicles;

public class XROriginAttacher : MonoBehaviour
{
    [Header("Objects")]
    public GameObject xrOrigin;            // XR Rig
    [Tooltip("우주선 내부에서 XR Origin을 붙일 자식 이름(또는 경로)")]
    public string cockpitMountName = "Ship";

    // 이벤트 연결용 – LoadoutVehicleSpawner 의 OnVehicleSpawned 에 넣는다
    public void AttachToVehicle(Vehicle vehicle)
    {
        if (vehicle == null || xrOrigin == null) return;

        // 코루틴으로 한-프레임 뒤에 실행 → 자식들이 모두 생성된 뒤라 실패 확률 ↓
        StartCoroutine(AttachRoutine(vehicle));
    }

    IEnumerator AttachRoutine(Vehicle vehicle)
    {
        // 한 프레임 대기 (필요하면 2-3 프레임로 늘릴 수 있음)
        yield return null;

        // 1) 직속/경로로 시도
        Transform mountPoint = vehicle.transform.Find(cockpitMountName);

        // 2) 못 찾으면 재귀 검색 (이름만 일치해도 연결)
        if (mountPoint == null)
            mountPoint = FindDeepChild(vehicle.transform, cockpitMountName);

        if (mountPoint == null)
        {
            Debug.LogWarning($"[XROriginAttacher] '{cockpitMountName}'을(를) '{vehicle.name}' 내에서 찾지 못했습니다.");
            yield break;
        }

        // XR Origin 이동
        xrOrigin.transform.SetParent(mountPoint, false);
        xrOrigin.transform.localPosition = Vector3.zero;
        xrOrigin.transform.localRotation = Quaternion.identity;

        // (선택) 비활성 상태였다면 활성화
        if (!xrOrigin.activeSelf) xrOrigin.SetActive(true);

        Debug.Log($"[XROriginAttacher] XR Origin attached under {mountPoint.name}");
    }

    // 재귀 검색 헬퍼
    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            var result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }
}

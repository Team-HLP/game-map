using UnityEngine;

public class IntroConfirmHandler : MonoBehaviour
{
    public GameObject canvasToDisable;
    public Camera mainCamera;

    public void OnConfirmClicked()
    {
        // 캔버스 비활성화
        if (canvasToDisable != null)
            canvasToDisable.SetActive(false);

        // 카메라 Clear Flags 변경
        if (mainCamera != null)
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
    }
}

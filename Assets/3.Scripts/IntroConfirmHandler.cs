using UnityEngine;

public class IntroConfirmHandler : MonoBehaviour
{
    public static IntroConfirmHandler Instance { get; private set; }

    public GameObject canvasToDisable;
    public GameObject dotDisplayCanvas;
    public GameObject notice;
    public Camera mainCamera;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (dotDisplayCanvas != null) 
        {
            dotDisplayCanvas.SetActive(false);
        }
    }

    public void OnConfirmClicked()
    {
        if (canvasToDisable != null) 
        {
            canvasToDisable.SetActive(false);
            dotDisplayCanvas.SetActive(true);
        }

        if (mainCamera != null) 
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
        }
    }

    public void rollBack()
    {
        if (canvasToDisable != null) 
        {
            canvasToDisable.SetActive(true);
        }

        if (mainCamera != null) 
        {
            mainCamera.clearFlags = CameraClearFlags.Skybox;
        }

        if (notice != null)
        {
            notice.SetActive(false);
        }
    }
}

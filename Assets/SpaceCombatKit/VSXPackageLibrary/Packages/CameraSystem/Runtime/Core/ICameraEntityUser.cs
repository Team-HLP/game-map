

namespace VSX.CameraSystem
{
    /// <summary>
    /// Interface for a class on an object that needs a reference to the camera following that object.
    /// </summary>
    public interface ICameraEntityUser
    {
        void SetCameraEntity(CameraEntity cameraEntity);
    }
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using VSX.Utilities;

namespace VSX.CameraSystem
{
    /// <summary>
    /// Unity event for functions with a Camera Target parameter.
    /// </summary>
    [System.Serializable]
    public class OnCameraTargetChangedEventHandler : UnityEvent<CameraTarget> { }


    /// <summary>
    /// Unity event for functions with a Camera View Target parameter.
    /// </summary>
    [System.Serializable]
    public class OnCameraViewTargetChangedEventHandler : UnityEvent<CameraViewTarget> { }


    /// <summary>
    /// Camera manager script - represents a camera in the scene that can follow objects and switch camera views.
    /// </summary>
    public class CameraEntity : MonoBehaviour
    {

        [Tooltip("The camera target that this camera will follow when the scene starts.")]
        [SerializeField]
        protected CameraTarget startingCameraTarget;
        

        protected CameraTarget cameraTarget;
        public CameraTarget CameraTarget { get { return cameraTarget; } }


        [Tooltip("Reference to the main camera.")]
        [SerializeField]
        protected Camera mainCamera;
        public Camera MainCamera { get { return mainCamera; } }


        [SerializeField]
        protected List<Camera> cameras = new List<Camera>();
        public List<Camera> Cameras { get { return cameras; } }


        [SerializeField]
        protected bool cameraControlEnabled = true;
        public virtual bool CameraControlEnabled
        {
            get { return cameraControlEnabled; }
            set
            {
                cameraControlEnabled = value;
                for (int i = 0; i < cameraControllers.Count; ++i)
                {
                    cameraControllers[i].ControllerEnabled = value;
                }
            }
        }


        [SerializeField]
        protected float defaultFieldOfView;
        public float DefaultFieldOfView
        {
            get { return defaultFieldOfView; }
            set { defaultFieldOfView = value; }
        }


        public virtual float FieldOfView { get => mainCamera.fieldOfView; }


        [Header("Camera Collisions")]


        [Tooltip("Whether the camera will react to obstacles (staying within the line of sight to the focused object). Useful for third person view.")]
        [SerializeField]
        protected bool cameraCollisionEnabled = true;
        public bool CameraCollisionEnabled
        {
            get { return cameraCollisionEnabled; }
            set { cameraCollisionEnabled = value; }
        }


        [SerializeField]
        protected bool ignoreTriggerColliders = true;
        public bool IgnoreTriggerColliders
        {
            get { return ignoreTriggerColliders; }
            set { ignoreTriggerColliders = value; }
        }


        [SerializeField]
        protected LayerMask collisionMask = ~0;
        public LayerMask CollisionMask
        {
            get { return collisionMask; }
            set { collisionMask = value; }
        }


        [Header("Default Camera Controller")]


        [Tooltip("The camera view that is selected upon switching to a new camera target.")]
        [SerializeField]
        protected CameraView startingView;


        protected bool controllerOverriding = false;

        protected RaycastHitComparer raycastHitComparer;    // Used to compare raycast distances for camera collision

        protected Coroutine cameraCollisionCoroutine;   

        // List of all the camera controllers in the hierarchy
        protected List<CameraController> cameraControllers = new List<CameraController>();

        protected CameraViewTarget currentViewTarget;
        public CameraViewTarget CurrentViewTarget { get { return currentViewTarget; } }

        protected bool hasCameraViewTarget;
        public bool HasCameraViewTarget { get { return hasCameraViewTarget; } }

        public CameraView CurrentView { get { return hasCameraViewTarget ? currentViewTarget.CameraView : null; } }


        [Header("Events")]
        

        public OnCameraTargetChangedEventHandler onCameraTargetChanged;

        public OnCameraViewTargetChangedEventHandler onCameraViewTargetChanged;



        // Called when the component is first added to a gameobject or the component is reset
        protected virtual void Reset()
        {
            // Look for a camera in the hierarchy
            mainCamera = transform.root.GetComponentInChildren<Camera>();

            // If none found, look for a camera tagged 'MainCamera'
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            
            // If found, initialize the default field of view.
            if (mainCamera != null)
            {
                defaultFieldOfView = Camera.main.fieldOfView;
                cameras.Add(mainCamera);
            }
        }


        protected virtual void Awake()
		{
            raycastHitComparer = new RaycastHitComparer();  

            // Get all the camera controllers in the hierarchy
            cameraControllers = new List<CameraController>(transform.GetComponentsInChildren<CameraController>());

            CameraControlEnabled = cameraControlEnabled;

            foreach (CameraController cameraController in cameraControllers)
            {
                cameraController.SetCamera(this);
            }
        }


        // Called at the start
        protected virtual void Start()
        {
            // Start targeting the starting camera target
            if (startingCameraTarget != null)
            {
                SetCameraTarget(startingCameraTarget);
            }
        }


        /// <summary>
        /// Called when this gameobject is activated
        /// </summary>
        protected virtual void OnEnable()
        {
            // Start a new collision coroutine
            cameraCollisionCoroutine = StartCoroutine(CameraCollisionUpdateCoroutine());
        }


        /// <summary>
        /// Called when this gameobject is deactivated
        /// </summary>
        protected virtual void OnDisable()
        {
            // Stop any collision coroutine that is running
            if (cameraCollisionCoroutine != null)
            {
                StopCoroutine(cameraCollisionCoroutine);
            }
        }


        // Returns whether a camera view target is selectable.
        protected virtual bool IsSelectable(CameraViewTarget cameraViewTarget)
        {
            if (cameraViewTarget == null) return false;

            if (!cameraViewTarget.gameObject.activeInHierarchy) return false;

            return true;
        }


        /// <summary>
        /// Select the first available camera view on the current camera target.
        /// </summary>
        public virtual void SelectFirstAvailableCameraView()
        {
            if (cameraTarget != null)
            {
                for(int i = 0; i < cameraTarget.CameraViewTargets.Count; ++i)
                {
                    if (IsSelectable(cameraTarget.CameraViewTargets[i]))
                    {
                        SetCameraViewTarget(cameraTarget.CameraViewTargets[i]);
                        return;
                    }
                }
            }
        }


        /// <summary>
        /// Set a new camera target to follow.
        /// </summary>
        /// <param name="target">The new camera target.</param>
        public virtual void SetCameraTarget (CameraTarget target)
		{
            if (target == cameraTarget) return;

            // Clear parent
            transform.SetParent(null);  // When called in OnDestroy this may cause an error like this: https://issuetracker.unity3d.com/issues/child-object-destruction-is-delayed-when-setting-the-parent-during-the-scene-unloading
            
            // Deactivate all the camera controllers
            for (int i = 0; i < cameraControllers.Count; ++i)
            {
                cameraControllers[i].OnCameraTargetChanged(null);
            }

            controllerOverriding = false;

            // Set the following camera to null on previous target
            if (cameraTarget != null)
            {
                if (currentViewTarget != null)
                {
                    currentViewTarget.OnUnselected();
                }

                cameraTarget.SetCamera(null);
            }

            // Update the camera target reference
            cameraTarget = null;
            currentViewTarget = null;
            if (target != null)
            {
                cameraTarget = target;
                
            }
            
            // Activate the appropriate controller(s)
            if (cameraTarget != null)
            {

                cameraTarget.SetCamera(this);

                // If no camera view targets on camera target, issue a warning
                if (cameraTarget.CameraViewTargets.Count == 0)
                {
                    Debug.LogWarning("No Camera View Target components found in camera target object's hierarchy, please add one or more.");
                }

                // Activate the appropriate camera controller(s)
                int numControllers = 0;
                for (int i = 0; i < cameraControllers.Count; ++i)
                {
                    cameraControllers[i].OnCameraTargetChanged(cameraTarget);

                    if (cameraControllers[i].Initialized)
                    {
                        numControllers++;
                    }
                }

                if (numControllers == 0)
                {
                    // Set the starting view
                    if (startingView != null)
                    {
                        SetView(startingView);
                    }
                    else
                    {
                        SelectFirstAvailableCameraView();
                    }
                }
                else
                {
                    controllerOverriding = true;
                }

                onCameraTargetChanged.Invoke(cameraTarget);
            }
		}


        /// <summary>
        /// Cycle the camera view forward or backward.
        /// </summary>
        /// <param name="forward">Whether to cycle forward.</param>
        public virtual void CycleCameraView(bool forward)
        {

            // If the camera target has no camera view targets, return.
            if (cameraTarget == null || cameraTarget.CameraViewTargets.Count == 0) return;

            // Get the index of the current camera view target
            int index = cameraTarget.CameraViewTargets.IndexOf(currentViewTarget);
            

            for(int i = 0; i < cameraTarget.CameraViewTargets.Count; ++i)
            {
                index += forward ? 1 : -1;

                // Wrap the index between 0 and the number of camera view targets on the camera target.
                if (index >= cameraTarget.CameraViewTargets.Count)
                {
                    index = 0;
                }
                else if (index < 0)
                {
                    index = cameraTarget.CameraViewTargets.Count - 1;
                }

                if (IsSelectable(cameraTarget.CameraViewTargets[index]))
                {
                    // Set the new camera view target
                    SetCameraViewTarget(cameraTarget.CameraViewTargets[index]);
                    break;
                }
            }
        }


        /// <summary>
        /// Set the camera view target that this camera is following.
        /// </summary>
        /// <param name="cameraViewTarget">The new camera view target.</param>
        public virtual void SetCameraViewTarget(CameraViewTarget cameraViewTarget)
        {

            if (cameraViewTarget == currentViewTarget) return;

            if (currentViewTarget != null)
            {
                currentViewTarget.OnUnselected();
            }

            // Update the current view target info
            this.currentViewTarget = cameraViewTarget;

            if (IsSelectable(cameraViewTarget))
            {
                hasCameraViewTarget = true;

                if (cameraViewTarget.ParentCameraOnSelected)
                {
                    transform.SetParent(cameraViewTarget.transform);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                }
                else
                {
                    transform.SetParent(null);
                    transform.position = cameraViewTarget.transform.position;
                    transform.rotation = cameraViewTarget.transform.rotation;
                }

                cameraViewTarget.OnSelected();
            }
            else
            {
                hasCameraViewTarget = false;
            }

            onCameraViewTargetChanged.Invoke(cameraViewTarget);
        }


        /// <summary>
        /// Select a new camera view.
        /// </summary>
        /// <param name="newView">The new camera view.</param>
        public virtual void SetView(CameraView newView)
		{

            // If no camera target or null view, set to null and exit.
            if (newView == null || cameraTarget == null)
            {
                SetCameraViewTarget(null);
                return;
            }

            // Search all camera views on camera target for desired view
            for (int i = 0; i < cameraTarget.CameraViewTargets.Count; ++i)
			{
				if (IsSelectable(cameraTarget.CameraViewTargets[i]) && cameraTarget.CameraViewTargets[i].CameraView == newView)
				{
                    SetCameraViewTarget(cameraTarget.CameraViewTargets[i]);
                    return;
				}
			}

            SelectFirstAvailableCameraView();
		}


        /// <summary>
        /// Set the field of view for the camera.
        /// </summary>
        /// <param name="newFieldOfView">The new field of view.</param>
        public virtual void SetFieldOfView(float newFieldOfView)
        {
            mainCamera.fieldOfView = newFieldOfView;
        }


        /// <summary>
        /// Coroutine for managing camera collisions.
        /// </summary>
        /// <returns></returns>
        protected IEnumerator CameraCollisionUpdateCoroutine()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();  // Wait until the camera controller fixed update is finished

                CameraCollisionUpdate();
            }
        }


        // Called every frame to avoid the camera being occluded by scene geometry.
        protected void CameraCollisionUpdate()
        {
            if (cameraCollisionEnabled && cameraTarget != null)
            {
                // Initialize the target position for the camera
                Vector3 targetPosition = transform.position;

                // Prepare the spherecast
                Vector3 sphereCastStart = CameraTarget.LookTarget.position;
                Vector3 sphereCastEnd = transform.position;
                Vector3 sphereCastDir = (sphereCastEnd - sphereCastStart).normalized;
                float dist = (sphereCastEnd - sphereCastStart).magnitude;

                // Do spherecast
                RaycastHit[] hits = Physics.SphereCastAll(sphereCastStart, 0.1f, sphereCastDir, dist, collisionMask, ignoreTriggerColliders ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);

                // Sort hits by distance
                System.Array.Sort(hits, raycastHitComparer);

                for (int i = 0; i < hits.Length; ++i)
                {
                    if (Mathf.Approximately(hits[i].distance, 0)) continue;

                    // Ignore hits with the camera target
                    Rigidbody hitRigidbody = hits[i].collider.attachedRigidbody;
                    if (hitRigidbody != null && CameraTarget != null && hitRigidbody.transform == CameraTarget.transform) continue;

                    // Ignore hits with camera target (necessary since Character Controller collider doesn't use attachedRigidbody)
                    if (hits[i].collider.transform == cameraTarget.transform) continue;

                    // Set the camera position to the hit point
                    targetPosition = hits[i].point;

                    break;
                }

                // Update the camera position
                transform.position = targetPosition;
            }
        }


        // Called every frame
        protected virtual void Update()
        {

            if (cameraControllers.Count > 0 && cameraControllers[0].ControllerEnabled != cameraControlEnabled)
            {
                CameraControlEnabled = cameraControlEnabled;
            }

            CameraControlUpdate();
        }


        // Called every frame
        protected virtual void CameraControlUpdate()
        {

            if (controllerOverriding || !cameraControlEnabled || cameraTarget == null || cameraTarget.Rigidbody != null) return;

            if (currentViewTarget != null)
            {
                // Calculate the target position for the camera
                Vector3 targetPosition = currentViewTarget.transform.position;

                // Update position
                transform.position = (1 - currentViewTarget.PositionFollowStrength) * transform.position +
                                            currentViewTarget.PositionFollowStrength * targetPosition;

                // Update rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, currentViewTarget.transform.rotation,
                                                        currentViewTarget.RotationFollowStrength);

                CameraCollisionUpdate();
            }           
        }


        // Called every physics update.
        protected virtual void FixedUpdate()
        {
            CameraControlFixedUpdate();
        }


        // Called every physics update.
        protected virtual void CameraControlFixedUpdate()
        {

            if (controllerOverriding || !cameraControlEnabled || cameraTarget == null || cameraTarget.Rigidbody == null) return;

            if (currentViewTarget != null)
            {
                // Calculate the target position for the camera
                Vector3 targetPosition = currentViewTarget.transform.position;

                // Update position
                transform.position = (1 - currentViewTarget.PositionFollowStrength) * transform.position +
                                            currentViewTarget.PositionFollowStrength * targetPosition;

                // Update rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, currentViewTarget.transform.rotation,
                                                        currentViewTarget.RotationFollowStrength);
            }  
        }


        // Called after all the game's Update functions are complete.
        protected virtual void LateUpdate()
        {
            CameraControlLateUpdate();
        }


        // Called after all the game's Update functions are complete.
        protected virtual void CameraControlLateUpdate()
        {

            if (controllerOverriding || !cameraControlEnabled || cameraTarget == null) return;

            // If position and/or rotation are locked for the selected camera view target, the position and rotation must be updated in 
            // late update to make sure that there is no lag.
            if (currentViewTarget != null)
            {
                if (currentViewTarget.LockPosition)
                {
                    transform.position = currentViewTarget.transform.position;
                }
                if (currentViewTarget.LockRotation)
                {
                    transform.rotation = currentViewTarget.transform.rotation;
                }

                if (currentViewTarget.LockCameraForwardVector)
                {
                    // Always point the camera directly forward 
                    transform.rotation = Quaternion.LookRotation(currentViewTarget.transform.forward, transform.up);
                }

                // Lock upright if necessary
                if (currentViewTarget.LockCameraUpright)
                {
                    transform.LookAt(transform.position + transform.forward, Vector3.up);
                }

                CameraCollisionUpdate();
            }
        }
    }
}

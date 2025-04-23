using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using VSX.Utilities;
using VSX.Vehicles;

namespace VSX.Loadouts
{
    /// <summary>
    /// Manages the behaviour of the camera in the loadout menu.
    /// </summary>
	public class LoadoutCameraController : MonoBehaviour
    {
        [Tooltip("The loadout UI controller.")]
        [SerializeField]
        protected LoadoutUIController loadoutUIController;

        [Tooltip("The loadout manager.")]
        [SerializeField]
        protected LoadoutManager loadoutManager;

        [Tooltip("The loadout display manager.")]
        [SerializeField]
        protected LoadoutDisplayManager displayManager;

        [Tooltip("The camera to control.")]
        [SerializeField]
        protected Camera m_Camera;

        [Tooltip("Whether to snap the position and rotation of the camera (will smoothly transition if unchecked).")]
        [SerializeField]
        protected bool snapPositionAndRotation = true;

        [Tooltip("The camera move speed.")]
        [SerializeField]
        protected float cameraTransitionMoveSpeed = 4;

        [Tooltip("The camera rotation speed during transitions.")]
        [SerializeField]
        protected float cameraTransitionRotationSpeed = 4;

        protected float cameraDistanceTarget;
        protected bool cameraDistanceInitialized = false;

        [Header("View Rotation")]

        [Tooltip("The gimbal that allows the player to rotate the view to inspect a vehicle or module in the loadout.")]
        [SerializeField]
        protected GimbalController viewRotationGimbal;

        [Tooltip("How fast the view rotates.")]
        [SerializeField]
        protected float viewRotationSpeed = 100;

        [Tooltip("How quickly the view rotation responds to input changes. Reduce the value for a smoother rotation.")]
        [SerializeField]
        protected float viewRotationLerpSpeed = 10;

        [Tooltip("The object layers that will block view rotation - usually will be the UI layer.")]
        [SerializeField]
        protected LayerMask viewRotationBlockingLayers;

        protected Vector2 gimbalRotationInputCurrent;
        protected Vector2 gimbalRotationInputTarget;

        protected bool cameraGimbalRotationsInitialized = false;


        [Header("Vehicle Focus")]

        [Tooltip("The default angles for the camera view gimbal when entering vehicle selection mode (showing a vehicle in the loadout).")]
        [SerializeField]
        protected Vector3 defaultVehicleFocusAngles = new Vector3(30, 140, 0);

        [Tooltip("The parent for the camera during vehicle selection.")]
        [SerializeField]
        protected Transform vehicleFocusCameraHolder;

        [Tooltip("The maximum diameter of the vehicle in the viewport during vehicle selection.")]
        [SerializeField]
        protected float maxViewportVehicleDiameter = 0.6f;

        [Tooltip("Whether to adjust the camera distance based on the vehicle size. Will keep the vehicle viewport size approximately within the 'Max Viewport Vehicle Diameter'.")]
        [SerializeField]
        protected bool adjustViewDistanceToVehicleSize = true;

        [Tooltip("The default distance between the camera and the vehicle during vehicle selection.")]
        [SerializeField]
        protected float defaultVehicleViewDistance = 20;


        [Header("Module Mount Focus")]

        [Tooltip("The viewport position to place the module mount in during module selection.")]
        [SerializeField]
        protected Vector2 moduleViewportPosition = new Vector2(0.5f, 0.5f);

        [Tooltip("The distance from the module mount to place the camera during module selection.")]
        [SerializeField]
        protected float moduleMountViewDistance = 6;

        [Tooltip("The maximum vertical angle to view the module mount during module selection.")]
        [SerializeField]
        protected float maxModuleMountViewingAngle = 30;

        protected float gimbalHorizontalTargetAngle = 0;
        protected float gimbalVerticalTargetAngle = 0;


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

        protected RaycastHitComparer raycastHitComparer;



        protected virtual void Reset()
        {
            loadoutManager = transform.root.GetComponentInChildren<LoadoutManager>();
            loadoutUIController = transform.root.GetComponentInChildren<LoadoutUIController>();
            displayManager = transform.root.GetComponentInChildren<LoadoutDisplayManager>();
            viewRotationGimbal = GetComponentInChildren<GimbalController>();

            viewRotationBlockingLayers = LayerMask.GetMask("UI");        
        }


        protected virtual void Awake()
        {
            loadoutUIController.onVehicleSelectionMode.AddListener(OnVehicleSelectionMode);
            loadoutUIController.onModuleSelectionMode.AddListener(OnModuleSelectionMode);

            raycastHitComparer = new RaycastHitComparer();
        }


        /// <summary>
        /// Called when the loadout menu enters Vehicle Selection Mode.
        /// </summary>
        protected virtual void OnVehicleSelectionMode()
        {
            if (viewRotationGimbal != null)
            {
                // Set the view rotation angle targets
                gimbalHorizontalTargetAngle = defaultVehicleFocusAngles.y;
                gimbalVerticalTargetAngle = defaultVehicleFocusAngles.x;
            }
        }


        /// <summary>
        /// Called when the loadout menu enters Module Selection Mode.
        /// </summary>
        protected virtual void OnModuleSelectionMode() { }


        /// <summary>
        /// Called every frame that the loadout menu is in Vehicle Selection Mode.
        /// </summary>
        protected virtual void VehicleSelectionModeUpdate()
        {

            if (loadoutManager.LoadoutData.SelectedSlot == null) return;
            if (loadoutManager.LoadoutData.SelectedSlot.selectedVehicleIndex == -1) return;

            Vehicle vehicle = displayManager.DisplayVehicles[loadoutManager.LoadoutData.SelectedSlot.selectedVehicleIndex];           

            if (viewRotationGimbal != null)
            {
                // Position the camera gimbal

                if (snapPositionAndRotation)
                {
                    viewRotationGimbal.transform.position = displayManager.VehicleHolder.transform.position;
                }
                else
                {
                    viewRotationGimbal.transform.position = Vector3.Lerp(viewRotationGimbal.transform.position, displayManager.VehicleHolder.transform.position, cameraTransitionMoveSpeed * Time.deltaTime);
                }


                // Adjust the camera distance

                if (adjustViewDistanceToVehicleSize)
                {
                    cameraDistanceTarget = GetVehicleViewDistance(vehicle);
                }
                else
                {
                    cameraDistanceTarget = defaultVehicleViewDistance;
                }
            }
        }


        /// <summary>
        /// Called every frame that the loadout menu is in Module Selection Mode.
        /// </summary>
        protected virtual void ModuleSelectionModeUpdate()
        {
            if (loadoutManager.LoadoutData.SelectedSlot == null) return;
            if (loadoutManager.LoadoutData.SelectedSlot.selectedVehicleIndex == -1) return;
            if (loadoutManager.SelectedModuleMountIndex == -1) return;


            // Get references

            LoadoutVehicleItem loadoutVehicleItem = loadoutManager.Items.vehicles[loadoutManager.LoadoutData.SelectedSlot.selectedVehicleIndex];
            Vehicle vehicle = displayManager.DisplayVehicles[loadoutManager.LoadoutData.SelectedSlot.selectedVehicleIndex];
            ModuleMount moduleMount = vehicle.ModuleMounts[loadoutManager.SelectedModuleMountIndex];

            if (viewRotationGimbal != null)
            {
                // Position the camera

                if (snapPositionAndRotation)
                {
                    viewRotationGimbal.transform.position = moduleMount.transform.position;
                }
                else
                {
                    viewRotationGimbal.transform.position = Vector3.Lerp(viewRotationGimbal.transform.position, moduleMount.transform.position, cameraTransitionMoveSpeed * Time.deltaTime);
                }


                // Calculate the camera gimbal rotation targets

                int index = vehicle.ModuleMounts.IndexOf(moduleMount);
                if (loadoutVehicleItem.ModuleMountViewingAngles.Count > index)
                {
                    gimbalHorizontalTargetAngle = loadoutVehicleItem.ModuleMountViewingAngles[index].horizontalViewAngle;
                    gimbalVerticalTargetAngle = loadoutVehicleItem.ModuleMountViewingAngles[index].verticalViewAngle;
                }
                else
                {
                    // Calculate the position target for the camera

                    Vector3 offset = vehicle.transform.TransformPoint(vehicle.Bounds.center) - moduleMount.transform.position;
                    float maxOffsetY = Mathf.Tan(maxModuleMountViewingAngle * Mathf.Deg2Rad) * offset.magnitude;
                    offset.y = Mathf.Clamp(offset.y, -maxOffsetY, maxOffsetY);

                    if (offset.magnitude < 0.0001f)
                    {
                        offset = Vector3.forward;
                    }
                    else
                    {
                        offset.Normalize();
                    }

                    gimbalVerticalTargetAngle = Vector3.Angle(offset, new Vector3(offset.x, 0, offset.z).normalized);
                    if (offset.y > 0) gimbalVerticalTargetAngle *= -1;

                    gimbalHorizontalTargetAngle = Vector3.Angle(new Vector3(offset.x, 0, offset.z).normalized, Vector3.forward);
                    if (offset.x < 0) gimbalHorizontalTargetAngle *= -1;
                }


                // Adjust the camera distance

                cameraDistanceTarget = GetModuleMountViewDistance(moduleMount, vehicle);
            }
        }


        /// <summary>
        /// Set the view rotation inputs (for input scripts).
        /// </summary>
        /// <param name="inputValues">The input values (positive/negative for each rotation axis).</param>
        public virtual void SetViewRotationInputs(Vector3 inputValues)
        {
            if (IsPointerOverBlockingLayer()) return;

            gimbalRotationInputTarget = inputValues;
        }


        /// <summary>
        /// Rotate the camera view based on the current inputs.
        /// </summary>
        protected virtual void RotateView()
        {
            if (viewRotationGimbal == null) return;

            if (loadoutUIController.State == LoadoutUIController.UIState.ModuleSelection) return;

            if (IsPointerOverBlockingLayer())
            {
                gimbalRotationInputTarget = Vector2.zero;
            }

            gimbalHorizontalTargetAngle += gimbalRotationInputCurrent.x * viewRotationSpeed * Time.deltaTime;
            gimbalHorizontalTargetAngle %= 360f;

            gimbalVerticalTargetAngle += gimbalRotationInputCurrent.y * viewRotationSpeed * Time.deltaTime;
            gimbalVerticalTargetAngle %= 360f;

            // Apply the view rotation immediately to the gimbal to separate it from the camera transition rotation smoothing.
            viewRotationGimbal.Rotate(gimbalRotationInputCurrent.x * viewRotationSpeed * Time.deltaTime, gimbalRotationInputCurrent.y * viewRotationSpeed * Time.deltaTime);
        }


        /// <summary>
        /// Update the camera gimbal.
        /// </summary>
        protected virtual void UpdateGimbal()
        {
            viewRotationGimbal.ApplyConstraints(ref gimbalHorizontalTargetAngle, ref gimbalVerticalTargetAngle);

            Quaternion horizontalRotation = Quaternion.Euler(0, gimbalHorizontalTargetAngle, 0);
            Quaternion verticalRotation = Quaternion.Euler(gimbalVerticalTargetAngle, 0, 0);

            if (!cameraGimbalRotationsInitialized)
            {
                cameraGimbalRotationsInitialized = true;
            }
            else
            {
                if (!snapPositionAndRotation)
                {
                    horizontalRotation = Quaternion.Slerp(viewRotationGimbal.HorizontalPivot.localRotation, horizontalRotation, cameraTransitionRotationSpeed * Time.deltaTime);
                    verticalRotation = Quaternion.Slerp(viewRotationGimbal.VerticalPivot.localRotation, verticalRotation, cameraTransitionRotationSpeed * Time.deltaTime);
                }
            }

            viewRotationGimbal.SetGimbalRotation(horizontalRotation, verticalRotation);
        }


        /// <summary>
        /// Update the camera view distance.
        /// </summary>
        protected virtual void UpdateCameraViewDistance()
        {
            if (!cameraDistanceInitialized)
            {
                viewRotationGimbal.GimbalChild.localPosition = new Vector3(0, 0, -cameraDistanceTarget);
                cameraDistanceInitialized = true;
            }
            else
            {
                if (snapPositionAndRotation)
                {
                    viewRotationGimbal.GimbalChild.localPosition = new Vector3(0, 0, -cameraDistanceTarget);
                }
                else
                {
                    viewRotationGimbal.GimbalChild.localPosition = Vector3.Lerp(viewRotationGimbal.GimbalChild.localPosition, new Vector3(0, 0, -cameraDistanceTarget), cameraTransitionMoveSpeed * Time.deltaTime);
                }
            }
        }


        /// <summary>
        /// Calculate the vehicle view distance for the camera.
        /// </summary>
        /// <param name="vehicle">The vehicle the camera is focused on.</param>
        /// <returns>The vehicle view distance.</returns>
        protected virtual float GetVehicleViewDistance(Vehicle vehicle)
        {
            float diameter = Mathf.Max(new float[] { vehicle.Bounds.size.x, vehicle.Bounds.size.y, vehicle.Bounds.size.z });

            float halfAngle;
            bool horizontalLimited = Mathf.Max(vehicle.Bounds.size.x, vehicle.Bounds.size.z) > vehicle.Bounds.size.y * m_Camera.aspect;
            if (horizontalLimited)
            {
                float tmp = 0.5f / Mathf.Tan((m_Camera.fieldOfView / 2) * Mathf.Deg2Rad);
                halfAngle = Mathf.Atan((0.5f * m_Camera.aspect) / tmp) * Mathf.Rad2Deg;
            }
            else
            {
                halfAngle = m_Camera.fieldOfView / 2;
            }

            // Calculate the distance of the camera to the target vehicle to achieve the viewport size
            float distance = ((diameter / 2) / maxViewportVehicleDiameter) / Mathf.Tan(halfAngle * Mathf.Deg2Rad);

            return distance;
        }


        /// <summary>
        /// Get the camera view distance for a module mount.
        /// </summary>
        /// <param name="moduleMount">The module mount.</param>
        /// <param name="vehicle">The vehicle that the module mount is part of.</param>
        /// <returns>The camera view distance.</returns>
        protected virtual float GetModuleMountViewDistance(ModuleMount moduleMount, Vehicle vehicle)
        {
            return moduleMountViewDistance;
        }


        /// <summary>
        /// Get whether the cursor is over a blocking layer (e.g. UI) to prevent unwanted view rotation.
        /// </summary>
        /// <returns>Whether the cursor is over a blocking layer.</returns>
        protected virtual bool IsPointerOverBlockingLayer()
        {
            // Get event system raycast results
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);

            // Check if the raycast results are on a blocking layer
            for (int i = 0; i < raycastResults.Count; i++)
            {
                RaycastResult result = raycastResults[i];
                if (viewRotationBlockingLayers == (viewRotationBlockingLayers | (1 << result.gameObject.layer))) return true;
            }

            return false;
        }


        /// <summary>
        /// Called every frame.
        /// </summary>
        protected virtual void Update()
        {
            if (loadoutUIController.State == LoadoutUIController.UIState.ModuleSelection)
            {
                ModuleSelectionModeUpdate();
            }
            else
            {
                VehicleSelectionModeUpdate();
            }

            RotateView();           

            UpdateGimbal();

            UpdateCameraViewDistance();

            CameraCollisionUpdate();
        }


        protected virtual void LateUpdate()
        {
            gimbalRotationInputCurrent = Vector2.Lerp(gimbalRotationInputCurrent, gimbalRotationInputTarget, viewRotationLerpSpeed * Time.deltaTime);
        }


        /// <summary>
        /// Apply camera collisions to make sure the camera never intersects geometry.
        /// </summary>
        protected void CameraCollisionUpdate()
        {
            if (cameraCollisionEnabled)
            {
                Vehicle vehicle = null;

                if (loadoutManager.LoadoutData.SelectedSlot != null)
                {
                    if (loadoutManager.LoadoutData.SelectedSlot.selectedVehicleIndex != -1)
                    {
                        vehicle = displayManager.DisplayVehicles[loadoutManager.LoadoutData.SelectedSlot.selectedVehicleIndex];

                    }
                }

                // Initialize the target position for the camera
                float targetDistance = vehicleFocusCameraHolder.transform.localPosition.z;


                // Prepare the spherecast
                Vector3 sphereCastStart = viewRotationGimbal.HorizontalPivot.position;
                Vector3 sphereCastEnd = vehicleFocusCameraHolder.transform.position;
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
                    if (hitRigidbody != null) continue;

                    // Set the camera position to the hit point
                    targetDistance = -hits[i].distance;

                    break;
                }

                // Update the camera position
                vehicleFocusCameraHolder.transform.localPosition = new Vector3(0, 0, targetDistance);
            }
        }
    }
}

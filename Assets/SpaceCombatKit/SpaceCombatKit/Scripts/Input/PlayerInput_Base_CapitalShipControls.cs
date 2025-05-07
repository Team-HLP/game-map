using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VSX.Utilities;
using VSX.Vehicles;
using VSX.Engines3D;
using VSX.VehicleCombatKits;
using VSX.CameraSystem;

namespace VSX.SpaceCombatKit
{

    /// <summary>
    /// Base class for a player input script that controls a capital ship.
    /// </summary>
    public class PlayerInput_Base_CapitalShipControls : VehicleInput
    {

        [Tooltip("How fast the capital ship camera rotates.")]
        [SerializeField]
        protected float minLookRotationSpeed = 20;

        [Tooltip("How fast the capital ship camera rotates.")]
        [SerializeField]
        protected float maxLookRotationSpeed = 80;

        [Tooltip("Whether to invert the look rotation vertical input for each input device type.")]
        [SerializeField]
        protected InputInvertSettings invertLookVertical;

        [Tooltip("Whether to set the throttle to full when input is pressed (vs continuously moving the throttle up and down while the input is pressed).")]
        [SerializeField]
        protected bool setThrottle = false;

        [Tooltip("How fast the throttle changes when accelerating or decelerating.")]
        [SerializeField]
        protected float throttleSensitivity = 0.5f;

        [Tooltip("How fast the boost level changes to full when the input is pressed.")]
        [SerializeField]
        protected float boostChangeSpeed = 3;

        [Tooltip("The PID controller for auto levelling the ship.")]
        [SerializeField]
        protected ShipPIDController shipPIDController;




        [Tooltip("The minimum field-of-view (represents the zoom-in limit)")]
        [SerializeField]
        protected float minFOV = 20;

        [Tooltip("The speed that the zoom changes in response to input.")]
        [SerializeField]
        protected float zoomSpeed = 2;

        protected float currentFOV;

        protected float zoomInputValue;




        protected VehicleEngines3D engines;
        protected GimbalController gimbalController;
        CameraTarget cameraTarget;
        protected CameraEntity cameraEntity;
        protected float initialFOV;

        protected Vector3 movementInputValue;
        protected Vector3 steeringInputValue;
        protected Vector3 boostInputValue;
        protected Vector2 lookInputValue;



        /// <summary>
        /// Initialize this input script with a vehicle.
        /// </summary>
        /// <param name="vehicle">The input target vehicle.</param>
        /// <returns>Whether initialization succeeded.</returns>
        protected override bool Initialize(Vehicle vehicle)
        {
            if (!base.Initialize(vehicle)) return false;

            engines = vehicle.GetComponent<VehicleEngines3D>();

            if (engines == null)
            {
                if (debugInitialization)
                {
                    Debug.LogWarning(GetType().Name + " failed to initialize - the required " + engines.GetType().Name + " component was not found on the vehicle.");
                }

                return false;
            }

            if (debugInitialization)
            {
                Debug.Log(GetType().Name + " successfully initialized.");
            }

            return true;

        }


        protected override void OnInitialized(GameObject targetObject)
        {
            base.OnInitialized(targetObject);

            engines = targetObject.GetComponent<VehicleEngines3D>();
            gimbalController = targetObject.GetComponent<GimbalController>();

            cameraTarget = targetObject.GetComponentInChildren<CameraTarget>();
            if (cameraTarget != null)
            {
                cameraEntity = cameraTarget.CameraEntity;
                cameraTarget.onCameraEntityTargeting.AddListener(SetCameraEntity);
            }
        }


        protected override void OnUninitialized(GameObject targetObject)
        {
            base.OnUninitialized(targetObject);

            cameraTarget.onCameraEntityTargeting.RemoveListener(SetCameraEntity);
            if (cameraEntity != null) cameraEntity.SetFieldOfView(cameraEntity.DefaultFieldOfView);
        }


        protected virtual void SetCameraEntity(CameraEntity cameraEntity)
        {
            if (this.cameraEntity != null) this.cameraEntity.SetFieldOfView(this.cameraEntity.DefaultFieldOfView);

            this.cameraEntity = cameraEntity;

            if (cameraEntity != null)
            {
                currentFOV = cameraEntity.FieldOfView;
            }
        }


        protected virtual void SteeringUpdate()
        {
            Vector3 nextSteeringInputs = engines.SteeringInputs;

            Vector3 flattenedForward = new Vector3(engines.transform.forward.x, 0f, engines.transform.forward.z).normalized;
            Maneuvring.TurnToward(engines.transform, engines.transform.position + flattenedForward, new Vector3(0f, 360f, 0f), shipPIDController.steeringPIDController);

            nextSteeringInputs.x = shipPIDController.steeringPIDController.GetControlValue(PIDController3D.Axis.X);
            nextSteeringInputs.y = steeringInputValue.y;
            nextSteeringInputs.z = shipPIDController.steeringPIDController.GetControlValue(PIDController3D.Axis.Z);

            engines.SetSteeringInputs(nextSteeringInputs);
        }


        protected virtual void MovementUpdate()
        {
            Vector3 nextMovementInputs = engines.MovementInputs;
            Vector3 nextBoostInputs = engines.BoostInputs;

            // Boost

            nextBoostInputs = Vector3.Lerp(nextBoostInputs, boostInputValue, boostChangeSpeed * Time.deltaTime);
            engines.SetBoostInputs(nextBoostInputs);
            if (engines.BoostInputs.z > 0.5f)
            {
                nextMovementInputs.z = 1;
            }

            // Movement

            if (setThrottle)
            {
                nextMovementInputs = movementInputValue;
            }
            else
            {
                nextMovementInputs.x = movementInputValue.x;
                nextMovementInputs.y = movementInputValue.y;
                nextMovementInputs.z += movementInputValue.z * throttleSensitivity * Time.deltaTime;
            }

            engines.SetMovementInputs(nextMovementInputs);
        }


        protected virtual void LookUpdate()
        {
            if (gimbalController != null)
            {
                
                float nextLookRotationSpeed = maxLookRotationSpeed;
                if (cameraEntity != null && !Mathf.Approximately(minFOV, cameraEntity.DefaultFieldOfView))
                {
                    float zoomOutAmount = Mathf.Clamp((cameraEntity.FieldOfView - minFOV) / (cameraEntity.DefaultFieldOfView - minFOV), 0, 1);
                    nextLookRotationSpeed = zoomOutAmount * maxLookRotationSpeed + (1 - zoomOutAmount) * minLookRotationSpeed;
                }
                Vector2 rotation = nextLookRotationSpeed * lookInputValue * Time.deltaTime;

                switch (GetLookInputDeviceType())
                {
                    case InputDeviceType.Mouse:

                        if (invertLookVertical.InvertMouse) rotation.y *= -1;

                        break;

                    case InputDeviceType.Keyboard:

                        if (invertLookVertical.InvertKeyboard) rotation.y *= -1;

                        break;

                    case InputDeviceType.Gamepad:

                        if (invertLookVertical.InvertGamepad) rotation.y *= -1;

                        break;

                    case InputDeviceType.Joystick:

                        if (invertLookVertical.InvertJoystick) rotation.y *= -1;

                        break;
                }

                gimbalController.Rotate(rotation.x, -rotation.y);
            }
        }


        protected virtual void ZoomUpdate()
        {
            if (cameraEntity == null) return;

            // Calculate the FOV
            currentFOV = Mathf.Clamp(currentFOV - zoomInputValue * zoomSpeed * Time.deltaTime, minFOV, cameraEntity.DefaultFieldOfView);

            // Set the FOV
            cameraEntity.SetFieldOfView(currentFOV);
        }


        protected virtual InputDeviceType GetLookInputDeviceType()
        {
            return InputDeviceType.None;
        }


        protected override void OnInputUpdate()
        {
            SteeringUpdate();

            MovementUpdate();

            LookUpdate();

            ZoomUpdate();
        }
    }
}

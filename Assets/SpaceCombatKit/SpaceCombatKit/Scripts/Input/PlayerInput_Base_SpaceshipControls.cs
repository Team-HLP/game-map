using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;
using VSX.Engines3D;
using VSX.Utilities;
using VSX.VehicleCombatKits;

namespace VSX.SpaceCombatKit
{
    /// <summary>
    /// Base class for a player input script that controls a spacefighter style ship.
    /// </summary>
    public class PlayerInput_Base_SpaceshipControls : VehicleInput
    {

        [Tooltip("Whether the vehicle should yaw when rolling.")]
        [SerializeField]
        protected bool linkYawAndRoll = false;

        [Tooltip("How much the vehicle should yaw when rolling.")]
        [SerializeField]
        protected float yawRollRatio = 0.5f;

        /// <summary>
        /// Stores settings for whether to enable auto roll while specific input devices are operating.
        /// </summary>
        [System.Serializable]
        public class AutoLevelDeviceSettings
        {
            public bool mouse;
            public bool keyboard;
            public bool gamepad;
            public bool joystick;
        }

        [Tooltip("Whether auto roll should be activated - based on the last steering input device.")]
        [SerializeField]
        protected AutoLevelDeviceSettings autoLevelSettings;
        protected bool autoLevelActive;

        [Tooltip("The amount of roll that is added proportional to the bank angle.")]
        [SerializeField]
        protected float autoLevelStrength = 0.04f;

        [Tooltip("The maximum roll input value that auto roll can set.")]
        [SerializeField]
        protected float maxAutoLevelInput = 0.2f;

        [Tooltip("The time after the last roll input when auto roll will kick in.")]
        [SerializeField]
        protected float autoLevelDelay = 0.5f;

        protected float lastRollInputTime;

        protected float autoLevelInputValue;

        [Tooltip("Whether to invert the vertical steering for each input device.")]
        [SerializeField]
        protected InputInvertSettings invertVerticalSteering;


        [Header("Mouse Steering")]

        [Tooltip("Whether mouse input is enabled.")]
        [SerializeField]
        protected bool mouseEnabled = true;
        public bool MouseEnabled
        {
            get { return mouseEnabled; }
            set { mouseEnabled = value; }
        }

        [Tooltip("The type of mouse steering to apply.")]
        [SerializeField]
        protected MouseSteeringType mouseSteeringType;
        public MouseSteeringType MouseSteeringType
        {
            get { return mouseSteeringType; }
            set { mouseSteeringType = value; }
        }

        [Tooltip("Whether this script controls the HUD cursor.")]
        [SerializeField]
        protected bool controlHUDCursor = true;


        [Header("Mouse Screen Position Settings")]


        [Tooltip("The fraction of the viewport (based on the screen width) around the screen center inside which the mouse position does not affect the ship steering.")]
        [SerializeField]
        protected float mouseDeadRadius = 0.1f;

        [Tooltip("How far the mouse reticule is allowed to get from the screen center.")]
        [SerializeField]
        protected float maxReticleDistanceFromCenter = 0.475f;

        [Tooltip("How fast the reticle moves in response to input.")]
        [SerializeField]
        protected float reticleMovementSpeed = 1;

        [Tooltip("How much the ship pitches (local X axis rotation) based on the mouse distance from screen center.")]
        [SerializeField]
        protected AnimationCurve mousePositionInputCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Tooltip("Whether to center the cursor when this input starts running.")]
        [SerializeField]
        protected bool centerCursorOnInputEnabled = true;


        [Header("Mouse Delta Position Settings")]

        [Tooltip("How fast the ship responds to mouse delta input.")]
        [SerializeField]
        protected float mouseDeltaPositionSensitivity = 0.05f;

        [Tooltip("The curve that describes the input value that is applied, based on the mouse delta value.")]
        [SerializeField]
        protected AnimationCurve mouseDeltaPositionInputCurve = AnimationCurve.Linear(0, 0, 1, 1);


        [Header("Throttle")]

        [Tooltip("Whether the throttle is set to full when input is pressed, vs moving continuously over time while input is pressed.")]
        [SerializeField]
        protected bool setThrottle = false;

        [Tooltip("How fast the throttle moves up and down when Set Throttle is set to false.")]
        [SerializeField]
        protected float throttleSensitivity = 1;

        protected bool steeringEnabled = true;

        protected bool movementEnabled = true;


        [Header("Boost")]

        [Tooltip("How fast the boost responds to input.")]
        [SerializeField]
        protected float boostChangeSpeed = 3;

        // Reference to the engines component on the current vehicle
        protected VehicleEngines3D engines;

        protected CustomCursor hudCursor;
        protected Vector3 reticleViewportPosition = new Vector3(0.5f, 0.5f, 0);

        protected Vector3 steeringInputs = Vector3.zero;
        protected Vector3 movementInputs = Vector3.zero;
        protected Vector3 boostInputs = Vector3.zero;


        /// <summary>
        /// Initialize this input script with a vehicle.
        /// </summary>
        /// <param name="vehicle">The input target vehicle.</param>
        /// <returns>Whether initialization succeeded.</returns>
        protected override bool Initialize(Vehicle vehicle)
        {

            reticleViewportPosition = new Vector3(0.5f, 0.5f, 0);

            if (!base.Initialize(vehicle)) return false;

            // Clear dependencies
            engines = null;

            // Make sure the vehicle has a space vehicle engines component
            engines = vehicle.GetComponent<VehicleEngines3D>();

            hudCursor = vehicle.GetComponentInChildren<CustomCursor>();

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
                Debug.Log(GetType().Name + " component successfully initialized.");
            }

            if (centerCursorOnInputEnabled && controlHUDCursor && hudCursor != null)
            {
                hudCursor.CenterCursor();
            }

            return true;
        }


        /// <summary>
        /// Enable steering input.
        /// </summary>
        public virtual void EnableSteering()
        {
            steeringEnabled = true;
        }


        /// <summary>
        /// Disable steering input.
        /// </summary>
        /// <param name="clearCurrentValues">Whether to clear current steering values.</param>
        public virtual void DisableSteering(bool clearCurrentValues)
        {
            steeringEnabled = false;

            if (clearCurrentValues)
            {
                engines.SetSteeringInputs(Vector3.zero);
            }
        }


        /// <summary>
        /// Enable movement input.
        /// </summary>
        public virtual void EnableMovement()
        {
            movementEnabled = true;
        }


        /// <summary>
        /// Disable the movement input.
        /// </summary>
        /// <param name="clearCurrentValues">Whether to clear current throttle values.</param>
        public virtual void DisableMovement(bool clearCurrentValues)
        {
            movementEnabled = false;

            if (clearCurrentValues)
            {
                engines.SetMovementInputs(Vector3.zero);

                engines.SetBoostInputs(Vector3.zero);
            }
        }


        // Get the current mouse viewport position.
        protected virtual Vector3 GetMouseViewportPosition()
        {
            return new Vector3(0.5f, 0.5f, 0);
        }


        // Get the input device that is currently controlling the steering.
        protected virtual InputDeviceType GetSteeringInputDeviceType()
        {
            return InputDeviceType.None;
        }


        // Apply mouse steering.
        protected virtual void MouseSteeringUpdate()
        {
            if (!mouseEnabled) return;

            if (mouseSteeringType == MouseSteeringType.ScreenPosition)
            {
                if (hudCursor != null)
                {
                    Vector3 delta = new Vector3(steeringInputs.y / Screen.width, steeringInputs.x / Screen.height, 0);
                    
                    if (invertVerticalSteering.InvertMouse) delta.y *= -1;

                    // Add the delta 
                    reticleViewportPosition += delta * reticleMovementSpeed;

                    // Center it
                    Vector3 centeredReticleViewportPosition = reticleViewportPosition - new Vector3(0.5f, 0.5f, 0);

                    // Prevent distortion before clamping
                    centeredReticleViewportPosition.x *= (float)Screen.width / Screen.height;

                    // Clamp
                    centeredReticleViewportPosition = Vector3.ClampMagnitude(centeredReticleViewportPosition, maxReticleDistanceFromCenter);

                    // Convert back to proper viewport
                    centeredReticleViewportPosition.x /= (float)Screen.width / Screen.height;

                    reticleViewportPosition = centeredReticleViewportPosition + new Vector3(0.5f, 0.5f, 0);
                }
                else
                {
                    reticleViewportPosition = GetMouseViewportPosition();
                }
            }
            else if (mouseSteeringType == MouseSteeringType.DeltaPosition)
            {
                reticleViewportPosition = new Vector3(0.5f, 0.5f, 0);
            }

            if (controlHUDCursor && hudCursor != null)
            {
                hudCursor.SetViewportPosition(reticleViewportPosition);
            }

            // Implement control type
            Vector3 processedScreenInputs = Vector3.zero;
            if (mouseSteeringType == MouseSteeringType.ScreenPosition)
            {
                processedScreenInputs = reticleViewportPosition - new Vector3(0.5f, 0.5f, 0);

                processedScreenInputs.x *= (float)Screen.width / Screen.height;

                float amount = Mathf.Max(processedScreenInputs.magnitude - mouseDeadRadius, 0) / (maxReticleDistanceFromCenter - mouseDeadRadius);

                processedScreenInputs.x /= (float)Screen.width / Screen.height;

                processedScreenInputs = mousePositionInputCurve.Evaluate(amount) * processedScreenInputs.normalized;
            }
            else if (mouseSteeringType == MouseSteeringType.DeltaPosition)
            {
                processedScreenInputs = mouseDeltaPositionSensitivity * new Vector3(steeringInputs.y, steeringInputs.x, 0);
                processedScreenInputs = Mathf.Clamp(mouseDeltaPositionInputCurve.Evaluate(processedScreenInputs.magnitude), 0, 1) * processedScreenInputs.normalized;
            }

            Vector3 nextSteeringInputs = Vector3.zero;
            nextSteeringInputs.x = -processedScreenInputs.y;
            nextSteeringInputs.y = processedScreenInputs.x;


            if (mouseSteeringType != MouseSteeringType.ScreenPosition && invertVerticalSteering.InvertMouse)
            {
                nextSteeringInputs.x *= -1;
            }

            nextSteeringInputs.x = Mathf.Clamp(nextSteeringInputs.x, -1f, 1f);

            nextSteeringInputs.y = Mathf.Clamp(nextSteeringInputs.y, -1f, 1f);

            // Linked yaw and roll
            if (linkYawAndRoll && mouseSteeringType != MouseSteeringType.ScreenPosition)
            {
                if(Mathf.Abs(steeringInputs.z) > Mathf.Abs(nextSteeringInputs.y * yawRollRatio))
                {
                    nextSteeringInputs.z = steeringInputs.z;
                }
                else
                {
                    nextSteeringInputs.z = Mathf.Clamp(-nextSteeringInputs.y * yawRollRatio, -1f, 1f);
                }
            }
            else
            {
                nextSteeringInputs.z = steeringInputs.z;
            }

            if (autoLevelActive)
            {
                nextSteeringInputs.z = autoLevelInputValue;
            }

            engines.SetSteeringInputs(nextSteeringInputs);
        }


        // Apply non-mouse (gamepad/keyboard/etc) steering.
        protected virtual void NonMouseSteeringUpdate()
        {
            reticleViewportPosition = new Vector3(0.5f, 0.5f, 0);

            if (controlHUDCursor && hudCursor != null)
            {
                hudCursor.SetViewportPosition(reticleViewportPosition);
            }

            Vector3 nextSteeringInputs = Vector3.zero;
            nextSteeringInputs.x = -steeringInputs.x;
            nextSteeringInputs.y = steeringInputs.y;

            // Apply invert
            switch (GetSteeringInputDeviceType())
            {
                case InputDeviceType.Keyboard:

                    if (invertVerticalSteering.InvertKeyboard) nextSteeringInputs.x *= -1;

                    break;

                case InputDeviceType.Gamepad:

                    if (invertVerticalSteering.InvertGamepad) nextSteeringInputs.x *= -1;

                    break;

                case InputDeviceType.Joystick:

                    if (invertVerticalSteering.InvertJoystick) nextSteeringInputs.x *= -1;

                    break;
            }

            // Linked yaw and roll
            if (linkYawAndRoll)
            {
                if (Mathf.Abs(steeringInputs.z) > Mathf.Abs(nextSteeringInputs.y * yawRollRatio))
                {
                    nextSteeringInputs.z = steeringInputs.z;
                }
                else
                {
                    nextSteeringInputs.z = Mathf.Clamp(-nextSteeringInputs.y * yawRollRatio, -1f, 1f);
                }
            }
            else
            {
                nextSteeringInputs.z = steeringInputs.z;
            }

            if (autoLevelActive)
            {
                nextSteeringInputs.z = autoLevelInputValue;
            }           

            engines.SetSteeringInputs(nextSteeringInputs);
        }


        protected virtual void StartAutoRoll()
        {
            if (!autoLevelActive)
            {
                autoLevelActive = true;
            }
        }


        protected virtual void StopAutoRoll()
        {
            if (autoLevelActive)
            {
                Vector3 _steeringInputs = engines.SteeringInputs;
                _steeringInputs.z = 0;
                engines.SetSteeringInputs(_steeringInputs);
            }

            autoLevelActive = false;
        }


        protected virtual void UpdateAutoRollState()
        {
            if (Mathf.Abs(steeringInputs.z) > 0.001f || (linkYawAndRoll && Mathf.Abs(steeringInputs.y * yawRollRatio) > 0.001f))
            {
                lastRollInputTime = Time.time;
                StopAutoRoll();
                return;
            }

            if (GetSteeringInputDeviceType() == InputDeviceType.Mouse && !autoLevelSettings.mouse) { StopAutoRoll(); return; }
            else if (GetSteeringInputDeviceType() == InputDeviceType.Keyboard && !autoLevelSettings.keyboard) { StopAutoRoll(); return; }
            else if (GetSteeringInputDeviceType() == InputDeviceType.Gamepad && !autoLevelSettings.gamepad) { StopAutoRoll(); return; }
            else if (GetSteeringInputDeviceType() == InputDeviceType.Joystick && !autoLevelSettings.joystick) { StopAutoRoll(); return; }

            if (Time.time - lastRollInputTime < autoLevelDelay) { StopAutoRoll(); return; }

            StartAutoRoll();
        }


        // Auto roll returns the ship to level flight slowly over time while roll input is not being applied.
        protected virtual void AutoRollUpdate()
        {
          
            // Project the forward vector down
            Vector3 flattenedFwd = engines.transform.forward;
            flattenedFwd.y = 0;
            flattenedFwd.Normalize();

            // Get the right
            Vector3 right = Vector3.Cross(Vector3.up, flattenedFwd);

            float angle = Vector3.Angle(right, engines.transform.right);

            if (Vector3.Dot(engines.transform.up, right) > 0)
            {
                angle *= -1;
            }

            autoLevelInputValue = Mathf.Clamp(angle * -1 * autoLevelStrength, -1, 1) * maxAutoLevelInput;

            autoLevelInputValue *= 1 - Mathf.Abs(Vector3.Dot(engines.transform.forward, Vector3.up));
        }


        // Called every frame this input is running.
        protected override void OnInputUpdate()
        {

            Vector3 nextMovementInputs = engines.MovementInputs;
            Vector3 nextBoostInputs = engines.BoostInputs;

            nextBoostInputs = Vector3.Lerp(nextBoostInputs, boostInputs, boostChangeSpeed * Time.deltaTime);
            engines.SetBoostInputs(nextBoostInputs);

            if (engines.BoostInputs.z > 0.5f)
            {
                nextMovementInputs.z = 1;
            }


            if (setThrottle)
            {
                nextMovementInputs = movementInputs;
            }
            else
            {
                nextMovementInputs.x = movementInputs.x;
                nextMovementInputs.y = movementInputs.y;
                nextMovementInputs.z += movementInputs.z * throttleSensitivity * Time.deltaTime;
            }

            engines.SetMovementInputs(nextMovementInputs);


            if (GetSteeringInputDeviceType() == InputDeviceType.Mouse)
            {
                MouseSteeringUpdate();
            }
            else
            {
                NonMouseSteeringUpdate();
            }

            UpdateAutoRollState();

            if (autoLevelActive)
            {
                AutoRollUpdate();
            }
        }
    }
}
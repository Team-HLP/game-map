using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


namespace VSX.Utilities
{
    /// <summary>
    /// Enables running of events and actions when specified input devices are connected 
    /// </summary>
    public class InputSystemDeviceConnectedEvents : MonoBehaviour
    {
        /// <summary>
        /// Input device types.
        /// </summary>
        public enum InputSystemDeviceType
        {
            Keyboard,
            Mouse,
            Gamepad,
            Joystick
        }

        [Tooltip("The device types to run events/actions for.")]
        [SerializeField]
        protected List<InputSystemDeviceType> deviceTypes = new List<InputSystemDeviceType>();

        [Tooltip("Called every time any of the device types listed are connected.")]
        public UnityEvent onRelevantDeviceConnected;

        [Tooltip("Called when none of the device types listed are connected.")]
        public UnityEvent onNoRelevantDevicesConnected;

        protected bool wasRelevantDeviceConnected = false;



        protected virtual void Awake()
        {
            InputSystem.onDeviceChange += (inputDevice, deviceChange) => { UpdateStatus(); };
        }


        protected virtual void Start()
        {
            UpdateStatus();

            if (!wasRelevantDeviceConnected) OnNoRelevantDevicesConnected();
        }


        protected virtual bool IsRelevantDevice(InputDevice device)
        {
            foreach (InputSystemDeviceType m_deviceType in deviceTypes)
            {
                System.Type comparisonType = GetDeviceTypeFromEnum(m_deviceType);
                if ((device.GetType() == comparisonType) || (device.GetType().IsSubclassOf(comparisonType)))
                {
                    return true;
                }
            }

            return false;
        }


        protected virtual System.Type GetDeviceTypeFromEnum(InputSystemDeviceType deviceType)
        {
            switch (deviceType)
            {
                case InputSystemDeviceType.Keyboard:

                    return typeof(Keyboard);

                case InputSystemDeviceType.Mouse:

                    return typeof(Mouse);

                case InputSystemDeviceType.Gamepad:

                    return typeof(Gamepad);

                case InputSystemDeviceType.Joystick:

                    return typeof(Joystick);

            }

            return null;
        }


        protected virtual void UpdateStatus()
        {
            foreach (InputDevice device in InputSystem.devices)
            {
                if (IsRelevantDevice(device))
                {
                    if (!wasRelevantDeviceConnected) OnRelevantDeviceConnected();
                    return;
                }
            }

            if (wasRelevantDeviceConnected) OnNoRelevantDevicesConnected();
        }


        protected virtual void OnRelevantDeviceConnected()
        {
            wasRelevantDeviceConnected = true;
            onRelevantDeviceConnected.Invoke();
        }


        protected virtual void OnNoRelevantDevicesConnected()
        {
            wasRelevantDeviceConnected = false;
            onNoRelevantDevicesConnected.Invoke();
        }
    }
}


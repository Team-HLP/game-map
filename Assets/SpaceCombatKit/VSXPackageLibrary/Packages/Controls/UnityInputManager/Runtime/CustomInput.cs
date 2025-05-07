using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.Controls.UnityInputManager
{
    /// <summary>
    /// This class provides a way to create customised input (using Unity's legacy Input Manager) that can be modified in the inspector.
    /// </summary>
    [System.Serializable]
    public class CustomInput
    {
        // Group

        public string group;

        // Action

        public string action;

        // Input type

        public CustomInputType inputType;

        // Key

        public KeyCode key;

        // Mouse button

        public int mouseButton;

        // Axis

        public bool getAxisRaw;

        public string axis;
        

        public CustomInput (string group, string action, KeyCode key)
        {
            this.group = group;
            this.action = action;
            this.inputType = CustomInputType.Key;
            this.key = key;
        }

        public CustomInput(string group, string action, int mouseButton)
        {
            this.group = group;
            this.action = action;
            this.inputType = CustomInputType.MouseButton;
            this.mouseButton = mouseButton;
        }

        public CustomInput(string group, string action, string axis)
        {
            this.group = group;
            this.action = action;
            this.inputType = CustomInputType.Axis;
            this.axis = axis;
        }


        /// <summary>
        /// Whether this input started being pressed this frame.
        /// </summary>
        /// <returns></returns>
        public virtual bool Down ()
        {
            switch (inputType)
            {
                case CustomInputType.Key:

                    return Input.GetKeyDown(key);
                    
                case CustomInputType.MouseButton:

                    return Input.GetMouseButtonDown(mouseButton);                    

                case CustomInputType.Axis:

                    return Input.GetAxis(axis) > 0.5f;

                default:

                    return false;
            }
        }


        /// <summary>
        /// Whether this input was released this frame.
        /// </summary>
        /// <returns></returns>
        public virtual bool Up ()
        {

            switch (inputType)
            {
                case CustomInputType.Key:

                    return Input.GetKeyUp(key);

                case CustomInputType.MouseButton:

                    return Input.GetMouseButtonUp(mouseButton);

                case CustomInputType.Axis:

                    return Input.GetAxis(axis) < 0.5f;

                default:

                    return false;
            }
        }


        /// <summary>
        /// Whether this input is pressed this frame.
        /// </summary>
        /// <returns></returns>
        public virtual bool Pressed()
        {
            switch (inputType)
            {
                case CustomInputType.Key:

                    return Input.GetKey(key);

                case CustomInputType.MouseButton:

                    return Input.GetMouseButton(mouseButton);

                case CustomInputType.Axis:

                    return Input.GetAxis(axis) > 0.5f;

                default:

                    return false;
            }
        }


        /// <summary>
        /// Get the float value of this input.
        /// </summary>
        /// <returns>The float value.</returns>
        public virtual float FloatValue()
        {
            switch (inputType)
            {
                case CustomInputType.Key:

                    return Input.GetKey(key) ? 1 : 0;

                case CustomInputType.MouseButton:

                    return Input.GetMouseButton(mouseButton) ? 1 : 0;

                case CustomInputType.Axis:

                    if (getAxisRaw)
                    {
                        return Input.GetAxisRaw(axis);
                    }
                    else
                    {
                        return Input.GetAxis(axis);
                    }

                default:

                    return 0;
            }
        }


        /// <summary>
        /// Get the input as a string.
        /// </summary>
        /// <returns>The input as a string.</returns>
        public string GetInputAsString()
        {
            switch (inputType)
            {
                case CustomInputType.Key:

                    return AddSpacesBeforeCapitals(key.ToString());

                case CustomInputType.MouseButton:

                    return "Mouse " + mouseButton.ToString();

                default:

                    return (axis.ToString() + " Axis");
            }
        }

        public string AddSpacesBeforeCapitals(string str)
        {
            string result = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsUpper(str[i]) && i != 0)
                {
                    result += " ";
                }
                result += str[i].ToString();
            }

            return result;
        }
    }
}
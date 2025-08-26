using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace foriver4725.InputManager
{
    internal enum InputType : byte
    {
        /// <summary>
        /// Default value. Has no meaning.<br/>
        /// Input should not be retrieved.<br/>
        /// </summary>
        Null,

        /// <summary>
        /// Whether the button was pressed during this frame.<br/>
        /// Input is retrieved as Bool.<br/>
        /// </summary>
        Click,

        /// <summary>
        /// Whether the button has been held down for a certain duration and detected during this frame.<br/>
        /// Input is retrieved as Bool.<br/>
        /// </summary>
        Hold,

        /// <summary>
        /// The input value in this frame (boolean).<br/>
        /// Input is retrieved as Bool.<br/>
        /// ※ Assumes Action Type is Button, not Pass Through.<br/>
        /// </summary>
        Value0,

        /// <summary>
        /// The input value in this frame (normalized scalar).<br/>
        /// Input is retrieved as Float.<br/>
        /// </summary>
        Value1,

        /// <summary>
        /// The input value in this frame (normalized 2D vector).<br/>
        /// Input is retrieved as Vector2.<br/>
        /// </summary>
        Value2,

        /// <summary>
        /// The input value in this frame (normalized 3D vector).<br/>
        /// Input is retrieved as Vector3.<br/>
        /// </summary>
        Value3
    }

    internal sealed class InputInfo
    {
        private readonly InputType type;

        internal bool Bool { get; private set; } = false;
        internal float Float { get; private set; } = 0;
        internal Vector2 Vector2 { get; private set; } = Vector2.zero;
        internal Vector3 Vector3 { get; private set; } = Vector3.zero;

        internal InputInfo(InputType type) => this.type = type;

        private void SetBoolTrue(InputAction.CallbackContext _) => Bool = true;
        private void SetBoolFalse(InputAction.CallbackContext _) => Bool = false;
        private void ReadToFloat(InputAction.CallbackContext c) => Float = c.ReadValue<float>();
        private void ReadToVector2(InputAction.CallbackContext c) => Vector2 = c.ReadValue<Vector2>();
        private void ReadToVector3(InputAction.CallbackContext c) => Vector3 = c.ReadValue<Vector3>();

        internal void Link(InputAction ia, bool doLink)
        {
            if (ia == null)
            {
                Debug.LogError("InputAction is null. Cannot link/unlink.");
                return;
            }

            if (doLink)
            {
                switch (type)
                {
                    case InputType.Null:
                        break;

                    case InputType.Click:
                        ia.performed += SetBoolTrue;
                        break;

                    case InputType.Hold:
                        ia.performed += SetBoolTrue;
                        break;

                    case InputType.Value0:
                        ia.performed += SetBoolTrue;
                        ia.canceled += SetBoolFalse;
                        break;

                    case InputType.Value1:
                        ia.started += ReadToFloat;
                        ia.performed += ReadToFloat;
                        ia.canceled += ReadToFloat;
                        break;

                    case InputType.Value2:
                        ia.started += ReadToVector2;
                        ia.performed += ReadToVector2;
                        ia.canceled += ReadToVector2;
                        break;

                    case InputType.Value3:
                        ia.started += ReadToVector3;
                        ia.performed += ReadToVector3;
                        ia.canceled += ReadToVector3;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case InputType.Null:
                        break;

                    case InputType.Click:
                        ia.performed -= SetBoolTrue;
                        break;

                    case InputType.Hold:
                        ia.performed -= SetBoolTrue;
                        break;

                    case InputType.Value0:
                        ia.performed -= SetBoolTrue;
                        ia.canceled -= SetBoolFalse;
                        break;

                    case InputType.Value1:
                        ia.started -= ReadToFloat;
                        ia.performed -= ReadToFloat;
                        ia.canceled -= ReadToFloat;
                        break;

                    case InputType.Value2:
                        ia.started -= ReadToVector2;
                        ia.performed -= ReadToVector2;
                        ia.canceled -= ReadToVector2;
                        break;

                    case InputType.Value3:
                        ia.started -= ReadToVector3;
                        ia.performed -= ReadToVector3;
                        ia.canceled -= ReadToVector3;
                        break;

                    default:
                        break;
                }
            }
        }

        internal void ResetFlags()
        {
            if (type == InputType.Click && Bool) Bool = false;
            else if (type == InputType.Hold && Bool) Bool = false;
        }
    }

    internal static partial class InputManager
    {
        private static MyActions source;
        private static List<(InputAction InputAction, InputInfo InputInfo)> inputList;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            source = new();
            inputList = new(64);

            source?.Enable();
            Bind();
            InputSystem.onBeforeUpdate += ResetFlags;

            foreach ((InputAction ia, InputInfo ii) in inputList)
                ii.Link(ia, true);

            Application.quitting += Dispose;
        }

        private static void Dispose()
        {
            foreach ((InputAction ia, InputInfo ii) in inputList)
                ii.Link(ia, false);

            InputSystem.onBeforeUpdate -= ResetFlags;
            source?.Disable();
            source?.Dispose();
            source = null;
            inputList = null;
        }

        private static void ResetFlags()
        {
            foreach ((_, InputInfo ii) in inputList)
                ii.ResetFlags();
        }

        private static InputInfo Create(InputAction inputAction, InputType type)
        {
            InputInfo info = new(type);
            inputList.Add((inputAction, info));
            return info;
        }
    }
}

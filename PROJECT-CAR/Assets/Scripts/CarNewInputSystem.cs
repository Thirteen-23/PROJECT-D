//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Scripts/CarNewInputSystem.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @CarNewInputSystem: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @CarNewInputSystem()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CarNewInputSystem"",
    ""maps"": [
        {
            ""name"": ""Movement"",
            ""id"": ""8fac8abd-f718-4b9e-95ad-40bbd4dbca53"",
            ""actions"": [
                {
                    ""name"": ""Acceleration"",
                    ""type"": ""Value"",
                    ""id"": ""1076a2c9-1f6a-4953-9358-676dde72db6c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Steering"",
                    ""type"": ""Value"",
                    ""id"": ""7f7af0e4-e3b6-4be7-9cf1-431a74dd75f3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""braking"",
                    ""type"": ""Value"",
                    ""id"": ""b9f0fff6-a1ec-4f26-9476-c665d7522c43"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Shifting up"",
                    ""type"": ""Value"",
                    ""id"": ""6c7de881-f1a8-481e-8e4f-f088578a3dc9"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=1)"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Shifting Down"",
                    ""type"": ""Value"",
                    ""id"": ""b4179924-ca8b-42ec-9c80-0a250be60dd8"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=1)"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Handbrake"",
                    ""type"": ""Value"",
                    ""id"": ""4e149a13-7ba9-432a-a63e-fea2898df491"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Throttle/Keyboard"",
                    ""id"": ""9d19ef26-ab4f-4999-9d27-ead1fae103de"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Acceleration"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""49224863-1db7-4e5a-a870-3229b8304605"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Acceleration"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""2e797c68-165f-4f4f-bd8a-74cb7dd4ae55"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Acceleration"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Gamepad"",
                    ""id"": ""51e90137-941b-4097-a23b-31e3be97e81b"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Acceleration"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""6bc10a86-6e50-4c28-b186-4fd4b21fe357"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Acceleration"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""fe57759f-a8b0-4f43-b810-1fd8d9c4d5b8"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Acceleration"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Gamepad"",
                    ""id"": ""2ab2bad3-bf19-44b8-8d74-437ea9eaa581"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steering"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Up"",
                    ""id"": ""a5cb45e9-c2de-4324-93ed-1b67b3879438"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steering"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Down"",
                    ""id"": ""59a69bc5-13c3-4dea-a7e4-c803e4d31a00"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steering"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""5603ae35-2dfe-4435-91bc-a378bc5c7d9a"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steering"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a214e6fb-c323-4085-8854-b11f74a2ded2"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steering"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f31bc3ed-f115-46eb-b59a-8ad1017bdc3c"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": ""Clamp(max=1)"",
                    ""groups"": """",
                    ""action"": ""braking"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""62b350dd-82b1-4046-af8b-41574e561d0d"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": ""Clamp(max=1)"",
                    ""groups"": """",
                    ""action"": ""braking"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9f06b613-a074-4a92-bdd5-caa454d1b94e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shifting up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b8ac3996-a93c-4065-9f42-5d0d1496d24f"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shifting up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""795259f5-288a-4a35-8b23-229052c57748"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shifting Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f6eab3db-1d41-47a1-a094-651d12ea0994"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shifting Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fe6efbeb-5296-48a0-8ba0-b5b1e4bd63c4"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": ""Clamp(max=1)"",
                    ""groups"": """",
                    ""action"": ""Handbrake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aa037426-8864-43c7-bfac-2a5a42bba705"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": ""Clamp(max=1)"",
                    ""groups"": """",
                    ""action"": ""Handbrake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Movement
        m_Movement = asset.FindActionMap("Movement", throwIfNotFound: true);
        m_Movement_Acceleration = m_Movement.FindAction("Acceleration", throwIfNotFound: true);
        m_Movement_Steering = m_Movement.FindAction("Steering", throwIfNotFound: true);
        m_Movement_braking = m_Movement.FindAction("braking", throwIfNotFound: true);
        m_Movement_Shiftingup = m_Movement.FindAction("Shifting up", throwIfNotFound: true);
        m_Movement_ShiftingDown = m_Movement.FindAction("Shifting Down", throwIfNotFound: true);
        m_Movement_Handbrake = m_Movement.FindAction("Handbrake", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Movement
    private readonly InputActionMap m_Movement;
    private List<IMovementActions> m_MovementActionsCallbackInterfaces = new List<IMovementActions>();
    private readonly InputAction m_Movement_Acceleration;
    private readonly InputAction m_Movement_Steering;
    private readonly InputAction m_Movement_braking;
    private readonly InputAction m_Movement_Shiftingup;
    private readonly InputAction m_Movement_ShiftingDown;
    private readonly InputAction m_Movement_Handbrake;
    public struct MovementActions
    {
        private @CarNewInputSystem m_Wrapper;
        public MovementActions(@CarNewInputSystem wrapper) { m_Wrapper = wrapper; }
        public InputAction @Acceleration => m_Wrapper.m_Movement_Acceleration;
        public InputAction @Steering => m_Wrapper.m_Movement_Steering;
        public InputAction @braking => m_Wrapper.m_Movement_braking;
        public InputAction @Shiftingup => m_Wrapper.m_Movement_Shiftingup;
        public InputAction @ShiftingDown => m_Wrapper.m_Movement_ShiftingDown;
        public InputAction @Handbrake => m_Wrapper.m_Movement_Handbrake;
        public InputActionMap Get() { return m_Wrapper.m_Movement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MovementActions set) { return set.Get(); }
        public void AddCallbacks(IMovementActions instance)
        {
            if (instance == null || m_Wrapper.m_MovementActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MovementActionsCallbackInterfaces.Add(instance);
            @Acceleration.started += instance.OnAcceleration;
            @Acceleration.performed += instance.OnAcceleration;
            @Acceleration.canceled += instance.OnAcceleration;
            @Steering.started += instance.OnSteering;
            @Steering.performed += instance.OnSteering;
            @Steering.canceled += instance.OnSteering;
            @braking.started += instance.OnBraking;
            @braking.performed += instance.OnBraking;
            @braking.canceled += instance.OnBraking;
            @Shiftingup.started += instance.OnShiftingup;
            @Shiftingup.performed += instance.OnShiftingup;
            @Shiftingup.canceled += instance.OnShiftingup;
            @ShiftingDown.started += instance.OnShiftingDown;
            @ShiftingDown.performed += instance.OnShiftingDown;
            @ShiftingDown.canceled += instance.OnShiftingDown;
            @Handbrake.started += instance.OnHandbrake;
            @Handbrake.performed += instance.OnHandbrake;
            @Handbrake.canceled += instance.OnHandbrake;
        }

        private void UnregisterCallbacks(IMovementActions instance)
        {
            @Acceleration.started -= instance.OnAcceleration;
            @Acceleration.performed -= instance.OnAcceleration;
            @Acceleration.canceled -= instance.OnAcceleration;
            @Steering.started -= instance.OnSteering;
            @Steering.performed -= instance.OnSteering;
            @Steering.canceled -= instance.OnSteering;
            @braking.started -= instance.OnBraking;
            @braking.performed -= instance.OnBraking;
            @braking.canceled -= instance.OnBraking;
            @Shiftingup.started -= instance.OnShiftingup;
            @Shiftingup.performed -= instance.OnShiftingup;
            @Shiftingup.canceled -= instance.OnShiftingup;
            @ShiftingDown.started -= instance.OnShiftingDown;
            @ShiftingDown.performed -= instance.OnShiftingDown;
            @ShiftingDown.canceled -= instance.OnShiftingDown;
            @Handbrake.started -= instance.OnHandbrake;
            @Handbrake.performed -= instance.OnHandbrake;
            @Handbrake.canceled -= instance.OnHandbrake;
        }

        public void RemoveCallbacks(IMovementActions instance)
        {
            if (m_Wrapper.m_MovementActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMovementActions instance)
        {
            foreach (var item in m_Wrapper.m_MovementActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MovementActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MovementActions @Movement => new MovementActions(this);
    public interface IMovementActions
    {
        void OnAcceleration(InputAction.CallbackContext context);
        void OnSteering(InputAction.CallbackContext context);
        void OnBraking(InputAction.CallbackContext context);
        void OnShiftingup(InputAction.CallbackContext context);
        void OnShiftingDown(InputAction.CallbackContext context);
        void OnHandbrake(InputAction.CallbackContext context);
    }
}

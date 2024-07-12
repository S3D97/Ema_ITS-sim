//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.8.2
//     from Assets/InputMap/Input-ITS-MAP.inputactions
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
using UnityEngine;

public partial class @PlayerControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Input-ITS-MAP"",
    ""maps"": [
        {
            ""name"": ""uiFirstPerson"",
            ""id"": ""0e20d8cb-326b-4c51-8037-ee7dff02381a"",
            ""actions"": [
                {
                    ""name"": ""UnlockMouse"",
                    ""type"": ""Button"",
                    ""id"": ""5ec71022-f0b2-4130-9e9c-625017217c8e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""55fd3fc1-e88e-4391-b934-729c3a95ef8a"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UnlockMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // uiFirstPerson
        m_uiFirstPerson = asset.FindActionMap("uiFirstPerson", throwIfNotFound: true);
        m_uiFirstPerson_UnlockMouse = m_uiFirstPerson.FindAction("UnlockMouse", throwIfNotFound: true);
    }

    ~@PlayerControls()
    {
        Debug.Assert(!m_uiFirstPerson.enabled, "This will cause a leak and performance issues, PlayerControls.uiFirstPerson.Disable() has not been called.");
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

    // uiFirstPerson
    private readonly InputActionMap m_uiFirstPerson;
    private List<IUiFirstPersonActions> m_UiFirstPersonActionsCallbackInterfaces = new List<IUiFirstPersonActions>();
    private readonly InputAction m_uiFirstPerson_UnlockMouse;
    public struct UiFirstPersonActions
    {
        private @PlayerControls m_Wrapper;
        public UiFirstPersonActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @UnlockMouse => m_Wrapper.m_uiFirstPerson_UnlockMouse;
        public InputActionMap Get() { return m_Wrapper.m_uiFirstPerson; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UiFirstPersonActions set) { return set.Get(); }
        public void AddCallbacks(IUiFirstPersonActions instance)
        {
            if (instance == null || m_Wrapper.m_UiFirstPersonActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UiFirstPersonActionsCallbackInterfaces.Add(instance);
            @UnlockMouse.started += instance.OnUnlockMouse;
            @UnlockMouse.performed += instance.OnUnlockMouse;
            @UnlockMouse.canceled += instance.OnUnlockMouse;
        }

        private void UnregisterCallbacks(IUiFirstPersonActions instance)
        {
            @UnlockMouse.started -= instance.OnUnlockMouse;
            @UnlockMouse.performed -= instance.OnUnlockMouse;
            @UnlockMouse.canceled -= instance.OnUnlockMouse;
        }

        public void RemoveCallbacks(IUiFirstPersonActions instance)
        {
            if (m_Wrapper.m_UiFirstPersonActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUiFirstPersonActions instance)
        {
            foreach (var item in m_Wrapper.m_UiFirstPersonActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UiFirstPersonActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UiFirstPersonActions @uiFirstPerson => new UiFirstPersonActions(this);
    public interface IUiFirstPersonActions
    {
        void OnUnlockMouse(InputAction.CallbackContext context);
    }
}

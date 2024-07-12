using UnityEngine;
using UnityEngine.InputSystem;

public class MouseUnlocker : MonoBehaviour
{
    // Riferimento alla classe generata per l'Input System
    private PlayerControls controls;
    private bool isMouseLocked = true;

    private void Awake()
    {
        // Inizializza i controlli
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.uiFirstPerson.UnlockMouse.performed += HandleToggleMouseLock;
        controls.uiFirstPerson.Enable();
    }

    private void OnDisable()
    {
       controls.uiFirstPerson.UnlockMouse.performed -= HandleToggleMouseLock;
       controls.uiFirstPerson.Disable();
    }

    // Metodo chiamato quando il tasto E viene premuto
    private void HandleToggleMouseLock(InputAction.CallbackContext context)
    {
        if (isMouseLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        isMouseLocked = !isMouseLocked; // Inverti lo stato del mouse
        Debug.Log($"Mouse locked: {isMouseLocked}");
    }
}

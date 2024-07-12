using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class ItsPlayerCameraMovement : MonoBehaviour
{

    //public Mouse mouseLook;
    //public KeyCode sblocca;
    //public KeyCode blocca;

    public float sensX;
    public float sensY;

    public Transform itsorientation;

    float xRotation;
    float yRotation;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;  

        yRotation += mouseX;

        xRotation -= mouseY;
        
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        itsorientation.rotation = Quaternion.Euler(0, yRotation, 0);


       // if(Input.GetKey(KeyCode.E))
      // {
       //     Debug.Log("mouse Sbloccato");
       //     Cursor.lockState = CursorLockMode.None;
       //     Cursor.visible = true;
       //     //mouseLook.enabled = false;
       //}
       //else if (Input.GetKey(KeyCode.E))
       //{
       //     Debug.Log("mouse bloccato");
       //     Cursor.lockState = CursorLockMode.Locked;
       //     Cursor.visible = false;
            //mouseLook.enabled = true;
    //}
    
    }
    
    }


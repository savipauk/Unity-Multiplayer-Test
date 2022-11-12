using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool mouseRight;

    private void Start()
    {
        transform.position = Camera.main.transform.position;
        transform.rotation = Camera.main.transform.rotation;
    }

    private void Update()
    {
        mouseRight = Input.GetMouseButton(1);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorMode();
        }

        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
    }

    private void FixedUpdate()
    {
        if (!mouseRight)
        {
            transform.position = Camera.main.transform.position;
            transform.rotation = Camera.main.transform.rotation;

            ClientSend.Camera(transform.position, transform.rotation);
        }
    }

    private void ToggleCursorMode()
    {
        Cursor.visible = !Cursor.visible;

        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}

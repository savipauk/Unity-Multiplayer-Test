using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform camTransform;

    public float moveSpeed = 500f;
    public float torque = 500f;
    public float jumpSpeed = 500f;
    public Rigidbody rigidBody;

    public GameObject fakeCam;

    bool[] inputs;

    private void Start()
    {
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ClientSend.PlayerShoot(camTransform.forward);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ClientSend.PlayerThrowItem(camTransform.forward);
        }
    }

    private void FixedUpdate()
    {
        SendInputToServer();

        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0])
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            _inputDirection.x -= 1;
        }
        if (inputs[3])
        {
            _inputDirection.x += 1;
        }

        Move(_inputDirection);
    }

    private void Move(Vector2 _inputDirection)
    {
        Vector3 _moveDirection = Vector3.right * _inputDirection.x + Vector3.forward * _inputDirection.y;
        _moveDirection.Normalize();

        Vector3 rotDir = new Vector3(_inputDirection.y, 0f, -_inputDirection.x);

        if (inputs[4])
        {
            rigidBody.AddForce(Vector3.up * jumpSpeed);
        }

        _moveDirection = fakeCam.transform.TransformDirection(_moveDirection);
        rotDir = fakeCam.transform.TransformDirection(rotDir);

        if (_moveDirection.magnitude >= 0.1f)
        {
            rigidBody.AddTorque(rotDir * torque);
            rigidBody.AddForce(_moveDirection * moveSpeed);
        }
    }

    /// <summary>Sends player input to the server.</summary>
    private void SendInputToServer()
    {
        inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space)
        };

        ClientSend.PlayerMovement(inputs);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public Rigidbody rigidBody;
    public SphereCollider sphereCollider;
    public Transform shootOrigin;
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float torque = 10f;
    public float jumpSpeed = 5f;
    public float throwForce = 600f;
    public float health;
    public float maxHealth = 100f;
    public int itemAmount = 0;
    public int maxItemAmount = 3;

    private bool[] inputs;
    //private float yVelocity = 0;

    GameObject fakeCam;

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;

        fakeCam = new GameObject();
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;

        inputs = new bool[5];
    }

    private void OnDestroy()
    {
        Destroy(fakeCam);
    }

    public void FixedUpdate()
    {
        if (health <= 0f)
        {
            return;
        }

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
        //_moveDirection *= moveSpeed;
        _moveDirection.Normalize();

        Vector3 rotDir = new Vector3(_inputDirection.y, 0f, -_inputDirection.x);

        if (inputs[4])
        {
            rigidBody.AddForce(Vector3.up * jumpSpeed);
        }

        //yVelocity += gravity;

        //_moveDirection.y = yVelocity;
        //controller.Move(_moveDirection);

        //Convert movedirection and rotdir from local space to world space with player camera
        _moveDirection = fakeCam.transform.TransformDirection(_moveDirection);
        rotDir = fakeCam.transform.TransformDirection(rotDir);

        if(_moveDirection.magnitude >= 0.1f)
        {
            rigidBody.AddTorque(rotDir * torque);
            rigidBody.AddForce(_moveDirection * moveSpeed);
        }

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    public void SetCamera(Vector3 _position, Quaternion _rotation)
    {
        fakeCam.transform.position = _position;
        fakeCam.transform.rotation = _rotation;
    }

    /*public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }*/

    public void SetInput(bool[] _inputs)
    {
        inputs = _inputs;
    }

    public void Shoot(Vector3 _viewDirection)
    {
        if (health <= 0f)
        {
            return;
        }

        if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(50f);
            }
            if (_hit.collider.CompareTag("Enemy"))
            {
                _hit.collider.GetComponent<Enemy>().TakeDamage(50f);
            }
        }
    }

    public void ThrowItem(Vector3 _viewDirection)
    {
        if (health <= 0f)
        {
            return;
        }

        if (itemAmount > 0)
        {
            itemAmount--;
            NetworkManager.instance.InstantiateProjectile(shootOrigin).Initialize(_viewDirection, throwForce, id);
        }
    }

    public void TakeDamage(float _damage)
    {
        if (health <= 0f)
        {
            return;
        }

        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;
            //controller.enabled = false;
            rigidBody.isKinematic = true;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealth;
        //controller.enabled = true;
        rigidBody.isKinematic = false;
        ServerSend.PlayerRespawned(this);
    }

    public bool AttemptPickupItem()
    {
        if (itemAmount >= maxItemAmount)
        {
            return false;
        }

        itemAmount++;
        return true;
    }
}

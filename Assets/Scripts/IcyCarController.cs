using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcyCarController : MonoBehaviour
{
    KeyCode _forward = KeyCode.W;
    KeyCode _backward = KeyCode.S;
    KeyCode _left = KeyCode.A;
    KeyCode _right = KeyCode.D;

    [SerializeField] float _acceleration = 0.1f;
    [SerializeField] float _deAcceleration = 0.01f;
    [SerializeField] float _maxSpeed = 1;
    [SerializeField] float _maxRotation = 20;
    [SerializeField] float _rotationAcceleration = 30f;
    [SerializeField] float _rotationDeAcceleration = 0.1f;
    [SerializeField] float _driftRatio = 0.5f;

    [SerializeField] Rigidbody _rb;

    Vector3 _movementVector = Vector3.zero;
    Vector3 _rotationVector = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(_forward))
        {
            _movementVector = Vector3.ClampMagnitude(_movementVector + transform.up * _acceleration * Time.deltaTime, _maxSpeed);
        }
        else if(Input.GetKey(_backward))
        {
            _movementVector = Vector3.ClampMagnitude(_movementVector - transform.up * _acceleration * Time.deltaTime, _maxSpeed);
        }
        else
        {
            _movementVector = _movementVector * (1 - _deAcceleration);
        }

        if (Input.GetKey(_left))
        {
            _rotationVector = Vector3.ClampMagnitude(_rotationVector + transform.forward * _rotationAcceleration * Time.deltaTime, _maxRotation);
        }
        else if (Input.GetKey(_right))
        {
            _rotationVector = Vector3.ClampMagnitude(_rotationVector - transform.forward * _rotationAcceleration * Time.deltaTime, _maxRotation);
        }
        else
        {
            _rotationVector = _rotationVector * (1 - _rotationDeAcceleration);
        }

        _rb.MovePosition(_rb.position +  _movementVector); 
        _rb.MoveRotation(Quaternion.Euler(_rb.rotation.eulerAngles + (_rotationVector * _movementVector.magnitude)));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    KeyCode _forward = KeyCode.W;
    KeyCode _backward = KeyCode.S;
    KeyCode _left = KeyCode.A;
    KeyCode _right = KeyCode.D;

    [SerializeField] float _speed = 10;
    [SerializeField] float _rotation = 100;
    [SerializeField] Rigidbody _rb;

    // Update is called once per frame
    void Update()
    {
        Vector3 movementVector = Vector3.zero;
        Vector3 rotationVector = Vector3.zero;
        if(Input.GetKey(_forward))
        {
            movementVector += transform.up;
        }
        if (Input.GetKey(_backward))
        {
            movementVector -= transform.up;
        }
        
        
        if (Input.GetKey(_left))
        {
            rotationVector += new Vector3(0,0,1);
        }
        if (Input.GetKey(_right))
        {
            rotationVector += new Vector3(0, 0, -1);
        }
        _rb.MovePosition(_rb.position + movementVector.normalized * _speed * Time.deltaTime); 
        _rb.MoveRotation(Quaternion.Euler(_rb.rotation.eulerAngles + (rotationVector.normalized* Time.deltaTime* _rotation)));
    }
}

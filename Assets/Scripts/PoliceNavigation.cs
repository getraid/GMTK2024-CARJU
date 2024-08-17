using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PoliceNavigation : MonoBehaviour
{
    [Header("Game Reference")]
    [SerializeField] private Transform target;

    [SerializeField] private float speed = 1f;
    [SerializeField] private float turnRate = 1f;


    private void Update()
    {
        if (target == null)
            return;

        Vector3 direction = target.position - transform.position;
        
        // Turn towards the target
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnRate * Time.deltaTime);

        // Move forward
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}

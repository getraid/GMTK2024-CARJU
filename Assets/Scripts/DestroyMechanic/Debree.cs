using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Debree : MonoBehaviour
{
    [SerializeField] Rigidbody _rB;
    public Action<Debree> DebreeDeleteMessage { get; set; }

    private void Awake()
    {
        _rB.isKinematic = true;
        _rB.constraints = RigidbodyConstraints.None;
    }
    public bool IsDettached { 
        get; 
        set; 
    }
    public void MoveTowards(Vector3 direction)
    {
        _rB.AddForce(direction.normalized, ForceMode.VelocityChange);
    }
    public void AddExplosionForce(Vector3 origin,int destructionForceVal)
    {
        _rB.isKinematic = false;
        _rB.AddExplosionForce(destructionForceVal, origin, 50);

        StartCoroutine(DetachDelay());
        IEnumerator DetachDelay()
        {
            yield return new WaitForSeconds(3);
            IsDettached = true;

            StartCoroutine(DestroyDebree(Random.Range(10, 30)));        //When player doesnt pick up the debrie, it gets destroyed in random interval
        }

        IEnumerator DestroyDebree(int waitUntilDeletion)
        {
            yield return new WaitForSeconds(waitUntilDeletion);

            DebreeDeleteMessage?.Invoke(this);
        }
    }
}

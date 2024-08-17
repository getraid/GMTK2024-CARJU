using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debree : MonoBehaviour
{
    [SerializeField] Rigidbody _rB;

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
    public void AddExplosionForce(Vector3 origin)
    {
        _rB.isKinematic = false;
        _rB.AddExplosionForce(50, origin, 50);

        StartCoroutine(DetachDelay());
        IEnumerator DetachDelay()
        {
            yield return new WaitForSeconds(3);
            IsDettached = true;
        }
    }
}

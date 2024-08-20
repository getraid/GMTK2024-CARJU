using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debree : MonoBehaviour
{
    [SerializeField] Rigidbody _rB;
    public Action<Debree> DebreeDeleteMessage { get; set; }

    static LinkedList<Debree> AllDebries { get; set; }=new LinkedList<Debree>();
    int _maxDebrieLimitor = 200;
    private void Awake()
    {
        _rB.isKinematic = true;
        _rB.constraints = RigidbodyConstraints.None;
        AllDebries.AddLast(this);
        RemoveOverLimitDebree();
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
            StartCoroutine(DestroyDebree(UnityEngine.Random.Range(10, 30)));        //When player doesnt pick up the debrie, it gets destroyed in random interval
        }

        IEnumerator DestroyDebree(int waitUntilDeletion)
        {
            yield return new WaitForSeconds(waitUntilDeletion);

            DebreeDeleteMessage?.Invoke(this);
        }
    }

    public void RemoveOverLimitDebree()
    {
        if(AllDebries.Count>_maxDebrieLimitor)
        {
            int howManyToRemove = AllDebries.Count - _maxDebrieLimitor;

            LinkedListNode<Debree> d = AllDebries.First;
            for (int i = 0; i < howManyToRemove; i++)
            {
                if (d.Value == null)
                    AllDebries.RemoveFirst();
                else
                {
                    d.Value.DebreeDeleteMessage?.Invoke(d.Value);
                    d = d.Next;
                }
            }
        }
    }
}

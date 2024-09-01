using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debree : MonoBehaviour
{
    [SerializeField] Rigidbody _rB;
    public Action<Debree> DebreeDeleteMessage { get; set; }
    public static Action<Debree> DebreeAttaching { get; set; }
    public static int MaxDebrieLimitor { get; set; } = 200;

    public bool AlreadyExploded { get; set; } = false;
    static LinkedList<Debree> AllPhysicalDebries { get; set; }=new LinkedList<Debree>();
    bool _isAttachedToCar = false;
    private void Awake()
    {
        _rB.isKinematic = true;
        _rB.constraints = RigidbodyConstraints.None;
        RemoveOverLimitDebree();
    }
    public void AttachDebreeToCar()
    {
        _rB.velocity = Vector3.zero;
        DebreeAttaching?.Invoke(this);
        _isAttachedToCar=true;
    }
    public bool IsDettached { 
        get; 
        set; 
    }
    public void MoveTowards(Vector3 direction)
    {
        _rB.AddForce(direction, ForceMode.VelocityChange);
    }
    public void AddExplosionForce(Vector3 origin,int destructionForceVal)
    {
        AllPhysicalDebries.AddLast(this);
        AlreadyExploded = true;
        _rB.isKinematic = false;
        _rB.AddExplosionForce(destructionForceVal, origin, 50);


        StartCoroutine(DetachDelay());
        IEnumerator DetachDelay()
        {
            yield return new WaitForSeconds(1);

            if (!_isAttachedToCar)
                AttachDebreeToCar();
        }
    }

    public void RemoveOverLimitDebree()
    {
        if(AllPhysicalDebries.Count> MaxDebrieLimitor)
        {
            int howManyToRemove = AllPhysicalDebries.Count - MaxDebrieLimitor;

            LinkedListNode<Debree> d = AllPhysicalDebries.First;
            for (int i = 0; i < howManyToRemove; i++)
            {
                if (d.Value == null)
                    AllPhysicalDebries.RemoveFirst();
                else
                {
                    d.Value.DebreeDeleteMessage?.Invoke(d.Value);
                    d = d.Next;
                }
            }
        }
    }
}

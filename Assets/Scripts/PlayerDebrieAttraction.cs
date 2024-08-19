using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerDebrieAttraction : MonoBehaviour
{
    LinkedList<Debree> _listOfDebriesAttracting = new LinkedList<Debree>();
    float _shrinkBy = 0.002f;
    int _maxDebreeAttraction = 50;
    private void Awake()
    {
        DestroyableSystem.DebreeAttaching += DebreeRegistration;
    }
    void DebreeRegistration(Debree debree)
    {
        _listOfDebriesAttracting.AddLast(debree);
        LimitDebreeSize();
        debree.DebreeDeleteMessage += (Debree debree) => _listOfDebriesAttracting.Remove(debree);
    }
    private void OnDestroy()
    {
        DestroyableSystem.DebreeAttaching -= DebreeRegistration;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        List<Debree> toRemove = new List<Debree>();
        float valueToGrowBy = 0;

        for (LinkedListNode<Debree> currDebree = _listOfDebriesAttracting.First; currDebree != null; currDebree = currDebree.Next)
        {

            if (currDebree.Value==null)
            {
                toRemove.Add(currDebree.Value);
                continue;
            }

            Vector3 dir = transform.position - currDebree.Value.transform.position;
            currDebree.Value.MoveTowards(dir.normalized);

            Vector3 currScale = currDebree.Value.transform.localScale;
            currScale.x -= _shrinkBy;
            currScale.y -= _shrinkBy;
            currScale.z -= _shrinkBy;

            if (currScale.magnitude <= _shrinkBy)
            {
                currDebree.Value.DebreeDeleteMessage?.Invoke(currDebree.Value);
            }
            else
            {
                currDebree.Value.transform.localScale = currScale;
                valueToGrowBy += 0.01f;
            }
        }

        GameManager.Instance.DebreePartsTotalCollected = GameManager.Instance.DebreePartsTotalCollected + valueToGrowBy;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Debree>(out Debree debree))
        {
            if(debree!=null&& !_listOfDebriesAttracting.Contains(debree) && debree.IsDettached)
            {
                _listOfDebriesAttracting.AddLast(debree);
                LimitDebreeSize();
            }
        }
    }
    void LimitDebreeSize()
    {
        if (_listOfDebriesAttracting.Count > _maxDebreeAttraction)
        {
            int howManyToDelete = _listOfDebriesAttracting.Count - _maxDebreeAttraction;

            for(int i=0;i< howManyToDelete; i++)
            {
                Debree d = _listOfDebriesAttracting.First.Value;
                d.DebreeDeleteMessage?.Invoke(d);
            }
        }
    }
}

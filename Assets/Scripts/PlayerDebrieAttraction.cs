using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerDebrieAttraction : MonoBehaviour
{
    LinkedList<Debree> _listOfDebriesAttracting = new LinkedList<Debree>();
    float _shrinkByMult = 0.90f;
    int _maxDebreeAttraction = 50;
    private void Awake()
    {
        Debree.DebreeAttaching += DebreeRegistration;
    }
    void DebreeRegistration(Debree debree)
    {
        _listOfDebriesAttracting.AddLast(debree);
        debree.DebreeDeleteMessage += (Debree debree) => _listOfDebriesAttracting.Remove(debree);
    }
    private void OnDestroy()
    {
        Debree.DebreeAttaching -= DebreeRegistration;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float valueToGrowBy = 0;

        for (LinkedListNode<Debree> currDebree = _listOfDebriesAttracting.First; currDebree != null; currDebree = currDebree.Next)
        {
            if (currDebree.Value==null)
            {
                _listOfDebriesAttracting.Remove(currDebree);
                currDebree = currDebree.Next;
                continue;
            }

            Vector3 dir = transform.position - currDebree.Value.transform.position;
            currDebree.Value.MoveTowards(dir*0.5f);

            Vector3 currScale = currDebree.Value.transform.localScale * _shrinkByMult;

            if (currScale.magnitude <= 0.1f)
            {
                currDebree.Value.DebreeDeleteMessage?.Invoke(currDebree.Value);
            }
            else
            {
                currDebree.Value.transform.localScale = currScale;
                valueToGrowBy += 0.01f;
            }
        }

        
    }
   
   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerDebrieAttraction : MonoBehaviour
{
    List<Debree> _listOfDebriesAttracting = new List<Debree>();
    float _shrinkBy = 0.002f;
    private void Awake()
    {
        DestroyableSystem.DebreeAttaching += DebreeRegistration;
    }
    void DebreeRegistration(Debree debree)
    {
        _listOfDebriesAttracting.Add(debree);
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
        for(int i=0;i<_listOfDebriesAttracting.Count;i++)
        {
            Debree currDebree = _listOfDebriesAttracting[i];
            if(currDebree==null)
            {
                toRemove.Add(currDebree);
                continue;
            }

            Vector3 dir = transform.position - currDebree.transform.position;
            currDebree.MoveTowards(dir.normalized);

            Vector3 currScale = currDebree.transform.localScale;
            currScale.x -= _shrinkBy;
            currScale.y -= _shrinkBy;
            currScale.z -= _shrinkBy;

            if (currScale.magnitude <= _shrinkBy)
            {
                currDebree.DebreeDeleteMessage?.Invoke(currDebree);
            }
            else
            {
                currDebree.transform.localScale = currScale;
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
                _listOfDebriesAttracting.Add(debree);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
    private void OnDestroy()
    {
        DestroyableSystem.DebreeAttaching -= DebreeRegistration;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for(int i=0;i<_listOfDebriesAttracting.Count;i++)
        {
            Debree currDebree = _listOfDebriesAttracting[i];
            Vector3 dir = transform.position - currDebree.transform.position;
            currDebree.MoveTowards(dir.normalized);

            Vector3 currScale = currDebree.transform.localScale;
            currScale.x -= _shrinkBy;
            currScale.y -= _shrinkBy;
            currScale.z -= _shrinkBy;

            if(currScale.magnitude<=_shrinkBy)
            {
                _listOfDebriesAttracting.Remove(currDebree);
                Destroy(currDebree);
            }
            else
                currDebree.transform.localScale = currScale;

        }


    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Debree"))
        {
            Debree debree = other.GetComponent<Debree>();
            if(debree!=null&& !_listOfDebriesAttracting.Contains(debree) && debree.IsDettached)
            {
                _listOfDebriesAttracting.Add(debree);
            }
        }
    }
}

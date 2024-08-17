using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebrieAttraction : MonoBehaviour
{
    [SerializeField] float _magnetForceStrength = 10;
    List<Rigidbody> _listOfDebriesAttracting = new List<Rigidbody>();
    private void Awake()
    {
        DestroyableSystem.DebreeAttaching += DebreeRegistration;
    }
    void DebreeRegistration(Rigidbody rigidbody)
    {
        _listOfDebriesAttracting.Add(rigidbody);
    }
    private void OnDestroy()
    {
        DestroyableSystem.DebreeAttaching -= DebreeRegistration;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0;i<_listOfDebriesAttracting.Count;i++)
        {
            Vector3 dir = transform.position - _listOfDebriesAttracting[i].position;
            _listOfDebriesAttracting[i].AddForce(dir.normalized, ForceMode.VelocityChange);
        }
    }
}

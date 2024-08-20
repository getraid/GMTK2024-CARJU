using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardJokePicker : MonoBehaviour
{
    [SerializeField] List<GameObject> _billboards;
    void Start()
    {
        _billboards[Random.Range(0, _billboards.Count - 1)].SetActive(true);
    }

}

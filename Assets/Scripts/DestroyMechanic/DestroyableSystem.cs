using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore;
using Random = UnityEngine.Random;

public class DestroyableSystem : MonoBehaviour, IDestroyable
{
    public static Action<Debree> DebreeAttaching { get; set; }

     MeshRenderer _mainMeshRenderer;
     Collider _collider;
    [SerializeField] float _probabilityOfDebreeAttach = 0.3f;
    [SerializeField] float _probabilityPartialDesctruction = 0.1f;

    [SerializeField] int _levelOfTheCarNeededForDestroyment = 1;
    [SerializeField] GameObject _destroyParticles;
    [SerializeField] DestructionType _desctructionType;
    [SerializeField] List<Debree> _fragments;

    public event EventHandler DestructionEvent;

    public enum DestructionType
    {
        Building, Prop, TrafficCar, CopCar
    }

    private void Awake()
    {
        _mainMeshRenderer=GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {

            if (GameManager.Instance.CurrentPlayerLevel >= _levelOfTheCarNeededForDestroyment)
            {
                DestroyTheObject();
                Physics.IgnoreCollision(collision.collider, _collider);
            }
            else
            {
               PartiallyDestroyTheObject();
            }
        }
    }

    void PartiallyDestroyTheObject()
    {
        _mainMeshRenderer.enabled = false;
        _collider.enabled = false;

        for (int i = 0; i < _fragments.Count; i++)
        {
            _fragments[i].gameObject.SetActive(true);
            if (Random.Range(0, 1.0f) <= _probabilityPartialDesctruction)
            {
                _fragments[i].AddExplosionForce( transform.position);
            }

        }
    }
    void DestroyTheObject()
    {
        _mainMeshRenderer.enabled = false;
        _collider.enabled = false;

        for(int i=0;i<_fragments.Count;i++)
        {
            _fragments[i].gameObject.SetActive(true);
            _fragments[i].AddExplosionForce(transform.position);
            if (Random.Range(0, 1.0f)<=_probabilityOfDebreeAttach)
            {
                DebreeAttaching?.Invoke(_fragments[i]);
            }
        }
        _destroyParticles.SetActive(true);
        DestructionEvent?.Invoke(_desctructionType,EventArgs.Empty);

        StartCoroutine(TurnOffParticles());

        IEnumerator TurnOffParticles()
        {
            yield return new WaitForSeconds(1);
            _destroyParticles.SetActive(false);
        }
    }

}

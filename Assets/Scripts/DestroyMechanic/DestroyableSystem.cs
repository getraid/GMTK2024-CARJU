using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore;
using IDestroyable = Interfaces.IDestroyable;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
public class DestroyableSystem : MonoBehaviour, IDestroyable
{
    public static Action<Debree> DebreeAttaching { get; set; }

    float _probabilityOfDebreeAttach = 0.3f;
    float _probabilityPartialDesctruction = 0.1f;

    [SerializeField] MeshRenderer _initialMeshRenderer;
    [SerializeField] Collider _initialCollisionCollider;
    [SerializeField] int _levelOfTheCarNeededForDestroyment = 1;
    [SerializeField] GameObject _destroyParticles;
    [SerializeField] DestructionType _desctructionType;
    [SerializeField] float _slowCarVelocityMultiplier = 0.9f;
    [SerializeField] List<Debree> _fragments;

    int _howManyPartialDestructionUntilTheFullOne = 2;
    int _numberOfPartialDestructions = 0;
    public event IDestroyable.DestroyableDelegate DestructionEvent;

    private void Awake()
    {
        _fragments.ForEach(x => x.DebreeDeleteMessage += OnDebreeDeleted);
    }
    public enum DestructionType
    {
        Building, Prop, TrafficCar, CopCar
    }

    private void OnTriggerEnter(Collider other)
    {   
        if (other.CompareTag("Player"))
        {
            if ((GameManager.Instance.CurrentPlayerLevel >= _levelOfTheCarNeededForDestroyment) || (_numberOfPartialDestructions>=_howManyPartialDestructionUntilTheFullOne))
            {
                DestroyTheObject();
            }
            else
            {
                PartiallyDestroyTheObject();
            }
        }
    }
   
    void PartiallyDestroyTheObject()
    {
        _initialMeshRenderer.enabled = false;
        _initialCollisionCollider.enabled = false;

        for (int i = 0; i < _fragments.Count; i++)
        {
            _fragments[i].gameObject.SetActive(true);
            if (Random.Range(0, 1.0f) <= _probabilityPartialDesctruction)
            {
                _fragments[i].AddExplosionForce( transform.position);
            }

        }
        _numberOfPartialDestructions++;
    }
    void DestroyTheObject()
    {
        _initialMeshRenderer.enabled = false;
        _initialCollisionCollider.enabled = false;

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
        DestructionEvent?.Invoke(gameObject,EventArgs.Empty);

        StartCoroutine(TurnOffParticles());

        IEnumerator TurnOffParticles()
        {
            yield return new WaitForSeconds(1);
            _destroyParticles.SetActive(false);
        }
    }
    void OnDebreeDeleted(Debree debree)
    {
        _fragments.Remove(debree);

        if (_fragments.Count == 0)
            Destroy(gameObject);
    }
}
using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore;
using IDestroyable = Interfaces.IDestroyable;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
public class DestroyableSystem : MonoBehaviour, IDestroyable
{
    float _probabilityOfDebreeInstantAttach = 0.3f;
    float _probabilityPartialDesctruction = 0.2f;

    [SerializeField] Collider _initialCollisionCollider;
    [SerializeField] int _levelOfTheCarNeededForDestroyment = 1;
    [SerializeField] ParticleSystem _destroyParticles;
    [SerializeField] DestructionType _desctructionType;
    [SerializeField] float _slowCarVelocityMultiplier = 0.9f;
    [SerializeField] int _destructionForceMultiplier = 50;
    [SerializeField] List<MeshRenderer> _initialMeshRenderers;
    [SerializeField] List<Debree> _fragments;
    [SerializeField] MusicSfxManager.TypeOfSfx _typeOfSfxToPlayOnDestroy;

    int _howManyPartialDestructionUntilTheFullOne = 2;
    int _numberOfPartialDestructions = 0;
    int _ignoreCollisionsByLevelDifference = 1;
    float _ignorePartialDestructionMilTime = 1000;
    public event IDestroyable.DestroyableDelegate DestructionEvent;
    List<Collider> _playerColliderTouching = new List<Collider>();
    bool _isDestroying = false;
    DateTime _latestTimeOfPartiallyDestructed=DateTime.MinValue;

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
            _playerColliderTouching.Add(other);


            if ((GameManager.Instance.CurrentPlayerLevel >= _levelOfTheCarNeededForDestroyment) || (_numberOfPartialDestructions >= _howManyPartialDestructionUntilTheFullOne))
            {
                if ((GameManager.Instance.CurrentPlayerLevel - _levelOfTheCarNeededForDestroyment) <= _ignoreCollisionsByLevelDifference)
                {
                    if (!_isDestroying)
                    {
                        DateTime destTime = DateTime.Now;
                        if ((destTime - _latestTimeOfPartiallyDestructed).TotalMilliseconds < _ignorePartialDestructionMilTime)         //If the object was partially destructed few moments ago dont turn total destruction (like when wheel hits it)
                            return;

                        DestroyTheObject(other.transform.position,GameManager.Instance.GetActiveVehicle().GetVelocityRatio());
                        MusicSfxManager.Instance.PlaySingleSfx(transform.position, _typeOfSfxToPlayOnDestroy);
                    }
                }
                else
                {
                    GameManager.Instance.DebreePartsTotalCollected += 0.25f * _fragments.Where(x => !x.AlreadyExploded).Count();      //Even when only disabled, it also adds to debree meter

                    _initialCollisionCollider.enabled = false;
                    Destroy(gameObject);
                }
            }
            else if ((_levelOfTheCarNeededForDestroyment - GameManager.Instance.CurrentPlayerLevel) == 1)        //Make partial destruction on object only one level above (so tiny car doesnt destroy scyscraper)
            {
                PartiallyDestroyTheObject(other.transform.position, GameManager.Instance.GetActiveVehicle().GetVelocityRatio());
                _initialCollisionCollider.enabled = true;
            }
            else
            {
                _initialCollisionCollider.enabled = true;
                MusicSfxManager.Instance.PlaySingleSfx(transform.position, MusicSfxManager.TypeOfSfx.car_crash);
            }
        }
        else if (_levelOfTheCarNeededForDestroyment <= 2 && other.CompareTag("Police") && !_isDestroying)
        {
            Rigidbody pRb = other.GetComponent<Rigidbody>();

            DestroyTheObject(other.transform.position, 1);
        }
        else if (other.CompareTag("Police"))
            _initialCollisionCollider.enabled = true;

    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            _initialCollisionCollider.enabled = false;
    }

    void PartiallyDestroyTheObject(Vector3 collisionPoint, float force)
    {
        DateTime destTime = DateTime.Now;
        if ((destTime - _latestTimeOfPartiallyDestructed).TotalMilliseconds< _ignorePartialDestructionMilTime)         //Ignore other collider types like wheels
            return;
        
        _latestTimeOfPartiallyDestructed=DateTime.Now;
        _initialMeshRenderers.ForEach(x=>x.enabled = false);
        _initialCollisionCollider.enabled = false;

        int howManyPartialled = 0;
        for (int i = 0; i < _fragments.Count; i++)
        {
            _fragments[i].gameObject.SetActive(true);
            if (Random.Range(0, 1.0f) <= _probabilityPartialDesctruction)
            {
                _fragments[i].AddExplosionForce(_fragments[i].transform.position + ((_fragments[i].transform.position-collisionPoint).normalized * 10), _destructionForceMultiplier* force);

                _fragments[i].AttachDebreeToCar();
                howManyPartialled++;
            }

        }
        GameManager.Instance.DebreePartsTotalCollected += 0.25f *howManyPartialled;
        _numberOfPartialDestructions++;
        MusicSfxManager.Instance.PlaySingleSfx(transform.position, MusicSfxManager.TypeOfSfx.car_crash);

    }
    void DestroyTheObject(Vector3 collisionPoint,float force)
    {
        _isDestroying = true;
        _initialMeshRenderers.ForEach(x => x.enabled = false);
        _initialCollisionCollider.enabled = false;

        int howManyDestroyed = 0;

        for (int i=0;i<_fragments.Count;i++)
        {
            if (_fragments[i].AlreadyExploded)
                continue;

            _fragments[i].gameObject.SetActive(true);
            _fragments[i].AddExplosionForce(_fragments[i].transform.position + ((_fragments[i].transform.position - collisionPoint).normalized * 10), _destructionForceMultiplier*force);
            _fragments[i].AddExplosionForce(collisionPoint, _destructionForceMultiplier*force);

            if (Random.Range(0, 1.0f)<= _probabilityOfDebreeInstantAttach)
            {
                _fragments[i].AttachDebreeToCar();
            }
            howManyDestroyed++;
        }
        if (_destroyParticles != null)
        {
            _destroyParticles.gameObject?.SetActive(true);
            StartCoroutine(TurnOffParticles());
        }

        GameManager.Instance.DebreePartsTotalCollected += 0.25f * howManyDestroyed;
        DestructionEvent?.Invoke(gameObject,EventArgs.Empty);

        IEnumerator TurnOffParticles()
        {
            yield return new WaitForSeconds(_destroyParticles.main.duration);
            _destroyParticles.gameObject?.SetActive(false);
        }
    }
    void OnDebreeDeleted(Debree debree)
    {
        if (debree != null)
        {
            _fragments.Remove(debree);
            Destroy(debree.gameObject);

            if (_fragments.Count == 0)
                Destroy(gameObject);
        }
    }
}
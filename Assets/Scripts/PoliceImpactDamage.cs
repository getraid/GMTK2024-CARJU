using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceImpactDamage : MonoBehaviour
{
    

    [SerializeField] float damage = 10f;
    [SerializeField] float cooldown = 1f;
    [SerializeField] float impactForce = 10f;

    [SerializeField] Transform impactVFX;

    private float _cooldownTimer;

    private void Update()
    {
        _cooldownTimer = Mathf.Clamp(_cooldownTimer - Time.deltaTime, 0, cooldown);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_cooldownTimer > 0)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.CurrentFuelAmount -= damage;
            _cooldownTimer = cooldown;

            if (collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForceAtPosition(rb.velocity.normalized * impactForce, collision.contacts[0].point, ForceMode.Impulse);
            }

            if (impactVFX)
            {
                Instantiate(impactVFX, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            }
        }
    }
}

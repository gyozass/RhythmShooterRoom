using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] Camera FPCamera;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 30f;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;

    [SerializeField] private InputActionReference fire;

    private void OnEnable()
    {
        fire.action.performed += PerformAttack;
    }

    private void OnDisable()
    {
        fire.action.performed -= PerformAttack;
    }

    private void PerformAttack(InputAction.CallbackContext obj)
    {
        Shoot(); 
    }

    public void Shoot()
    {
        PlayMuzzleFlash();
        ProcessRaycast();
    }

    private void PlayMuzzleFlash()
    {
        muzzleFlash.Play();
    }

    private void ProcessRaycast()
    {
        RaycastHit hit;
        //out = allows to show 2 values > wat it raycast + whr
        if (Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range))
        {
            CreateHitImpact(hit);
            Debug.Log(hit.transform.name);
            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
              if (enemy == null)
                return;
            enemy.TakeDamage(damage);
        }

        else
        {
            return;
        }
    }

    private void CreateHitImpact(RaycastHit hit)
    {
        //hit.point is the point whr raycast collides
        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        //1 second 
        Destroy(impact, 1f);

    }
}


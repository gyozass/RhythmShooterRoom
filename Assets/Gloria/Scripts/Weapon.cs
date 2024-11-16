using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] Camera FPCamera;
    [SerializeField] float range = 100f;
    [SerializeField] float damage;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;

    BeatSpawner shooter;

    [SerializeField] private InputActionReference fire;

    public BeatSpawner BeatSpawner;

  // private void OnEnable()
  // {
  //     fire.action.performed += PerformAttack;
  // }
  //
  // private void OnDisable()
  // {
  //     fire.action.performed -= PerformAttack;
  // }

    public void PerformAttack(InputAction.CallbackContext obj)
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

            switch(BeatSpawner.CurrentJudgement)
            {
                case BeatSpawner.Judgements.Perfect:
                    damage = 100f;
                    //instantiate the effects

                    break;
                case BeatSpawner.Judgements.Great:
                    damage = 50f; 

                    break;
                case BeatSpawner.Judgements.Good:
                    damage = 25f;

                    break;
                default :
                    damage = 0f;

                    break;

             };
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


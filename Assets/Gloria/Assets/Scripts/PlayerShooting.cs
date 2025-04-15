using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private Camera FPCamera;
    //[SerializeField] private GameObject bulletPrefab;
    [SerializeField] private BeatManager beatManager;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject muzzleEffect;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Canvas reticleCanvas;
    [SerializeField] private AnimationCurve knockbackCurve;
    [SerializeField] public GameObject glowEffect;
    [SerializeField] private GameObject gunTip;
    [SerializeField] private Animation gunRecoil;
    [SerializeField] float showLineDuration = 0.2f;
    public float recoilForce = 5f; 
    private float range = 100f;
    //private Image clickEffectGlow;
    private CharacterController playerController;
    private FirstPersonController firstPersonController;
    public RaycastHit hit;
    public bool isWithinThreshold = false;


    private Vector3 _knockbackDirection;
    [SerializeField] private float knockbackForce = 20f;
    [SerializeField] private float knockbackDuration = 0.5f; 

    private void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        playerController = FindObjectOfType<CharacterController>();
        firstPersonController = FindObjectOfType<FirstPersonController>();
        gunRecoil = FindObjectOfType<Animation>();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    public void ApplyKnockback()
    {
        _knockbackDirection = hit.normal; 
        StartCoroutine(ApplyKnockbackCoroutine());
    }

    private IEnumerator ApplyKnockbackCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < knockbackDuration)
        {
            float curveValue = knockbackCurve.Evaluate(elapsedTime / knockbackDuration);
            Vector3 force = _knockbackDirection * (knockbackForce * curveValue * Time.deltaTime);
            playerController.Move(force);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void Shoot()
    {
        if (!ProcessRaycast(out hit))
        {
            Debug.Log("No hit detected");
            return;
        }

        float damage = GetDamageBasedOnThreshold(beatManager.roundedDifference);

        StartCoroutine(ShowShootingLine(gunTip.transform.position, hit.point));

        if (beatManager.roundedDifference < beatManager._terribleThreshold)
        {
            isWithinThreshold = true;
            CreateHitImpact(hit.point, hit.normal);
            ApplyKnockback();
            gunRecoil.Play();

            StartCoroutine(HideGlowAfterSeconds());
        }

        if (hit.transform.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log("Enemy damage dealt : " + damage);
            }
        }

    }

    IEnumerator HideGlowAfterSeconds()
    {
        glowEffect.SetActive(true);

        float duration = 0.5f;

        while (duration > 0 ) 
        {
            duration -= Time.deltaTime;
            yield return null; 
        }

        glowEffect.SetActive(false);

    }

    public void CreateHitImpact(Vector3 hitPoint, Vector3 normal)
    {
        GameObject impact = Instantiate(hitEffect, hitPoint, Quaternion.LookRotation(normal));
        Destroy(impact, 1f);

    }

    public bool ProcessRaycast(out RaycastHit raycastHit)
    {
        return Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out raycastHit, range);
    }

    private IEnumerator ShowShootingLine(Vector3 start, Vector3 end)
    {
        Instantiate(muzzleEffect, gunTip.transform.position, Quaternion.identity);

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        yield return new WaitForSeconds(showLineDuration);
        lineRenderer.enabled = false;
    }

    public float GetDamageBasedOnThreshold(float roundedDifference)
    {
        //Debug.Log(roundedDifference);
        if (roundedDifference < beatManager._badThreshold && roundedDifference > beatManager._okThreshold) return 10f;
        if (roundedDifference < beatManager._okThreshold && roundedDifference > beatManager._goodThreshold) return 30f;
        if (roundedDifference < beatManager._goodThreshold && roundedDifference > beatManager._perfectThreshold) return 50f;
        if (roundedDifference < beatManager._perfectThreshold && roundedDifference > 0) return 100f;
        return 0f;
    }

}

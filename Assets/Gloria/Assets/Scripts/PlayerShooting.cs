using StarterAssets;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Camera FPCamera;
    //[SerializeField] private GameObject bulletPrefab;
    //[SerializeField] private AdvBeatManager beatManager;
    [SerializeField] private MusicNote musicNote;
    [SerializeField] private Canvas reticleCanvas;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private AnimationCurve knockbackCurve;
    private CharacterController playerController;
    private FirstPersonController firstPersonController;
    public RaycastHit hit;
    private PlayerInput playerInput;
    private InputAction fireAction;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject muzzleEffect;
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private Animation gunRecoil;

    [Header("Position")]
    [SerializeField] private GameObject gunTip;
    private Vector3 _knockbackDirection;
    public bool isWithinThreshold = false;

    [Header("Values")]
    [SerializeField] float showLineDuration = 0.2f;
    public float recoilForce = 5f; 
    private float range = 100f;
    //private Image clickEffectGlow;
    [SerializeField] private float knockbackForce = 20f;
    [SerializeField] private float knockbackDuration = 0.5f;
    private PlayerScore playerScore;

    void OnShoot(HitType hitType)
    {
        playerScore.AddScore(hitType);
    }

    private void Start()
    {
        playerScore = FindObjectOfType<PlayerScore>();  
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        playerController = FindObjectOfType<CharacterController>();
        firstPersonController = FindObjectOfType<FirstPersonController>();
        gunRecoil = FindObjectOfType<Animation>();

        playerInput = FindObjectOfType<PlayerInput>(); 
        fireAction = playerInput.actions["Fire"];
        fireAction.performed += ctx => Shoot(); 
    }

  //private void Update()
  //{
  //    if (Mouse.current.leftButton.wasPressedThisFrame)
  //    {
  //        Shoot();
  //    }
  //}

    public void ApplyKnockback()
    {
        _knockbackDirection = hit.normal; 
        StartCoroutine(ApplyKnockbackCoroutine());
    }

    public IEnumerator ApplyKnockbackCoroutine()
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

        float damage = GetDamageBasedOnHitType(musicNote.currentHitType);

        StartCoroutine(ShowShootingLine(gunTip.transform.position, hit.point));
    
        if (musicNote.currentOffset < musicNote._okPercentage)
        {
            CreateHitImpact(hit.point, hit.normal);
            ApplyKnockback();
            gunRecoil.Play();
            OnShoot(musicNote.currentHitType);
            isWithinThreshold = true;

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

    public float GetDamageBasedOnHitType(HitType hitType)
    {
        switch (hitType)
        {
            case HitType.Perfect:
                return 100f;
            case HitType.Good:
                return 50f;
            case HitType.Okay:
                return 30f;
            default:
                return 0f;
        }
    }
    private void OnDestroy()
    {
        if (fireAction != null)
            fireAction.performed -= ctx => Shoot();
    }

}

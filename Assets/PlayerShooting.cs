using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private Camera FPCamera;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private BeatManager beatManager;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float range = 100f;
    [SerializeField] private float force;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private float flightForce = 10f; // Adjust flight force

    private RaycastHit hit;
    private float roundedDifference;

    private void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
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

    public void Shoot()
    {
        if (!ProcessRaycast(out hit))
        {
            Debug.Log("No hit detected");
            return;
        }

        float damage = GetDamageBasedOnThreshold(beatManager.roundedDifference);

        Debug.Log(hit.point);
        Vector3 screenToWorldPoint =  FPCamera.ScreenToWorldPoint(hit.point);

        StartCoroutine(ShowShootingLine(transform.position, screenToWorldPoint));

        if (hit.transform.CompareTag("Room"))
        {
            Vector3 forceDirection = -hit.normal * flightForce;
            playerRb.AddForce(forceDirection, ForceMode.Impulse);
            Debug.Log("Flight force applied");
        }

        if (beatManager.roundedDifference < beatManager._terribleThreshold)
        {
            CreateHitImpact(hit.point, hit.normal);

            Vector3 targetPoint = hit.point;
            Vector3 direction = targetPoint - transform.position;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.transform.forward = direction.normalized;

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.AddForce(direction.normalized * force, ForceMode.Impulse);
            bulletRb.AddForce(FPCamera.transform.up * force, ForceMode.Impulse);
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

    private IEnumerator ShowShootingLine(Vector3 start, Vector3 end)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        yield return new WaitForSeconds(0.2f);
        lineRenderer.enabled = false;
    }

    public float GetDamageBasedOnThreshold(float roundedDifference)
    {
        Debug.Log(roundedDifference);
        if (roundedDifference < beatManager._badThreshold && roundedDifference > beatManager._okThreshold) return 5f;
        if (roundedDifference < beatManager._okThreshold && roundedDifference > beatManager._goodThreshold) return 10f;
        if (roundedDifference < beatManager._goodThreshold && roundedDifference > beatManager._perfectThreshold) return 20f;
        if (roundedDifference < beatManager._perfectThreshold && roundedDifference > 0) return 30f;
        return 0f;
    }
}

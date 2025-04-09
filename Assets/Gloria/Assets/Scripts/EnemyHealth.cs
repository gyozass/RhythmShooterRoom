using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MoreMountains.Feedbacks;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitPoints = 100f;
    [SerializeField] Color damageColor = Color.red;
    [SerializeField] float flickerDuration = 2f;

    private Color originalColor;
    private MeshRenderer rend;
    private bool isFlickering = false;

    private void Start()
    {
        rend = GetComponentInChildren<MeshRenderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }
    }

    public void TakeDamage(float damage)
    {
        BroadcastMessage("OnDamageTaken", SendMessageOptions.DontRequireReceiver);
        hitPoints -= damage;

        if (!isFlickering && rend != null)
        {
            StartCoroutine(FlickerColor());
        }

        if (hitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FlickerColor()
    {
        isFlickering = true;
        float timer = 0f;

        while (timer < flickerDuration)
        {
            rend.material.color = damageColor;
            yield return new WaitForSeconds(1f);
            rend.material.color = originalColor;
            yield return new WaitForSeconds(1f);
            timer += 2f;
        }

        rend.material.color = originalColor;
        isFlickering = false;
    }
}
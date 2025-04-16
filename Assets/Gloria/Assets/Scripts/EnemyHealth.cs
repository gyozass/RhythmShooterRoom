using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MoreMountains.Feedbacks;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitPoints = 100f;
    [SerializeField] Color damageColor = Color.red;
    [SerializeField] float flickerDuration = 2f;
    [SerializeField] GameObject robotDieEffect;

    Animator animator;
    private Color originalColor;
    [SerializeField] private SkinnedMeshRenderer[] rend;
    private bool isFlickering = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        if (rend != null)
        {
            originalColor = rend[0].material.color;
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
            animator.SetTrigger("Dead");
            Invoke("RobotDie", 1f);
        }
    }

    private void RobotDie()
    {
        gameObject.SetActive(false);
        Instantiate(robotDieEffect, transform.position, Quaternion.identity);
        robotDieEffect.SetActive(false);
    }
    private IEnumerator FlickerColor()
    {
        //  isFlickering = true;
        //  float timer = 0f;
        //
        //  while (timer < flickerDuration)
        //  {
        //      rend.material.color = damageColor;
        //      yield return new WaitForSeconds(1f);
        //      rend.material.color = originalColor;
        //      yield return new WaitForSeconds(1f);
        //      timer += 2f;
        //  }
        //
        //  rend.material.color = originalColor;
        //  isFlickering = false;
        for (int i = 0; i < rend.Length; i++)
        {
            rend[i].material.color = damageColor;
        }

        yield return null;
    }

    [ContextMenu("ChangeMatColor")]
    public void ChangeMatColor()
    {
        for (int i = 0; i < rend.Length; i++)
        {
            rend[i].material.color = damageColor;
        }
    }
}
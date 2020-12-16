using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public LayerMask layerMask;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.layer == 16)
        {
            animator.Play("Boom");
            StartCoroutine(RemoveObject());
            var collider = Physics2D.OverlapCircle(transform.position, 7.84f, layerMask);

            if (collider != null)
            {
                collider.GetComponent<HealthPoints>().TakeDamage(100);
            }
        }
    }

    private IEnumerator RemoveObject()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    
}

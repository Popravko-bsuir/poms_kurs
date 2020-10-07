using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Components")] public Rigidbody2D rb;

    public GameObject impactEffect;
    public float speed = 20f;

    void Start()
    {
        Destroy(gameObject, 3f);
        rb.velocity = transform.right * speed;
    }

    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
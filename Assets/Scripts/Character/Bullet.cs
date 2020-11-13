using Enemies.Bug;
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

    

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("EnemieBug"))
        {
            col.GetComponentInParent<BugAI>().CrushingEffect();
            Instantiate(impactEffect, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }
}
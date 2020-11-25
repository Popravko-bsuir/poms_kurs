using System;
using Enemies.Bug;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Components")] public Rigidbody2D rb;

    public GameObject impactEffect;
    public float speed = 20f;
    [SerializeField] private float bulletLifeTime;
    private float _time;

    void Start()
    {
        //Destroy(gameObject, 3f);
        //rb.velocity = transform.right * speed;
    }

    private void Update()
    {
        rb.velocity = transform.right * speed;
        _time += Time.deltaTime;
        if (_time > bulletLifeTime)
        {
            _time = 0;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("EnemieBug"))
        {
            col.GetComponentInParent<BugAI>().CrushingEffect();
            Instantiate(impactEffect, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }

        // if (col.gameObject.CompareTag("Player"))
        // {
        //     col.gameObject.GetComponent<HealthPoints>().TakeDamage(10);
        //     Instantiate(impactEffect, transform.position, transform.rotation);
        //     gameObject.SetActive(false);
        // }
    }
}
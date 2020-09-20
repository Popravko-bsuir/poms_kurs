using System;
using UnityEngine;

namespace Character
{
    public class Grenade : MonoBehaviour
    {
        public Rigidbody2D rb;
        public GameObject explosionEffect;
        public static float ForceScale = 10f;
        public static float ForceUpScale = 5f;
        public float explosionTimer;
        public float explosionTime = 5f;

        void Start()
        {
           // rb.AddForce(Vector2.up * ForceUpScale, ForceMode2D.Impulse);
            Throw(Movement.isFacingRight ? Vector2.right : Vector2.left);
        }

        private void Throw(Vector2 direction)
        {
            rb.AddForce(direction * ((Weapon.chargeForce * ForceScale) + Math.Abs(Movement.characterSpeed.x)), ForceMode2D.Impulse);
            rb.AddForce(Vector2.up * ((Weapon.chargeForce * ForceUpScale) + Movement.characterSpeed.y),ForceMode2D.Impulse);
        }

        void Update()
        {
            explosionTimer += Time.deltaTime;
            if (explosionTimer > explosionTime)
            {
                Instantiate(explosionEffect,transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }
}
using System;
using UnityEngine;

namespace Character
{
    public class Grenade : MonoBehaviour
    {
        private Weapon _weapon;
        private Movement _movement;
        public Rigidbody2D rb;
        public GameObject explosionEffect;
        public float explosionTimer;
        public float explosionTime = 5f;
        
        void Start()
        {
            _movement = FindObjectOfType<Movement>();
            _weapon = FindObjectOfType<Weapon>();
            //Throw(_movement.IsFacingRight ? Vector2.right : Vector2.left);
            Throw2();
        }

        // private void Throw(Vector2 direction)
        // {
        //     rb.AddForce(direction * ((_weapon.ChargeForce * _weapon.ForceScale) + Math.Abs(_movement.rb.velocity.x)), ForceMode2D.Impulse);
        //     rb.AddForce(Vector2.up * ((_weapon.ChargeForce * _weapon.ForceUpScale) + _movement.rb.velocity.y),ForceMode2D.Impulse);
        // }   
        private void Throw2()
        {
            rb.AddForce(transform.right * ((_weapon.ChargeForce * _weapon.ForceScale) + Math.Abs(_movement.rb.velocity.x)), ForceMode2D.Impulse);
            rb.AddForce(Vector2.up * ((_weapon.ChargeForce * _weapon.ForceUpScale) + _movement.rb.velocity.y),ForceMode2D.Impulse);
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
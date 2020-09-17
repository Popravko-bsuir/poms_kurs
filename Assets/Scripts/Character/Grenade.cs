using System;
using UnityEngine;

namespace Character
{
    public class Grenade : MonoBehaviour
    {
        public Rigidbody2D rb;
        private const float ForceScale = 10f;
        private const float ForceUpScale = 5f;

        // Start is called before the first frame update
        void Start()
        {
            rb.AddForce(Vector2.up * ForceUpScale, ForceMode2D.Impulse);
            Throw(Movement.isFacingRight ? Vector2.right : Vector2.left);
        }

        private void Throw(Vector2 direction)
        {
            rb.AddForce(direction * (Weapon.chargeForce * ForceScale), ForceMode2D.Impulse);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
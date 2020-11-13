using System;
using UnityEngine;

namespace Enemies.Bug
{
    public class BugBite : MonoBehaviour
    {
        public ParticleSystem byteEffect;
        [SerializeField] private int byteDamage = 5;
        [SerializeField] private Vector2 backJumpForce;
        
        private HealthPoints _healthPoints;
        public Rigidbody2D bugRb;
        public BugAI bugAi;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (_healthPoints == null)
                {
                    _healthPoints = other.gameObject.GetComponent<HealthPoints>();
                }
                _healthPoints.TakeDamage(byteDamage);
                JumpBack(bugAi.IsFacingLeft? Vector2.right : Vector2.left);
                byteEffect.Play(false);
            }
        }

        private void JumpBack(Vector2 direction)
        {
            bugRb.AddForce(direction * backJumpForce.x, ForceMode2D.Impulse);
            bugRb.AddForce(Vector2.up * backJumpForce.y, ForceMode2D.Impulse);
        }
    }
}

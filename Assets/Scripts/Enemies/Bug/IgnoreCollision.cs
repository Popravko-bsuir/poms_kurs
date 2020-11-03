using System;
using UnityEngine;

namespace Enemies.Bug
{
    public class IgnoreCollision : MonoBehaviour
    {
        public CapsuleCollider2D thisCollider;
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("EnemieSite"))
            {
                Physics2D.IgnoreCollision(other.gameObject.GetComponentInChildren<CapsuleCollider2D>(), thisCollider);
            }
        }
    }
}

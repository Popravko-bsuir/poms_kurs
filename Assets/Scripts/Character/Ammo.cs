using System;
using UnityEngine;

namespace Character
{
    public class Ammo : MonoBehaviour
    {
        [SerializeField] private int ammoToAdd;
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.AddTorque(0.5f, ForceMode2D.Impulse);
           // _rb.AddForce((transform.right * (-1) + Vector3.up).normalized * 10f, ForceMode2D.Impulse);
            _rb.AddForce(transform.right *  10f, ForceMode2D.Impulse);
        }
        

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponentInChildren<Weapon>().AddAmmo(ammoToAdd);
                gameObject.SetActive(false);
            }
        }
    }
}

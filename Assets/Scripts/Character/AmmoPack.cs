using System;
using UnityEngine;

namespace Character
{
    public class AmmoPack : MonoBehaviour
    {
        [SerializeField] private int ammoToAdd;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != 10)
            {
                return;
            }
            
            other.gameObject.GetComponentInChildren<Weapon>().AddAmmo(30);
            gameObject.SetActive(false);
        }
    }
}

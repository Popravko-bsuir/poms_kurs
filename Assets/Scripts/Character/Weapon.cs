using System;
using UnityEngine;

namespace Character
{
    public class Weapon : MonoBehaviour
    {
        public Transform firePoint;
        
        [Header("Fire")]
        public float rateOfFire = 0.5f;
        private float _timeTilNextShot;
        public GameObject bulletPrefab;
    
        [Header("Alternative Fire")]
        public float rateOfFireAlt = 5f;
        public float chargeTimeMax = 1f;
        public float grenadeCooldown = 5f;
        public GameObject grenadePrefab;
        public Trajectory trajectory;
        public Vector2 forceApplied;
        private float _canShootGranade;
        private float _timeTilNextShotAlt;
        public static float chargeTime;
        public static float chargeForce { get; set; }
    
        void Start()
        { 
            _canShootGranade = grenadeCooldown;
        }

        void Update()
        {
            _canShootGranade += Time.deltaTime;
            if (Input.GetMouseButton(1) && _canShootGranade >= grenadeCooldown)
            {
                if (chargeTime < chargeTimeMax)
                {
                    chargeTime += Time.deltaTime;   
                }
                
                trajectory.Show(); 
                forceApplied = new Vector2((chargeTime * Grenade.ForceScale) + Math.Abs(Movement.characterSpeed.x), (chargeTime * Grenade.ForceUpScale) + Movement.characterSpeed.y);
                trajectory.UpdateDots(transform.position, forceApplied);
            }
        

            if (Input.GetMouseButtonUp(1) && _timeTilNextShotAlt < Time.time)
            {
                chargeForce = chargeTime;
                ShootAlternative();
                chargeTime = 0;
                _timeTilNextShotAlt = Time.time + rateOfFireAlt;
                trajectory.Hide();
                _canShootGranade = 0;
            }

            if (Input.GetButton("Fire1") && _timeTilNextShot < Time.time)
            {
                Shoot();
                _timeTilNextShot = Time.time + rateOfFire;
            }
        }

        void ShootAlternative()
        {
            Prefab(grenadePrefab);
        }

        void Shoot()
        {
            Prefab(bulletPrefab);
        }

        private void Prefab(GameObject prefab)
        {
            Instantiate(prefab, firePoint.position, firePoint.rotation);
        }

    }
}

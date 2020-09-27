using System;
using UnityEngine;

namespace Character
{
    public class Weapon : MonoBehaviour
    {
        public Transform firePoint;
        public Movement movement;
        
        [Header("Fire")]
        public float rateOfFire = 0.5f;
        private float _timeTilNextShot;
        public GameObject bulletPrefab;
    
        [Header("Alternative Fire")]
        [SerializeField] private float forceScale = 10f;
        [SerializeField] private float forceUpScale = 5f;
        public float rateOfFireAlt = 5f;
        public float chargeTimeMax = 1f;
        public float grenadeCooldown = 5f;
        public GameObject grenadePrefab;
        public Trajectory trajectory;
        private Vector2 _forceApplied;
        private float _canShootGranade;
        private float _timeTilNextShotAlt;
        private float _chargeTime;
        private float _chargeForce;

        public float ForceScale => forceScale;

        public float ForceUpScale => forceUpScale;

        public float ChargeForce => _chargeForce;

        void Start()
        {
            _canShootGranade = grenadeCooldown;
            movement = FindObjectOfType<Movement>();
        }

        void Update()
        {
            _canShootGranade += Time.deltaTime;
            if (Input.GetMouseButton(1) && _canShootGranade >= grenadeCooldown)
            {
                if (_chargeTime < chargeTimeMax)
                {
                    _chargeTime += Time.deltaTime;   
                }
                
                trajectory.Show(); 
                _forceApplied = new Vector2((_chargeTime * forceScale) + Math.Abs(movement.rb.velocity.x),
                    (_chargeTime * forceUpScale) + movement.rb.velocity.y);
                trajectory.UpdateDots(transform.position, _forceApplied);
            }
        

            if (Input.GetMouseButtonUp(1) && _timeTilNextShotAlt < Time.time)
            {
                _chargeForce = _chargeTime;
                ShootAlternative();
                _chargeTime = 0;
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

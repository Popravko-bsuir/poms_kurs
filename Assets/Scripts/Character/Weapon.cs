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
        private bool _canShootAlt = true;
        private bool _canShoot = true;
        private bool _isChargingAlt;
        public GameObject grenadePrefab;
        public Trajectory trajectory;
        private Vector2 _forceApplied;
        private float _timeTilNextShotAlt;
        private float _chargeTime;
        private float _chargeForce;

        public float ForceScale => forceScale;
        public float ForceUpScale => forceUpScale;
        public float ChargeForce => _chargeForce;
        public bool IsChargingAlt => _isChargingAlt;

        void Update()
        {
            if (Input.GetMouseButton(1) && _canShootAlt && _timeTilNextShotAlt < Time.time && !movement.IsAimingUp && !movement.IsAimingDown)
            {
                _isChargingAlt = true;
                _canShoot = false;
                if (_chargeTime < chargeTimeMax)
                {
                    _chargeTime += Time.deltaTime;   
                }
                
                trajectory.Show(); 
                _forceApplied = new Vector2((_chargeTime * forceScale) + Math.Abs(movement.rb.velocity.x),
                    (_chargeTime * forceUpScale) + movement.rb.velocity.y);
                trajectory.UpdateDots(transform.position, _forceApplied);
            }
        

            if (Input.GetMouseButtonUp(1) && _canShootAlt && _timeTilNextShotAlt < Time.time && !movement.IsAimingUp && !movement.IsAimingDown)
            {
                _isChargingAlt = false;
                _canShoot = true;
                _chargeForce = _chargeTime;
                ShootAlternative();
                _chargeTime = 0;
                _timeTilNextShotAlt = Time.time + rateOfFireAlt;
                trajectory.Hide();
            }

            if (Input.GetButton("Fire1") && _canShoot && _timeTilNextShot < Time.time)
            {
                _canShootAlt = false;
                Shoot();
                _timeTilNextShot = Time.time + rateOfFire;
            }

            if (Input.GetButtonUp("Fire1") && _canShoot)
            {
                _canShootAlt = true;
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

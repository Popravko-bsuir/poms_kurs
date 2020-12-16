using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace Character
{
    public class Weapon : MonoBehaviour
    {
        public Transform firePoint;
        public Movement movement;
        [SerializeField] private int ammo;

        [Header("Fire")] 
        [SerializeField] private int layerOfEnemyToDamage;
        private int _bulletNumber;
        private GameObject[] _bulletList;
        [SerializeField] private int numberOfBullets;
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
        private Grenade _grenadeScript;
        
        [Header("Canvas")]
        public Text ammoCounter;
        
        public float ForceScale => forceScale;
        public float ForceUpScale => forceUpScale;
        public float ChargeForce => _chargeForce;
        public bool IsChargingAlt => _isChargingAlt;

        public int Ammo
        {
            get => ammo;
            set => ammo = value;
        }


        private void Start()
        {
            PrepareBullets();
            _grenadeScript = grenadePrefab.GetComponent<Grenade>();
            _grenadeScript.SetMovement(movement);
            _grenadeScript.SetWeapon(this);
            ammoCounter.text = ": " + ammo;
        }

        void Update()
        {
            if (CrossPlatformInputManager.GetButton("FireAlt") && _canShootAlt && _timeTilNextShotAlt < Time.time && !movement.IsAimingUp && !movement.IsAimingDown)
            {
                if (ammo > 10)
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
                else
                {
                    Debug.Log("no ammo");
                }
            }
        

            if (CrossPlatformInputManager.GetButtonUp("FireAlt") && _canShootAlt && _timeTilNextShotAlt < Time.time && 
                !movement.IsAimingUp && !movement.IsAimingDown && ammo > 10)
            {
                _isChargingAlt = false;
                _canShoot = true;
                _chargeForce = _chargeTime;
                ShootAlternative();
                _chargeTime = 0;
                _timeTilNextShotAlt = Time.time + rateOfFireAlt;
                trajectory.Hide();
                ammo -= 10;
                ammoCounter.text = ": " + ammo;
            }

            if (CrossPlatformInputManager.GetButton("Fire") && _canShoot && _timeTilNextShot < Time.time)
            {
                _canShootAlt = false;
                _timeTilNextShot = Time.time + rateOfFire;
                if (ammo > 0)
                {
                    Shoot();
                    ammo--;
                    ammoCounter.text = ": " + ammo;
                }
                else
                {
                    //play sound
                    Debug.Log("no ammo");
                }
            }

            if (CrossPlatformInputManager.GetButton("Fire") && _canShoot)
            {
                _canShootAlt = true;
            }
        }

        public void AddAmmo(int ammoToAdd)
        {
            ammo += ammoToAdd;
            ammoCounter.text = ": " + ammo;
        }

        void ShootAlternative()
        {
            Prefab(grenadePrefab);
        }

        void Shoot()
        {
            var firePointTransform = firePoint.transform;
            _bulletList[_bulletNumber].transform.position =  firePointTransform.position;
            _bulletList[_bulletNumber].transform.rotation = firePointTransform.rotation;
            _bulletList[_bulletNumber].SetActive(true);
            _bulletNumber++;
            if (_bulletNumber == numberOfBullets)
            {
                _bulletNumber = 0;
            }
        }

        private void Prefab(GameObject prefab)
        {
            Instantiate(prefab, firePoint.position, firePoint.rotation);
        }

        private void PrepareBullets()
        {
            _bulletList = new GameObject[numberOfBullets];
            for (int i = 0; i < numberOfBullets; i++)
            {
                _bulletList[i] = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                _bulletList[i].GetComponent<Bullet>().SetEnemyToDamage(layerOfEnemyToDamage); 
                _bulletList[i].SetActive(false);
            }
        }
    }
}

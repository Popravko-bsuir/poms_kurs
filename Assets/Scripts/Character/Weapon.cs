using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    public Vector2 forceApplied;
    public Trajectory trajectory;
    
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject grenadePrefab;

    public float rateOfFire = 0.5f;
    public float chargeTimeMax = 1f;
    private bool cantShoot;
    public static float chargeForce { get; set; }
    public static float chargeTime;


    private float _timeTilNextShot;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButton(1) && chargeTime < chargeTimeMax)
        {
            chargeTime += Time.deltaTime;
            trajectory.Show();
            forceApplied = new Vector2(chargeTime * Grenade.ForceScale, chargeTime * Grenade.ForceUpScale);
            trajectory.UpdateDots(transform.position, forceApplied);
        }
        

        if (Input.GetMouseButtonUp(1) && _timeTilNextShot < Time.time)
        {
            chargeForce = chargeTime;
            ShootAlternative();
            chargeTime = 0;
            _timeTilNextShot = Time.time + rateOfFire;
            trajectory.Hide();
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

    /*IEnumerator Shoot()
    {
        canShoot = false;
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        yield return new WaitForSeconds(0.5f);
        canShoot = true;
    }*/
    
}

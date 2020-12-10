using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using Enemies.Bug;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoldierAi : MonoBehaviour
{
    private Vector3 _targetPosition;
    [SerializeField] private int layerOfEnemieToDamage;
    private GameObject[] _bulletList;
    private const int NumberOfBullets = 30;
    private int _bulletNumber;
    public HealthPoints hp;
    public Animator animator;
    public GameObject bulletPrefab;
    public GameObject ammoPrefab;
    private Rigidbody2D _ammoRb;
    public GameObject corpsesPrefab;
    public CapsuleCollider2D capsuleCollider;
    [SerializeField] private Vector3 corpsesOffset;
    public CircleCollider2D trigger;
    public Rigidbody2D rb;
    public Transform firePoint;
    public Transform rayCastPosition;
    [SerializeField] private float raycastLength;
    public LayerMask groundLayer;
    public LayerMask characterLayer;
    private bool _isDead;
    private bool _showIdle;
    private bool _isFacingRight = true;
    private bool _isAimingUp;
    private bool _isAimingForward;
    private bool _isAimingDown;
    private bool _canShoot = true;
    [SerializeField] private float shootingPauseTime;
    [SerializeField] private float shootingBurstTime;
    private float _shootingTime;
    public bool _targetIsInRange;
    private bool _stopCoroutine;
    [SerializeField] private float patrollingTriggerRadius;
    [SerializeField] private float agroTriggerRadius;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 _targetDirection;
    private float _sign = 1f;
    private Quaternion _maxRotationValue;
    private float _zAngle;
    [SerializeField] private float shakeMaxAngle;
    private Quaternion _currentRotation;
    float time;
    
    void Start()
    {
        _ammoRb = ammoPrefab.GetComponent<Rigidbody2D>();
        ChangeTriggerRadius(patrollingTriggerRadius);
        StartCoroutine(Patrolling());
        PrepareBullets();
    }

    void Update()
    {
        animator.SetFloat("HorizontalSpeed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsAimingUp", _isAimingUp);
        animator.SetBool("IsAimingForward", _isAimingForward);
        animator.SetBool("IsAimingDown", _isAimingDown);
        animator.SetBool("StopShooting", !_stopCoroutine);
        animator.SetBool("ShowIdle", _showIdle);
        animator.SetBool("IsDead", _isDead);

    }


    private IEnumerator Patrolling()
    {
        while (true)
        {
            rb.drag = 0;
            while (Physics2D.Raycast(rayCastPosition.position,
                Vector2.down, raycastLength, groundLayer))
            {
                _showIdle = false;
                //rb.velocity = new Vector2(_isFacingRight ? speed : (-1) * speed, rb.velocity.y);
                rb.velocity = transform.right * speed;
                if (_stopCoroutine)
                {
                    rb.drag = 100;
                    yield break;
                }
                yield return new WaitForSeconds(0.02f);
            }

            rb.drag = 100;
            for (float t = 0; t <= 3f; t += Time.deltaTime)
            {
                _showIdle = true;
                if (_stopCoroutine)
                {
                    yield break;
                }

                yield return null;
            }

            Flip();
        }
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        transform.rotation = Quaternion.Euler(0, _isFacingRight ? 0 : 180, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var positionOfRayCast = rayCastPosition.position;
        Gizmos.DrawLine(positionOfRayCast, positionOfRayCast + Vector3.down * raycastLength);
        Gizmos.color = Color.blue;
        var transformPosition = transform.position;
        Gizmos.DrawLine(transformPosition, transformPosition + _targetDirection);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transformPosition, transformPosition + corpsesOffset); 
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transformPosition, _targetPosition);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _stopCoroutine = true;
            ChangeTriggerRadius(agroTriggerRadius);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        var transformPosition = transform.position;
        var targetPosition = other.gameObject.transform.position;
        
        _targetDirection = ((Vector2) targetPosition - (Vector2) transformPosition).normalized;
        _targetPosition = targetPosition - transformPosition;
        //Debug.DrawRay(transformPosition, targetPosition - transformPosition, Color.cyan , rayCastDirection.magnitude);
        // if (Physics2D.Raycast((Vector2) transformPosition, _targetDirection, rayCastDirection.magnitude, groundLayer))
        // {
        //     return;
        // }

        if (Physics2D.Linecast(transformPosition, targetPosition, groundLayer))
        {
            _isAimingDown = false;
            _isAimingForward = false;
            _isAimingUp = false;
            _showIdle = true;
            return;
        }

        Shoot();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        ChangeTriggerRadius(patrollingTriggerRadius);
        _stopCoroutine = false;
        _isAimingDown = false;
        _isAimingForward = false;
        _isAimingUp = false;
        _shootingTime = 0;
        animator.speed = 1;
        if (hp.GetHp > 0)
        {
            StartCoroutine(Patrolling());
        }
    }

    private void Shoot()
    {
        
        if (_targetDirection.x < 0 && _isFacingRight || _targetDirection.x > 0 && !_isFacingRight)
        {
            Flip();
        }

        if (_targetDirection.y < 0.4f && _targetDirection.y > -0.4f && time < Time.time && !_isDead && _canShoot)
        {
            _showIdle = false;
            _isAimingDown = false;
            _isAimingUp = false;
            _isAimingForward = true;
            //firePoint.localPosition = new Vector3(0.858f, 0.433f, 0f);
            time = Time.time + 0.1f;
            ShakeWeapon(0.858f, 0.433f, 0f, 0);
        }
        else if (_targetDirection.y < 0.9f && _targetDirection.y > 0.4f && time < Time.time && !_isDead && _canShoot)
        {
            _showIdle = false;
            _isAimingDown = false;
            _isAimingForward = false;
            _isAimingUp = true;
            //firePoint.localPosition = new Vector3(0.887f, 1.554f, 0f);
            time = Time.time + 0.1f;
            ShakeWeapon(0.887f, 1.554f, 0f , 45f);
        }
        else if (_targetDirection.y < -0.4f && _targetDirection.y > -0.9f && time < Time.time && !_isDead && _canShoot)
        {
            _showIdle = false;
            _isAimingForward = false;
            _isAimingUp = false;
            _isAimingDown = true;
            //firePoint.localPosition = new Vector3(0.956f, -0.275f, 0f);
            time = Time.time + 0.1f;
            ShakeWeapon(0.956f, -0.275f, 0f, -45f);
        }
        else if (_targetDirection.y < -0.9f || _targetDirection.y > 0.9f)
        {
            _isAimingDown = false;
            _isAimingUp = false;
            _showIdle = true;
        }
    }

    private void ShakeWeapon(float x, float y, float z, float weaponAngle)
    {
        if (_shootingTime > shootingBurstTime)
        {
            _shootingTime = 0;
            StartCoroutine(ShootingPause());
        }
        _shootingTime += 0.1f;
        firePoint.localPosition = new Vector3(x, y, z);
        firePoint.localRotation = Quaternion.Euler(0f, 0f, Random.Range(weaponAngle + (-30f), weaponAngle + 30f));
        LaunchBullet();
    }

    private void LaunchBullet()
    {
        var firePointTransform = firePoint.transform;
        _bulletList[_bulletNumber].transform.position =  firePointTransform.position;
        _bulletList[_bulletNumber].transform.rotation = firePointTransform.rotation;
        _bulletList[_bulletNumber].SetActive(true);
        _bulletNumber++;
        if (_bulletNumber == NumberOfBullets)
        {
            _bulletNumber = 0;
        }
    }

    private void ChangeTriggerRadius(float r)
    {
        trigger.radius = r;
    }

    private void PrepareBullets()
    {
        _bulletList = new GameObject[NumberOfBullets];
        for (int i = 0; i < NumberOfBullets; i++)
        {
            _bulletList[i] = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            _bulletList[i].GetComponent<Bullet>().SetEnemyToDamage(layerOfEnemieToDamage); 
            _bulletList[i].SetActive(false);
        }
    }

    public IEnumerator HideSoldier()
    {
        _isDead = true;
        capsuleCollider.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        yield return new WaitForSeconds(0.2f);
        var soldierTransform = transform;
        var soldierRotationEuler = soldierTransform.rotation.eulerAngles;
        Instantiate(corpsesPrefab, soldierTransform.position + corpsesOffset, soldierTransform.rotation);
        var ammoPrefabRotation = ammoPrefab.transform.rotation.eulerAngles;
        var ammoRotation = Quaternion.Euler(0, ammoPrefabRotation.y + soldierRotationEuler.y, ammoPrefabRotation.z);
        Instantiate(ammoPrefab, soldierTransform.position, ammoRotation);
        Destroy(gameObject);
    }

    private IEnumerator ShootingPause()
    {
        _canShoot = false;
        animator.speed = 0f;
        for (float t = 0; t <= shootingPauseTime; t += Time.deltaTime)
        {
            if (_isDead)
            {
                animator.speed = 1f;
                yield break;
            }
            yield return null;
        }

        _canShoot = true;
        animator.speed = 1f;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoldierAi : MonoBehaviour
{
    public Animator animator;
    public GameObject bulletPrefab;
    public CircleCollider2D trigger;
    public Rigidbody2D rb;
    public Transform firePoint;
    public Transform rayCastPosition;
    [SerializeField] private float raycastLength;
    public LayerMask groundLayer;
    private bool _showIdle;
    private bool _isFacingRight = true;
    private bool _isAimingUp;
    private bool _isAimingForward;
    private bool _isAimingDown;
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
        ChangeTriggerRadius(patrollingTriggerRadius);
        StartCoroutine(Patrolling());
    }

    void Update()
    {
        animator.SetFloat("HorizontalSpeed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsAimingUp", _isAimingUp);
        animator.SetBool("IsAimingForward", _isAimingForward);
        animator.SetBool("IsAimingDown", _isAimingDown);
        animator.SetBool("StopShooting", !_stopCoroutine);
        animator.SetBool("ShowIdle", _showIdle);

    }


    private IEnumerator Patrolling()
    {
        while (true)
        {
            while (Physics2D.Raycast(rayCastPosition.position,
                Vector2.down, raycastLength, groundLayer))
            {
                _showIdle = false;
                rb.velocity = new Vector2(_isFacingRight ? speed : (-1) * speed, rb.velocity.y);
                if (_stopCoroutine)
                {
                    yield break;
                }
                yield return new WaitForSeconds(0.02f);
            }

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
        Gizmos.DrawLine(rayCastPosition.position, rayCastPosition.position + Vector3.down * raycastLength);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + _targetDirection);
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
        if (other.gameObject.CompareTag("Player"))
        {
            _targetDirection = ((Vector2) other.gameObject.transform.position - (Vector2) transform.position).normalized;
            Shoot();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ChangeTriggerRadius(patrollingTriggerRadius);
            _stopCoroutine = false;
            _isAimingDown = false;
            _isAimingForward = false;
            _isAimingUp = false;
            StartCoroutine(Patrolling());
        }
    }

    private void Shoot()
    {
        
        if (_targetDirection.x < 0 && _isFacingRight || _targetDirection.x > 0 && !_isFacingRight)
        {
            Flip();
        }

        if (_targetDirection.y < 0.4f && _targetDirection.y > -0.4f && time < Time.time)
        {
            _showIdle = false;
            _isAimingDown = false;
            _isAimingUp = false;
            _isAimingForward = true;
            //firePoint.localPosition = new Vector3(0.858f, 0.433f, 0f);
            time = Time.time + 0.1f;
            ShakeWeapon(0.858f, 0.433f, 0f, 0);
        }
        else if (_targetDirection.y < 0.9f && _targetDirection.y > 0.4f && time < Time.time)
        {
            _showIdle = false;
            _isAimingDown = false;
            _isAimingForward = false;
            _isAimingUp = true;
            //firePoint.localPosition = new Vector3(0.887f, 1.554f, 0f);
            time = Time.time + 0.1f;
            ShakeWeapon(0.887f, 1.554f, 0f , 45f);
        }
        else if (_targetDirection.y < -0.4f && _targetDirection.y > -0.9f && time < Time.time)
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
        firePoint.localPosition = new Vector3(x, y, z);
        firePoint.localRotation = Quaternion.Euler(0f, 0f, Random.Range(weaponAngle + (-30f), weaponAngle + 30f));
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
    
    private void ChangeTriggerRadius(float r)
    {
        trigger.radius = r;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierAi : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform firePoint;
    [SerializeField] private float raycastLength;
    public LayerMask groundLayer;
    private bool _obstacleIsAhead;
    private bool _isFacingRight = true;
    public bool _targetIsInRange;
    [SerializeField] private float speed;
    void Start()
    {
        
    }

    void Update()
    {
        if (!_targetIsInRange)  // patrolling
        {
            if (!Physics2D.Raycast(firePoint.position, Vector2.down, raycastLength, groundLayer))
            {
                Flip();
            }
            
            rb.velocity = new Vector2(_isFacingRight ? speed : (-1) * speed, rb.velocity.y);

        }
        
    }

    // private IEnumerator Patrolling()
    // {
    //     while (!_targetIsInRange)
    //     {
    //         
    //     }
    // }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        transform.rotation = Quaternion.Euler(0, _isFacingRight ? 0 : 180, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(firePoint.position, firePoint.position + Vector3.down * raycastLength);
    }
}

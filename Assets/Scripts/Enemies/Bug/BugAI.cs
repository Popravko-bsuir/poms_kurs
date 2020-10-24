﻿using System;
using Pathfinding;
using UnityEngine;

namespace Enemies.Bug
{
    public class BugAI : MonoBehaviour
    {
        public Transform targetForBug;
        public Transform patrolingRaycastPosition;
        private Vector2 target;
        public float speed = 200f;
        [SerializeField] private float patrolingSpeed;
        public float nextWayPoyntDistance = 1f;
        public Vector2 direction;

        private Path _path;
        private int _currentWaypoynt;
        private bool _isReachedEndOfPath;

        [SerializeField] private Seeker seeker;
        [SerializeField] private Rigidbody2D rb;

        [SerializeField] private bool _characterIsInRange;
        private bool _corpsesIsInRange;
        
        private bool _isFacingLeft = true;

        [SerializeField] private float jumpForce = 50f;

        private const float RateOfJumping = 1f;
        [SerializeField] private float _jumpTimer;
        public bool isOnGround;
        [SerializeField] private float rayCastLength;
        [SerializeField] private float patrolHorizontalRayCastLength;
        public LayerMask groundLayer;

        void Start()
        {
            InvokeRepeating("UpdatePath", 0f, 0.5f);
        }

        void UpdatePath()
        {
            if (_characterIsInRange && seeker.IsDone() || _corpsesIsInRange && seeker.IsDone())
            {
                seeker.StartPath(rb.position, target, OnPathComplete);
            }
        }

        void Update()
        {
            isOnGround = Physics2D.Raycast(rb.position, Vector2.down, rayCastLength, groundLayer);
        }

        void FixedUpdate()
        {
            if (!_characterIsInRange && !_corpsesIsInRange)
            {
                bool pitIsAhead = Physics2D.Raycast(patrolingRaycastPosition.position, Vector3.down, patrolHorizontalRayCastLength, groundLayer);
                transform.Translate(Vector2.left * (patrolingSpeed * Time.deltaTime));
                Debug.Log("hui" + pitIsAhead);
                if (CheckCollisionAhead(_isFacingLeft? Vector2.left : Vector2.right) || !pitIsAhead)
                {
                    Flip();
                }

            }
            else
            {
                if (_path == null)
                {
                    return;
                }

                if (_currentWaypoynt >= _path.vectorPath.Count)
                {
                    _isReachedEndOfPath = true;
                    return;
                }
                else
                {
                    _isReachedEndOfPath = false;
                }
                
                direction = ((Vector2) _path.vectorPath[_currentWaypoynt] - rb.position).normalized;
                rb.AddForce(Vector2.right * (direction.x * speed * Time.deltaTime));
                if (direction.y > 0.7f && isOnGround && _jumpTimer < Time.time)
                {
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    _jumpTimer = RateOfJumping + Time.time;
                }

                float distance = Vector2.Distance(rb.position, _path.vectorPath[_currentWaypoynt]);
                if (distance < nextWayPoyntDistance)
                {
                    _currentWaypoynt++;
                }

                if (direction.x < 0 && !_isFacingLeft || direction.x > 0 && _isFacingLeft)
                {
                    Flip();
                }
            }
        }

        private bool CheckCollisionAhead(Vector2 direction)
        {
            bool collisionIsAhead;
            collisionIsAhead = Physics2D.Raycast(rb.position, direction, patrolHorizontalRayCastLength, groundLayer);
            return collisionIsAhead;
        }

        void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                _path = p;
                _currentWaypoynt = 0;
            }
        }

        void Flip()
        {
            _isFacingLeft = !_isFacingLeft;
            transform.rotation = Quaternion.Euler(0, _isFacingLeft ? 0 : 180, 0);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rb.position, rb.position + Vector2.down * rayCastLength);
            Gizmos.DrawLine(rb.position, rb.position + direction);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(patrolingRaycastPosition.position, patrolingRaycastPosition.position + Vector3.down * patrolHorizontalRayCastLength);
            Gizmos.DrawLine(rb.position, rb.position + Vector2.left * patrolHorizontalRayCastLength);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _characterIsInRange = true;
            }

            if (other.gameObject.CompareTag("Corpses"))
            {
                _corpsesIsInRange = true;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Corpses"))
            {
                targetForBug = other.gameObject.transform;
                target = new Vector2(targetForBug.position.x, targetForBug.position.y);
                // target = new Vector2(other.gameObject.transform.position.x, other.gameObject.transform.position.y);
            }

            if (other.gameObject.CompareTag("Player") && !_corpsesIsInRange)
            {
                targetForBug = other.gameObject.transform;
                target = new Vector2(targetForBug.position.x, targetForBug.position.y);
                // target = new Vector2(other.gameObject.transform.position.y, other.gameObject.transform.position.y);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _characterIsInRange = false;
                BugStop();
            }
        }

        private void BugStop()
        {
            seeker.StartPath(rb.position, rb.position, OnPathComplete);
        }

        public void DestroyBug()
        {
            Destroy(gameObject);
        }
    }
}
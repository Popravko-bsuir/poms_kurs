using System;
using System.Collections;
using Pathfinding;
using Pathfinding.Util;
using TreeEditor;
using UnityEngine;

namespace Enemies.Bug
{
    public class BugAI : MonoBehaviour
    {
        public Rigidbody2D target;
        public Transform targetForBug;
        private Vector2 trgt;
        public float speed = 200f;
        public float nextWayPoyntDistance = 1f;

        private Path _path;
        private int _currentWaypoynt;
        private bool _isReachedEndOfPath;

        private Seeker seeker;
        [SerializeField] private Rigidbody2D rb;

        private bool _isFacingLeft = true;

        [SerializeField] private float jumpForce = 50f; 
        public bool isOnGround;
        public bool canJump = true;
        [SerializeField] private float rayCastLength;
        public LayerMask groundLayer;

        void Start()
        {
            seeker = FindObjectOfType<Seeker>();
            
            InvokeRepeating("UpdatePath", 0f, 0.5f);
        }

        void UpdatePath()
        {
            if (targetForBug != null && seeker.IsDone())
            {
                seeker.StartPath(rb.position, trgt, OnPathComplete);
            }
        }

        void Update()
        { 
            isOnGround = Physics2D.Raycast(rb.position, Vector2.down, rayCastLength, groundLayer);
            
        }

        void FixedUpdate()
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

            _isReachedEndOfPath = false;

            float direction = _path.vectorPath[_currentWaypoynt].x - rb.position.x;
            rb.AddForce(Vector2.right * (direction * speed * Time.deltaTime));

            if (Mathf.Abs(rb.position.x - _path.vectorPath[_currentWaypoynt].x) < 2f &&
                _path.vectorPath[_currentWaypoynt].y - rb. position.y > 1f && isOnGround && canJump)
            {
                rb.AddForce(Vector2.up * jumpForce,ForceMode2D.Impulse);
                canJump = false;
                StartCoroutine(JumpTimer());
            }

            float distance = Vector2.Distance(rb.position, _path.vectorPath[_currentWaypoynt]);

            if (distance < nextWayPoyntDistance)
            {
                _currentWaypoynt++;
            }

            if (rb.velocity.x < 0 && !_isFacingLeft || rb.velocity.x > 0 && _isFacingLeft)
            {
                Flip();
            }
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
        }

        private IEnumerator JumpTimer()
        {
            yield return new WaitForSeconds(1f);
            canJump = true;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                targetForBug = other.gameObject.transform;
                trgt = new Vector2(targetForBug.position.x, targetForBug.position.y);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                targetForBug = null;
            }
        }
    }
}
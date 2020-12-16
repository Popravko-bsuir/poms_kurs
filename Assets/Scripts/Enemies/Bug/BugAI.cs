using System;
using System.Collections;
using Pathfinding;
using UnityEngine;

namespace Enemies.Bug
{
    public class BugAI : MonoBehaviour
    {
        public Transform startPoint;
        public SpriteRenderer spriteRenderer;
        public CapsuleCollider2D capsuleCollider2D;
        public BoxCollider2D biteCollider;
        private bool _isDead;
        public ParticleSystem crushEffectParticleSystem;
        public CircleCollider2D trigger;
        public float agroTriggerRadius;
        public float patrolTriggerRadius;
        public Transform targetForBug;
        public Transform patrolingRaycastPosition;
        private Vector2 target;
        public float speed = 200f;
        [SerializeField] private float patrolingSpeed;
        public float nextWayPoyntDistance = 1f;
        public Vector2 direction;
        public Vector2 pathStartPointOffset;
        [SerializeField] private Vector2 checkGroundRaycastOffset;
        [SerializeField] private Vector2 checkCharacterRaycastOffset;

        private Path _path;
        private int _currentWaypoynt;
        private bool _isReachedEndOfPath;

        [SerializeField] private Seeker seeker;
        [SerializeField] private Rigidbody2D rb;
        public Animator animator;

        [SerializeField] private bool _characterIsInRange;
        private bool _characterIsAbove;
        private bool _corpsesIsInRange;
        
        private bool _isFacingLeft = true;
        
        public bool IsFacingLeft => _isFacingLeft;

        [SerializeField] private float jumpForce = 50f;
        [SerializeField] private float maxSpeed = 15;
        
        private const float RateOfJumping = 3f;
        [SerializeField] private float _jumpTimer;
        public bool isOnGround;
        [SerializeField] private float rayCastLength;
        [SerializeField] private float patrolHorizontalRayCastLength;
        public LayerMask groundLayer;
        public LayerMask characterLayer;

        void Start()
        {
            InvokeRepeating("UpdatePath", 0f, 0.5f);
        }

        void UpdatePath()
        {
            if (_characterIsInRange && seeker.IsDone() || _corpsesIsInRange && seeker.IsDone())
            {
                seeker.StartPath((Vector2)startPoint.position, target, OnPathComplete);
            }
        }

        void Update()
        {
            isOnGround = Physics2D.Raycast(rb.position + checkGroundRaycastOffset,
                Vector2.down, rayCastLength, groundLayer) ||
            Physics2D.Raycast(rb.position - checkGroundRaycastOffset, Vector2.down, rayCastLength, groundLayer);
            _characterIsAbove = Physics2D.Raycast(rb.position + checkCharacterRaycastOffset,
                                    Vector2.up, rayCastLength, characterLayer) ||
            Physics2D.Raycast(rb.position - checkCharacterRaycastOffset, Vector2.up, rayCastLength, characterLayer);
            
            animator.SetFloat("Vertical", rb.velocity.y);
            animator.SetBool("IsOnGround", isOnGround);

            if (isOnGround && _characterIsAbove)
            {
                CrushingEffect();
            }
        }

        void FixedUpdate()
        {
            if (!_characterIsInRange && !_corpsesIsInRange && isOnGround && !_isDead)
            {
                bool pitIsAhead = Physics2D.Raycast(patrolingRaycastPosition.position, 
                    Vector3.down, patrolHorizontalRayCastLength, groundLayer);
                transform.Translate(Vector2.left * (patrolingSpeed * Time.deltaTime));
                if (CheckCollisionAhead(_isFacingLeft? Vector2.left : Vector2.right) && isOnGround || !pitIsAhead && isOnGround)
                {
                    Flip();
                }

            }
            else
            {
                if (_path == null || _isDead)
                {
                    return;
                }
                
                if (_currentWaypoynt >= _path.vectorPath.Count)
                {
                    _isReachedEndOfPath = true;
                    return;
                }
                // else
                // {
                //     _isReachedEndOfPath = false;
                // }
                
                direction = ((Vector2) _path.vectorPath[_currentWaypoynt] - (Vector2)startPoint.position).normalized;
                rb.AddForce(Vector2.right * (direction.x * speed /** Time.deltaTime*/));
                if (direction.y > 0.7f && isOnGround && _jumpTimer < Time.time)
                {
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    _jumpTimer = RateOfJumping + Time.time;
                }

                float distance = Vector2.Distance(startPoint.position, _path.vectorPath[_currentWaypoynt]);
                if (distance < nextWayPoyntDistance)
                {
                    _currentWaypoynt++;
                }

                if (rb.velocity.x < 0 && !_isFacingLeft || rb.velocity.x > 0 && _isFacingLeft)
                {
                    Flip();
                }
            }
            
            if (Mathf.Abs(rb.velocity.x) > maxSpeed)
            {
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
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
            Gizmos.DrawLine(rb.position + checkGroundRaycastOffset, rb.position + checkGroundRaycastOffset + Vector2.down * rayCastLength);
            Gizmos.DrawLine(rb.position - checkGroundRaycastOffset, rb.position - checkGroundRaycastOffset + Vector2.down * rayCastLength);
            Gizmos.DrawLine(rb.position + checkCharacterRaycastOffset, rb.position + checkCharacterRaycastOffset + Vector2.up * rayCastLength);
            Gizmos.DrawLine(rb.position - checkCharacterRaycastOffset, rb.position - checkCharacterRaycastOffset + Vector2.up * rayCastLength);
            var position = (Vector2)startPoint.position;
            Gizmos.DrawLine(position, position + direction);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(patrolingRaycastPosition.position, patrolingRaycastPosition.position + Vector3.down * patrolHorizontalRayCastLength);
            Gizmos.DrawLine(rb.position, rb.position + Vector2.left * patrolHorizontalRayCastLength);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                trigger.radius = agroTriggerRadius;
                _characterIsInRange = true;
            }

            if (other.gameObject.CompareTag("Corpses"))
            {
                trigger.radius = agroTriggerRadius;
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
                trigger.radius = patrolTriggerRadius;
                _characterIsInRange = false;
                BugStop();
            }

            if (other.gameObject.CompareTag("Corpses"))
            {
                trigger.radius = patrolTriggerRadius;
                _corpsesIsInRange = false;
                BugStop();
            }
        }

        private void BugStop()
        {
            var position = rb.position;
            seeker.StartPath(position, position, OnPathComplete);
        }

        public IEnumerator DestroyBug()
        {
            yield return new WaitForSeconds(2f);
            spriteRenderer.enabled = true;
            capsuleCollider2D.enabled = true;
            biteCollider.enabled = true;
            _isDead = false;
            HideBug();
        }

        public void CrushingEffect()
        {
            spriteRenderer.enabled = false;
            capsuleCollider2D.enabled = false;
            biteCollider.enabled = false;
            _isDead = true;
            crushEffectParticleSystem.Play(false);
            StartCoroutine(DestroyBug());
        }

        public void HideBug()
        {
            BugStop();
            gameObject.SetActive(false);
        }
    }
}
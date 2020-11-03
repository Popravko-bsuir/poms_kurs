using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Character
{
    public class Movement : MonoBehaviour
    {
        public DashAnimation dashAnimation;
        public Trajectory trajectory;
        [SerializeField] private float moveSpeed = 10f;
        private Vector2 _direction;
        private bool _isFacingRight = true;
        private bool _isAimingUp;
        private bool _isAimingDown;

        public bool IsAimingUp => _isAimingUp;
        public bool IsAimingDown => _isAimingDown;
        public bool IsFacingRight
        {
            get => _isFacingRight;
        }

        [Header("Vertical Movement")] 
        [SerializeField] private float jumpSpeed = 15f;

        [SerializeField] private float jumpDalay = 0.25f;
        private float _jumpTimer;


        [Header("Abilities")]
        [SerializeField] private float dashMagnitude = 20f;

        [SerializeField] private float dashTime = 0.2f;
        [SerializeField] private float earthHitJumpForce = 20f;
        [SerializeField] private float earthHitChargeTime = 0.2f;
        [SerializeField] private float earthHitChargeForce = 20f;
        private bool _isCollisionAhead;
        private bool _canDodge;
        private bool _earthHitIsStarted;
        private bool _isAlreadyStarted;


        [Header("Components")] 
        public Weapon weapon;
        public Transform firePoint;
        [SerializeField] private GameObject earthHitExplosion;
        public GameObject earthHitChargingEffectPrefab;
        public Animator animator;
        public Rigidbody2D rb;
        public LayerMask groundLayer;
        public GameObject characterHolder;


        [Header("Physics")] 
        public float maxSpeed = 10f;

        public float linearDrag = 4f;
        public float gravity = 1;
        public float fallMultiplier = 5f;


        [Header("Collision")] 
        public bool onGround = false;

        public float groundLength = 0.3f;
        public Vector3 colliderOffset;
        public Vector3 rayCastPosition;
        public float dashLength = 2.4f;
        public Vector3 dashOffset;
        
        void Update()
        {
            if (onGround)
            {
                _isAlreadyStarted = false;
                _canDodge = true;
            }
            
            animator.SetFloat("horizontal", Mathf.Abs(Input.GetAxis("Horizontal")));
            animator.SetFloat("vertical", rb.velocity.y);
            animator.SetBool("onGround", onGround);
            animator.SetBool("earthHitIsStarted", _earthHitIsStarted);
            animator.SetBool("earthHitIsCharging", _isAlreadyStarted);
            animator.SetBool("aimingUp", _isAimingUp);
            animator.SetBool("aimingDown", _isAimingDown);
            animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));
            

            bool wasOnGround = onGround;
            onGround =
                Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) ||
                Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
            
            if (!wasOnGround && onGround)
            {
                StartCoroutine(JumpSqueeze(1.2f, 0.8f, 0.1f));
                if (_isAlreadyStarted)
                {
                    Instantiate(earthHitExplosion, transform.position, transform.rotation);
                }
            }

            if (Input.GetKeyDown(KeyCode.W) && !weapon.IsChargingAlt && onGround)
            {
                _isAimingDown = false;
                firePoint.localPosition = new Vector3(0.506f, 1.631f, 0f);
                firePoint.localRotation = Quaternion.Euler(0, 0, 45f);
                _isAimingUp = true;
            }
            
            if (Input.GetKeyDown(KeyCode.S) && !weapon.IsChargingAlt && onGround)
            {
                _isAimingUp = false;
                firePoint.localPosition = new Vector3(0.55f, 0.55f, 0f);
                firePoint.localRotation = Quaternion.Euler(0, 0, -45f);
                _isAimingDown = true;
            }
            
            if (Input.GetKeyUp(KeyCode.W) && _isAimingUp || Input.GetKeyUp(KeyCode.S) && _isAimingDown || 
                Mathf.Abs(_direction.x) > 0 && _isAimingUp || Mathf.Abs(_direction.x) > 0 && _isAimingDown)
            {
                AimForward();
            }

            // if (_direction.y > 0 && _direction.x == 0 && onGround)
            // {
            //     firePoint.localPosition = new Vector3(0.506f, 1.631f, 0f);
            //     firePoint.localRotation = Quaternion.Euler(0, 0, 45f);
            //     _isAimingUp = true;
            //    // Debug.Log("dick");
            // }
            //
            // if (_direction.y < 0 && _direction.x == 0 && onGround)
            // {
            //     firePoint.localPosition = new Vector3(0.55f, 0.55f, 0f);
            //     firePoint.localRotation = Quaternion.Euler(0, 0, -45f);
            //     _isAimingDown = true;
            //     //Debug.Log("anal");
            //
            // }
            //
            // if (!onGround || _direction.y == 0 || Mathf.Abs(_direction.x) > 0 )
            // {
            //     firePoint.localPosition = new Vector3(0.807f, 1.109f, 0f);
            //     firePoint.localRotation = Quaternion.Euler(0, 0, 0);
            //     _isAimingUp = false;
            //     _isAimingDown = false;
            //     //Debug.Log("balls");
            // }

            if (Input.GetButtonDown("Jump"))
            {
                if (_isAimingUp || _isAimingDown)
                {
                    AimForward();
                }

                _jumpTimer = Time.time + jumpDalay;
            }

            if (Input.GetButtonDown("EarthHit") && !onGround && !_earthHitIsStarted)
            {
                _earthHitIsStarted = true;
                EarthHitPreparation();
            }

            if (Input.GetButtonDown("AirDodge") && !onGround && _canDodge)
            {
                StartCoroutine(dashAnimation.ShowDashAnimation(dashTime));
                CheckCollisionAhead(_isFacingRight ? Vector2.right : Vector2.left);
                if((_direction.x > 0 && rb.velocity.x < 0) || (_direction.x < 0 && rb.velocity.x > 0))
                {
                    AirDodgeBackvard(_isFacingRight ? Vector2.right : Vector2.left);
                }
                if (_isCollisionAhead)
                {
                    AirDodgeBackvard(_isFacingRight ? Vector2.right : Vector2.left);
                }
                else
                {
                    StartCoroutine(AirDodgeForvard(_isFacingRight ? Vector2.right : Vector2.left));
                }

                _isCollisionAhead = false;
                _canDodge = false;
            }

            _direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        private void FixedUpdate()
        {
            MoveCharacter(_direction.x);

            if (_jumpTimer > Time.time && onGround)
            {
                Jump();
            }

            ModifyPhysics();
        }

        void ModifyPhysics()
        {
            bool changingDirections = (_direction.x > 0 && rb.velocity.x < 0) || (_direction.x < 0 && rb.velocity.x > 0);

            if (onGround)
            {
                if (Mathf.Abs(_direction.x) < 0.4f || changingDirections)
                {
                    rb.drag = linearDrag;
                }
                else
                {
                    rb.drag = 0f;
                }

                rb.gravityScale = 0;
            }
            
            else
            {
                rb.gravityScale = gravity;
                rb.drag = linearDrag * 0.15f;
                
                if (_earthHitIsStarted && !onGround && !_isAlreadyStarted && Math.Abs(rb.velocity.y) < 0.3f)
                {
                    _isAlreadyStarted = true;
                    StartCoroutine(EarthHitCharging());
                    ShowChargingEffect();
                }
                else
                {
                    if (rb.velocity.y < 0)
                    {
                        rb.gravityScale = gravity * fallMultiplier;
                    }
                    else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
                    {
                        rb.gravityScale = gravity * (fallMultiplier / 2);
                    }
                }


            }
        }

        private void CheckAirtime()
        {
        }

        private void MoveCharacter(float horizontal)
        {
            rb.AddForce(Vector2.right * (horizontal * moveSpeed));

            if ((horizontal > 0 && !_isFacingRight) || (horizontal < 0 && _isFacingRight))
            {
                Flip();
            }

            if (Mathf.Abs(rb.velocity.x) > maxSpeed && onGround)
            {
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
            }
        }

        IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
        {
            Vector3 originalSize = Vector3.one;
            Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originalSize.z);
            float t = 0f;

            while (t <= 1.0)
            {
                t += Time.deltaTime / seconds;
                characterHolder.transform.localScale = Vector3.Lerp(originalSize, newSize, t);
                yield return null;
            }

            t = 0f;
            while (t <= 1.0)
            {
                t += Time.deltaTime / seconds;
                characterHolder.transform.localScale = Vector3.Lerp(newSize, originalSize, t);
                yield return null;
            }
        }

        void AimForward()
        {
            firePoint.localPosition = new Vector3(0.807f, 1.109f, 0f);
            firePoint.localRotation = Quaternion.Euler(0, 0, 0);
            _isAimingUp = false;
            _isAimingDown = false;
        }

        void Flip()
        {
            _isFacingRight = !_isFacingRight;
            transform.rotation = Quaternion.Euler(0, _isFacingRight ? 0 : 180, 0);
            trajectory.Hide();
        }

        private void Jump()
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            _jumpTimer = 0;
            StartCoroutine(JumpSqueeze(0.8f, 1.3f, 0.1f));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
            Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
            Gizmos.DrawLine((transform.position + rayCastPosition) + dashOffset, (transform.position + rayCastPosition) + dashOffset + Vector3.right * dashLength);
            Gizmos.DrawLine((transform.position + rayCastPosition) - dashOffset, (transform.position + rayCastPosition) - dashOffset + Vector3.right * dashLength);
        }

        private IEnumerator AirDodgeForvard(Vector2 direction)
        {
            rb.AddForce(direction * dashMagnitude, ForceMode2D.Impulse);
            yield return new WaitForSeconds(dashTime); 
            rb.AddForce(direction * ((dashMagnitude - 5) * (-1)), ForceMode2D.Impulse);
        }

        private void AirDodgeBackvard(Vector2 direction)
        {
            rb.AddForce(direction * dashMagnitude, ForceMode2D.Impulse);
        }

        private void CheckCollisionAhead(Vector2 direction)
        {
            _isCollisionAhead = 
                Physics2D.Raycast((transform.position + rayCastPosition) + dashOffset, direction, dashLength, groundLayer) || 
                Physics2D.Raycast((transform.position + rayCastPosition) - dashOffset, direction, dashLength, groundLayer);
        }

        private void EarthHitPreparation()
        {
            rb.AddForce(Vector2.up * (Math.Abs(rb.velocity.y) + earthHitJumpForce), ForceMode2D.Impulse);
        }

        private IEnumerator EarthHitCharging()
        {
            _earthHitIsStarted = false;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            yield return new WaitForSeconds(earthHitChargeTime);
            rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            rb.AddForce(Vector2.down * earthHitChargeForce, ForceMode2D.Impulse);
        }

        private void ShowChargingEffect()
        {
            Instantiate(earthHitChargingEffectPrefab, transform.position, transform.rotation);
        }
    }
}
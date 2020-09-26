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
        public AirDodgeAnimationController adaController;
        public Trajectory trajectory;
        public float moveSpeed = 10f;
        public Vector2 direction;
        public static bool isFacingRight = true;

        [Header("Vertical Movement")] 
        public float jumpSpeed = 15f;
        public float jumpDalay = 0.25f;
        public float jumpTimer;
        public static Vector2 characterSpeed;
        
        [Header("Abilities")]
        public float airDodgeMagnitude;
        public float airDodgeTime = 0.5f;
        private bool _isCollisionAhead;
        private bool _canDodge;
        public float earthHitJumpForce;
        public bool earthHitIsStarted;
        public bool isAlreadyStarted;
        public float earthHitChargeTime;
        public float earthHitChargeForce;

        [Header("Components")] 
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
        public float airDodgeLength = 2.4f;
        public Vector3 airDodgeOffset;

        void Start()
        {
        }

        void Update()
        {
            

            if (onGround)
            {
                //earthHitIsStarted = false;
                isAlreadyStarted = false;
                _canDodge = true;
            }

            var velocity = rb.velocity;
            characterSpeed = new Vector2(velocity.x, velocity.y);
            
            animator.SetFloat("horizontal", Mathf.Abs(Input.GetAxis("Horizontal")));
            animator.SetFloat("vertical", rb.velocity.y);
            animator.SetBool("onGround", onGround);
            animator.SetBool("earthHitIsStarted", earthHitIsStarted);
            animator.SetBool("earthHitIsCharging", isAlreadyStarted);

            bool wasOnGround = onGround;
            onGround =
                Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) ||
                Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
            
            if (!wasOnGround && onGround)
            {
                StartCoroutine(JumpSqueeze(1.2f, 0.8f, 0.1f));
            }

            if (Input.GetButtonDown("Jump"))
            {
                jumpTimer = Time.time + jumpDalay;
            }

            if (Input.GetButtonDown("EarthHit") && !onGround && !earthHitIsStarted)
            {
                earthHitIsStarted = true;
                EarthHitPreparation();
            }

            if (Input.GetButtonDown("AirDodge") && !onGround && _canDodge)
            {
                StartCoroutine(adaController.ShowAirDodgeEffect(airDodgeTime));
                CheckCollisionAhead(isFacingRight ? Vector2.right : Vector2.left);
                if((direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0))
                {
                    AirDodgeBackvard(isFacingRight ? Vector2.right : Vector2.left);
                }
                if (_isCollisionAhead)
                {
                    AirDodgeBackvard(isFacingRight ? Vector2.right : Vector2.left);
                }
                else
                {
                    StartCoroutine(AirDodgeForvard(isFacingRight ? Vector2.right : Vector2.left));
                }

                _isCollisionAhead = false;
                _canDodge = false;
            }

            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        private void FixedUpdate()
        {
            MoveCharacter(direction.x);

            if (jumpTimer > Time.time && onGround)
            {
                Jump();
            }

            ModifyPhysics();
        }

        void ModifyPhysics()
        {
            bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

            if (onGround)
            {
                if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
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
                
                if (earthHitIsStarted && !onGround && !isAlreadyStarted && Math.Abs(characterSpeed.y) < 0.2f)
                {
                    isAlreadyStarted = true;
                    Debug.Log("Bad");
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

            if ((horizontal > 0 && !isFacingRight) || (horizontal < 0 && isFacingRight))
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

        void Flip()
        {
            isFacingRight = !isFacingRight;
            transform.rotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);
            trajectory.Hide();
        }

        private void Jump()
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            jumpTimer = 0;
            StartCoroutine(JumpSqueeze(0.8f, 1.3f, 0.1f));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
            Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
            Gizmos.DrawLine((transform.position + rayCastPosition) + airDodgeOffset, (transform.position + rayCastPosition) + airDodgeOffset + Vector3.right * airDodgeLength);
            Gizmos.DrawLine((transform.position + rayCastPosition) - airDodgeOffset, (transform.position + rayCastPosition) - airDodgeOffset + Vector3.right * airDodgeLength);
        }

        private IEnumerator AirDodgeForvard(Vector2 direction)
        {
            rb.AddForce(direction * airDodgeMagnitude, ForceMode2D.Impulse);
            yield return new WaitForSeconds(airDodgeTime); 
            rb.AddForce(direction * ((airDodgeMagnitude - 5) * (-1)), ForceMode2D.Impulse);
        }

        private void AirDodgeBackvard(Vector2 direction)
        {
            rb.AddForce(direction * airDodgeMagnitude, ForceMode2D.Impulse);
        }

        private void CheckCollisionAhead(Vector2 direction)
        {
            _isCollisionAhead = 
                Physics2D.Raycast((transform.position + rayCastPosition) + airDodgeOffset, direction, airDodgeLength, groundLayer) || 
                Physics2D.Raycast((transform.position + rayCastPosition) - airDodgeOffset, direction, airDodgeLength, groundLayer);
        }

        private void EarthHitPreparation()
        {
            rb.AddForce(Vector2.up * (Math.Abs(characterSpeed.y) + earthHitJumpForce), ForceMode2D.Impulse);
        }

        private IEnumerator EarthHitCharging()
        {
            earthHitIsStarted = false;
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
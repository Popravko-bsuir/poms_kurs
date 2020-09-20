using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class Movement : MonoBehaviour
    {
        public Trajectory trajectory;
        public float moveSpeed = 10f;
        public Vector2 direction;
        public static bool isFacingRight = true;

        [Header("Vertical Movement")] 
        public float jumpSpeed = 15f;
        public float jumpDalay = 0.25f;
        public float jumpTimer;
        public static Vector2 characterSpeed;


        [Header("Components")] 
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

        void Start()
        {
        }

        void Update()
        {
            var velocity = rb.velocity;
            characterSpeed = new Vector2(velocity.x, velocity.y);
            
            animator.SetFloat("horizontal", Mathf.Abs(Input.GetAxis("Horizontal")));
            animator.SetFloat("vertical", rb.velocity.y);
            animator.SetBool("onGround", onGround);

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

            if (Mathf.Abs(rb.velocity.x) > maxSpeed)
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
            // if (Weapon.chargeTime >= 3f)
            // {
            //     trajectory.Show();
            // }
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
            Gizmos.DrawLine(transform.position + colliderOffset,
                transform.position + colliderOffset + Vector3.down * groundLength);
            Gizmos.DrawLine(transform.position - colliderOffset,
                transform.position - colliderOffset + Vector3.down * groundLength);
        }
    }
}
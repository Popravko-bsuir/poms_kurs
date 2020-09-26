using System.Collections;
using UnityEngine;

namespace Character
{
    public class AirDodgeAnimationController : MonoBehaviour
    {
        [Header("Components")] public Animator animator;
        public SpriteRenderer spriteRenderer;

        void Start()
        {
        }

        void Update()
        {
        }

        public IEnumerator ShowAirDodgeEffect(float time)
        {
            animator.enabled = true;
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(time);
            spriteRenderer.enabled = false;
            animator.enabled = false;
        }
    }
}
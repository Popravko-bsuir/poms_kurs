using System.Collections;
using UnityEngine;

namespace Character
{
    public class DashAnimation : MonoBehaviour
    {
        [Header("Components")] public Animator animator;
        public SpriteRenderer spriteRenderer;

        public IEnumerator ShowDashAnimation(float time)
        {
            animator.enabled = true;
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(time);
            spriteRenderer.enabled = false;
            animator.enabled = false;
        }
    }
}
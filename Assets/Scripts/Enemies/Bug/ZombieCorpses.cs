using System.Collections;
using System.Security.Cryptography;
using Character;
using UnityEngine;

namespace Enemies.Bug
{
    public class ZombieCorpses : MonoBehaviour
    {
        [SerializeField] private int bugsToSpawn;
        public GameObject bugPrefab;
        public Animator animator;
        public SpriteRenderer bugSpriteRanderer;
        public ParticleSystem bloodBurst;
        public ParticleSystem bloodSquirt;
        public SpriteRenderer spriteRenderer;
        public BoxCollider2D trigger;
        public BugAI bugAi;
        public Movement movement;
        private bool _swellingIsStarted;
        [SerializeField] private float velocityForCrush;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("EnemieBug"))
            {
                bugAi = other.gameObject.GetComponentInParent<BugAI>();
                bugAi.HideBug();
                StartCoroutine(Swelling());
            }

            if (other.gameObject.CompareTag("Player"))
            {
                movement = other.GetComponent<Movement>();
                if (movement.rb.velocity.y < velocityForCrush && !_swellingIsStarted)
                {
                    StartCoroutine(Crush());
                }
            }
        }

        private IEnumerator Swelling()
        {
            trigger.enabled = false;
            _swellingIsStarted = true;
            bugSpriteRanderer.enabled = true;
            bloodSquirt.Play();
            yield return new WaitForSeconds(2f);
            bugSpriteRanderer.enabled = false;
            animator.Play("Swelling");
            yield return new WaitForSeconds(2.5f);
            bloodBurst.Play(false);
            InstantiateBugs();
            spriteRenderer.enabled = false;
            trigger.enabled = false;
            yield return new WaitForSeconds(2.5f);
            gameObject.SetActive(false);
        }

        private IEnumerator Crush()
        {
            bloodBurst.Play(false);
            trigger.enabled = false;
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(3f);
            gameObject.SetActive(false);
        }

        private void InstantiateBugs()
        {
            var transform1 = transform;
            for (int i = 0; i <= bugsToSpawn; i++)
            {
                var offset = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(0, 3), 0);
                Instantiate(bugPrefab, transform1.position + offset, transform1.rotation);
            }
        }
    }
}
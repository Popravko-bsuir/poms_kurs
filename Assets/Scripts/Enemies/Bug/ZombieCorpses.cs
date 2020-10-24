using UnityEngine;

namespace Enemies.Bug
{
    public class ZombieCorpses : MonoBehaviour
    {
        public ParticleSystem particleSystem;
        public BugAI bugAi;
        [SerializeField] private int bugCount;
        [SerializeField] private int bugCountMax = 3;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("EnemieBug"))
            {
                particleSystem.Play();
                //TODO: check for memory leak
                bugAi = other.gameObject.GetComponentInParent<BugAI>();
                bugAi.DestroyBug();
                bugCount++;
                if (bugCount == bugCountMax)
                {
                    Destroy(gameObject, 3f);
                }
            }
        }
    }
}
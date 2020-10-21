using UnityEngine;

namespace Enemies.Bug
{
    public class ZombieCorpses : MonoBehaviour
    {
        public BugAI bugAi;
        [SerializeField] private int bugCount;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("EnemieBug"))
            {
                //TODO: check for memory leak
                bugAi = other.gameObject.GetComponentInParent<BugAI>();
                bugAi.DestroyBug();
                bugCount++;
                if (bugCount == 3)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
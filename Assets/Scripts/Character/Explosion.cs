using UnityEngine;

namespace Character
{
    public class Explosion : MonoBehaviour
    {
        public float deathTime = 1f;
        void Start()
        {
            DestroyObjectDelayed();
        }

        void Update()
        {
        
        }

        private void DestroyObjectDelayed()
        {
            Destroy(gameObject, deathTime);
        }

    }
}

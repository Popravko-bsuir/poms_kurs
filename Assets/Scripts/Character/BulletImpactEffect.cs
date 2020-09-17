using UnityEngine;

namespace Character
{
    public class BulletImpactEffect : MonoBehaviour
    {
        public float effectTime = 1;

        void Start()
        {
            DestroyObjectDelayed();
        }

        void Update()
        {
            // Destroy(GameObject, 2);
        }

        void DestroyObjectDelayed()
        {
            Destroy(gameObject, effectTime);
        }
    }
}
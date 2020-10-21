using UnityEngine;

namespace Character
{
    public class BulletImpactEffect : MonoBehaviour
    {
        public float effectTime = 1;

        void Start()
        {
            Destroy(gameObject, effectTime);
        }
    }
}
using UnityEngine;

namespace Character
{
    public class Explosion : MonoBehaviour
    {
        public float deathTime = 1f;
        void Start()
        {
            Destroy(gameObject, deathTime);
        }
    }
}

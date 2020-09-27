using UnityEngine;

namespace Character
{
    public class EarthHitCharging : MonoBehaviour
    {
        [SerializeField] private float effectTime = 1f;
        void Start()
        {
            Destroy(gameObject, effectTime);
        }
    }
}

using UnityEngine;

namespace Character
{
    public class SuperCharging : MonoBehaviour
    {
        [SerializeField] private float effectTime = 1f;
        void Start()
        {
            Destroy(gameObject, effectTime);
        }
    }
}

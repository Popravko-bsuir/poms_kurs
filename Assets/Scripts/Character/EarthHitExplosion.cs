using UnityEngine;

namespace Character
{
    public class EarthHitExplosion : MonoBehaviour
    {
        [SerializeField] private float time = 1f;
        [SerializeField] private float explsionTime = 0.5f;
        [SerializeField] private PointEffector2D pe;
        private float _explosionTimer;
        
        void Start()
        {
            Destroy(gameObject,time);
        }

        void Update()
        {
            _explosionTimer += Time.deltaTime;
            if (_explosionTimer >= explsionTime)
            {
                pe.enabled = false;
            }
        }
    }
}

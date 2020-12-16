using UnityEngine;

namespace Character
{
    public class Levetating : MonoBehaviour
    {
        [SerializeField] private Vector3 amplitude;
        [SerializeField] private float speed;
    
        private void Update()
        {
            transform.position += amplitude * Mathf.Sin(speed * Time.time);
        }
    }
}

using System.Collections;
using Enemies.Bug;
using UnityEngine;

namespace Character
{
    public class Bullet : MonoBehaviour
    {
        [Header("Components")] 
        public Rigidbody2D rb;
        public ParticleSystem hitEffect;
        public SpriteRenderer spriteRenderer;

        public GameObject impactEffect;
        public float speed = 20f;
        [SerializeField] private float bulletLifeTime;
        private float _time;
        private int _enemyToDamage;
        private bool _isCollided;

        public void SetEnemyToDamage(int enemyLayer)
        {
            _enemyToDamage = enemyLayer;
        }
    

        private void Update()
        {
            if (!_isCollided)
            {
                rb.velocity = transform.right * speed;
            }

            _time += Time.deltaTime;
            if (_time > bulletLifeTime && !_isCollided)
            {
                _time = 0;
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("EnemieBug"))
            {
                col.GetComponentInParent<BugAI>().CrushingEffect();
                var transform1 = transform;
                Instantiate(impactEffect, transform1.position, transform1.rotation);
                gameObject.SetActive(false);
            }

            if (col.gameObject.layer == _enemyToDamage)
            {
                hitEffect.Play(false);
                col.gameObject.GetComponent<HealthPoints>().TakeDamage(10);
                var transform1 = transform;
                Instantiate(impactEffect, transform1.position, transform1.rotation);
                StartCoroutine(HideBullet());
            }
        }

        IEnumerator HideBullet()
        {
            spriteRenderer.enabled = false;
            _isCollided = true;
            yield return new WaitForSeconds(2f);
            _isCollided = false;
            _time = 0f;
            spriteRenderer.enabled = true;
            gameObject.SetActive(false);
        }
    }
}
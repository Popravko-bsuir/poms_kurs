using UnityEngine;

namespace Character
{
    public class FirstAid : MonoBehaviour
    {
        [SerializeField] private int hpToHeal;
        private HealthPoints _playerHp;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _playerHp = other.gameObject.GetComponent<HealthPoints>();
                if (_playerHp.CompareHp())
                {
                    _playerHp.Heal(hpToHeal);
                    gameObject.SetActive(false);
                }
            }
        }
    }
}

using System.Collections;
using UnityEngine;

namespace Character
{
    public class SuperExplosion : MonoBehaviour
    {
        [SerializeField] private float explsionTime = 0.5f;
        [SerializeField] private PointEffector2D pe;
        
        void Start()
        {
            StartCoroutine(HideEffect());
        }

        private IEnumerator HideEffect()
        {
            yield return new WaitForSeconds(explsionTime);
            pe.enabled = false;
            yield return new WaitForSeconds(explsionTime);
            gameObject.SetActive(false);
        }
    }
}

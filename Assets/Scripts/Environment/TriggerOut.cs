using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public class TriggerOut : MonoBehaviour
    {
        [SerializeField] private bool wait;
        [SerializeField] private float timeToWait;
        [SerializeField] private int layerToCheck;
        public UnityEvent onOut;

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == layerToCheck)
            {
                if (!wait)
                {
                    onOut.Invoke();

                }
                else
                {
                    StartCoroutine(Wait());
                }
            }
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(timeToWait);
            onOut.Invoke();
        }
    }
}

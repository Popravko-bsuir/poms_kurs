using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public class TriggerEnter : MonoBehaviour
    {
        [SerializeField] private bool wait;
        [SerializeField] private float timeToWait;
        [SerializeField] private int layerToCheck;
        public UnityEvent onEnter;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != layerToCheck) return;
            if (!wait)
            {
                onEnter.Invoke();

            }
            else
            {
                StartCoroutine(Wait());
            }
        }
        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(timeToWait);
            onEnter.Invoke();
        }
    }
}

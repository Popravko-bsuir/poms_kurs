using System;
using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public class TriggerStay : MonoBehaviour
    {
        [SerializeField] private float timeToWait;
        private float _time;
        [SerializeField] private int layerToCheck;
        public UnityEvent onStay;
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == layerToCheck && _time < Time.time)
            {
                _time = timeToWait + Time.time;
                onStay.Invoke();
            }
        }
    }
}

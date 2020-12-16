using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeathTrigger : MonoBehaviour
{
    public UnityEvent onDeath;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != 10)
        {
            return;
        }
        other.gameObject.SetActive(false);
        onDeath.Invoke();
    }
}

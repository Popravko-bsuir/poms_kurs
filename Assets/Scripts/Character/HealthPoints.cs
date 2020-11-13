using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPoints : MonoBehaviour
{
    [SerializeField] private int healthPoints;

    public void TakeDamage(int damage)
    {
        if (damage >= healthPoints)
        {
            healthPoints = 0;
            gameObject.SetActive(false);
        }
        else
        {
            healthPoints -= damage;
        }
        Debug.Log(healthPoints);
    }
}

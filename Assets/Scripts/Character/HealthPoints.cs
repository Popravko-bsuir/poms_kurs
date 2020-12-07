using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;

public class HealthPoints : MonoBehaviour
{
    public Slider healthBar;
    [SerializeField] private int healthPoints;
    private int _maxHealthPoints;
    
    public int GetHp=> healthPoints;

    private void Start()
    {
        _maxHealthPoints = healthPoints;
        if (gameObject.layer == 10)
        {
            healthBar.maxValue = healthPoints;
            healthBar.value = healthPoints;
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage >= healthPoints)
        {
            healthPoints = 0;
            if (gameObject.layer == 11)
            { 
                StartCoroutine(gameObject.GetComponentInParent<SoldierAi>().HideSoldier());
            }
            else if(gameObject.layer == 10)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            healthPoints -= damage;
            if (gameObject.layer == 10)
            {
                healthBar.value = healthPoints;
            }
        }
    }

    public void Heal(int hpToHeal)
    {
        if (healthPoints + hpToHeal >= _maxHealthPoints)
        {
            healthPoints = _maxHealthPoints;
            healthBar.value = healthPoints;
        }
        else
        {
            healthPoints += hpToHeal;
            healthBar.value = healthPoints;
        }
    }

    public bool CompareHp()
    {
        if (healthPoints == _maxHealthPoints)
        {
            return false;
        }
        
        return true;
        
    }

}

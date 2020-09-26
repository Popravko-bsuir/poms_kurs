using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthHitCharging : MonoBehaviour
{
    [SerializeField] private float effectTime;
    void Start()
    {
        Destroy(gameObject, effectTime);
    }
}

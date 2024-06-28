using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public float startHealth = 100.0f;
    public float currentHealth;
    // Start is called before the first frame update

    void Start()
    {
        currentHealth = startHealth;
    }


    public void TakeDamage(float damageAmount)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damageAmount;
        }
        if (currentHealth <= 0)
        {
            //Die();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag ("Projectile"))
        {
            TakeDamage(10);
        }
    }


}

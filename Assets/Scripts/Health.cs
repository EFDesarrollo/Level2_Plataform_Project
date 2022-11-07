using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public Image healthBar;
    public UnityEvent onDie;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUIBar();
    }

    public void UpdateHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateUIBar();
    }

    public void TakeDamage(int amount)
    {
        UpdateHealth(-amount);
        if (currentHealth <= 0)
        {
            Die();
        }
        if (GetComponent<ColorModifier>() != null)
        {
            GetComponent<ColorModifier>().Blink();
        }
    }

    public void AddHealth(int amount)
    {
        UpdateHealth(amount);
    }

    public void RecoverAllHealth()
    {
        UpdateHealth(maxHealth);
    }

    public float GetPercentageHealth()
    {
        return (float)currentHealth / maxHealth;
    }

    public void Die()
    {
        onDie.Invoke();
    }

    public void DestroyObj()
    {
        Destroy(gameObject);
    }

    public void UpdateUIBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = GetPercentageHealth();
        }
    }
}

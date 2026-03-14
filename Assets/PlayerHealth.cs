using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public float hitStunDuration = 0.2f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isHitStunned = false;
    private bool isDefeated = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damageAmount, Vector2 hitDirection, float knockbackForce)
    {
        if (isDefeated)
        {
            return;
        }

        currentHealth -= damageAmount;

        if (currentHealth < 0f)
        {
            currentHealth = 0f;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode2D.Impulse);
        }

        StartCoroutine(HitReaction());

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private IEnumerator HitReaction()
    {
        isHitStunned = true;

        if (sr != null)
        {
            sr.color = Color.red;
        }

        yield return new WaitForSeconds(hitStunDuration);

        if (sr != null && !isDefeated)
        {
            sr.color = Color.white;
        }

        isHitStunned = false;
    }

    private void Die()
    {
        isDefeated = true;

        if (sr != null)
        {
            sr.color = Color.gray;
        }

    }

    public bool IsHitStunned()
    {
        return isHitStunned;
    }

    public bool IsDefeated()
    {
        return isDefeated;
    }
}
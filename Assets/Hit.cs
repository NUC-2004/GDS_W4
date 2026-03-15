using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    public float damage = 10f;
    public float knockbackForce = 5f;
    public bool canHit = true;

    private bool hasHitThisPunch = false;

    private void OnEnable()
    {
        hasHitThisPunch = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canHit || hasHitThisPunch)
        {
            return;
        }

        if (other.CompareTag("Glove"))
        {
            hasHitThisPunch = true;
            return;
        }

        if (other.CompareTag("Face"))
        {
            PlayerHealth targetHealth = other.GetComponentInParent<PlayerHealth>();

            if (targetHealth != null)
            {
                Vector2 hitDirection = (targetHealth.transform.position - transform.position).normalized;
                targetHealth.TakeDamage(damage, hitDirection, knockbackForce);
                hasHitThisPunch = true;
            }
        }
    }
}
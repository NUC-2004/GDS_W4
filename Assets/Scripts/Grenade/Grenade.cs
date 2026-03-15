using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public enum GrenadeType { Auto, Throwable }

    [Header("Type")]
    public GrenadeType grenadeType = GrenadeType.Auto;

    [Header("Auto Explode")]
    public float autoExplodeDelay = 3f;

    [Header("Throwable")]
    public float throwForce = 8f;

    [Header("Explosion")]
    public float explosionDamage = 50f;
    public float explosionKnockback = 15f;
    public float explosionRadius = 0.6f;

    [Header("Visuals")]
    public Color liveColor = Color.red;
    public Color inactiveColor = new Color(0.5f, 0.8f, 1f);

    [Header("Effects")]
    public GameObject explosionEffectPrefab;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool isLive = false;
    private bool hasExploded = false;
    private bool isHeld = false;
    private bool isThrown = false;
    private PlayerController2D holder = null;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        UpdateVisual();

        if (grenadeType == GrenadeType.Auto)
        {
            Activate();
        }
    }

    public void Activate()
    {
        isLive = true;
        UpdateVisual();

        if (grenadeType == GrenadeType.Auto)
        {
            StartCoroutine(AutoExplodeCountdown());
        }
    }

    private void UpdateVisual()
    {
        if (sr == null) return;
        sr.color = isLive ? liveColor : inactiveColor;
    }

    private IEnumerator AutoExplodeCountdown()
    {
        yield return new WaitForSeconds(autoExplodeDelay);
        Explode();
    }

    public bool TryPickUp(PlayerController2D player)
    {
        if (grenadeType != GrenadeType.Throwable) return false;
        if (isHeld || hasExploded || isThrown) return false;

        isHeld = true;
        holder = player;
        isLive = true;
        UpdateVisual();

        rb.simulated = false;
        transform.SetParent(player.transform);
        transform.localPosition = new Vector3(0.5f, 0.5f, 0f);

        return true;
    }

    public void Throw(Vector2 direction)
    {
        if (!isHeld || hasExploded) return;

        isHeld = false;
        isThrown = true;
        holder = null;

        transform.SetParent(null);
        rb.simulated = true;
        rb.AddForce(direction.normalized * throwForce, ForceMode2D.Impulse);

        StartCoroutine(AutoExplodeCountdown());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return;
        if (!isThrown) return;

        if (other.CompareTag("Face"))
        {
            Explode();
        }
    }

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Face"))
            {
                PlayerHealth health = hit.GetComponentInParent<PlayerHealth>();
                if (health != null)
                {
                    Vector2 dir = (health.transform.position - transform.position).normalized;
                    health.TakeDamage(explosionDamage, dir, explosionKnockback);
                }
            }
        }

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public bool IsHeld() => isHeld;
    public bool IsLive() => isLive;
}
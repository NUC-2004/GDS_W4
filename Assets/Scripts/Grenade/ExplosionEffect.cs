using System.Collections;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public float duration = 0.8f;
    public Color explosionColor = new Color(1f, 0.5f, 0f, 1f);

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.color = explosionColor;
            Debug.Log("SpriteRenderer found, color: " + sr.color);
        }
        else
        {
            Debug.Log("NO SpriteRenderer found");
        }

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float timer = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 3f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            if (sr != null)
            {
                Color c = sr.color;
                c.a = 1f - t;
                sr.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
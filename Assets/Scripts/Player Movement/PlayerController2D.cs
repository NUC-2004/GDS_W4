using System.Collections;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;

    [Header("Punch Hitboxes")]
    public PunchHitbox leftPunchHitbox;
    public PunchHitbox rightPunchHitbox;

    [Header("Punch Visuals")]
    public Transform leftGloveVisual;
    public Transform rightGloveVisual;

    [Header("Punch Target Points")]
    public Transform leftPunchPoint;
    public Transform rightPunchPoint;

    [Header("Punch Timing")]
    public float punchMoveDuration = 0.08f;
    public float punchActiveTime = 0.12f;
    public float punchCooldown = 0.35f;

    [Header("Input")]
    public KeyCode moveUp;
    public KeyCode moveDown;
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode leftPunchKey;
    public KeyCode rightPunchKey;

    [Header("Facing")]
    public Vector2 initialFacing = Vector2.right;
    public bool invertLeftRightFacing = false;

    private Vector2 moveInput;
    private Vector2 facingDirection;
    private bool canPunch = true;

    private PlayerHealth playerHealth;

    private Vector3 leftGloveStartLocalPos;
    private Vector3 rightGloveStartLocalPos;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        playerHealth = GetComponent<PlayerHealth>();

        facingDirection = initialFacing.normalized;
        ApplyFacingVisual();

        if (leftGloveVisual != null)
        {
            leftGloveStartLocalPos = leftGloveVisual.localPosition;
        }

        if (rightGloveVisual != null)
        {
            rightGloveStartLocalPos = rightGloveVisual.localPosition;
        }
    }

    private void Update()
    {
        HandleInput();
        HandleFacingByMovement();
        HandlePunchInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(moveLeft)) x -= 1f;
        if (Input.GetKey(moveRight)) x += 1f;
        if (Input.GetKey(moveUp)) y += 1f;
        if (Input.GetKey(moveDown)) y -= 1f;

        moveInput = new Vector2(x, y).normalized;
    }

    private void HandleMovement()
    {
        if (playerHealth != null && playerHealth.IsDefeated())
        {
            return;
        }

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleFacingByMovement()
    {
        if (moveInput == Vector2.zero)
        {
            return;
        }

        facingDirection = moveInput;
        ApplyFacingVisual();
    }

    private void ApplyFacingVisual()
    {
        Vector3 scale = transform.localScale;
        float absX = Mathf.Abs(scale.x);

        if (!invertLeftRightFacing)
        {
            if (facingDirection.x > 0.01f)
            {
                scale.x = absX;
            }
            else if (facingDirection.x < -0.01f)
            {
                scale.x = -absX;
            }
        }
        else
        {
            if (facingDirection.x > 0.01f)
            {
                scale.x = -absX;
            }
            else if (facingDirection.x < -0.01f)
            {
                scale.x = absX;
            }
        }

        transform.localScale = scale;
    }

    private void HandlePunchInput()
    {
        if (!canPunch)
        {
            return;
        }

        if (playerHealth != null && (playerHealth.IsDefeated() || playerHealth.IsHitStunned()))
        {
            return;
        }

        if (Input.GetKeyDown(leftPunchKey))
        {
            StartCoroutine(DoPunch(leftPunchHitbox, leftGloveVisual, leftPunchPoint, true));
        }
        else if (Input.GetKeyDown(rightPunchKey))
        {
            StartCoroutine(DoPunch(rightPunchHitbox, rightGloveVisual, rightPunchPoint, false));
        }
    }

    private IEnumerator DoPunch(PunchHitbox punchHitbox, Transform gloveVisual, Transform punchPoint, bool isLeftPunch)
    {
        canPunch = false;

        if (punchHitbox != null)
        {
            punchHitbox.canHit = true;
        }

        if (gloveVisual != null && punchPoint != null)
        {
            yield return StartCoroutine(AnimatePunchToPoint(gloveVisual, punchPoint));
        }
        else
        {
            yield return new WaitForSeconds(punchMoveDuration * 2f);
        }

        if (punchHitbox != null)
        {
            punchHitbox.canHit = false;
        }

        yield return new WaitForSeconds(punchCooldown);

        canPunch = true;
    }

    private IEnumerator AnimatePunchToPoint(Transform gloveVisual, Transform punchPoint)
    {
        Vector3 startLocalPos = gloveVisual.localPosition;
        Vector3 targetWorldPos = punchPoint.position;
        Vector3 targetLocalPos = gloveVisual.parent.InverseTransformPoint(targetWorldPos);

        float timer = 0f;

        while (timer < punchMoveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / punchMoveDuration;
            gloveVisual.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, t);
            yield return null;
        }

        gloveVisual.localPosition = targetLocalPos;

        timer = 0f;

        while (timer < punchMoveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / punchMoveDuration;
            gloveVisual.localPosition = Vector3.Lerp(targetLocalPos, startLocalPos, t);
            yield return null;
        }

        gloveVisual.localPosition = startLocalPos;
    }
}
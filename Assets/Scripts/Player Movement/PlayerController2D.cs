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

    [Header("Punch Timing")]
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

    void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        playerHealth = GetComponent<PlayerHealth>();

        // 设置初始面向
        facingDirection = initialFacing.normalized;
        ApplyFacingVisual();
    }

    void Update()
    {
        HandleInput();
        HandleFacingByMovement();
        HandlePunchInput();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleInput()
    {
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(moveLeft)) x -= 1f;
        if (Input.GetKey(moveRight)) x += 1f;
        if (Input.GetKey(moveUp)) y += 1f;
        if (Input.GetKey(moveDown)) y -= 1f;

        moveInput = new Vector2(x, y).normalized;
    }

    void HandleMovement()
    {
        if (playerHealth != null && playerHealth.IsDefeated())
        {
            return;
        }

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    void HandleFacingByMovement()
    {
        if (moveInput == Vector2.zero)
        {
            return;
        }

        facingDirection = moveInput;
        ApplyFacingVisual();
    }

    void ApplyFacingVisual()
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

    void HandlePunchInput()
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
            StartCoroutine(DoPunch(leftPunchHitbox));
        }
        else if (Input.GetKeyDown(rightPunchKey))
        {
            StartCoroutine(DoPunch(rightPunchHitbox));
        }
    }

    IEnumerator DoPunch(PunchHitbox punchHitbox)
    {
        canPunch = false;

        if (punchHitbox != null)
        {
            punchHitbox.canHit = true;
        }

        yield return new WaitForSeconds(punchActiveTime);

        if (punchHitbox != null)
        {
            punchHitbox.canHit = false;
        }

        yield return new WaitForSeconds(punchCooldown);

        canPunch = true;
    }
}
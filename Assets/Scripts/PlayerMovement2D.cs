
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 8f;

    [Tooltip("How much upward velocity remains when jump is released early.")]
    [Range(0.1f, 1f)]
    [SerializeField] private float jumpCutMultiplier = 0.45f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.18f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private PlayerHealth playerHealth;
    private PlayerCombat playerCombat;

    private float horizontalInput;
    private bool jumpRequested;
    private bool jumpCutRequested;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        playerCombat = GetComponent<PlayerCombat>();

        if (playerSprite == null)
            playerSprite = GetComponentInChildren<SpriteRenderer>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        isGrounded = CheckGrounded();

        if (IsPlayerDead() || IsPlayerHurt() ||
            IsDialogueActive() || IsPlayerAttacking())
        {
            ClearMovementInput();
            UpdateAnimator();
            return;
        }

        ReadInput();
        UpdateFacingDirection();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (IsPlayerDead())
        {
            StopHorizontalMovement();
            return;
        }

        // Preserve PlayerHealth knockback while hurt.
        if (IsPlayerHurt())
            return;

        if (IsDialogueActive() || IsPlayerAttacking())
        {
            StopHorizontalMovement();
            return;
        }

        Move();
        HandleJump();
        HandleJumpCut();
    }

    private bool IsPlayerDead() =>
        playerHealth != null && playerHealth.IsDead;

    private bool IsPlayerHurt() =>
        playerHealth != null && playerHealth.IsHurt;

    private bool IsPlayerAttacking() =>
        playerCombat != null && playerCombat.IsAttacking;

    private bool IsDialogueActive() =>
        DialogueUI.Instance != null &&
        DialogueUI.Instance.IsDialogueActive;

    private void ReadInput()
    {
        float keyboardInput = 0f;

        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.LeftArrow))
            keyboardInput -= 1f;

        if (Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.RightArrow))
            keyboardInput += 1f;

        float mobileInput = MobileHoldButton.HorizontalInput;

        horizontalInput = Mathf.Abs(keyboardInput) > 0.01f
            ? Mathf.Clamp(keyboardInput, -1f, 1f)
            : mobileInput;

        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow))
            jumpRequested = true;

        if (Input.GetKeyUp(KeyCode.Space) ||
            Input.GetKeyUp(KeyCode.W) ||
            Input.GetKeyUp(KeyCode.UpArrow))
            jumpCutRequested = true;
    }

    public void MobileJumpPressed()
    {
        if (IsPlayerDead() || IsPlayerHurt() ||
            IsDialogueActive() || IsPlayerAttacking())
            return;

        jumpRequested = true;
    }

    public void MobileJumpReleased()
    {
        jumpCutRequested = true;
    }

    private void ClearMovementInput()
    {
        horizontalInput = 0f;
        jumpRequested = false;
        jumpCutRequested = false;
    }

    private void Move()
    {
        if (rb == null) return;

        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void StopHorizontalMovement()
    {
        if (rb == null) return;

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        jumpRequested = false;
        jumpCutRequested = false;
    }

    private void HandleJump()
    {
        if (!jumpRequested) return;

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            isGrounded = false;
        }

        jumpRequested = false;
    }

    private void HandleJumpCut()
    {
        if (!jumpCutRequested) return;

        if (rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }

        jumpCutRequested = false;
    }

    private bool CheckGrounded()
    {
        if (groundCheck == null) return false;

        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void UpdateFacingDirection()
    {
        if (playerSprite == null) return;

        if (horizontalInput > 0.01f)
            playerSprite.flipX = false;
        else if (horizontalInput < -0.01f)
            playerSprite.flipX = true;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}

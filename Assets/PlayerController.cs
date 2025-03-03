using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxHorizontalSpeed = 8f;
    [SerializeField] private float maxVerticalSpeed = 15f;
    
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float airDragMultiplier = 0.3f;
    [SerializeField] private int maxJumps = 2;
    
    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    
    [Header("Ground Detection")]
    [SerializeField] private string groundTag = "Ground";
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private Transform groundCheck;
    
    [Header("References")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Camera mainCamera;

    private Rigidbody rb;
    private Transform cameraTransform;
    private bool isGrounded;
    private bool canJump = true;
    private int jumpsRemaining;
    private bool isDashing;
    private bool canDash = true;
    private float dashTimeRemaining;
    private Vector3 dashDirection;

    void Start()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        inputManager.OnMove.AddListener(MovePlayer);
        inputManager.OnSpacePressed.AddListener(Jump);
        inputManager.OnDashPressed.AddListener(Dash);
        rb = GetComponent<Rigidbody>();
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        cameraTransform = mainCamera.transform;
        
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.parent = transform;
            groundCheckObj.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = groundCheckObj.transform;
        }
        
        ResetJumps();
        Debug.Log("PlayerController initialized. Make sure ground objects have the tag: " + groundTag);
    }

    private void MovePlayer(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.1f || isDashing)
            return;
            
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        Vector3 moveDirection = cameraForward * direction.y + cameraRight * direction.x;
        
        // Apply less force in the air for better control
        float currentSpeed = isGrounded ? speed : speed * 0.8f;
        
        // Only apply force if we're under the speed limit
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude < maxHorizontalSpeed)
        {
            rb.AddForce(currentSpeed * moveDirection);
        }
    }
    
    private void Jump()
    {
        Debug.Log($"Jump called. isGrounded: {isGrounded}, jumpsRemaining: {jumpsRemaining}");
        
        if (jumpsRemaining > 0 && canJump)
        {
            Debug.Log("Applying jump force: " + jumpForce);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpsRemaining--;
            canJump = false;
            Invoke("ResetJump", 0.1f);
        }
    }
    
    private void Dash()
    {
        if (!canDash || isDashing) return;
        
        Vector3 dashDir = rb.linearVelocity.magnitude > 0.1f ? 
            rb.linearVelocity.normalized : transform.forward;
        dashDir.y = 0; // Keep dash horizontal
        dashDirection = dashDir.normalized;
        
        isDashing = true;
        canDash = false;
        dashTimeRemaining = dashDuration;
        
        rb.useGravity = false;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        
        Invoke("ResetDash", dashCooldown);
    }
    
    private void ResetJump()
    {
        canJump = true;
    }
    
    private void ResetDash()
    {
        canDash = true;
    }
    
    private void ResetJumps()
    {
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        CheckGrounded();
        
        if (isGrounded)
        {
            ResetJumps();
        }
    }
    
    void FixedUpdate()
    {
        ApplyAirDrag();
        ClampVelocity();
        UpdateDash();
    }
    
    private void UpdateDash()
    {
        if (isDashing)
        {
            if (dashTimeRemaining > 0)
            {
                rb.linearVelocity = dashDirection * dashForce;
                dashTimeRemaining -= Time.fixedDeltaTime;
            }
            else
            {
                isDashing = false;
                rb.useGravity = true;
            }
        }
    }
    
    private void ApplyAirDrag()
    {
        if (!isGrounded && !isDashing)
        {
            // Get only the horizontal velocity
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            
            if (horizontalVelocity.magnitude > 0.1f)
            {
                // Apply drag force opposite to the movement direction
                Vector3 dragForce = -horizontalVelocity * airDragMultiplier;
                rb.AddForce(dragForce, ForceMode.Acceleration);
            }
        }
    }
    
    private void ClampVelocity()
    {
        
        // Clamp horizontal velocity
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxHorizontalSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxHorizontalSpeed;
        }
        
        // Clamp vertical velocity
        float verticalVelocity = Mathf.Clamp(rb.linearVelocity.y, -maxVerticalSpeed, maxVerticalSpeed);
        
        // Apply the clamped velocity
        rb.linearVelocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
    }
    
    void CheckGrounded()
    {
        isGrounded = false;
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius);
        
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(groundTag) && collider.gameObject != gameObject)
            {
                isGrounded = true;
                break;
            }
        }
    }
}

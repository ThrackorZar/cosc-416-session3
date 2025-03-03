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

    void Start()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        inputManager.OnMove.AddListener(MovePlayer);
        inputManager.OnSpacePressed.AddListener(Jump);
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
        
        Debug.Log("PlayerController initialized. Make sure ground objects have the tag: " + groundTag);
    }

    private void MovePlayer(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.1f)
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
        Debug.Log("Jump called. isGrounded: " + isGrounded + ", canJump: " + canJump);
        
        if (isGrounded && canJump)
        {
            Debug.Log("Applying jump force: " + jumpForce);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canJump = false;
            Invoke("ResetJump", 0.1f);
        }
    }
    
    private void ResetJump()
    {
        canJump = true;
    }

    void Update()
    {
        CheckGrounded();
    }
    
    void FixedUpdate()
    {
        ApplyAirDrag();
        ClampVelocity();
    }
    
    private void ApplyAirDrag()
    {
        if (!isGrounded)
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

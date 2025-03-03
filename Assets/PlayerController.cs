using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private string groundTag = "Ground";
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private Transform groundCheck;

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
        
        rb.AddForce(speed * moveDirection);
    }
    
    private void Jump()
    {
        Debug.Log("Jump called. isGrounded: " + isGrounded + ", canJump: " + canJump);
        
        if (isGrounded && canJump)
        {
            Debug.Log("Applying jump force: " + jumpForce);
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
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

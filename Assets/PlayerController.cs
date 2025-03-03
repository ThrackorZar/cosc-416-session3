using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float speed = 10f;
    [SerializeField] private Camera mainCamera;

    private Rigidbody rb;
    private Transform cameraTransform;

    void Start()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        inputManager.OnMove.AddListener(MovePlayer);
        rb = GetComponent<Rigidbody>();
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        cameraTransform = mainCamera.transform;
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

    void Update()
    {
        
    }
}

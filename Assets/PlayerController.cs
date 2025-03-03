using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float speed;

    private Rigidbody rb;

    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        inputManager.OnMove.AddListener(MovePlayer);
        rb = GetComponent<Rigidbody>();
    }


    private void MovePlayer(Vector2 direction)
    {
        Vector3 moveDirection = new(0f, 0f, direction.x);
        rb.AddForce(speed * moveDirection);
    }

    void Update()
    {
        
    }
}

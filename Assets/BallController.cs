using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [SerializeField] private float force = 1f;
    [SerializeField] private Transform ballAnchor;
    [SerializeField] private Transform LaunchIndicator;

    private bool isBallLaunched;
    private Rigidbody ballRB;
    private InputManager inputManager;

    void Start()
    {
        ballRB = GetComponent<Rigidbody>();
        inputManager = FindFirstObjectByType<InputManager>();
        Cursor.lockState = CursorLockMode.Locked;
        inputManager.OnSpacePressed.AddListener(LaunchBall);
        ResetBall();
    }

    public void ResetBall()
    {
        isBallLaunched = false;
        ballRB.isKinematic = true;
        LaunchIndicator.gameObject.SetActive(true);
        transform.parent = ballAnchor;
        transform.localPosition = Vector3.zero;
    }

    private void LaunchBall()
    {
        if (isBallLaunched) return;
        
        isBallLaunched = true;
        transform.parent = null;
        ballRB.isKinematic = false;
        ballRB.AddForce(LaunchIndicator.forward * force, ForceMode.Impulse);
        
        LaunchIndicator.gameObject.SetActive(false);
    }
    
    void Update()
    {
        
    }
}

using UnityEngine;
using UnityEngine.Events;

public class CoinCollector : MonoBehaviour
{
    public UnityEvent OnCoinCollected = new();
    public float pointValue = 1f;
    private bool isCollected = false;

    [SerializeField] private float rotationSpeed = 100f;

    void Update()
    {
        // Rotate the coin
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            OnCoinCollected?.Invoke();
            
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddScore(pointValue);
            }
            
            gameObject.SetActive(false);
            Destroy(gameObject, 0.1f);
            
            Debug.Log("Coin collected!");
        }
    }
}

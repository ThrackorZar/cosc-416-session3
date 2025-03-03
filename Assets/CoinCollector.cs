using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.5f;
    [SerializeField] private int scoreValue = 1;
    
    [Header("Collection Effects")]
    [SerializeField] private bool useCollectionEffect = true;
    [SerializeField] private ParticleSystem collectionParticles;
    [SerializeField] private AudioClip collectionSound;
    
    private Vector3 startPosition;
    private float bobTime;
    private bool isCollected;
    private GameManager gameManager;
    
    void Start()
    {
        startPosition = transform.position;
        gameManager = FindFirstObjectByType<GameManager>();
        
        Debug.Log($"Coin initialized at {transform.position}");
        
        if (gameManager == null)
        {
            Debug.LogWarning("No GameManager found in the scene!");
        }
        
        Collider coinCollider = GetComponent<Collider>();
        if (coinCollider == null)
        {
            Debug.LogError("No Collider found on coin! Please add a collider.");
        }
        else if (!coinCollider.isTrigger)
        {
            Debug.LogWarning("Coin collider is not set as a trigger! Collection might not work.");
        }
        
        if (useCollectionEffect && collectionParticles == null)
        {
            Debug.LogWarning("Collection particles not assigned. Please assign a particle system in the inspector for collection effects.");
        }
    }
    
    void Update()
    {
        if (!isCollected)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
            
            bobTime += Time.deltaTime;
            float newY = startPosition.y + Mathf.Sin(bobTime * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by {other.gameObject.name} with tag {other.tag}");
        
        if (!isCollected && other.CompareTag("Player"))
        {
            Debug.Log("Valid player collision detected, collecting coin!");
            CollectCoin();
        }
        else
        {
            if (isCollected)
                Debug.Log("Coin is already collected!");
            if (!other.CompareTag("Player"))
                Debug.Log($"Object tag '{other.tag}' does not match 'Player' tag");
        }
    }
    
    private void CollectCoin()
    {
        isCollected = true;
        Debug.Log("Collecting coin...");
        
        if (gameManager != null)
        {
            gameManager.AddScore(scoreValue);
            Debug.Log($"Added {scoreValue} to score");
        }
        else
        {
            Debug.LogWarning("No GameManager found, score not updated!");
        }
        
        if (useCollectionEffect)
        {
            if (collectionParticles != null)
            {
                collectionParticles.transform.parent = null;
                collectionParticles.Play();
                Destroy(collectionParticles.gameObject, collectionParticles.main.duration);
            }
            
            if (collectionSound != null)
            {
                AudioSource.PlayClipAtPoint(collectionSound, transform.position);
            }
        }
        
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }
        else
        {
            Debug.LogWarning("No MeshRenderer found on coin!");
        }
        
        Collider coinCollider = GetComponent<Collider>();
        if (coinCollider != null)
        {
            coinCollider.enabled = false;
        }
        
        Destroy(gameObject, useCollectionEffect ? 2f : 0f);
    }
} 
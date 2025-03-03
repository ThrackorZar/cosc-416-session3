using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float score = 0;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private void Start()
    {
        inputManager.OnResetPressed.AddListener(HandleReset);
        UpdateScoreDisplay();
    }

    private void HandleReset()
    {
        score = 0;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = $"Score: {score}";
    }

    void Update()
    {
        
    }

    public float GetScore()
    {
        return score;
    }

    public void AddScore(float points)
    {
        score += points;
        UpdateScoreDisplay();
    }
}

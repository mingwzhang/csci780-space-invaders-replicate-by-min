using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    private int playerHealth = 3;
    //private float deathPauseDuration = 1.5f;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject healthUISpawnPoint;
    private float healthUIPosGap = 50.0f;


    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private GameObject gameOverText;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    
    private int score = 0;
    private int highScore = 0;

    private bool isGameOver = false;

    // Set up UI, score, high score, and player health icons
    void Start()
    {
        playerHealthText.text = playerHealth.ToString();
        gameOverText.SetActive(false);

        highScore = PlayerPrefs.GetInt("HighScore", 0);

        scoreText.text = score.ToString("D4");
        highScoreText.text = highScore.ToString("D4");

        SpawnHealthUI();

    }

    // Allow the player to restart after game over
    private void Update()
    {
        if (isGameOver && gameOverText.activeSelf && Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartGame();
        }
    }

    // Reload the current scene
    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Create the health icons based on current player health
    public void SpawnHealthUI()
    {
        for (int i = 0; i < playerHealth; i++)
        {
            // Spawn health UI, position based on spawn point
            GameObject newHealthUI = Instantiate(playerUIPrefab, healthUISpawnPoint.transform);
            newHealthUI.transform.localPosition = new Vector3(i * healthUIPosGap, 0, 0);
            //Debug.Log(i * healthUIPosGap);
        }
    }

    // Remove one health icon
    public void RemoveHealthUI()
    {
        if (healthUISpawnPoint.transform.childCount > 0)
        {
            Destroy(healthUISpawnPoint.transform.GetChild(healthUISpawnPoint.transform.childCount - 1).gameObject);
        }
    }

    // Remove all health icons
    public void RemoveAllHealthUI()
    {
        foreach (Transform child in healthUISpawnPoint.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Reduce player health and respawn or start game over
    public void LoseHealth(PlayerController player, Vector3 respawnPosition)
    {
        if (isGameOver || playerHealth <= 0)
        {
            return;
        }

        playerHealth--;
        playerHealthText.text = playerHealth.ToString();
        RemoveHealthUI();

        if (playerHealth <= 0)
        {
            BeginGameOver(player);
            return;
        }

        StartCoroutine(RespawnPlayer(player, respawnPosition));
    }

    // Start game over when something other than health loss causes it
    public void GameOver()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        BeginGameOver(player);
    }

    // Play player death, wait, then respawn the player
    private IEnumerator RespawnPlayer(PlayerController player, Vector3 respawnPosition)
    {
        Time.timeScale = 0f;

        yield return player.DestroyPlayer();

        // Extra pause after the destruction animation finishes.
        yield return new WaitForSecondsRealtime(1.5f);

        Instantiate(playerPrefab, respawnPosition, Quaternion.identity);

        Time.timeScale = 1f;
    }

    // Set the game into game over state
    private void BeginGameOver(PlayerController player)
    {
        if (isGameOver) return;

        isGameOver = true;
        StopAllCoroutines();  

        playerHealth = 0;
        playerHealthText.text = playerHealth.ToString();
        RemoveAllHealthUI();

        Time.timeScale = 0f;

        StartCoroutine(GameOverSequence(player));
    }

    // Play the final player death animation, then show game over text
    private IEnumerator GameOverSequence(PlayerController player)
    {
        if (player != null)
        {
            yield return player.DestroyPlayer();
        }

        gameOverText.SetActive(true);
    }

    // Add score and update the saved high score
    public void AddScore(int amount)
    {
        score += amount;

        scoreText.text = score.ToString("D4");

        if (score > highScore)
        {
            highScore = score;
            highScoreText.text = highScore.ToString("D4");

            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }
}
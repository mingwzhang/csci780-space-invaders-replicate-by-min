using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

// Controls overall game state, player health, score, high score, game over, and scene restarts

public class GameManager : MonoBehaviour
{
    private static int playerHealth = 3;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject healthUISpawnPoint;
    private float healthUIPosGap = 50.0f;

    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private GameObject gameOverText;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;

    private int enemyCount = 55;
    private static int score = 0;
    private static bool addHPBonus = false;

    private int highScore = 0;
    private bool isGameOver = false;
    private bool isRestarting = false;
 
    private AudioManager audioManager;

    public enum LevelType
    {
        Standard,
        Standard_Boss,

        Limitless,
    }
    [Header("Level Settings")]
    [SerializeField] private LevelType levelType = LevelType.Standard;

    public LevelType CurrentLevelType => levelType;

    [Header("Boss Settings")]
    [SerializeField] private string bossSceneName = "StandardBoss";

    [SerializeField] private int bossMaxHealth = 20;
    private int bossHealth;


    private void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    // Set up UI, score, high score, and player health icons
    void Start()
    {
        // Reset the saved high score for a fresh game release (Comment out or remove these 2 lines after resetting for release)
        //PlayerPrefs.DeleteKey("HighScore");
        //PlayerPrefs.Save();

        enemyCount = 55;

        playerHealthText.text = playerHealth.ToString();
        gameOverText.SetActive(false);

        highScore = PlayerPrefs.GetInt("HighScore", 0);

        scoreText.text = score.ToString("D4");
        highScoreText.text = highScore.ToString("D4");

        SpawnHealthUI();
        bossHealth = bossMaxHealth;
    }

    // Allow the player to restart after game over
    private void Update()
    {
        if (isGameOver && gameOverText.activeSelf && Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartGame();
        }

        if (Keyboard.current.pKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Time.timeScale == 1f)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }

    public int GetEnemyCount()
    {
        return enemyCount;
    }

    // To trigger win condition
    // Track destroyed enemies for Standard and Limitless levels
    public void EnemyDestroyed()
    {
        if (levelType == LevelType.Standard_Boss || isRestarting || isGameOver)
            return;

        enemyCount--;

        if (enemyCount <= 0)
        {
            isRestarting = true;
            StartCoroutine(RestartAfterClear());
        }
    }

    // Reduce boss health and trigger win condition
    public void DamageBoss(int damage)
    {
        if (levelType != LevelType.Standard_Boss || isRestarting || isGameOver || damage <= 0)
            return;

        bossHealth = Mathf.Max(0, bossHealth - damage);

        if (bossHealth <= 0)
        {
            isRestarting = true;
            StartCoroutine(RestartAfterClear());
        }
    }

    // Reload the current scene
    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Handle level completion based on level type
    private IEnumerator RestartAfterClear()
    {
        yield return new WaitForSecondsRealtime(2f);

        Time.timeScale = 1f;

        switch (levelType)
        {
            case LevelType.Standard:
                SceneManager.LoadScene(bossSceneName);
                break;

            case LevelType.Standard_Boss:
                Debug.Log("Boss Defeated! Level Complete!");
                break;

            case LevelType.Limitless:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
        }
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

    // Add one health icon
    public void AddHealthUI()
    {
        playerHealthText.text = playerHealth.ToString();

        GameObject newHealthUI = Instantiate(playerUIPrefab, healthUISpawnPoint.transform);

        // NOTE: This does NOT decrease player health. It only borrows the value 
        // Index positions start at 0, so the Nth life icon sits at position (N - 1)
        newHealthUI.transform.localPosition = new Vector3((playerHealth - 1) * healthUIPosGap, 0, 0);
    }

    // Remove one health icon
    public void RemoveHealthUI()
    {
        playerHealthText.text = playerHealth.ToString();
        if (healthUISpawnPoint.transform.childCount > 0)
        {
            Destroy(healthUISpawnPoint.transform.GetChild(healthUISpawnPoint.transform.childCount - 1).gameObject);
        }
    }

    // Remove all health icons
    public void RemoveAllHealthUI()
    {
        playerHealthText.text = playerHealth.ToString();
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
        RemoveAllHealthUI();

        ResetData();
        scoreText.text = score.ToString("D4");

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

        if (!addHPBonus && score >= 1500)
        {
            addHPBonus = true;
            playerHealth++;
            AddHealthUI();

        }
    }

    public static void ResetData()
    {
        score = 0;
        playerHealth = 3;
        addHPBonus = false;
    }
}
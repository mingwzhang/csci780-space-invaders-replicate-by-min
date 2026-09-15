using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    private int playerHealth = 5;
    //private float deathPauseDuration = 1.5f;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject healthUISpawnPoint;
    private float healthUIPosGap = 2.5f;


    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private GameObject gameOverText;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    
    private int score = 0;
    private int highScore = 0;

    private bool isGameOver = false;

    void Start()
    {
        playerHealthText.text = playerHealth.ToString();
        gameOverText.SetActive(false);

        highScore = PlayerPrefs.GetInt("HighScore", 0);

        scoreText.text = score.ToString("D4");
        highScoreText.text = highScore.ToString("D4");

        SpawnHealthUI();

    }

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

    public void RemoveHealthUI()
    {
        if (healthUISpawnPoint.transform.childCount > 0)
        {
            Destroy(healthUISpawnPoint.transform.GetChild(healthUISpawnPoint.transform.childCount - 1).gameObject);
        }
    }

    public void RemoveAllHealthUI()
    {
        foreach (Transform child in healthUISpawnPoint.transform)
        {
            Destroy(child.gameObject);
        }
    }

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

    public void GameOver()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        BeginGameOver(player);
    }

    private IEnumerator RespawnPlayer(PlayerController player, Vector3 respawnPosition)
    {
        Time.timeScale = 0f;

        yield return player.DestroyPlayer();

        // Extra pause after the destruction animation finishes.
        yield return new WaitForSecondsRealtime(1.5f);

        Instantiate(playerPrefab, respawnPosition, Quaternion.identity);

        Time.timeScale = 1f;
    }

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

    private IEnumerator GameOverSequence(PlayerController player)
    {
        if (player != null)
        {
            yield return player.DestroyPlayer();
        }

        gameOverText.SetActive(true);
    }

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
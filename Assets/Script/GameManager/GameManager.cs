using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    private int playerHealth = 5;
    private float deathPauseDuration = 1.5f;

    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject playerPrefab;

    private int score = 0;
    private int highScore = 0;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;

    void Start()
    {
        playerHealthText.text = playerHealth.ToString();
        gameOverText.SetActive(false);

        highScore = PlayerPrefs.GetInt("HighScore", 0);

        scoreText.text = score.ToString("D4");
        highScoreText.text = highScore.ToString("D4");

    }

    public void LoseHealth(Vector3 respawnPosition)
    {
        if (playerHealth <= 0)
        {
            return;
        }

        playerHealth--;

        playerHealthText.text = playerHealth.ToString();

        if (playerHealth <= 0)
        {
            Debug.Log("Game Over");
            gameOverText.SetActive(true);
            return;
        }

        StartCoroutine(RespawnPlayer(respawnPosition));
    }

    private IEnumerator RespawnPlayer(Vector3 respawnPosition)
    {
        Time.timeScale = 0f; // PAUSE GAME

        yield return new WaitForSecondsRealtime(deathPauseDuration);

        Instantiate(playerPrefab, respawnPosition, Quaternion.identity);

        Time.timeScale = 1; // RESUME GAME
    }

    public void GameOver()
    {
        Debug.Log("Game Over");

        StopAllCoroutines();


        playerHealth = 0;
        playerHealthText.text = playerHealth.ToString();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Destroy(player);
        }

        gameOverText.SetActive(true);
        Time.timeScale = 0f;
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
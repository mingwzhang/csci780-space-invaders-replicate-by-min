using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private int playerHealth = 5;
    private float deathPauseDuration = 1.5f;

    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private TMP_Text gameOverText;

    [SerializeField] private GameObject playerPrefab;

    void Start()
    {
        playerHealthText.text = playerHealth.ToString();
        gameOverText.enabled = false;
    }

    public void LoseHealth(Vector3 respawnPosition)
    {
        if (playerHealth <= 0)
        {
            return;
        }

        playerHealth--;

        playerHealthText.text = playerHealth.ToString();

        if (playerHealth == 0)
        {
            Debug.Log("Game Over");
            gameOverText.enabled = true;
            return;
        }

        StartCoroutine(RespawnPlayer(respawnPosition));
    }

    private IEnumerator RespawnPlayer(Vector3 respawnPosition)
    {
        Time.timeScale = 0f; // PAUSE GAME

        yield return new WaitForSecondsRealtime(1f);

        Instantiate(playerPrefab, respawnPosition, Quaternion.identity);

        Time.timeScale = deathPauseDuration; // RESUME GAME
    }
}
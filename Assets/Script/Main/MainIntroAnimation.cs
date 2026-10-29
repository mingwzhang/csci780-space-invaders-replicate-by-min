using System.Collections;
using UnityEngine;

// Handles alien spawning and the additional boss introduction animation.

public class MainIntroAnimation : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Intro References")]
    [SerializeField] private Transform enemyGroup;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform bossPosition;

    [Header("Intro Settings")]
    [SerializeField] private float enemySpawnDelay = 0.02f;
    [SerializeField] private float enemyMoveDuration = 1.5f;
    [SerializeField] private float bossScaleDuration = 1f;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        Time.timeScale = 0f;
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        // Disable all aliens
        for (int row = 0; row < enemyGroup.childCount; row++)
        {
            Transform alienRow = enemyGroup.GetChild(row);

            for (int alien = 0; alien < alienRow.childCount; alien++)
            {
                alienRow.GetChild(alien).gameObject.SetActive(false);
            }
        }

        // Disable player
        player.SetActive(false);

        // Disable boss initially
        if (gameManager.CurrentLevelType == GameManager.LevelType.Standard_Boss && boss != null)
        {
            boss.SetActive(false);
        }

        // Activate aliens one by one
        for (int row = 0; row < enemyGroup.childCount; row++)
        {
            Transform alienRow = enemyGroup.GetChild(row);

            for (int alien = 0; alien < alienRow.childCount; alien++)
            {
                alienRow.GetChild(alien).gameObject.SetActive(true);

                yield return new WaitForSecondsRealtime(enemySpawnDelay);
            }
        }

        // Additional boss introduction
        if (gameManager.CurrentLevelType == GameManager.LevelType.Standard_Boss)
        {
            yield return StartCoroutine(PlayBossIntro());
        }

        // Activate player last
        player.SetActive(true);

        // Start the actual game
        Time.timeScale = 1f;

        Debug.Log("Intro Finished! Time Scale: " + Time.timeScale);

        // Disable intro object
        gameObject.SetActive(false);
    }

    private IEnumerator PlayBossIntro()
    {
        if (boss == null || bossPosition == null)
        {
            Debug.LogWarning("Boss or Boss Position is missing.", this);
            yield break;
        }

        // Collect all individual aliens and their starting world positions
        int totalAliens = 0;

        foreach (Transform alienRow in enemyGroup)
        {
            totalAliens += alienRow.childCount;
        }

        Transform[] aliens = new Transform[totalAliens];
        Vector3[] startPositions = new Vector3[totalAliens];

        int index = 0;

        foreach (Transform alienRow in enemyGroup)
        {
            foreach (Transform alien in alienRow)
            {
                aliens[index] = alien;
                startPositions[index] = alien.position;
                index++;
            }
        }

        // Move every alien individually toward the boss position
        Vector3 targetPosition = bossPosition.position;
        float elapsed = 0f;

        while (elapsed < enemyMoveDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / enemyMoveDuration);

            for (int i = 0; i < aliens.Length; i++)
            {
                aliens[i].position = Vector3.Lerp(startPositions[i], targetPosition, t);
            }

            yield return null;
        }

        // Ensure all aliens reach the exact boss position
        for (int i = 0; i < aliens.Length; i++)
        {
            aliens[i].position = targetPosition;
        }

        // Hide aliens after convergence
        enemyGroup.gameObject.SetActive(false);

        // Activate boss and scale from 0 to its original size
        Vector3 originalScale = boss.transform.localScale;

        boss.transform.localScale = Vector3.zero;
        boss.SetActive(true);

        elapsed = 0f;

        while (elapsed < bossScaleDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / bossScaleDuration);

            boss.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);

            yield return null;
        }

        boss.transform.localScale = originalScale;
    }
}
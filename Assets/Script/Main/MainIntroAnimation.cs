using System.Collections;
using UnityEngine;

public class MainIntroAnimation : MonoBehaviour
{
    [SerializeField] private Transform enemyGroup;
    [SerializeField] private GameObject player;
    private float enemySpawnDelay = 0.02f;

    private void Start()
    {
        // Pauses the game but StartCoroutine() itself is not affected by Time.timeScale
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

        // Activate player last
        player.SetActive(true);

        // Start the actual game
        Time.timeScale = 1f;

        // Disables the IntroAnimation GameObject itself
        gameObject.SetActive(false);
    }
}
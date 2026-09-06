using System.Collections.Generic;
using UnityEngine;

public class AlienGroup : MonoBehaviour
{
    private float moveDistance = 0.2f;
    private float moveIntervalTimer = 0.5f;
    private float leftBorder = -11f;
    private float rightBorder = 11f;
    private float dropDistance = 0.25f;

    private int direction = 1;
    private float timer;
    private bool reachedBorder = false;

    [SerializeField] private float minShootInterval = 0.3f;
    [SerializeField] private float maxShootInterval = 1.5f;

    private float shootInterval;
    private float shootTimer;

    void Start()
    {
        shootInterval = Random.Range(minShootInterval, maxShootInterval);
    }

    void Update()
    {
        // Shooting timer
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval)
        {
            shootTimer = 0f;
            RandomAlienShoot();

            // Pick a new delay for the next shot
            shootInterval = Random.Range(minShootInterval, maxShootInterval);
        }

        // Movement timer
        timer += Time.deltaTime;

        if (timer < moveIntervalTimer)
        {
            return;
        }

        timer = 0f;

        // Drop and reverse after reaching a border
        if (reachedBorder)
        {
            transform.Translate(0f, -dropDistance, 0f, Space.World);
            direction *= -1;
            reachedBorder = false;
            return;
        }

        float nextMove = direction * moveDistance;

        foreach (Transform row in transform)
        {
            foreach (Transform alien in row)
            {
                float nextPosition = alien.position.x + nextMove;

                if (direction == 1 && nextPosition >= rightBorder)
                {
                    nextMove = rightBorder - alien.position.x;
                    reachedBorder = true;
                }

                if (direction == -1 && nextPosition <= leftBorder)
                {
                    nextMove = leftBorder - alien.position.x;
                    reachedBorder = true;
                }
            }
        }

        transform.Translate(nextMove, 0f, 0f, Space.World);
    }

    private void RandomAlienShoot()
    {
        // Find the Alien scripts inside the group, including inside rows
        Alien[] aliens = GetComponentsInChildren<Alien>();

        List<Alien> bottomAliens = new List<Alien>();

        foreach (Alien alien in aliens)
        {
            bool isBottom = true;

            // Check whether another alien is below this one
            foreach (Alien other in aliens)
            {
                bool sameColumn = Mathf.Abs(other.transform.position.x - alien.transform.position.x) < 0.1f;

                bool below = other.transform.position.y < alien.transform.position.y;

                if (sameColumn && below)
                {
                    isBottom = false;
                    break;
                }
            }

            if (isBottom)
            {
                bottomAliens.Add(alien);
            }
        }

        // No aliens remain
        if (bottomAliens.Count == 0)
        {
            return;
        }

        // Choose one bottom alien and tell it to shoot
        int randomIndex = Random.Range(0, bottomAliens.Count);
        bottomAliens[randomIndex].Shoot();
    }
}
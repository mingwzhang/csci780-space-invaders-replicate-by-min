using System.Collections.Generic;
using UnityEngine;

public class AlienGroup : MonoBehaviour
{
    private float moveDistance = 0.2f;
    private float moveIntervalTimer = 0.5f;
    private float leftBorder = -10f;
    private float rightBorder = 10f;
    private float dropDistance = 0.25f;

    private int direction = 1;
    private float timer;

    private float minShootInterval = 0.3f;
    private float maxShootInterval = 1.5f;

    private float shootInterval;
    private float shootTimer;

    private float timeDeductionPerKill = 0.0095f;
    private float minimumInterval = 0.01f;
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

        float nextMove = direction * moveDistance;

        foreach (Transform row in transform)
        {
            foreach (Transform alien in row)
            {
                float nextPosition = alien.position.x + nextMove;

                if (direction == 1 && nextPosition >= rightBorder)
                {
                    // Move down and reverse direction
                    transform.Translate(0f, -dropDistance, 0f, Space.World);
                    direction = -1;
                    // Immediate change horizontal movement for this update
                    nextMove = -moveDistance;
                }

                else if(direction == -1 && nextPosition <= leftBorder)
                {
                    // Move down and reverse direction
                    transform.Translate(0f, -dropDistance, 0f, Space.World);
                    direction = 1;
                    nextMove = moveDistance;
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

    public void AlienDestroyed()
    {
        // Deduct time from the interval to make them step faster
        moveIntervalTimer -= timeDeductionPerKill;

        // Ensure it doesn't drop past the absolute maximum speed threshold
        if (moveIntervalTimer < minimumInterval)
        {
            moveIntervalTimer = minimumInterval;
        }
       // Debug.Log(moveIntervalTimer);

    }
}



using System.Collections.Generic;
using UnityEngine;

// Controls alien group movement, shooting, speed progression, and game-over detection

public class AlienGroup : MonoBehaviour
{
    private float moveDistance = 0.2f;
    private float startingInterval = 1.0f;
    private float moveIntervalTimer = 1.0f;
    private float minimumInterval = 0.007f;
    private float timeDeductionPerKill;
    private float movementSoundTimer;
    private float minimumSoundInterval = 0.05f;


    private float leftBorder = -9.5f;
    private float rightBorder = 9.5f;
    private float dropDistance = 0.25f;

    private int direction = 1;
    private float timer;

    private float minShootInterval = 0.2f;
    private float maxShootInterval = 1.0f;

    private float shootInterval;
    private float shootTimer;

    [SerializeField] private GameManager gameManager;
    private float gameOverY = 4.0f;
    private int bulletAnimationNumber = 1;
    private int movementSoundNumber = 1;
    private AudioManager audioManager;


    void Awake()
    {
        audioManager = audioManager = FindFirstObjectByType<AudioManager>();
    }


    void Start()
    {
        shootInterval = Random.Range(minShootInterval, maxShootInterval);

        // Calculate how much to reduce the movement interval for each destroyed alien
        timeDeductionPerKill = (startingInterval - minimumInterval) / gameManager.GetEnemyCount() * 1.015f;
    }

    void Update()
    {
        CheckGameOver();


        // Shooting timer
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval)
        {
            shootTimer = 0f;
            RandomAlienShoot();

            // Pick a new delay for the next shot
            shootInterval = Random.Range(minShootInterval, maxShootInterval);
        }

        MoveGroup();
    }

    private void MoveGroup()
    {
        if (gameManager.GetEnemyCount() <= 0)
        {
            return;
        }

        // Movement timer
        timer += Time.deltaTime;
        // Movement sound timer
        movementSoundTimer += Time.deltaTime;

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

                else if (direction == -1 && nextPosition <= leftBorder)
                {
                    // Move down and reverse direction
                    transform.Translate(0f, -dropDistance, 0f, Space.World);
                    direction = 1;
                    nextMove = moveDistance;
                }
            }
        }

        transform.Translate(nextMove, 0f, 0f, Space.World);

        // Play movement sound, but prevent it from playing too rapidly
        if (movementSoundTimer >= minimumSoundInterval)
        {
            if (movementSoundNumber == 1)
            {
                audioManager.PlayFastInvader1();
            }
            else if (movementSoundNumber == 2)
            {
                audioManager.PlayFastInvader2();
            }
            else if (movementSoundNumber == 3)
            {
                audioManager.PlayFastInvader3();
            }
            else
            {
                audioManager.PlayFastInvader4();
            }

            movementSoundNumber++;

            if (movementSoundNumber > 4)
            {
                movementSoundNumber = 1;
            }

            movementSoundTimer = 0f;
        }
    }



    private void UpdateAnimationSpeed(Transform destroyedAlien)
    {
        float animationSpeed = 1f / moveIntervalTimer;

        foreach (Transform row in transform)
        {
            foreach (Transform alien in row)
            {
                if (alien == destroyedAlien)
                {
                    continue;
                }

                Animator animator = alien.GetComponentInChildren<Animator>();
                animator.speed = animationSpeed;
            }
        }
    }


    // Speeds up the group for each alien destroyed
    public void AlienDestroyedSpeedUp(Transform alien)
    {
        // Detach the destroyed alien so it no longer moves with the group
        alien.SetParent(null, true);

        // Deduct time from the interval to make them step faster
        moveIntervalTimer -= timeDeductionPerKill;

        // Ensure the movement interval doesn't go below the minimum
        if (moveIntervalTimer < minimumInterval)
        {
            moveIntervalTimer = minimumInterval;
        }

        UpdateAnimationSpeed(alien);
    }


    private void RandomAlienShoot()
    {
        // The array is rebuilt every time this method runs, searches this GameObject and all of its children for Alien components
        Alien[] aliens = GetComponentsInChildren<Alien>();

        // Temporary list rebuilt from scratch every time this method runs
        List<Alien> bottomAliens = new List<Alien>();

        // Check each alien to see if another alien is below it
        foreach (Alien alien in aliens)
        {
            bool isBottom = true;

            // Compare this alien with every other alien
            foreach (Alien other in aliens)
            {
                // Check if both aliens are roughly in the same column
                bool sameColumn = Mathf.Abs(other.transform.position.x - alien.transform.position.x) < 0.1f;

                // Check if the other alien is below this alien
                bool below = other.transform.position.y < alien.transform.position.y;

                // If another alien is below it in the same column, this alien is not the bottom alien
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

        // Choose one bottom alien
        int randomIndex = Random.Range(0, bottomAliens.Count);

        // Fire using the current animation
        bottomAliens[randomIndex].Shoot(bulletAnimationNumber);

        // Move to the next animation: 1 -> 2 -> 3 -> 1
        bulletAnimationNumber++;

        if (bulletAnimationNumber > 3)
        {
            bulletAnimationNumber = 1;
        }
    }

    private void CheckGameOver()
    {
        foreach (Transform row in transform)
        {
            foreach (Transform alien in row)
            {
                if (alien.position.y <= gameOverY)
                {
                    gameManager.GameOver();
                    return;
                }
            }
        }
    }
}



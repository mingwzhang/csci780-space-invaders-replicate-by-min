using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private float speed = 5f;
    private float leftBorder = -9f;
    private float rightBorder = 9f;

    [SerializeField] private GameObject playerBullet;

    private float fireRate = 0.4f;  // Minimum number of seconds between shots
    private float nextFireTime;
    private bool isDying = false;


    // Time.deltaTime = the time since the previous frame, making movement frame-rate independent
    // Time.time = the total time since the game started

    // Assign the Animator from the player's child.
    private Animator childAnimator;


    void Awake()
    {
        if (childAnimator == null) childAnimator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        
    }

    void Update()
    {
        float move = 0;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            move = -1;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            move = 1;


        transform.Translate(move * speed * Time.deltaTime, 0, 0);

        if (transform.position.x < leftBorder)
            transform.position = new Vector2(leftBorder, transform.position.y);

        if (transform.position.x > rightBorder)
            transform.position = new Vector2(rightBorder, transform.position.y);

        if (Keyboard.current.spaceKey.isPressed && Time.time >= nextFireTime)
        {
            Instantiate(playerBullet, transform.position, transform.rotation);

            // Allow the next shot after fireRate seconds have passed
            nextFireTime = Time.time + fireRate;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDying) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") || collision.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
        {
            isDying = true;

            Vector3 respawnPosition = transform.position;

            // Destroy the enemy or enemy bullet only.
            Destroy(collision.gameObject);

            FindFirstObjectByType<GameManager>().LoseHealth(this, respawnPosition);
        }
    }

    public IEnumerator DestroyPlayer()
    {
        isDying = true;

        // Force the animator to run even if the game is paused (Time.timeScale = 0)
        childAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        childAnimator.Play("player_destroyed");

        yield return null;

        // Read the exact length of the animation currently playing on the child
        float clipLength = childAnimator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSecondsRealtime(clipLength);

        Destroy(gameObject);
    }
}
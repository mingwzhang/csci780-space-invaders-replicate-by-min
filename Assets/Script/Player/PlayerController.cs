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

    private bool isSpecialAttacking = false;

    private bool isDying = false;

    private AudioManager audioManager;

    private CameraShake cameraShake;

    // Time.deltaTime = the time since the previous frame, making movement frame-rate independent
    // Time.time = the total time since the game started

    // Assign the Animator from the player's child.
    [SerializeField] private Animator childAnimator;
    [SerializeField] private Animator childSpecialAnimator;

    void Awake()
    {
        if (childAnimator == null || childSpecialAnimator == null)
        {
            Debug.LogError("Drag Child Animator 1 and 2 into their Inspector slots on the Player script", this);
        }

        audioManager = FindFirstObjectByType<AudioManager>();
        cameraShake = FindFirstObjectByType<CameraShake>();

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

        // Regular attack (Z / Space)
        if ((Keyboard.current.spaceKey.isPressed || Keyboard.current.zKey.isPressed) && Time.time >= nextFireTime)
        {
            Instantiate(playerBullet, transform.position, transform.rotation);

            // Allow the next shot after fireRate seconds have passed
            nextFireTime = Time.time + fireRate;
        }

        // Special attack (X Key)
        if (Keyboard.current.xKey.isPressed)
        {
            SpecialAttack();
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

    void SpecialAttack()
    {
        if (isSpecialAttacking) return;

        StartCoroutine(SpecialAttackSequence());
    }

    private IEnumerator SpecialAttackSequence()
    {
        isSpecialAttacking = true;

        childAnimator.Play("player_default", 0, 0f);

        // Play the special attack initiation animation
        childSpecialAnimator.Play("player_special_atk_initiate", 0, 0f);

        audioManager.PlayLaserLoop();
        cameraShake.StartShake();

        // Wait until the initiation animation finishes
        yield return null;

        float initiateLength = childSpecialAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(initiateLength);

        // Play the looping special attack animation
        childSpecialAnimator.Play("player_special_atk_loop", 0, 0f);

        // Keep the loop animation playing for 2 seconds
        yield return new WaitForSeconds(2f);

        // Play the ending animation
        childSpecialAnimator.Play("player_special_atk_done", 0, 0f);
        audioManager.PlayLaserEnding();
        cameraShake.StopShake();

        // Wait until the ending animation finishes
        yield return null;

        float doneLength = childSpecialAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(doneLength);

        // Return to default
        childSpecialAnimator.Play("default", 0, 0f);

        isSpecialAttacking = false;
    }


    public IEnumerator DestroyPlayer()
    {
        isDying = true;

        // Force the animator to run even if the game is paused (Time.timeScale = 0)
        childAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        childSpecialAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        childAnimator.Play("player_destroyed", 0, 0f);
        childSpecialAnimator.Play("default", 0, 0f);

        yield return null;

        // Read the exact length of the animation currently playing on the child
        float clipLength = childAnimator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSecondsRealtime(clipLength);

        Destroy(gameObject);
    }


}
using UnityEngine;
using System.Collections;

public class Alien : MonoBehaviour
{
    [SerializeField] private GameObject alienBullet;
    [SerializeField] private int scoreValue;

    private GameManager gameManager;

    private Animator childAnimator;
    private Collider2D alienCollider;
    private bool isDestroyed = false;

    void Awake()
    {
        if (childAnimator == null) childAnimator = GetComponentInChildren<Animator>();

        alienCollider = GetComponent<Collider2D>();
    }


    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (childAnimator == null)   
            Debug.LogError("No Animator found on any child GameObject!", this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(int animationIndex)
    {
        GameObject newBullet = Instantiate(alienBullet, transform.position, Quaternion.identity);

        AlienBullet bulletScript = newBullet.GetComponent<AlienBullet>();
        bulletScript.SetAnimation(animationIndex);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            // Instant guard to prevent multiple bullets from registering the same enemy
            if (isDestroyed)
            {
                return;
            }

            // Mark as destroyed immediately before another collision can occur
            isDestroyed = true;

            alienCollider.enabled = false;

            AlienGroup group = GetComponentInParent<AlienGroup>();

            if (group != null)
            {
                // For each alien destroyed, speed up the group
                group.AlienDestroyedSpeedUp(transform);
            }

            gameManager.AddScore(scoreValue);
            StartCoroutine(DestroyAlien());
        }
    }

    private IEnumerator DestroyAlien()
    {
        childAnimator.speed = 1f;
        childAnimator.Play("alien_destroyed");

        // Wait 1 next frame so the animator can process the play request. Without this, the line below reads 0 seconds because the state hasn't shifted yet.
        yield return null;

        // Read the exact length of the animation currently playing on the child
        float clipLength = childAnimator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSecondsRealtime(clipLength);

        gameManager.EnemyDestroyed();
        Destroy(gameObject);
    }
}

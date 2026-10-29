
using System.Collections;
using UnityEngine;

// Controls boss main body movement, damage detection, and hit effects.

public class BossBase : MonoBehaviour
{
    [Header("References")]
    private GameManager gameManager;
    [SerializeField] private SpriteRenderer bodySprite;

    [Header("Movement Settings")]
    [SerializeField] private float moveDistance = 0.2f;
    [SerializeField] private float moveInterval = 1f;
    [SerializeField] private float leftBorder = -7f;
    [SerializeField] private float rightBorder = 7f;

    [Header("Hit Effect")]
    [SerializeField] private float hitFlashDuration = 0.15f;

    private int direction = 1;
    private float timer;
    private Color originalColor;
    private Coroutine hitFlashCoroutine;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        originalColor = bodySprite.color;
    }
    
    private void Update()
    {
        MoveBoss();
    }

    // Move boss horizontally without descending
    private void MoveBoss()
    {
        timer += Time.deltaTime;

        if (timer < moveInterval) return;

        timer = 0f;

        float nextMove = direction * moveDistance;
        float nextPosition = transform.position.x + nextMove;

        if (direction == 1 && nextPosition >= rightBorder)
        {
            direction = -1;
            nextMove = -moveDistance;
        }
        else if (direction == -1 && nextPosition <= leftBorder)
        {
            direction = 1;
            nextMove = moveDistance;
        }

        transform.Translate(nextMove, 0f, 0f, Space.World);
    }

    // Detect player projectile and laser hits
    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;

        if (layer == LayerMask.NameToLayer("PlayerLaser"))
        {
            TakeDamage(1);
        }
    }

    // Main boss health is managed by GameManager
    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;

        gameManager.DamageBoss(damage);

        if (hitFlashCoroutine != null)
            StopCoroutine(hitFlashCoroutine);

        hitFlashCoroutine = StartCoroutine(HitFlash());

    }

    private IEnumerator HitFlash()
    {
        bodySprite.color = Color.red;

        yield return new WaitForSeconds(hitFlashDuration);

        bodySprite.color = originalColor;
        hitFlashCoroutine = null;
    }

    private void OnDisable()
    {
        if (hitFlashCoroutine != null)
            StopCoroutine(hitFlashCoroutine);

        hitFlashCoroutine = null;

        if (bodySprite != null)
            bodySprite.color = originalColor;
    }
}


using System.Collections;
using UnityEngine;

// Controls the boss right arm health, hit effect, attack timing, and aimed projectiles.

public class BossRightArm : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator armAnimator;

    [Header("Health")]
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private float hitFlashDuration = 0.15f;

    [Header("Attack Settings")]
    [SerializeField] private float minAttackDelay = 2f;
    [SerializeField] private float maxAttackDelay = 5f;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private int bulletCount = 5;
    [SerializeField] private float bulletSpawnDelay = 0.2f;

    private int currentHealth;
    private bool isDestroyed;
    private bool isFiring;
    private bool hasFired;

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private Coroutine hitFlashCoroutine;

    private static readonly int DefaultHash = Animator.StringToHash("boss_right_default");
    private static readonly int AttackHash = Animator.StringToHash("boss_right_attack");

    private void Awake()
    {
        currentHealth = maxHealth;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }
    }

    private void OnEnable()
    {
        if (isDestroyed) return;

        armAnimator.Play(DefaultHash, 0, 0f);
        StartCoroutine(AttackLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isFiring = false;
        hasFired = false;
        hitFlashCoroutine = null;
        RestoreColor();
    }

    private IEnumerator AttackLoop()
    {
        while (!isDestroyed)
        {
            float delay = Random.Range(minAttackDelay, maxAttackDelay);

            yield return new WaitForSeconds(delay);

            if (isDestroyed) yield break;

            hasFired = false;
            armAnimator.Play(AttackHash, 0, 0f);

            // Wait until the animation event starts firing
            yield return new WaitUntil(() => hasFired || isDestroyed);

            // Stay in attack until ALL projectiles finish firing
            yield return new WaitUntil(() => !isFiring || isDestroyed);

            if (isDestroyed) yield break;

            // Only return to default after the last projectile fires
            armAnimator.Play(DefaultHash, 0, 0f);
        }
    }

    // Called by the attack animation event
    public void FireSnipe()
    {
        if (isDestroyed || isFiring) return;

        hasFired = true;
        isFiring = true;

        StartCoroutine(FireSnipeSequence());
    }

    // Fire consecutive projectiles toward the player's current position
    // Fire consecutive projectiles toward the player's current position
    private IEnumerator FireSnipeSequence()
    {
        isFiring = true;

        for (int i = 0; i < bulletCount; i++)
        {
            if (isDestroyed) yield break;

            PlayerController player = FindFirstObjectByType<PlayerController>();

            if (player != null)
            {
                Vector3 spawnPosition = bulletSpawnPoint.position;
                Vector3 direction = player.transform.position - spawnPosition;

                // Aim directly at the player's current position
                float angle = Vector2.SignedAngle(Vector2.down, direction);
                Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

                // Spawn the right arm's own projectile prefab
                Instantiate(bulletPrefab, spawnPosition, rotation);
            }

            if (i < bulletCount - 1)
                yield return new WaitForSeconds(bulletSpawnDelay);
        }

        isFiring = false;
    }

    // Reduce arm health and flash red when damaged
    public void TakeDamage(int damage)
    {
        if (isDestroyed || damage <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log("Arm Right = " + currentHealth);
        if (currentHealth <= 0)
        {
            DestroyArm();
            return;
        }

        if (hitFlashCoroutine != null)
            StopCoroutine(hitFlashCoroutine);

        hitFlashCoroutine = StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        foreach (SpriteRenderer sprite in spriteRenderers)
        {
            if (sprite != null) sprite.color = Color.red;
        }

        yield return new WaitForSeconds(hitFlashDuration);

        RestoreColor();
        hitFlashCoroutine = null;
    }

    private void RestoreColor()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                spriteRenderers[i].color = originalColors[i];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerLaser"))
        {
            TakeDamage(1);
        }
    }

    private void DestroyArm()
    {
        isDestroyed = true;

        StopAllCoroutines();

        gameObject.SetActive(false);
    }
}

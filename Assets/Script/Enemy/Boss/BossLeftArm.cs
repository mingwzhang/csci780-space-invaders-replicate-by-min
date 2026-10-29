
using System.Collections;
using UnityEngine;

// Controls the boss left arm health, hit effect, attack timing, and projectiles.

public class BossLeftArm : MonoBehaviour
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
    [SerializeField] private int bulletCount = 7;
    [SerializeField] private float bulletSpacing = 0.5f;
    [SerializeField] private float bulletSpawnDelay = 0.08f;
    [SerializeField] private float spreadAngle = 25f;

    private int currentHealth;
    private bool isDestroyed;
    private int bulletAnimationNumber = 1;

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private Coroutine hitFlashCoroutine;

    private static readonly int DefaultHash = Animator.StringToHash("boss_left_default");
    private static readonly int AttackHash = Animator.StringToHash("boss_left_attack");

    private void Awake()
    {
        currentHealth = maxHealth;

        // Store the original colors of all left arm sprites
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

            armAnimator.Play(AttackHash, 0, 0f);

            yield return null;

            float attackLength = armAnimator.GetCurrentAnimatorStateInfo(0).length;

            yield return new WaitForSeconds(attackLength);

            if (isDestroyed) yield break;

            armAnimator.Play(DefaultHash, 0, 0f);
        }
    }

    // Called by the attack animation event
    public void FireSpread()
    {
        if (isDestroyed) return;

        StartCoroutine(FireSwipe());
    }

    // Fire projectiles sequentially with outward rotation
    private IEnumerator FireSwipe()
    {
        float startOffset = -(bulletCount - 1) * bulletSpacing / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            if (isDestroyed) yield break;

            float t = bulletCount > 1 ? (float)i / (bulletCount - 1) : 0.5f;

            // Horizontal spawn position
            float offset = startOffset + i * bulletSpacing;
            Vector3 spawnPosition = bulletSpawnPoint.position + Vector3.right * offset;

            // Rotate projectiles outward
            float angle = Mathf.Lerp(-spreadAngle, spreadAngle, t);
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

            // Spawn projectile
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, rotation);

            // Cycle bullet animation: 1 -> 2 -> 3 -> 1
            AlienBullet alienBullet = bullet.GetComponent<AlienBullet>();

            if (alienBullet != null)
            {
                alienBullet.SetAnimation(bulletAnimationNumber);
            }

            bulletAnimationNumber++;

            if (bulletAnimationNumber > 3)
            {
                bulletAnimationNumber = 1;
            }

            yield return new WaitForSeconds(bulletSpawnDelay);
        }
    }

    // Reduce arm health and flash red when damaged
    public void TakeDamage(int damage)
    {
        if (isDestroyed || damage <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        Debug.Log("Arm Left = " + currentHealth);

        if (currentHealth <= 0)
        {
            DestroyArm();
            return;
        }

        // Restart the flash duration when hit repeatedly
        if (hitFlashCoroutine != null)
            StopCoroutine(hitFlashCoroutine);

        hitFlashCoroutine = StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        // Turn all left arm sprites red
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

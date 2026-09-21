using UnityEngine;

public class BunkerBlock : MonoBehaviour
{
    // Width and height of the oval explosion crater
    private float damageRadiusX = 0.1f;
    private float damageRadiusY = 0.2f;

    // Controls how irregular the crater edges are (higher = more irregular)
    private float roughness = 0.8f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int playerBulletLayer = LayerMask.NameToLayer("PlayerBullet");
        int enemyBulletLayer = LayerMask.NameToLayer("EnemyBullet");

        // Ignore the collision if it wasn't caused by a player or enemy bullet
        if (collision.gameObject.layer != playerBulletLayer && collision.gameObject.layer != enemyBulletLayer)
        {
            return;
        }

        collision.enabled = false;

        Vector2 damageCenter = transform.position;

        // Get distance ratios along the X and Y axes
        if (collision.gameObject.layer == playerBulletLayer)
        {
            // Player bullets bite upward
            damageCenter += Vector2.up * 0.1f;
        }
        else
        {
            // Enemy bullets bite downward
            damageCenter += Vector2.down * 0.1f;
        }

        // Find all bunker blocks within a rectangular bounding box around the explosion center
        Collider2D[] nearbyBlocks = Physics2D.OverlapBoxAll(damageCenter, new Vector2(damageRadiusX * 2f, damageRadiusY * 2f), 0f, LayerMask.GetMask("Bunker"));

        foreach (Collider2D block in nearbyBlocks)
        {
            // Calculate how far away this specific block is from the explosion center
            Vector2 offset = (Vector2)block.transform.position - damageCenter;

            // Normalize distance based on the X and Y radii fields
            float x = offset.x / damageRadiusX;
            float y = offset.y / damageRadiusY;

            // Elliptical distance math; values under 1.0 fall inside the oval boundaries
            float distance = x * x + y * y;

            // Add random variance to make pixelated, irregular damage profiles
            float randomEdge = Random.Range(-roughness, roughness);

            // Core zone (< 0.4f) always vaporizes; outer edge zone threshold changes randomly
            if (distance < 0.4f || distance < 1f + randomEdge)
            {
                Destroy(block.gameObject);
            }
        }

        Destroy(collision.gameObject);
    }
}
using UnityEngine;

public class BunkerBlock : MonoBehaviour
{
    private float damageRadiusX = 0.1f;
    private float damageRadiusY = 0.2f;
    private float roughness = 0.8f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int playerBulletLayer = LayerMask.NameToLayer("PlayerBullet");
        int enemyBulletLayer = LayerMask.NameToLayer("EnemyBullet");

        if (collision.gameObject.layer != playerBulletLayer && collision.gameObject.layer != enemyBulletLayer)
        {
            return;
        }

        collision.enabled = false;

        Vector2 damageCenter = transform.position;

        // Push damage into bunker
        if (collision.gameObject.layer == playerBulletLayer)
        {
            damageCenter += Vector2.up * 0.1f;
        }
        else
        {
            damageCenter += Vector2.down * 0.1f;
        }

        Collider2D[] nearbyBlocks = Physics2D.OverlapBoxAll(damageCenter, new Vector2(damageRadiusX * 2f, damageRadiusY * 2f), 0f, LayerMask.GetMask("Bunker"));

        foreach (Collider2D block in nearbyBlocks)
        {
            Vector2 offset = (Vector2)block.transform.position - damageCenter;

            float x = offset.x / damageRadiusX;
            float y = offset.y / damageRadiusY;

            float distance = x * x + y * y;

            // Rough random edge
            float randomEdge = Random.Range(-roughness, roughness);

            // Center always destroyed, edge is irregular
            if (distance < 0.4f || distance < 1f + randomEdge)
            {
                Destroy(block.gameObject);
            }
        }

        Destroy(collision.gameObject);
    }
}
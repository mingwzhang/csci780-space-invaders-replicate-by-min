using UnityEngine;

public class Bunker : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Texture2D texture;

    private void Awake()
    {
        Texture2D originalTexture = spriteRenderer.sprite.texture;

        texture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);

        texture.filterMode = FilterMode.Point;

        texture.SetPixels(originalTexture.GetPixels());
        texture.Apply();

        Sprite newSprite = Sprite.Create(texture, spriteRenderer.sprite.rect, spriteRenderer.sprite.pivot / spriteRenderer.sprite.rect.size, spriteRenderer.sprite.pixelsPerUnit);

        spriteRenderer.sprite = newSprite;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        int playerBulletLayer = LayerMask.NameToLayer("PlayerBullet");
        int alienBulletLayer = LayerMask.NameToLayer("AlienBullet");

        if (collision.gameObject.layer != playerBulletLayer && collision.gameObject.layer != alienBulletLayer)
        {
            return;
        }

        Vector2 hitPosition;

        if (collision.gameObject.layer == playerBulletLayer)
        {
            hitPosition = new Vector2(collision.bounds.center.x, collision.bounds.max.y);
        }
        else
        {
            hitPosition = new Vector2(collision.bounds.center.x, collision.bounds.min.y);
        }

        Vector2Int pixel = WorldToPixel(hitPosition);

        if (pixel.x < 0 || pixel.x >= texture.width || pixel.y < 0 || pixel.y >= texture.height)
        {
            return;
        }

        Color pixelColor = texture.GetPixel(pixel.x, pixel.y);

        if (pixelColor.a == 0)
        {
            return;
        }

        Damage(pixel.x, pixel.y);

        Destroy(collision.gameObject);
    }

    private Vector2Int WorldToPixel(Vector2 worldPosition)
    {
        Vector2 localPosition = spriteRenderer.transform.InverseTransformPoint(worldPosition);

        Sprite sprite = spriteRenderer.sprite;

        int x = Mathf.FloorToInt(localPosition.x * sprite.pixelsPerUnit + sprite.pivot.x);
        int y = Mathf.FloorToInt(localPosition.y * sprite.pixelsPerUnit + sprite.pivot.y);

        return new Vector2Int(x, y);
    }

    private void Damage(int centerX, int centerY)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                int pixelX = centerX + x;
                int pixelY = centerY + y;

                if (pixelX < 0 || pixelX >= texture.width || pixelY < 0 || pixelY >= texture.height)
                {
                    continue;
                }

                texture.SetPixel(pixelX, pixelY, Color.clear);
            }
        }

        texture.Apply();
    }
}
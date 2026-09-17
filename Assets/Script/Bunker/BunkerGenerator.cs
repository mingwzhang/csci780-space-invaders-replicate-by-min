using UnityEngine;

public class BunkerGenerator : MonoBehaviour
{
    // Small block prefab
    [SerializeField] private GameObject blockPrefab;

    // Space between block centers
    [SerializeField] private float blockSize = 0.1f;

    // Makes blocks slightly overlap
    [SerializeField] private float overlap = 1.02f;

    // 1 = block, 0 = empty
    private readonly int[,] bunkerLayout = new int[,]
    {
    {0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
    {0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0},
    {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
    {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},

    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},

    {1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1},
    {1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1},
    {1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1}
    };

    private void Start()
    {
        GenerateBunker();
    }

    private void GenerateBunker()
    {
        // Get grid size
        int rows = bunkerLayout.GetLength(0);
        int cols = bunkerLayout.GetLength(1);

        // Center the bunker
        float startX = -((cols - 1) * blockSize) / 2f;
        float startY = ((rows - 1) * blockSize) / 2f;

        // Loop through each grid spot
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // Skip empty spaces
                if (bunkerLayout[row, col] == 0)
                {
                    continue;
                }

                // Calculate block position
                float posX = startX + col * blockSize;
                float posY = startY - row * blockSize;

                // Spawn block
                GameObject newBlock = Instantiate(blockPrefab, transform);

                newBlock.transform.localPosition = new Vector3(posX, posY, 0f);
                newBlock.transform.localRotation = Quaternion.identity;

                SpriteRenderer sr = newBlock.GetComponent<SpriteRenderer>();

                if (sr != null)
                {
                    // Make block slightly bigger to remove gaps
                    float targetSize = blockSize * overlap;

                    float scaleX = targetSize / sr.sprite.bounds.size.x;
                    float scaleY = targetSize / sr.sprite.bounds.size.y;

                    newBlock.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                }

                // Match parent layer
                newBlock.layer = gameObject.layer;
            }
        }
    }
}
using UnityEngine;

public class AlienGroup : MonoBehaviour
{
    [SerializeField] private float moveDistance = 0.3f;
    [SerializeField] private float moveIntervalTimer = 0.4f;
    [SerializeField] private float leftBorder = -10f;
    [SerializeField] private float rightBorder = 10f;
    [SerializeField] private float dropDistance = 0.25f;

    [SerializeField] private int direction = 1;
    private float timer;

    private bool reachedBorder = false;

    private void Start()
    {
        // Set the group's starting world X position to 0 for consistency
        // transform.position = new Vector3(0f, transform.position.y, transform.position.z);
    }

    void Update()
    {
        timer += Time.deltaTime;


        if (timer < moveIntervalTimer)
        {
            return;
        }

        timer = 0f;

        // Specific case to move group down when reachedBorder, then disable reachedBorder

        if (reachedBorder) 
        {
            // Space.World, tells Unity to use global coordinate
            transform.Translate(0f, -dropDistance, 0f, Space.World);
            direction *= -1;
            reachedBorder = false;
            return;
        }

        // Move the group when the movement timer finishes

        // If any child alien would reach a border (based on world position), set reachedBorder = true, else keep moving

        float nextMove = direction * moveDistance;

        foreach (Transform alien in transform) 
        {
            float nextPosition = alien.position.x + nextMove;

            if (direction == 1 && nextPosition >= rightBorder)
            {
                nextMove = rightBorder - alien.position.x;
                reachedBorder = true;
            }

            if (direction == -1 && nextPosition <= leftBorder)
            {
                nextMove = leftBorder - alien.position.x;
                reachedBorder = true;
            }
        }

        // No alien reached a border, so move the group horizontally
        transform.Translate(nextMove, 0f, 0f, Space.World);

    }
}
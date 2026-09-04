using UnityEngine;

public class AlienGroup : MonoBehaviour
{
    [SerializeField] private float speed = 0f;
    [SerializeField] private float leftBorder = -11f;
    [SerializeField] private float rightBorder = 11f;
    [SerializeField] private float dropDistance = 0.1f;

    private int direction = 1;

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, 0, 0);

        foreach (Transform alien in transform)
        {
            if (alien.position.x >= rightBorder && direction == 1)
            {
                direction = -1;
                transform.Translate(0, -dropDistance, 0);
                break;
            }

            if (alien.position.x <= leftBorder && direction == -1)
            {
                direction = 1;
                transform.Translate(0, -dropDistance, 0);
                break;
            }
        }
    }
}
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 15f;

    [SerializeField] private float leftBorder = -11f;
    [SerializeField] private float rightBorder = 11f;

    [SerializeField] private GameObject playerBullet;

    [SerializeField] private float fireRate = 0.1f;  // Minimum number of seconds between shots
    private float nextFireTime;

    // Time.deltaTime = the time since the previous frame, making movement frame-rate independent
    // Time.time = the total time since the game started


    void Update()
    {
        float move = 0;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            move = -1;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            move = 1;


        transform.Translate(move * speed * Time.deltaTime, 0, 0);

        if (transform.position.x < leftBorder)
            transform.position = new Vector2(leftBorder, transform.position.y);

        if (transform.position.x > rightBorder)
            transform.position = new Vector2(rightBorder, transform.position.y);

        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time >= nextFireTime)
        {
            Instantiate(playerBullet, transform.position, transform.rotation);

            // Allow the next shot after fireRate seconds have passed
            nextFireTime = Time.time + fireRate;
             
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") || collision.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
        {
            Destroy(gameObject);
        }
    }
}
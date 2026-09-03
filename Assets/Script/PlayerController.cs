using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 15f;

    public GameObject playerBullet;

    void Update()
    {
        float move = 0;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            move = -1;
        }

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            move = 1;
        }

        transform.Translate(move * speed * Time.deltaTime, 0, 0);

        if (transform.position.x < -18)
            transform.position = new Vector3(-18, transform.position.y, transform.position.z);

        if (transform.position.x > 18)
            transform.position = new Vector3(18, transform.position.y, transform.position.z);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            Instantiate(playerBullet, transform.position, transform.rotation);
    }
}
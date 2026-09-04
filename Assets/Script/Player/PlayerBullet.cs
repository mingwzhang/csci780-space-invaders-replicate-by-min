using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 50f;

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (transform.position.y > 18)
            Destroy(gameObject);
    }
}
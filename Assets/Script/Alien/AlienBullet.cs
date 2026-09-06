using UnityEngine;

public class AlienBullet : MonoBehaviour
{
    private float speed = 20f;

    private float bottomDistnaceLimit = -15f;
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < bottomDistnaceLimit)
            Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            Destroy(gameObject);
        }
    }
}
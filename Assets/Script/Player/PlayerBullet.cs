using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private float speed = 30f;

    private float upDistnaceLimit = 30f;
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (transform.position.y > upDistnaceLimit)
            Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") || collision.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
        {
            Destroy(gameObject);
        }
    }
}
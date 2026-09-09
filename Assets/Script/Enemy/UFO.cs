using UnityEngine;

public class UFO : MonoBehaviour
{

    [SerializeField] private GameObject ufoBullet;
    private float speed = 3f;

    private float rightDistnaceLimit = 13f;

    private float minShootInterval = 0.3f;
    private float maxShootInterval = 1.5f;

    private float shootInterval;
    private float shootTimer;


    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (transform.position.x > rightDistnaceLimit)
            Destroy(gameObject);

        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval)
        {
            shootTimer = 0f;

            Instantiate(ufoBullet, transform.position, Quaternion.identity);

            // Pick a new delay for the next shot
            shootInterval = Random.Range(minShootInterval, maxShootInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            Destroy(gameObject);
        }
    }

}

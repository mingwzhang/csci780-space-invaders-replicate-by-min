using UnityEngine;

public class Alien : MonoBehaviour
{
    [SerializeField] private GameObject alienBullet;
    [SerializeField] private int scoreValue;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        Instantiate(alienBullet, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            AlienGroup group = GetComponentInParent<AlienGroup>();

            if (group != null)
            {
                group.AlienDestroyed(); // For each alien destroyed, speed up the group
            }
            gameManager.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }


}

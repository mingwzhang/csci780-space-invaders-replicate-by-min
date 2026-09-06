using UnityEngine;

public class Alien : MonoBehaviour
{
    [SerializeField] private GameObject alienBullet;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
            Destroy(gameObject);
        }
        //if (collision.gameObject.CompareTag("PlayerBullet"))
        //{
        //    Destroy(gameObject);
        //    Debug.Log("Hit2");
        //}
    }


}

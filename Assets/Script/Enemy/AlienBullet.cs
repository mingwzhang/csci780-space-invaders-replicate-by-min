using UnityEngine;

public class AlienBullet : MonoBehaviour
{
    private float speed = 10f;
    private float bottomDistanceLimit = 0.0f;
    private Animator bulletAnimator;

    private void Awake()
    {
        bulletAnimator = GetComponentInChildren<Animator>();
    }

    public void SetAnimation(int animationNumber)
    {
        string animationName = "alien_bullet" + animationNumber;
        bulletAnimator.Play(animationName, 0, 0f);
    }

    private void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < bottomDistanceLimit)
        {
            Destroy(gameObject);
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
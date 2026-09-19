using System.Collections;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private float speed = 30.0f;

    private float upDistnaceLimit = 15.5f;
    private Animator animator;
    private bool isExploding = false;

    void Start()
    {

        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Only move the bullet if didn't trigger explode boolean

        if(!isExploding)
        {

            transform.Translate(Vector3.up * speed * Time.deltaTime);

            if (transform.position.y > upDistnaceLimit)
            {
                TriggerExplosion();
            }
        }   
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isExploding) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") || collision.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
        {
            Destroy(gameObject);
        }
    }
    private void TriggerExplosion()
    {
        isExploding = true;
        StartCoroutine(PlayAnimationAndDestroy());
    }

    private IEnumerator PlayAnimationAndDestroy()
    {
        if (animator != null)
        {
            animator.Play("player_projectile_explode");

            yield return null;

            float duration = animator.GetCurrentAnimatorStateInfo(0).length;

            yield return new WaitForSeconds(duration);
        }

        Destroy(gameObject);
    }
}
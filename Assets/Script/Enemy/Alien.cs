using UnityEngine;
using System.Collections;

public class Alien : MonoBehaviour
{
    [SerializeField] private GameObject alienBullet;
    [SerializeField] private int scoreValue;

    private GameManager gameManager;

    private Animator childAnimator;
    private Collider2D alienCollider;

    void Awake()
    {
        if (childAnimator == null) childAnimator = GetComponentInChildren<Animator>();
        alienCollider = GetComponent<Collider2D> ();
    }


    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        childAnimator = GetComponentInChildren<Animator>();

        if (childAnimator == null)   
            Debug.LogError("No Animator found on any child GameObject!", this);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(int animationIndex)
    {
        GameObject newBullet = Instantiate(alienBullet, transform.position, Quaternion.identity);

        AlienBullet bulletScript = newBullet.GetComponent<AlienBullet>();
        bulletScript.SetAnimation(animationIndex);
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
            StartCoroutine(DestroyAlien());
        }
    }


    private IEnumerator DestroyAlien()
    {
        childAnimator.Play("alien_destroyed");
        alienCollider.enabled = false;

        yield return null;

        // Read the exact length of the animation currently playing on the child
        float clipLength = childAnimator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSecondsRealtime(clipLength);
        Destroy(gameObject);

    }
}

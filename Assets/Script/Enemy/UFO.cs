using UnityEngine;
using System.Collections;

public class UFO : MonoBehaviour
{

    [SerializeField] private GameObject ufoBullet;
    private float speed = 3f;

    private float rightDistnaceLimit = 10f;

    private GameManager gameManager;
    [SerializeField] private int[] scoreArray = {50, 100, 150, 200, 300};
    [SerializeField] private int scoreValue = 0;

    private Animator childAnimator;
    private Collider2D ufoCollider;

    void Awake()
    {
        if (childAnimator == null) childAnimator = GetComponentInChildren<Animator>();
        ufoCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        scoreValue = scoreArray[Random.Range(0,5)];
        print(scoreValue);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (transform.position.x > rightDistnaceLimit)
            Destroy(gameObject);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            gameManager.AddScore(scoreValue);
            StartCoroutine(DestroyUFO());
        }
    }

    private IEnumerator DestroyUFO()
    {
        // Force the animator to run aeven if the game is paused (Time.timeScale = 0)
        childAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        childAnimator.Play("alien_destroyed");
        ufoCollider.enabled = false;
        speed = 0;

        yield return null;

        // Read the exact length of the animation currently playing on the child
        float clipLength = childAnimator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSecondsRealtime(clipLength);
        Destroy(gameObject);

    }
}

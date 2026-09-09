using UnityEngine;

public class UFOSpawner : MonoBehaviour
{

    [SerializeField] private GameObject ufoPrefab;

    private float spawnTimer = 25f;
    private float currentTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer += Time.deltaTime;

        if (currentTimer > spawnTimer)
        {
            Instantiate(ufoPrefab, transform.position, Quaternion.identity);
            currentTimer = 0;
        }
    }
}

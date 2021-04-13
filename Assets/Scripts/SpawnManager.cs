using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemySpawnner;
    [SerializeField] private GameObject[] powerups;
    [SerializeField] private int enemiesInWave;
    [SerializeField] private int waveNumber;
    [SerializeField] private int enemiesAmount;
    [SerializeField] private float startTime;

    private bool isPlayerAlive = true;
    private bool isWaveRunning;

    private void Start()
    {
        waveNumber = 1;
        isWaveRunning = true;

        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpEnemies());
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Return) && isWaveRunning == false && enemiesAmount == 0)
        //{
        //    StartCoroutine(SpawnEnemiesWaveRoutine());
        //}

        if (enemiesAmount == 0)
            StartCoroutine(SpEnemies());
    }

    public void DecreaseEnemiesAmount()
    {
        enemiesAmount--;
    }

    public void StartSpawnning()
    {
        //StartCoroutine(SpawnEnemiesWaveRoutine());
        //StartCoroutine(SpawnPowerupRoutine());
    }

   
        
    IEnumerator SpEnemies()
    {
        while (isPlayerAlive && enemiesAmount < 10)
        {
            Instantiate(enemyPrefab, new Vector2(Random.Range(-7f, 7f), 5f), Quaternion.identity);
            enemiesAmount++;
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator SpawnEnemiesWaveRoutine()
    {
        startTime = Time.time;

        while (Time.time - startTime < (waveNumber * 5f))
        {
            Debug.Log("Time - startTime " + (Time.time - startTime));

            float xRandomPos = Random.Range(-9.5f, 9.5f);
            Vector2 enemySpawnPos = new Vector2(xRandomPos, 5f);

            GameObject newEnemy = Instantiate(enemyPrefab, enemySpawnPos, Quaternion.identity);
            newEnemy.transform.parent = enemySpawnner.transform;

            enemiesAmount++;

            yield return new WaitForSeconds(1f);
        }

        waveNumber++;
        isWaveRunning = false;
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (isPlayerAlive)
        {
            int randomPowerup = Random.Range(0, 7);
            Instantiate(powerups[randomPowerup], new Vector2(Random.Range(-9.5f, 9.5f), 5f), Quaternion.identity);
            yield return new WaitForSeconds(4);
        }
    }

    public void OnPlayerDestroyed()
    {
        isPlayerAlive = false;
    }
}
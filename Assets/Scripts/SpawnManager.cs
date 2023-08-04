using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private float _difficultyScalingFactor = 1.0f;

    [SerializeField]
    private int _baseEnemies = 10;

    [SerializeField]
    private int _enemiesPerLevel = 2;

    [SerializeField]
    private GameObject _enemyPrefab;
    
    [SerializeField]
    private float _minimumSpawnDelayInSeconds = 5f;
    
    [SerializeField]
    private float _maximumSpawnDelayInSeconds = 10f;

    private int _enemiesDestroyedThisLevel = 0;
    private int _maximumEnemiesThisLevel = 0;
    private int _level = 1; // The current level the player is on
    private float _levelClearedTime; // Time the player takes to clear the level
    private float _levelStartTime; // Time this level started so we can calculate time taken to clear level

    private bool _canSpawnEnemies = false;
    private int _spawnedEnemyThisLevel = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(_enemyPrefab == null)
        {
            Debug.LogError("SpawnManager::Start() - Enemy Prefab is NULL.");
        }

        StartNewEnemyWave(_level); // start level 1
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void calculateTimeTakenToClearLevel()
    {
        _levelClearedTime = Time.time - _levelStartTime;
    }
    public void StartNewEnemyWave(int currentLevel)
    {
        _levelStartTime = Time.time; // timestamp the start of this level.
        _enemiesDestroyedThisLevel = 0; // reset to zero kills this level.
        _maximumEnemiesThisLevel = CalculateMaxEnemies(currentLevel);
        StartCoroutine(SpawnEnemies(true));

    }
    public void EnemyDestroyed()
    {
        _enemiesDestroyedThisLevel++;

        if(_enemiesDestroyedThisLevel >= _maximumEnemiesThisLevel)
        {
            WaveCleared();
        }
    }
    private void WaveCleared()
    {
        // stop the SpawnEnemies Coroutine.
        // get the time taken to clear this level.
        // calculate the next wave enemy maximum.
        StopCoroutine(SpawnEnemies(false));
        calculateTimeTakenToClearLevel();
        StartNewEnemyWave(_level++);
    }
    
    private int CalculateMaxEnemies(int level)
    {
        // Adjust the scaling factor based on clear time
        if (_levelClearedTime < 60.0f) // 60 seconds as a fast clear time, adjust as needed
        {
            _difficultyScalingFactor = 1.2f; // Increase difficulty if cleared quickly
        }
        else if (_levelClearedTime > 120.0f) // 120 seconds as a slow clear time, adjust as needed
        {
            _difficultyScalingFactor = 0.9f; // Decrease difficulty if cleared slowly
        }

        return (int)(_baseEnemies + _enemiesPerLevel * (level - 1) * level * _difficultyScalingFactor);
    }

    private bool AreEnemeisAllowedToSpawn() {
        return _spawnedEnemyThisLevel < _maximumEnemiesThisLevel;
    }
    IEnumerator SpawnEnemies(bool canSpawn)
    {
        while(canSpawn)
        {
            Instantiate(_enemyPrefab);
            _spawnedEnemyThisLevel++;

            canSpawn = AreEnemeisAllowedToSpawn();
            float timeToWait = Random.Range(_minimumSpawnDelayInSeconds, _maximumSpawnDelayInSeconds);
            // Don't count the time we wait as part of the time taken to clear the level.
            _levelStartTime -= timeToWait;
            yield return new WaitForSeconds(timeToWait);
        }
    }
}

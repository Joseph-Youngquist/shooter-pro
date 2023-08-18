using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private float _difficultyScalingFactor = 1.0f;

    [SerializeField]
    private int _baseEnemies = 10;

    [SerializeField]
    private int _enemiesPerLevel = 2;

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private GameObject[] _powerUpPrefabs;

    [SerializeField]
    private float _minimumPowerUpTimeLimit = 3.0f;
    [SerializeField]
    private float _maximumPowerUpTimeLimit = 7.0f;

    [SerializeField]
    private float _minimumSpawnDelayInSeconds = 0.5f;
    
    [SerializeField]
    private float _maximumSpawnDelayInSeconds = 3.0f;

    private int _enemiesDestroyedThisLevel = 0;
    [SerializeField]
    private int _maximumEnemiesThisLevel = 0;

    [SerializeField]
    private bool _spawnAllowed;

    private int _level = 1; // The current level the player is on
    private float _levelClearedTime; // Time the player takes to clear the level
    private float _levelStartTime; // Time this level started so we can calculate time taken to clear level

    [SerializeField]
    private int _spawnedEnemyThisLevel = 0;

    private List<GameObject> _poolOfEnemies = new List<GameObject>();

    // used to set horizontal bounds for player and enemies
    private float _screenHalfWidth;
    private float _cameraX;

    private Vector3 _cameraScreenToWorldPoint;

    private bool _gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        if(_enemyPrefab == null)
        {
            Debug.LogError("SpawnManager::Start() - Enemy Prefab is NULL.");
        }

        if(_enemyContainer == null)
        {
            Debug.LogError("SpawnManager::Start() - Enemy Container is NULL.");
        }

        _cameraScreenToWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height, Camera.main.transform.position.z));
        // Get the width of the game window to calculate the left and right edges
        _screenHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        // Need to take into account if the camera isn't in the center as it is right now...
        _cameraX = Camera.main.transform.position.x;

        StartNewEnemyWave(_level); // start level 1
        Debug.Log("Screen Bounds: " + _cameraScreenToWorldPoint.ToString());

        StartCoroutine(SpawnPowerups());
        
    }
    public Vector3 GetPoint()
    {
        return _cameraScreenToWorldPoint;
    }
    public float GetScreenHalfWidth()
    {
        return _screenHalfWidth;
    }
    public float GetCameraX()
    {
        return _cameraX;
    }

    private void calculateTimeTakenToClearLevel()
    {
        _levelClearedTime = Time.time - _levelStartTime;
    }
    private void StartNewEnemyWave(int currentLevel)
    {
        _levelStartTime = Time.time; // timestamp the start of this level.
        _enemiesDestroyedThisLevel = 0; // reset to zero kills this level.
        _spawnedEnemyThisLevel = 0;     // reset spawned count to zero this level.
        _spawnAllowed = true;

        _maximumEnemiesThisLevel = CalculateMaxEnemies(currentLevel);
        
        StartCoroutine(SpawnEnemies());

    }
    public void EnemyDestroyed(GameObject theEnemyDestroyed)
    {
        _enemiesDestroyedThisLevel++;

        // TODO: We'll need to implement this a little differently later once we
        // add in blowing up animations >)

        theEnemyDestroyed.SetActive(false);
        _poolOfEnemies.Add(theEnemyDestroyed);

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
        _spawnAllowed = false;
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

    IEnumerator SpawnEnemies()
    {
        while(_spawnAllowed)
        {
            GetOrMakeEnemy();
            _spawnedEnemyThisLevel++;

            _spawnAllowed = _spawnedEnemyThisLevel < _maximumEnemiesThisLevel;

            float timeToWait = UnityEngine.Random.Range(_minimumSpawnDelayInSeconds, _maximumSpawnDelayInSeconds);
            // Don't count the time we wait as part of the time taken to clear the level.
            _levelStartTime -= timeToWait;
            yield return new WaitForSeconds(timeToWait);
        }
    }

    IEnumerator SpawnPowerups()
    {
        while (!_gameOver)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(_minimumPowerUpTimeLimit, _maximumPowerUpTimeLimit));
            float minLeft = -_cameraScreenToWorldPoint.x + 1.5f; // keep power up fully on screen
            float maxRight = _cameraScreenToWorldPoint.x - 1.5f; // ""
            float top = 7.5f; //_cameraScreenToWorldPoint.y + 4f; // spawn fully above screen

            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(minLeft, maxRight), top, 0f);

            int randomID = UnityEngine.Random.Range(0, _powerUpPrefabs.Length);
            GameObject powerup = Instantiate(_powerUpPrefabs[randomID], randomPosition, Quaternion.identity);
        }
    }

    private void GetOrMakeEnemy()
    {
        // Check pool and if there is an inactive enemy reuse it
        // if there isn't an available reuseable enemy then make a new one.
        if (_poolOfEnemies.Count > 0)
        {
            int _lastIndex = _poolOfEnemies.Count - 1;
            GameObject newEnemy = _poolOfEnemies[ _lastIndex ];
            _poolOfEnemies.RemoveAt(_lastIndex);
            Enemy enemy = newEnemy.GetComponent<Enemy>();
            enemy.ResetEnemy();  // sets new randomX and activates object.
        } else
        {
            GameObject newEnemy = Instantiate(_enemyPrefab);
            newEnemy.transform.SetParent(_enemyContainer.transform);
        }
    }
    public void OnPlayerDeath()
    {
        _spawnAllowed = false;
        _gameOver = true;
    }

    public void GameOver(bool status)
    {
        // TODO: Need to implement game over...
        _spawnAllowed = false;
    }
}

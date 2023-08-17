using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField]
    private SpawnManager _spawnManager;

    [SerializeField]
    private float _speed = 5.5f; // initial player speed

    [SerializeField]
    private float _verticalStartPosition = -3.8f;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _trippleShotPreFab;

    private float _rightEdgeOfScreen;

    [SerializeField]
    private float _laserYOffset = 0.5f;

    [SerializeField]
    private float _fireDelay = 0.2f;
    private float _nextFireAt = -1f;

    [SerializeField]
    private int _lives = 3;
    private int _score = 0;

    [SerializeField]
    private bool _isTrippleShotActive = false;
    [SerializeField]
    private float _trippleShotDuration = 3f;

    private int _totalShotsFired = 0;
    private int _totalKills = 0;
    private float _shotAccuracy = 0;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, _verticalStartPosition, 0);

        if(_spawnManager == null)
        {
            Debug.LogError("Player::Start() - SpawnManager is null.");
        }

        if (_laserPrefab == null)
        {
            Debug.LogError("Player::Start() - LaserPreFab is null.");
        }

        if ( _trippleShotPreFab == null)
        {
            Debug.LogError("Player::Start() - TrippleShotPreFab is null.");
        }

        CalculatePlayerMovementBounds();
    }

    // Update is called once per frame
    void Update()
    {
        CalculatePlayerMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFireAt) 
        {
            _nextFireAt = Time.time + _fireDelay;
            Shoot();
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label($"Score: {_score}");
        GUILayout.Label($"Lives: {_lives}");
        GUILayout.Label($"Shots Fired: {_totalShotsFired}");
        GUILayout.Label($"Total Kills: {_totalKills}");
        GUILayout.Label($"Accuracy: {_shotAccuracy.ToString("F2")}%");
        GUILayout.EndArea();
    }
    private void CalculatePlayerMovementBounds()
    {
        // Get the width of the game window to calculate the left and right edges
        Vector3 screenSize = Camera.main
            .ScreenToWorldPoint(
                new Vector3(0f, Screen.height, Camera.main.transform.position.z)
            );
        /*
         *  We want to limit the player's ship to stay on the screen at all times.
         *  To do this we get the screenSize and subtract off the player's bound x / 2
         */
        float playerBoundsX = transform.GetComponent<BoxCollider2D>().bounds.size.x;
        // we'll just use the negative of this for the left side of the screen
        _rightEdgeOfScreen = screenSize.x - playerBoundsX / 2;

    }

    public void AddScore(int points)
    {
        _totalKills++;
        _shotAccuracy = (float)_totalKills / _totalShotsFired * 100;
        _score += points;
    }
    public void Hit()
    {
        _lives -= 1;
        if (_lives == 0)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }
    
    public void CollectPowerUp(int powerupID)
    {
        if (powerupID == 0)
        {
            _isTrippleShotActive = true;
            StartCoroutine(PowerUpTimer(_trippleShotDuration));
        } else if (powerupID == 1)
        {
            Debug.Log("Collected " + powerupID);
        }
    }
    void Shoot()
    {
        _totalShotsFired++;
        FireWeapons();
    }

    IEnumerator PowerUpTimer(float durration)
    {
        yield return new WaitForSeconds(durration);
        _isTrippleShotActive = false;
    }

    void FireWeapons()
    {

        Vector3 laserPosition;
        GameObject prefabToUse;
        
        if (_isTrippleShotActive)
        {
            prefabToUse = _trippleShotPreFab;
            laserPosition = transform.position;
        } else
        {
            prefabToUse = _laserPrefab;
            laserPosition = transform.position + new Vector3(0f, _laserYOffset, 0f);
        }

        Instantiate(prefabToUse, laserPosition, Quaternion.identity);
    }
    void CalculatePlayerMovement()
    {
        float inputDirection = Input.GetAxis("Horizontal");
        Vector3 newDirection = new Vector3(inputDirection, 0, 0);
        transform.Translate(newDirection * _speed * Time.deltaTime);

        float currentXPosition = transform.position.x;
        float newPositionX = Mathf.Clamp(currentXPosition, -_rightEdgeOfScreen, _rightEdgeOfScreen);
        transform.position = new Vector3(newPositionX, _verticalStartPosition, 0);

    }
}

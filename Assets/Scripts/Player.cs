using System.Collections;
using System.Collections.Generic;
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
    private float _playerScaleFactorAdjustment = 0.05f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _trippleShotPreFab;

    private float _leftEdgeOfScreen = -9.2f;
    private float _rightEdgeOfScreen = 9.2f;

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
            FireLaser();
        }
    }
    private void CalculatePlayerMovementBounds()
    {
        // Get the width of the game window to calculate the left and right edges
        float screenHalfWidth = _spawnManager.GetScreenHalfWidth();

        // Need to take into account if the camera isn't in the center as it is right now...
        float cameraX = _spawnManager.GetCameraX();
        /*
         *  And we need to account for the width of the player's ship. 
         * We'll use the SpriteRender to get the size.x and we'll multiply
         * it by the player's slightly smaller scale.x value so we keep the
         * player completely in bounds of the game window.
         */
        float playerScaleX = transform.localScale.x - _playerScaleFactorAdjustment;
        float playerHalfWidth = transform.GetComponent<SpriteRenderer>().bounds.size.x * playerScaleX;
        _leftEdgeOfScreen = cameraX - screenHalfWidth - playerHalfWidth;
        _rightEdgeOfScreen = cameraX + screenHalfWidth + playerHalfWidth;
    }

    public void AddScore(int points)
    {
        _score += points;
        Debug.Log("Player's Score: " + _score);
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
    void FireLaser()
    {
        GetOrMakeLaser();
    }

    void GetOrMakeLaser()
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
        float newPositionX = Mathf.Clamp(currentXPosition, _leftEdgeOfScreen, _rightEdgeOfScreen);
        transform.position = new Vector3(newPositionX, _verticalStartPosition, 0);

    }
}

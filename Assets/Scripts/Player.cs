using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.5f; // initial player speed

    [SerializeField]
    private float _verticalStartPosition = -4.0f;

    [SerializeField]
    private GameObject _laserPrefab;

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

    private List<GameObject> _availableLasers = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, _verticalStartPosition, 0);

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
            Destroy(this.gameObject);
        }
    }
    void FireLaser()
    {
        GetOrMakeLaser();
    }

    void GetOrMakeLaser()
    {
        Vector3 laserPosition = transform.position + new Vector3(0f, _laserYOffset, 0f);

        if (_availableLasers.Count > 0)
        {
            int lastIndex = _availableLasers.Count - 1;
            GameObject laser = _availableLasers[lastIndex];
            _availableLasers.RemoveAt(lastIndex);
            laser.transform.position = laserPosition;
            laser.SetActive(true);
        }
        else
        {
            Instantiate(_laserPrefab, laserPosition, Quaternion.identity);
        }

    }
    public void AddLaserToPool(GameObject laser)
    {
        _availableLasers.Add(laser);
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

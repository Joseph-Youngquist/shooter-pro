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

    private GameObject[] _availableLasers;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, _verticalStartPosition, 0);

    }

    // Update is called once per frame
    void Update()
    {
        CalculatePlayerMovement();

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            FireLaser();
        }
    }

    void FireLaser()
    {
        Instantiate(_laserPrefab, transform.position, Quaternion.identity);
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

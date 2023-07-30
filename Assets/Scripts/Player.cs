using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.5f; // initial player speed

    [SerializeField]
    private float _verticalStartPosition = -4.0f;

    private float _leftEdgeOfScreen = -9.2f;
    private float _rightEdgeOfScreen = 9.2f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, _verticalStartPosition, 0);

    }

    // Update is called once per frame
    void Update()
    {
        float inputDirection = Input.GetAxis("Horizontal");
        Vector3 direction = new Vector3(inputDirection, 0, 0);
        transform.Translate( direction * _speed * Time.deltaTime);

        // Need to restrict player movement along the horizonal.
        // Need to keep user from moving out of view left and right.
        // If player is along the left side of the screen
        // then keep player from moving past the left side of the screen.
        // If player is along the right side of the screen
        // then keep player from moving past the right side of the screen.
        float currentX = transform.position.x;
        float newPositionX = Mathf.Clamp(currentX, _leftEdgeOfScreen, _rightEdgeOfScreen);
        transform.position = new Vector3(newPositionX, _verticalStartPosition, 0);
    }
}

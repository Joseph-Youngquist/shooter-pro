using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.5f; // initial player speed

    // Start is called before the first frame update
    void Start()
    {
        // Set the starting player position to a new position at (0, -4, 0)
        transform.position = new Vector3(0, -4, 0);

    }

    // Update is called once per frame
    void Update()
    {
        // Player movement for our game will only be in the horizontal space
        // and will not allow vertical movement.

        float inputDirection = Input.GetAxis("Horizontal");
        Vector3 direction = new Vector3(inputDirection, 0, 0);
        transform.Translate( direction * _speed * Time.deltaTime);

    }
}

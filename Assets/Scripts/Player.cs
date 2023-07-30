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
        float inputDirection = Input.GetAxis("Horizontal");

        transform.Translate(Vector3.right * inputDirection * _speed * Time.deltaTime);
    }
}

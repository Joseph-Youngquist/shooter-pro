using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1.0f;
    [SerializeField]
    private float _minSpeed = 0.5f;
    [SerializeField]
    private float _maxSpeed = 4.0f;

    private void Start()
    {
        _speed = Random.Range(_minSpeed, _maxSpeed);
    }
    void Update()
    {
        transform.Translate(Vector3.down * _speed *  Time.deltaTime);
    }
}

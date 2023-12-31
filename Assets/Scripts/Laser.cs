﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10.0f;

    private Player _player;

    private float _maxYPosition = 8.0f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.Log("Laser::Start - Player is null.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {

        if (transform.position.y < _maxYPosition)
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
            return;
        }

        CallDestroy();
    }

    private void CallDestroy()
    {
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}

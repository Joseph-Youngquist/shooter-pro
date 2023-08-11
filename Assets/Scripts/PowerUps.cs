using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    
    private SpawnManager _spawnManager;

    private float _bottomOfScreen;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.CollectPowerUp(0);
            }

            Destroy(this.gameObject);
        }
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        //if(transform.position.y < _bottomOfScreen)
        //{
          //  Destroy(this.gameObject);
        //}
    }
}

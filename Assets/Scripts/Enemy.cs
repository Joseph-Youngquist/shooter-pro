using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int _enemyScoreValue = 10;
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private float _offsetPositionY = 2f;
    [SerializeField]
    private float _enemyScaleFactorAdjustment = 0.03f;
    [SerializeField]
    private SpawnManager _spawnManager;

    private float _leftMinX;
    private float _topMaxY;
    private float _offscreenY;

    private Player _player;
    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;

        
        if (GameObject.Find("Spawn_Manager").TryGetComponent<SpawnManager>(out SpawnManager outManager))
        {
            _spawnManager = outManager;
        } else
        {
            Debug.LogError("Enemy::Start() - SpawnManager is Null.");
        }

        if (GameObject.Find("Player").TryGetComponent<Player>(out Player outPlayer))
        {
            _player = outPlayer;
        }
        else
        {
            // TODO: Game OVER!
            _spawnManager.GameOver(true);
            Destroy(this.gameObject);

        }

        CalculateMovementBounds();
        
        PickRandomHorizontalPosition();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovementBounds()
    {
        Vector3 point = _spawnManager.GetPoint();
        float enemyScaleX = transform.localScale.x - _enemyScaleFactorAdjustment;
        float enemyHalfWidth = transform.GetComponent<BoxCollider2D>().size.x * enemyScaleX;
        _leftMinX = -point.x + enemyHalfWidth;
        _topMaxY = -point.y;
        _offscreenY = -(_topMaxY + transform.localScale.y);
    }
    public void ResetEnemy()
    {
        // Resets the recyled enemy object
        PickRandomHorizontalPosition();
        this.gameObject.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        bool destroyed = false;
        if (other.transform.CompareTag("Player"))
        {
            _player.Hit();
            destroyed = true;
        } else if (other.transform.CompareTag("Laser"))
        {
            Transform laserParent = other.transform.parent;

            _player.AddScore(_enemyScoreValue);

            if (laserParent)
            {
                Destroy(laserParent.gameObject);
            } else
            {
                Destroy(other.gameObject);
            }
            destroyed = true;
        }

        if (destroyed)
        {
            this.gameObject.SetActive(false);
            _spawnManager.EnemyDestroyed(this.gameObject);
        }
    }
    void PickRandomHorizontalPosition()
    {
        float randomXPosition = Random.Range(_leftMinX, Mathf.Abs(_leftMinX));
        transform.position = new Vector3(randomXPosition, _topMaxY + _offsetPositionY, 0f);
         
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < _offscreenY)
        {
            PickRandomHorizontalPosition();
        }
    }
}

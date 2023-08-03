using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private float _offsetPositionY = 2f;

    private float _leftMinX;
    private float _topMaxY;
    private float _offscreenY;

    // Start is called before the first frame update
    void Start()
    {
        Camera cam = Camera.main;

        Vector3 point = cam.ScreenToWorldPoint(new Vector3(0f, Screen.height, cam.transform.position.z));
        _leftMinX = -1 * point.x;
        _topMaxY = -1 * point.y;
        _offscreenY = (_topMaxY + transform.localScale.y) * -1;

        PickRandomHorizontalPosition();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
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

using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float xOffset;
    [SerializeField] private float startTime;
    private float _transformPositionX;
    private bool _changeDirection;

    void Start()
    {
        _transformPositionX = transform.position.x;
        InvokeRepeating("ChangeDirection", 1f, 1f);
    }

    private void ChangeDirection()
    {
        _changeDirection = !_changeDirection;
        _transformPositionX = transform.position.x;
    }

    private void FixedUpdate()
    {
        //GOVNOCODE
        if (!_changeDirection &&
            transform.position.x >= _transformPositionX - xOffset &&
            transform.position.x <= _transformPositionX + xOffset)
        {
            transform.Translate(Vector3.left * speed, Space.Self);
        }
        else if (_changeDirection &&
                 transform.position.x <= _transformPositionX + xOffset &&
                 transform.position.x >= _transformPositionX - xOffset)
        {
            transform.Translate(Vector3.right * speed, Space.Self);
        }
        //GOVNOCODE_END
    }
}
using System;
using UnityEngine;

public class Bucket : MonoBehaviour
{
    private BucketCatch _bucketCatch;

    private float _moveRange = 3.55f;
    private float _moveSpeed = 1.5f;
    private Vector3 _startPosition;

    private void Awake()
    {
        _bucketCatch = GetComponentInChildren<BucketCatch>();

        _startPosition = transform.position;
    }
    
    public void Update()
    {
        float x = Mathf.PingPong(Time.time * _moveSpeed, 7.1f) - 3.55f;
        transform.localPosition = new Vector3(x, _startPosition.y, _startPosition.z);
    }

    public void OnChildTriggerEntered(Collider2D col)
    {
        Debug.Log("Parent received trigger from child with: " + col.name);
    }
}

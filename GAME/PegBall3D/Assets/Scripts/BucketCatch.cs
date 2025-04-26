using System;
using UnityEngine;

public class BucketCatch : MonoBehaviour
{
    private Bucket parentBucket;

    private void Awake()
    {
        GetComponentInParent<Bucket>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        parentBucket?.OnChildTriggerEntered(col);
    }
    
}

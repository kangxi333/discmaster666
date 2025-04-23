using System;
using UnityEngine;


// TODO: change this back to an abstract class so that we can extend it to have different peg types

// TODO: add a prefab that allows the PegboardMaster script to instantiate all these pegs when it wishes

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Peg : MonoBehaviour
{

    private SpriteRenderer _spriteRenderer;
    public PegType PegType { get; }
    
    public bool IsHit { get; private set; }

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            Hit();
        }
    }

    protected virtual bool Hit()
    {
        if (IsHit) return true; // cancels early if already hit
        IsHit = true;
        
        // add scoring and addition to deletion list for manager here
        return false;
    }
    
}

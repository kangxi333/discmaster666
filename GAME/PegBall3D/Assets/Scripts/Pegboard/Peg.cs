using System;
using UnityEngine;


// TODO: change this back to an abstract class so that we can extend it to have different peg types

// TODO: add a prefab that allows the PegboardMaster script to instantiate all these pegs when it wishes

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Peg : MonoBehaviour
{
    private readonly Color _defaultColor = new Color(0.25f, 0.85f, 0.85f);
    private readonly Color _activatedColor = new Color(1f,.2f,1f);

    private SpriteRenderer _spriteRenderer;
    public PegType PegType { get; }
    
    public bool IsHit { get; private set; }

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = _defaultColor;
        Debug.Log("set color");
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("hit:" + other.gameObject.name);
        if (other.gameObject.CompareTag("Ball"))
        {
            Hit();
        }
    }

    private bool Hit()
    {
        if (IsHit) return true;
        IsHit = true;
        
        _spriteRenderer.color = _activatedColor;
        
        // add scoring and addition to deletion list for manager here
        return false;
    }
    
}

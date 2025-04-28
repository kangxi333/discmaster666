using System;
using UnityEngine;


// TODO: change this back to an abstract class so that we can extend it to have different peg types

// TODO: add a prefab that allows the PegboardMaster script to instantiate all these pegs when it wishes

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class BasePeg : MonoBehaviour
{

    private SpriteRenderer _spriteRenderer;
    public PegType PegType { get; protected set; }
    [SerializeField] public PegData PegData;
    public bool IsHit { get; private set; }

    public void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = PegData.pegSpriteNormal;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            Hit();
        }
    }

    protected virtual void Hit()
    {
        if (IsHit) return; // cancels early if already hit
        IsHit = true;
        
        _spriteRenderer.sprite = PegData.pegSpriteHit;
        GameMaster.Instance.PegboardMaster.RegisterHitPeg(this);
        
        // add scoring and addition to deletion list for manager here
    }
}

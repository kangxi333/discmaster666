using System;
using UnityEngine;

public enum PegType
{
    FillerPeg = 0,
    ObjectivePeg = 1,
    SpecialPeg = 2
}
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class PegBase : MonoBehaviour
{
    private readonly Color _defaultColor = new Color(0.25f, 0.85f, 0.85f);
    private readonly Color _activatedColor = new Color(1f,.2f,1f);

    private SpriteRenderer _spriteRenderer;

    public abstract PegType PegType { get; }
    
    public bool IsHit { get; private set; }

    protected virtual void Start()
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

    public virtual bool Hit()
    {
        if (IsHit) return true;
        IsHit = true;
        
        _spriteRenderer.color = _activatedColor;

        int score = PegScoreValues.PegScoreMap[PegType];
        
        // add scoring and addition to deletion list for manager here
        return false;
    }
    
}

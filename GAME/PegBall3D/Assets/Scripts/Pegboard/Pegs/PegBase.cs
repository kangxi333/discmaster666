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
    private readonly Color _defaultColor = new Color(0.85f, 0.85f, 0.85f);
    private readonly Color _activatedColor = new Color(1f,1f,1f);

    private SpriteRenderer _spriteRenderer;

    public abstract PegType PegType { get; }
    
    public bool IsHit { get; private set; }

    public void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        // _spriteRenderer.color = _defaultColor;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Ball"))
        {
            Hit();
        }
    }

    public virtual void Hit()
    {
        int score = PegScoreValues.PegScoreMap[PegType];

        if (!IsHit)
        {
            _spriteRenderer.color = _activatedColor;
        }
        
        // add scoring and addition to deletion list for manager here
    }
    
}

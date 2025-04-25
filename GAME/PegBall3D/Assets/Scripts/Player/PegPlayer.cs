using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PegPlayer : MonoBehaviour
{
    
    [Header("GameObject References")]
    [SerializeField] private GameObject _launcherPos;

    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private GameObject ballPrefab;

    private float _ballRadius;
    
    private Camera playerCamera;

    private float _dotSpacing = .15f;
    private float _maxDots = 20;
    private float _maxDistance = 4f;
    private LayerMask collisionMask;

    private Vector2 direction;
    
    private float _ballShotForce = 6f;

    private List<GameObject> dots = new List<GameObject>();

    private void Start()
    {
        collisionMask = LayerMask.GetMask("2D");

        playerCamera = Camera.main;
        // accounts for ball not having normalised scale
        _ballRadius = ballPrefab.transform.localScale.x * ballPrefab.GetComponent<CircleCollider2D>().radius;
    }

    // Update is called once per frame
    void Update()
    {
        direction = ((Vector2)Input.mousePosition - new Vector2(Screen.width / 2f, Screen.height)).normalized;

        UpdateAimDots();
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    public void Fire()
    {
        // if not waiting for previously shot ball to finish
        GameObject ball = Instantiate(ballPrefab, _launcherPos.transform.position, Quaternion.identity, transform);
        ball.GetComponent<Rigidbody2D>().AddForce(direction * _ballShotForce, ForceMode2D.Impulse);
    }
    
    private void UpdateAimDots()
    {
        foreach (var dot in dots)
        {
            Destroy(dot);
        }
        dots.Clear();

        Vector2 origin = _launcherPos.transform.position;
        Vector2 velocity = (direction * _ballShotForce)/_dotSpacing; // ← start with initial "direction" * force
    
        float distanceTravelled = 0f;
        Vector2 currentPos = origin;
        float sizeMultiplier = 1f;

        for (int i = 0; i < _maxDots; i++)
        {
            // Simulate the next step with current velocity
            RaycastHit2D hit = Physics2D.CircleCast(currentPos, _ballRadius, velocity.normalized, _dotSpacing, collisionMask);
            Vector2 nextPos;

            if (hit.collider != null)
            {
                nextPos = hit.point;
                SpawnDot(nextPos, sizeMultiplier);
                break; // stop at first hit
            }
            else
            {
                nextPos = currentPos + velocity.normalized * _dotSpacing;
                distanceTravelled += _dotSpacing;

                if (distanceTravelled > _maxDistance)
                    break;

                SpawnDot(nextPos, sizeMultiplier);
                currentPos = nextPos;
            }

            // Apply gravity to velocity (Y axis)
            velocity.y += Physics2D.gravity.y * _dotSpacing;

            sizeMultiplier *= 0.95f;
        }
    }

    private void SpawnDot(Vector2 position, float sizeMultiplier = 1f)
    {
        GameObject dot = Instantiate(dotPrefab, position, Quaternion.identity, transform);
        dot.transform.localScale *= sizeMultiplier;
        dots.Add(dot);
    }
}

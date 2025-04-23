using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PegPlayer : MonoBehaviour
{
    [FormerlySerializedAs("camera")]
    [Header("GameObject References")]
    private Camera playerCamera;
    [SerializeField] private GameObject _launcherPos;

    [SerializeField] private GameObject dotPrefab;
    private float _dotSpacing = .15f;
    private float _maxDots = 20;
    private float _maxDistance = 4f;
    private LayerMask collisionMask;

    private List<GameObject> dots = new List<GameObject>();

    private void Start()
    {
        collisionMask = LayerMask.GetMask("2D");

        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAimDots();
    }

    private void UpdateAimDots()
    {
        foreach (var dot in dots)
        {
            Destroy(dot);
        }
        dots.Clear();

        Vector2 origin = _launcherPos.transform.position;
        // direction from top center of screen to mouse position
        Vector2 direction = ((Vector2)Input.mousePosition - new Vector2(Screen.width / 2f, Screen.height)).normalized;
        
        
        float distanceTravelled = 0f;
        Vector2 currentPos = origin;

        float sizeMultiplier = 1f;

        for (int i = 0; i < _maxDots; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPos, direction, _dotSpacing, collisionMask);
            Vector2 nextPos;

            if (hit.collider != null)
            {
                nextPos = hit.point;
                SpawnDot(nextPos, sizeMultiplier);
                break; // stop at first hit
            }
            else
            {
                nextPos = currentPos + direction * _dotSpacing;
                distanceTravelled += _dotSpacing;

                if (distanceTravelled > _maxDistance)
                    break;

                SpawnDot(nextPos, sizeMultiplier);
                currentPos = nextPos;
            }

            sizeMultiplier *= .95f;
        }
    }

    private void SpawnDot(Vector2 position, float sizeMultiplier = 1f)
    {
        GameObject dot = Instantiate(dotPrefab, position, Quaternion.identity, transform);
        dot.transform.localScale *= sizeMultiplier;
        dots.Add(dot);
    }
}

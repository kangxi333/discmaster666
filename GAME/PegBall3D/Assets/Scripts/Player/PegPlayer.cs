// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.Serialization;
//
// public class PegPlayer : MonoBehaviour
// {
//     
//     [Header("GameObject References")]
//
//     [SerializeField] private GameObject ballPrefab;
//     [SerializeField] private GameObject _launcherPos;
//
//     private float _ballRadius;
//     
//     private Camera playerCamera;
//
//     private float _dotSpacing = .15f;
//     private float _maxDots = 20;
//     private float _maxDistance = 4f;
//     private LayerMask collisionMask;
//
//     private Vector2 direction;
//     
//     private float _ballShotForce = 6f;
//
//
//     private void Start()
//     {
//         collisionMask = LayerMask.GetMask("2D");
//
//         playerCamera = Camera.main;
//         // accounts for ball not having normalised scale
//         
//         // do this shit EVERY TIME you load a level (when you move the code over)
//         _ballRadius = ballPrefab.transform.localScale.x * ballPrefab.GetComponent<CircleCollider2D>().radius;
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         direction = ((Vector2)Input.mousePosition - new Vector2(Screen.width / 2f, Screen.height)).normalized;
//
//         UpdateAimDots();
//     }
//     
//
// }

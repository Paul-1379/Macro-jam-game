using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private BoxCollider2D groundCheck;
    [SerializeField] private LayerMask groundLayerMask;
    
    private float _currentXMovement;
    private bool _isGrounded;
    
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        ApplyXMovement();
    }

    private void ApplyXMovement()
    {
        _rb.AddForce(Vector2.right * (_currentXMovement * speed));
    }
    private void UpdateGroundCheck()
    {
        List<Collider2D> overlappingColliders = new();
        var contactFilter = new ContactFilter2D()
        {
            layerMask = groundLayerMask,
        };
        _isGrounded = groundCheck.Overlap(contactFilter, overlappingColliders) > 0;
    }
    public void OnMovement(InputAction.CallbackContext context)
    {
        _currentXMovement = context.performed? context.ReadValue<Vector2>().x : 0;
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        UpdateGroundCheck();
        if (_isGrounded)
        {
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}

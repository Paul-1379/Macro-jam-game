using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private BoxCollider2D groundCheck;
    [SerializeField] private LayerMask groundLayerMask;
    [Header("Mirror")]
    [SerializeField] private float mirrorMoveSpeed;
    [SerializeField] private float mirrorRotSpeed;
    [SerializeField, Range(0f, 1f)] private float mirrorMovementDamping;
    [SerializeField, Range(0f, 1f)] private float mirrorRotDamping;
    [FormerlySerializedAs("_dustParticles")]
    [Header("Particules")]
    [SerializeField] private ParticleSystem dustParticles;


    private Mirror CurrentMirrorSelected => GameManager.Instance.mirrors[_currentMirrorSelectedIndex];

    private int _currentMirrorSelectedIndex;
    private float _currentXMovement;

    private bool IsGrounded
    {
        get
        {
            UpdateGroundCheck();
            return _isGrounded;
        }
    }
    private  bool _isGrounded;
    private bool _mirrorMode;
    private Vector2 _currentMirrorMovement;
    private float _currentMirrorRotation;
    private Vector2 _mirrorMoveInput;
    private float _mirrorRotInput;
    
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_mirrorMode)
        {
            ApplyMirrorMovementsAndRotation();
        }
        else
        {
            ApplyXMovement();
        }
    }

    private void ApplyMirrorMovementsAndRotation()
    {
        CurrentMirrorSelected.Move(_currentMirrorMovement);
        CurrentMirrorSelected.Rotate(_currentMirrorRotation);

        if (_mirrorMoveInput != Vector2.zero)
        {
            _currentMirrorMovement = _mirrorMoveInput * mirrorMoveSpeed;
        }
        else
        {
            _currentMirrorMovement *= mirrorMovementDamping;
        }

        if (_mirrorRotInput != 0)
        {
            _currentMirrorRotation = _mirrorRotInput * mirrorRotSpeed;
        }
        else
        {
            _currentMirrorRotation *= mirrorRotDamping;
        }
    }

    private void ApplyXMovement()
    {
        if (!(_rb.linearVelocity.x < maxSpeed)) return;
        _rb.AddForce(Vector2.right * (_currentXMovement * speed));
        if (IsGrounded)
        {
            dustParticles.Play();
        }
    }
    private void UpdateGroundCheck()
    {
        List<Collider2D> overlappingColliders = new();
        var contactFilter = new ContactFilter2D()
        {
            layerMask = groundLayerMask,
            useTriggers = true
        };
        _isGrounded = groundCheck.Overlap(contactFilter, overlappingColliders) > 0;
    }
    public void OnMovement(InputAction.CallbackContext context)
    {
        _mirrorMoveInput = context.performed? context.ReadValue<Vector2>() : Vector2.zero;
            
        _currentXMovement = context.performed ? context.ReadValue<Vector2>().x : 0;
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        UpdateGroundCheck();
        if (IsGrounded)
        {
            Jump();
        }
    }
    public void OnMirrorModeToggle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        _mirrorMode = !_mirrorMode;
        CurrentMirrorSelected.IsEnabled = _mirrorMode;
    }

    public void OnMirrorSelectToggle(InputAction.CallbackContext context)
    {
        if (context.performed && _mirrorMode)
        {
            CurrentMirrorSelected.IsEnabled = false;
            _currentMirrorSelectedIndex =  (GameManager.Instance.mirrors.IndexOf(CurrentMirrorSelected) + 1) % GameManager.Instance.mirrors.Count;
            CurrentMirrorSelected.IsEnabled = true;
        }
    }
    public void OnMirrorRotation(InputAction.CallbackContext context)
    {
        _mirrorRotInput = context.performed? context.ReadValue<float>() : 0;
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && _mirrorMode)
        {
            CurrentMirrorSelected.ToggleMode();
        }
    }
    private void Jump()
    {
        if (!_mirrorMode)
        {
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    
}

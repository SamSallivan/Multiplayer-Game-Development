using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class NetworkedFpsController : NetworkBehaviour
{
    [Header("References")]
    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;
    private Transform _headTransform;
    public TMP_Text scoreText;
    public Camera camera;
    
    [Header("Values")]
    private Vector3 _moveInput;
    private Vector3 _lookInput;
    private float _horizontalRotation = 0f;
    private float _verticalRotation = 0f;
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> score = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server);
    
    [Header("Settings")]
    public float walkSpeed = 5.0f;
    
    public float jumpForce = 25.0f;
    
    public float lookSensitivity = 0.5f;
    
    
    public void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();
        _headTransform = transform.GetChild(0);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _horizontalRotation = transform.rotation.eulerAngles.y;
        _verticalRotation = _headTransform.rotation.eulerAngles.x;
    }

    public override void OnNetworkSpawn()
    {
        score.OnValueChanged += OnScoreChanged;
        
        if (IsOwner)
        {
            camera.enabled = true;
        }
    }

    public void OnScoreChanged(int oldValue, int newValue)
    {
        scoreText.text = score.Value.ToString();
    }

    private void OnEnable()
    {
        _playerInput.enabled = true;
        _playerInput.actions.FindAction("Jump").performed += Jump_performed;
    }

    private void OnDisable()
    {
        _playerInput.enabled = false;
        _playerInput.actions.FindAction("Jump").performed -= Jump_performed;
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        
        if (!isDead.Value)
        {
            InputUpdate();
            MoveUpdate();
            LookUpdate();
        }
    }

    private void InputUpdate()
    {
        _moveInput.x = _playerInput.actions.FindAction("Move").ReadValue<Vector2>().x;
        _moveInput.y = 0f;
        _moveInput.z = _playerInput.actions.FindAction("Move").ReadValue<Vector2>().y;
        _moveInput = _moveInput.normalized;
        
        _lookInput = _playerInput.actions.FindAction("Look").ReadValue<Vector2>();
    }

    private void MoveUpdate()
    {
        Vector3 velocity = transform.TransformDirection(_moveInput) * walkSpeed;
        _rigidbody.velocity = new Vector3(velocity.x, _rigidbody.velocity.y, velocity.z);
    }

    private void LookUpdate()
    {
        _horizontalRotation += _lookInput.x * lookSensitivity;
        _verticalRotation -= _lookInput.y * lookSensitivity;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -90f, 90f);
        
        transform.localRotation = Quaternion.Euler(0, _horizontalRotation, 0);
        _headTransform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
    }

    private void Jump_performed(InputAction.CallbackContext context)
    {
        if (!IsOwner)
        {
            return;
        }


        if (isDead.Value)
        {
            return;
        }

        if (Mathf.Abs(_rigidbody.velocity.y) <= 0.05f)
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

    }

    [Rpc(SendTo.Server)]
    public void DieRpc(bool value = true)
    {
        isDead.Value = value;
    }
    
    [Rpc(SendTo.Owner)]
    public void RespawnRpc()
    {
        _rigidbody.MovePosition(new Vector3(0f, 0f, 0f));
    }

}
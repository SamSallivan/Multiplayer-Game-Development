using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;
    private Transform _headTransform;
    public GameObject gameOverPanel;
    
    [Header("Values")]
    private Vector3 _moveInput;
    private Vector3 _lookInput;
    private float _horizontalRotation = 0f;
    private float _verticalRotation = 0f;
    private float shootCooldown = 0f;
    private float _health = 100f;
    private bool _isDead = false;
    
    [Header("Settings")]
    public float walkSpeed = 5.0f;
    
    public float jumpForce = 25.0f;
    
    public float lookSensitivity = 0.5f;
    
    public float shootCooldownSetting = 0.5f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnTranform;
    public float bulletSpeed = 25.0f;
    
    
    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();
        _headTransform = transform.GetChild(0);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _horizontalRotation = transform.rotation.eulerAngles.y;
        _verticalRotation = _headTransform.rotation.eulerAngles.x;
        
        _rigidbody.MovePosition(new Vector3(Random.Range(-25f, 25f), 0f, Random.Range(-25f, 25f)));
        shootCooldown = shootCooldownSetting;
    }

    private void OnEnable()
    {
        _playerInput.enabled = true;
        _playerInput.actions.FindAction("Fire").performed += Fire_performed;
        _playerInput.actions.FindAction("Jump").performed += Jump_performed;
        _playerInput.actions.FindAction("Respawn").performed += Respawn_performed;
    }

    private void OnDisable()
    {
        _playerInput.enabled = false;
        _playerInput.actions.FindAction("Fire").performed -= Fire_performed;
        _playerInput.actions.FindAction("Jump").performed -= Jump_performed;
        _playerInput.actions.FindAction("Respawn").performed -= Respawn_performed;
    }

    void Update()
    {
        if (!_isDead)
        {
            InputUpdate();
            MoveUpdate();
            LookUpdate();

            if (shootCooldown > 0)
            {
                shootCooldown -= Time.deltaTime;
            }
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

    private void Fire_performed(InputAction.CallbackContext context)
    {
        if (shootCooldown <= 0)
        {
            shootCooldown = shootCooldownSetting;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnTranform.position, bulletSpawnTranform.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
        }
    }

    private void Jump_performed(InputAction.CallbackContext context)
    {
        if (_rigidbody.velocity.y == 0)
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

    }
    
    private void Respawn_performed(InputAction.CallbackContext context)
    {
        if (_isDead)
        {
            Respawn();
        }
    }

    public void Damage(float damage)
    {
        _health -= damage;

        if (_health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        _isDead = true;
        _rigidbody.MovePosition(new Vector3(0f, -10f, 0f));
        
        _playerInput.actions.FindAction("Fire").performed -= Fire_performed;
        _playerInput.actions.FindAction("Jump").performed -= Jump_performed;
        
        gameOverPanel.SetActive(true);
    }
    
    public void Respawn()
    {
        _isDead = false;
        _rigidbody.MovePosition(new Vector3(Random.Range(-25f, 25f), 0f, Random.Range(-25f, 25f)));
        
        _health = 100f;
        
        _playerInput.actions.FindAction("Fire").performed += Fire_performed;
        _playerInput.actions.FindAction("Jump").performed += Jump_performed;
        
        gameOverPanel.SetActive(false);
    }

}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TwoDController : MonoBehaviour
{
    [Header("References")]
    private PlayerInput _playerInput;
    public Rigidbody2D _rigidbody;
    public GameObject playerVisual;
    public Transform ArrowTransform;
    public Collider2D hitboxCollider;
    public TMP_Text scoreText;
    
    [Header("Values")]
    public bool canMove = true;
    private Vector3 _moveInput;
    public bool _isDead = false;
    private bool respawned = false;
    [Space]
    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    //public bool AutoClimableRight;
    //public bool AutoClimableLeft;
    [Space]
    public int side = 1;
    public int wallSide;
    [Space]
    public bool isDashing;
    public bool hasDashed;
    public bool wallSlide;
    public bool wallJumped;
    public bool grounded;
    public bool jumpButtonHeld;
    public int score = 0;
    
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f;
    public float jumpForce = 25.0f;
    [Space]
    public bool diagonalDash;
    public float dashSpeed = 20;
    public float dashDetectionStartDistance = 1.25f;
    public float dashLag = 0.1f;
    public float slideSpeed = 5;
    [Space] 
    /*public float dashHitSize = 1.5f;
    public float dashHitRange = 2f;*/
    public float dashHitForce = 10f;
    public float dashStunTime = 1f;
    [Space]
    public float gravityScaler = 10;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    
    [Header("Collision Settings")]
    public Vector2 bottomOffset, rightOffset1, rightOffset2, leftOffset1, leftOffset2;
    public float collisionRadius = 0.25f;
    public LayerMask collisionLayer;
    //public float AutoClimbHeight1 = 0.5f;
    //public float AutoClimbHeight2 = -0.5f;

    
    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody2D>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        transform.position = Camera.main.GetComponent<CameraMove>().TargetRoom.RespawnTransform.position;
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        switch (players.Length)
        {
            case 1:
                playerVisual.GetComponent<SpriteRenderer>().color = Color.white;
                break;
            case 2:
                playerVisual.GetComponent<SpriteRenderer>().color = Color.black;
                break;
            case 3:
                playerVisual.GetComponent<SpriteRenderer>().color = Color.magenta;
                break;
            case 4:
                playerVisual.GetComponent<SpriteRenderer>().color = Color.green;
                break;
        }
    }

    private void OnEnable()
    {
        _playerInput.enabled = true;
        _playerInput.actions.FindAction("Fire").performed += Fire_performed;
        _playerInput.actions.FindAction("Jump").performed += Jump_performed;
        _playerInput.actions.FindAction("Jump").canceled += Jump_canceled;
        _playerInput.actions.FindAction("Dash").performed += Dash_performed;
    }

    private void OnDisable()
    {
        _playerInput.enabled = false;
        _playerInput.actions.FindAction("Fire").performed -= Fire_performed;
        _playerInput.actions.FindAction("Jump").performed -= Jump_performed;
        _playerInput.actions.FindAction("Jump").canceled -= Jump_canceled;
        _playerInput.actions.FindAction("Dash").performed -= Dash_performed;
    }
    
    void Update()
    {
        if (!_isDead)
        {
            InputUpdate();
            MoveUpdate();
            CollisionUpdate();
            VariableJumpUpdate();
            WallSlideUpdate();

            Vector3 targetPosition;
            if (_moveInput != Vector3.zero && !wallSlide)
            {
                targetPosition = new Vector3(_moveInput.x, _moveInput.y, 0) * dashDetectionStartDistance;
            }
            else
            {
                targetPosition = new Vector3(side, 0, 0);
            }
            ArrowTransform.localPosition = Vector3.Lerp(ArrowTransform.localPosition, targetPosition, Time.deltaTime * 10f);
            ArrowTransform.rotation = Quaternion.LookRotation(-ArrowTransform.localPosition, Vector3.up);
            
        }
        else if (_isDead && !respawned)
        {
            StartCoroutine(Respawn());
            respawned = true;
        }
        
        scoreText.text = score.ToString();
    }
    
    public IEnumerator Kill()
    {
        _isDead = true;
        canMove = false;
        playerVisual.SetActive(false);
        /*Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.05f);
        Time.timeScale = 1f;*/
        yield return null;
    }
    
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1.0f);
        /*Color tmp = GetComponent<SpriteRenderer>().color; 
        tmp.a = 0.25f;
        GetComponent<SpriteRenderer>().color = tmp;*/
        transform.position = Camera.main.GetComponent<CameraMove>().TargetRoom.RespawnTransform.position;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        _isDead = false;
        canMove = true;
        respawned = false;
        playerVisual.SetActive(true);
        
        StartCoroutine(DisableMovement(0.5f));
        
        //yield return new WaitForSeconds(0.75f);
        /*tmp.a = 1.0f;
        GetComponent<SpriteRenderer>().color = tmp;*/
    }

    private void InputUpdate()
    {
        _moveInput.x = _playerInput.actions.FindAction("Move").ReadValue<Vector2>().x;
        _moveInput.y = _playerInput.actions.FindAction("Move").ReadValue<Vector2>().y;
        _moveInput.z = 0;
        _moveInput = _moveInput.normalized;
    }

    private void MoveUpdate()
    {
        if (onGround && !grounded && !isDashing)
        {
            OnGrounded();
            grounded = true;
        }

        if (grounded && (!onGround || hasDashed))
        {
            grounded = false;
        }

        if (!canMove)
        {
            return;
        }
        Vector3 velocity = transform.TransformDirection(_moveInput) * walkSpeed;
        _rigidbody.velocity = new Vector2(velocity.x, _rigidbody.velocity.y);
        
        
        if (_rigidbody.velocity.x > 0.1f && _moveInput.x > 0)
        {
            side = 1;
        }
        if (_rigidbody.velocity.x < 0.1f && _moveInput.x < 0)
        {
            side = -1;
        }

    }
    
    void OnGrounded()
    {
        isDashing = false;
        hasDashed = false;
        wallJumped = false;
    }

    private void WallSlideUpdate()
    {
        if (!canMove){
            return;
        }
        
        if (onWall && !onGround)
        {
            if (_moveInput.x != 0)
            {
                wallSlide = true;

                bool pushingWall = false;
                if ((_moveInput.x > 0 && onRightWall) || (_moveInput.x < 0 && onLeftWall))
                {
                    pushingWall = true;
                    if (onRightWall)// && !isDashing)
                        side = -1;
                    if (onLeftWall)// && !isDashing)
                        side = 1;
                }
                
                float push = pushingWall ? 0 : _rigidbody.velocity.x;
                if (_rigidbody.velocity.y <= 0)
                {
                    _rigidbody.velocity = new Vector2(push, -slideSpeed);
                }
            }
        }
        else 
        { 
            wallSlide = false;
        }
    }
    
    private void CollisionUpdate()
    {
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, bottomOffset.y) + new Vector2(bottomOffset.x, 0), collisionRadius, collisionLayer)
                   || Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, bottomOffset.y) - new Vector2(bottomOffset.x, 0), collisionRadius, collisionLayer);

        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset1, collisionRadius, collisionLayer)
                     || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset2, collisionRadius, collisionLayer);
        //AutoClimableLeft = !Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(leftOffset1.x, 0) + new Vector2(0, AutoClimbHeight1), collisionRadius, collisionLayer)
                           //&& Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(leftOffset1.x, 0) + new Vector2(0, AutoClimbHeight2), collisionRadius, collisionLayer);
        
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset1, collisionRadius, collisionLayer)
                      || Physics2D.OverlapCircle((Vector2)transform.position + rightOffset2, collisionRadius, collisionLayer);
        //AutoClimableRight = !Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(rightOffset1.x, 0) + new Vector2(0, AutoClimbHeight1), collisionRadius, collisionLayer)
                            //&& Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(rightOffset1.x, 0) + new Vector2(0, AutoClimbHeight2), collisionRadius, collisionLayer);

        onWall = onRightWall || onLeftWall;
        wallSide = onRightWall ? -1 : 1;
    }
    
    private void VariableJumpUpdate()
    {
        if (!isDashing)
        {
            if (_rigidbody.velocity.y < 0)
            {
                _rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
                if (jumpButtonHeld)
                {
                    _rigidbody.gravityScale = gravityScaler * 0.25f;
                }
                else
                {
                    _rigidbody.gravityScale = gravityScaler * 0.75f;
                }
            }
            else if (_rigidbody.velocity.y > 0.1f && !wallSlide)
            {
                if (!jumpButtonHeld)
                {
                    _rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        
                }
                else if (jumpButtonHeld)
                {
                }
            }
            else if (!onWall)
            {
                _rigidbody.gravityScale = gravityScaler;
            }
        }
    }
    
    public IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    private void Fire_performed(InputAction.CallbackContext context)
    {
        /*if (_shootCooldown <= 0)
        {
            _shootCooldown = shootCooldownSetting;

        }*/
    }

    private void Jump_performed(InputAction.CallbackContext context)
    {
        jumpButtonHeld = true;
        
        if (!canMove){
            return;
        }
        
        if (onGround)
        {
            _rigidbody.velocity = Vector2.up * jumpForce;
        }
        else
        {
            if (onRightWall)
            {
                StartCoroutine(DisableMovement(0.15f));
                if (wallSlide)
                {
                    _rigidbody.velocity = (Vector2.up + Vector2.left / 2f) * jumpForce;
                }
                else
                {
                    _rigidbody.velocity = (Vector2.up / 2f + Vector2.left / 2f) * jumpForce;
                }
                side = -1;
                wallJumped = true;
            }
            else if (onLeftWall)
            {
                StartCoroutine(DisableMovement(0.15f));
                if (wallSlide)
                {
                    _rigidbody.velocity = (Vector2.up + Vector2.right / 2f) * jumpForce;
                }
                else
                {
                    _rigidbody.velocity = (Vector2.up / 2f + Vector2.right / 2f) * jumpForce;
                }
                side = 1;
                wallJumped = true;
            }

        }
    }

    private void Jump_canceled(InputAction.CallbackContext context)
    {
        jumpButtonHeld = false;
    }

    private void Dash_performed(InputAction.CallbackContext context)
    {
        if (!canMove){
            return;
        }
        
        if (!hasDashed)
        {
            hasDashed = true;
            _rigidbody.velocity = Vector2.zero;
            
            StartCoroutine(Dash(_moveInput.x, _moveInput.y));
        }

    } 
    
    IEnumerator Dash(float x, float y)
    {
        isDashing = true;
        _rigidbody.gravityScale = 0;
        hitboxCollider.enabled = true;
        GetComponent<TrailRenderer>().enabled = true;

        StartCoroutine(DisableMovement(0.25f));
        yield return new WaitForSeconds(0.1f);

        RaycastHit2D detection;
        
        if (diagonalDash && _moveInput != Vector3.zero && !wallSlide)
        {
            detection = Physics2D.Raycast(transform.position + new Vector3(x, y, 0).normalized * dashDetectionStartDistance, new Vector2(x, y), dashSpeed, collisionLayer);
        
            if (detection.collider)
            {
                _rigidbody.MovePosition(detection.point);
            }
            else
            {
                _rigidbody.MovePosition(transform.position + new Vector3(x, y, 0).normalized * dashSpeed);
            }
        }
        else
        {
            detection = Physics2D.Raycast(transform.position + new Vector3(side , 0) * dashDetectionStartDistance, new Vector2(side, 0), dashSpeed, collisionLayer);
        
            if (detection.collider)
            {
                _rigidbody.MovePosition(detection.point);
            }
            else
            {
                _rigidbody.MovePosition(transform.position + new Vector3(side, 0, 0).normalized * dashSpeed);
            }
        }
        
        /*RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position + Vector3.left * dashHitRange / 2, new Vector2(dashHitSize, dashHitSize), 0, Vector2.right, dashHitRange, collisionLayer);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.TryGetComponent<TwoDController>(out var player) && player != this)
            {
                Debug.Log(player);
                Vector3 direction = hit.collider.transform.position - transform.position;
                //player.StopCoroutine(player.disableMovementCoroutine);
                player.StartCoroutine(player.DisableMovement(dashStunTime));
                player._rigidbody.velocity = direction * dashHitForce;
                //player._rigidbody.AddForce(direction * dashHitForce, ForceMode2D.Impulse); 
            }
        }*/

        wallJumped = true;
        
        yield return new WaitForSeconds(0.1f);
        GetComponent<TrailRenderer>().enabled = false;
        _rigidbody.velocity = Vector2.zero;
        //_rigidbody.velocity = new Vector2(x, y) * 12.5f;
        
        yield return new WaitForSeconds(dashLag);
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.gravityScale = gravityScaler;
        isDashing = false;
        wallJumped = false;
        hitboxCollider.enabled = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(0, bottomOffset.y) + new Vector2(bottomOffset.x, 0), collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(0, bottomOffset.y) - new Vector2(bottomOffset.x, 0), collisionRadius);

        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset1, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset1, collisionRadius);

        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset2, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset2, collisionRadius);

        /*Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(rightOffset1.x, 0) + new Vector2(0, AutoClimbHeight1), collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(rightOffset1.x, 0) + new Vector2(0, AutoClimbHeight2), collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(leftOffset1.x, 0) + new Vector2(0, AutoClimbHeight1), collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(leftOffset1.x, 0) + new Vector2(0, AutoClimbHeight2), collisionRadius); */
        
        RaycastHit2D detection = Physics2D.Raycast(transform.position + new Vector3(_moveInput.x, _moveInput.y, 0).normalized * dashDetectionStartDistance, _moveInput, dashSpeed, collisionLayer);
        if (detection.collider)
        {
            Debug.DrawRay(transform.position + new Vector3(_moveInput.x, _moveInput.y, 0).normalized * dashDetectionStartDistance, _moveInput * dashSpeed, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position + new Vector3(_moveInput.x, _moveInput.y, 0).normalized * dashDetectionStartDistance, _moveInput * dashSpeed, Color.blue);
        }
    }

    /*public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.gameObject != hitboxCollider.gameObject)
        {
            Debug.Log(other.transform.gameObject);
            Vector3 direction = transform.position - other.transform.position;
            StartCoroutine(DisableMovement(dashStunTime));
            _rigidbody.velocity = direction * dashHitForce;
        }
    }*/
}

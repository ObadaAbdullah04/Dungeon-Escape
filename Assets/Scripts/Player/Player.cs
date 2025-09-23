using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public int Health { get; set; }

    public int diamonds;

    [SerializeField] private int health = 4;

    [SerializeField]private LayerMask _groundLayer;
    [SerializeField] private float _playerSpeed = 1;
    [SerializeField] private float _jumpForce = 5;
    [SerializeField] private float attackCoolDown = 0.75f;
    [SerializeField] private float jumpCooldown = 0.25f;

    private bool _isJumped = false;
    private bool _grounded = false;
    private bool _isFacingRight = true; // // Flip based on movement direction without using SpriteRenderer.flipX, and avoid bugs with scaling or child objects.
    private float _rayLength= 1f;
    private bool _canAttack = true;

    private Rigidbody2D _rb;
    private PlayerAnimation _playerAnimation;

    public bool isDead = false;

    private bool jumpPressed;
    private bool attackPressed;

    private Coroutine attackCooldownRoutine;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerAnimation = GetComponent<PlayerAnimation>();
        Health = health;

        UIManager.Instance.UpdateGemCount(diamonds);
        UIManager.Instance.OnJumpPressed += HandleJumpPressed;
        UIManager.Instance.OnAttackPressed += HandleAttackPressed;
    }

    void Update()
    {
        if (UIManager.Instance.isStarted == false) return;

        if (isDead) return;

        Movement();

        if ((attackPressed || Input.GetKeyDown(KeyCode.F)) && _canAttack && IsGrounded() && !_playerAnimation._anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            Attack();
        }
        attackPressed = false;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.ResetGame();
        }

    }
    void HandleAttackPressed()
    {
        if (_canAttack && IsGrounded() && !_playerAnimation._anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            attackPressed = true;
        }
    }

    void HandleJumpPressed()
    {
        if (!_isJumped)
        {
            jumpPressed = true;
        }
    }

    void Movement()
    {
        float move = Application.isMobilePlatform
            ? UIManager.Instance.GetJoystickHorizontal()
            : Input.GetAxisRaw("Horizontal");

        _grounded = IsGrounded(); // what does this code do?

        if (_playerAnimation._anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            move = 0;
            if (!_playerAnimation._swordAnimator.gameObject.activeSelf)
            {
                StartCoroutine(PauseAnimations());
            }
        }

        if (move != 0)
            Flip(move > 0); // true when moving right (1 > 0) // false when moving left (-1 > 0 is false)


        if (jumpPressed || Input.GetKeyDown(KeyCode.Space))
        {
            if (_grounded)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
                StartCoroutine(JumpCoolDown());
                _playerAnimation.Jump(true);
                AudioManager.Instance.PlayPlayerJump();
            }
            jumpPressed = false;
        }

        _rb.velocity = new Vector2(move* _playerSpeed, _rb.velocity.y);
        _playerAnimation.Move(move);
    }
    bool IsGrounded()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, _rayLength, _groundLayer.value);
        Debug.DrawRay(transform.position, Vector2.down*_rayLength, Color.green);

        if (hitInfo.collider!=null)
        {
            if (_isJumped==false)
            {
                _playerAnimation.Jump(false);
                return true;
            }
        }
        return false;
    }

    void Flip(bool isFacingRight)
    {
        if (_isFacingRight == isFacingRight)
            return;

        _isFacingRight = isFacingRight; // remembers the current direction your character is facing.
        Vector3 scale = transform.localScale;
        scale.x = isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void Attack()
    {
        _playerAnimation.Attack();
        AudioManager.Instance.PlayPlayerAttack();
        if (attackCooldownRoutine != null)
            StopCoroutine(attackCooldownRoutine);

        attackCooldownRoutine = StartCoroutine(AttackCoolDownCoroutine());
    }

    IEnumerator JumpCoolDown()
    {
        _isJumped = true;
        yield return new WaitForSeconds(jumpCooldown);
        _isJumped = false;
    }
    IEnumerator AttackCoolDownCoroutine()
    {
        _canAttack = false;
        yield return new WaitForSeconds(attackCoolDown);
        _canAttack = true;
    }

    IEnumerator PauseAnimations()
    {
        _canAttack = false;

        _playerAnimation._swordAnimator.gameObject.SetActive(false);

        if (attackCooldownRoutine != null)
            StopCoroutine(attackCooldownRoutine);

        yield return new WaitUntil(() => _playerAnimation._anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"));
        yield return new WaitForSeconds(_playerAnimation._anim.GetCurrentAnimatorStateInfo(0).length);

        _playerAnimation._swordAnimator.gameObject.SetActive(true);

        attackCooldownRoutine = StartCoroutine(AttackCoolDownCoroutine());
    }

    public void Damage()
    {
        Health--;
        UIManager.Instance.UpdateLives(Health);

        if (Health > 0)
        {
            AudioManager.Instance.PlayPlayerHit();
            _playerAnimation.Damage();
        }

        else if (Health <= 0)
        {
            StartCoroutine(Die());
        }
    }
    public IEnumerator Die()
    {
        isDead = true;
        AudioManager.Instance.PlayPlayerDie();
        _rb.velocity = new Vector2(0f, _rb.velocity.y);
        _playerAnimation.Die();
        yield return new WaitUntil(() => _playerAnimation._anim.GetCurrentAnimatorStateInfo(0).IsName("Die"));
        yield return new WaitForSeconds(_playerAnimation._anim.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(2f);
        UIManager.Instance.LoseGame();
        //Destroy(this.gameObject);
    }

    public void AddGems(int ammount)
    {
        diamonds += ammount;
        UIManager.Instance.UpdateGemCount(diamonds);
        AudioManager.Instance.PlayPlayerCollect();
    }
}

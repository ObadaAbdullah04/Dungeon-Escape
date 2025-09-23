using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public abstract class Enemy : MonoBehaviour, IDamageable
{
    public GameObject diamondPrefab;

    public int Health { get; set; }

    [Header("Enemy Settings")]
    [SerializeField] protected int enemyHealth;
    [SerializeField] protected float speed;
    [SerializeField] protected int gems;
    [SerializeField] protected Transform pointA, pointB;

    protected Vector3 currentTarget;
    protected Vector3 previousPosition;

    protected Animator anim;
    protected BoxCollider2D boxCollider2D;

    protected bool isFacingRight = true;
    protected bool hasFlipped = false;
    protected bool isInIdlePause = false;
    protected bool isHited = false;

    protected Coroutine flipCoroutine = null;

    [Header("Combat Settings")]
    [SerializeField] protected float combatRange; // Area
    [SerializeField] protected float distanceToAttack; // Attack Range

    protected enum EnemyState { Patrol, Combat }
    protected EnemyState currentState = EnemyState.Patrol;

    protected Player Player;

    public virtual void Init()
    {
        Health = enemyHealth;

        anim = GetComponentInChildren<Animator>();
        Player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        previousPosition = transform.position;
    }

    private void Start() => Init();

    private void Update()
    {
        //if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            boxCollider2D.enabled = false;
            speed = 0;
        }

        CalculateMovementSpeed();

        if (!isInIdlePause)
        {
            UpdateEnemyState();
            HandleState();
        }

        UpdateAnimator();
    }

    // FSM logic
    public virtual void HandleState()
    {

        switch (currentState)
        {
            case EnemyState.Patrol:
                Movement();
                break;
            case EnemyState.Combat:
                CombatMode();
                break;
        }
    }

    public virtual void UpdateEnemyState()
    {
        if (Player == null) return;

        float distance = Vector2.Distance(transform.position, Player.transform.position);

        if (distance <= combatRange)
        {
            currentState = EnemyState.Combat;
        }
        else if (currentState != EnemyState.Patrol)
        {
            StartCoroutine(WaitThenPatrolAgain());
        }
    }

    // Animator sync
    public virtual void CalculateMovementSpeed()
    {
        if (isInIdlePause)
        {
            anim.SetFloat("Speed", 0f);
            return;
        }

        float moveSpeed = ((transform.position - previousPosition).magnitude) / Time.deltaTime;
        anim.SetFloat("Speed", moveSpeed);
        previousPosition = transform.position;
    }

    public virtual void UpdateAnimator()
    {
        if (Player == null || Player.isDead)
        {
            anim.SetBool("InCombat", false);
            return;
        }

        float playerDistance = Vector2.Distance(transform.position, Player.transform.position);
        bool shouldBeInCombat = (currentState == EnemyState.Combat && playerDistance < distanceToAttack);

        anim.SetBool("InCombat", shouldBeInCombat);
    }


    // Movement logic
    public virtual void Movement()
    {
        if (Vector3.Distance(transform.position, pointA.position) < 0.01f)
        {
            currentTarget = pointB.position;
            hasFlipped = false;
        }
        else if (Vector3.Distance(transform.position, pointB.position) < 0.01f)
        {
            currentTarget = pointA.position;
            hasFlipped = false;
        }

        if (!hasFlipped && flipCoroutine == null)
        {
            flipCoroutine = StartCoroutine(IdleThenFlip());
        }

        transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
    }


    public virtual float GetIdleDuration()
    {
        Debug.LogWarning("You are using the default Idle duration which is 1 second, Change it!");
        return 1f;
    }

    public virtual void Flip(bool faceRight)
    {
        if (isFacingRight == faceRight) return;

        isFacingRight = faceRight;
        Vector3 scale = transform.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    // Combat logic
    public virtual void CombatMode()
    {
        if (Player == null) return;

        FacePlayer();
        shouldFollowPlayer();
    }

    public virtual void FacePlayer()
    {
        bool shouldFaceRight = Player.transform.position.x > transform.position.x;
        if (isFacingRight != shouldFaceRight)
        {
            Flip(shouldFaceRight);
        }
    }

    public virtual void shouldFollowPlayer()
    {
        float playerDistance = Vector2.Distance(transform.position, Player.transform.position);
        float minX = Mathf.Min(pointA.position.x, pointB.position.x);
        float maxX = Mathf.Max(pointA.position.x, pointB.position.x);

        // Clamp the target X to patrol boundaries
        float targetX = Mathf.Clamp(Player.transform.position.x, minX, maxX);
        Vector3 clampedFollowPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        // Movement logic with hit handling
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            isHited = true;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);

        if (distanceToPlayer > distanceToAttack && distanceToPlayer < combatRange && isHited)
        {
            transform.position = Vector3.MoveTowards(transform.position, clampedFollowPosition, 0f);
            isHited = false;
        }
        else if (distanceToPlayer > distanceToAttack && distanceToPlayer < combatRange && !isHited)
        {
            transform.position = Vector3.MoveTowards(transform.position, clampedFollowPosition, speed * Time.deltaTime);
        }

    }

    public IEnumerator IdleThenFlip()
    {
        isInIdlePause = true;

        yield return new WaitForSeconds(GetIdleDuration());

        Flip(currentTarget.x > transform.position.x);
        hasFlipped = true;
        flipCoroutine = null;
        isInIdlePause = false;
    }
    public IEnumerator WaitThenPatrolAgain()
    {
        hasFlipped = false;
        Flip(currentTarget.x > transform.position.x);
        yield return new WaitForSeconds(0.5f);
        currentState = EnemyState.Patrol;
    }

    public virtual void Damage()
    {
        Health--;
        if (Health > 0)
        {
            anim.SetTrigger("Hit");
        }

        else if (Health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public virtual IEnumerator Die()
    {
        anim.SetTrigger("Die");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Die"));
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);


        float spacing = 0.7f;

        List<float> spawnOffsets = new List<float>();

        int half = gems / 2;

        if (gems % 2 == 1)
        {
            // Odd: center at 0, then expand symmetrically
            for (int i = -half; i <= half; i++)
            {
                spawnOffsets.Add(i * spacing);
            }
        }
        else
        {
            // Even: skip 0, anchor around it
            for (int i = 0; i < gems; i++)
            {
                float offset = (i - half + 0.5f) * spacing;
                spawnOffsets.Add(offset);
            }
        }

        // Shuffle (optional for visual randomness)
        for (int i = 0; i < spawnOffsets.Count; i++)
        {
            int randIndex = Random.Range(i, spawnOffsets.Count);
            (spawnOffsets[i], spawnOffsets[randIndex]) = (spawnOffsets[randIndex], spawnOffsets[i]);
        }

        // Instantiate gems
        for (int i = 0; i < gems; i++)
        {
            float offsetX = spawnOffsets[i];
            Vector3 spawnPos = new Vector3
            (
                transform.position.x + offsetX,
                transform.position.y - 0.35f,
                transform.position.z
            );

            Instantiate(diamondPrefab, spawnPos, Quaternion.identity);
        }

        Destroy(this.gameObject);
    }
}


using System.Collections.Generic;
using UnityEngine;

public class SwordSkillController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private SpriteRenderer sr;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;

    private float freezeTimeDuration;
    private float returnSpeed;

    [Header("Bounce info")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTargets;
    private int targetIndex;

    [Header("Pierce info")]
    private int pierceAmount;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCoolDown;

    private float spinDirection;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration, float _returnSpeed)
    {
        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;

        if (pierceAmount <= 0)
            anim.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);
    }

    public void SetupBounce(bool _isBouncing, int _bounceAmount, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _bounceAmount;
        bounceSpeed = _bounceSpeed;
        enemyTargets = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCoolDown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCoolDown = _hitCoolDown;
    }

    public void ReturnSword()
    {
        rb.isKinematic = true;
        transform.parent = null;
        isReturning = true;
    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturning)
        {
            sr.sortingOrder = 1;
            anim.SetBool("Rotation", true);
            rb.velocity = Vector2.zero;
            float distance = Vector2.Distance(transform.position, player.transform.position);
            float speed = distance > 20 ? distance * 10 : returnSpeed;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, player.transform.position) < 1)
            {
                player.CatchTheSword();
            }
        }
        SpinLogic();
        BounceLogic();
    }

    private void StopWhenSpinning()
    {
        if (wasStopped) return;
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                spinTimer -= Time.deltaTime;

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCoolDown;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
                    foreach (var hit in colliders)
                    {
                        SwordSkillDamage(hit.GetComponent<Enemy>());
                    }
                }
            }
        }
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTargets.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTargets[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTargets[targetIndex].position) < .1f)
            {
                SwordSkillDamage(enemyTargets[targetIndex].GetComponent<Enemy>());
                targetIndex = (targetIndex + 1) % enemyTargets.Count;
                bounceAmount--;
                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (isReturning) return;

        SwordSkillDamage(collision.GetComponent<Enemy>());

        SetupForBounce(collision);
        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        if(enemy == null) return;
        enemy.Damage();
        enemy.FreezeTimeFor(freezeTimeDuration);
    }

    private void SetupForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTargets.Count == 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTargets.Add(hit.transform);
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {

        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTargets.Count > 0)
            return;

        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}

using UnityEngine;

public class SkeletonBattleState : EnemyState
{
    private GameObject player;
    private EnemySkeleton enemy;
    private int moveDir;
    public SkeletonBattleState(EnemySkeleton _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.gameObject;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;
            if (enemy.IsPlayerDetected().distance < enemy.attackDistance && CanAttack())
            {
                stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 7)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }

        moveDir = player.transform.position.x > enemy.transform.position.x ? 1 : -1;
        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);

    }

    private bool CanAttack() => Time.time >= enemy.lastAttacked + enemy.attackCoolDown;
}

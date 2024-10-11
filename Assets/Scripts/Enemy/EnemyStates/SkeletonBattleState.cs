using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBattleState : EnemyState
{
    private Transform player;
    private EnemySkeleton enemy;
    private int moveDir;
    public SkeletonBattleState(EnemySkeleton _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = enemy.IsPlayerDetected().collider.gameObject.transform;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected() && enemy.IsPlayerDetected().distance < enemy.attackDistance)
        {
            Debug.Log("Attacking");
            enemy.ZeroVelocity();
            return;
        }

        moveDir = player.position.x > enemy.transform.position.x ? 1 : -1;

        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }
}

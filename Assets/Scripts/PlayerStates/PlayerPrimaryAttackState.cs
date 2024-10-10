using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int comboCounter;

    private float lastTimeAttacked;
    private float comboWindow = 2f;

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if(comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow)
        {
            comboCounter = 0;
        }

        player.anim.SetInteger("ComboCounter", comboCounter);

        float attackDir = xInput != 0 ? xInput : player.facingDir;

        var movement = player.attackMovements[comboCounter];
        player.SetVelocity(movement.x * attackDir, movement.y);

        stateTimer = .1f * (comboCounter + 1);
    }

    public override void Exit()
    {
        base.Exit();
        comboCounter++;
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0)
        {
            player.ZeroVelocity();
        }

        if(triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}

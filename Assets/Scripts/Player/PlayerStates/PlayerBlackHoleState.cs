using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackHoleState : PlayerState
{
    private float flyTime = .4f;
    private bool skillUsed;

    private float defaultGravityScale;

    public PlayerBlackHoleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        skillUsed = false;
        stateTimer = flyTime;

        defaultGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = defaultGravityScale;
        player.MakeTransparent(false);
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            player.SetVelocity(0, 15);
        }
        else if (stateTimer < 0)
        {
            player.SetVelocity(0, -.1f);
            if (!skillUsed)
            {
                if (player.skill.blackHole.CanUseSkill())
                {
                    skillUsed = true;
                }
            }
        }

        if (player.skill.blackHole.SkillCompleted())
        {
            stateMachine.ChangeState(player.airState);
        }
    }
}

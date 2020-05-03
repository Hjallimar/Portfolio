using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main Author: Hjalmar Andersson

[CreateAssetMenu(menuName = "States/PlayerAirState")]
public class PlayerAirState : PlayerBaseState
{

    private float startFallingAnimation = 0f;
    private const float FallingAnimationTimer = 0.2f;
    private bool hasBeganFall = false;

    public override void Enter()
    {
        maxSpeed = 10f;
        Anim.SetBool("Jump", true);
        ChangeGravityScale();
    }

    public override void Update()
    {
        SetFallAnimation();

        ApplyGravity();
        AirFriction();
        base.Update();

        if (Grounded() && Input.GetButton("Sprint"))
            owner.TransitionTo<PlayerSprintState>();
        else if (Grounded())
        {
            owner.TransitionTo<PlayerGroundState>();
        }
    }

    /// <summary>
    /// Activates the falling animation after a time
    /// </summary>
    private void SetFallAnimation()
    {
        startFallingAnimation += Time.deltaTime;

        if (startFallingAnimation >= FallingAnimationTimer && hasBeganFall == false)
        {
            hasBeganFall = true;
            //Loop falling
            Anim.SetBool("Falling", true);
            Anim.SetBool("Jump", false);

        }
    }

    /// <summary>
    /// Increases the gravity while in air
    /// </summary>
    private void ChangeGravityScale()
    {
        gravity = 15.7f;
    }

    public override void Exit()
    {
        //land anim
        Jumped = false;
        JumpTimer = 0;
        Anim.SetBool("Jump", false);
        Anim.SetBool("Falling", false);
    }
}

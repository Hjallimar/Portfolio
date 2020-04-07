using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main Author: Hjalmar Andersson

[CreateAssetMenu(menuName = "States/PlayerSprintState")]
public class PlayerSprintState : PlayerBaseState
{
    public override void Enter()
    {
        MovementTimer = 0.5f;
        acceleration = 20f;
        maxSpeed = 8f;
        jumpForce = 5f;
        Debug.Log("Entering sprint state");
    }

    public override void Update()
    {
        MovementTimer += Time.deltaTime / 2;
        if (CurrentlyInteracting == false && PlayingCintematic == false)
        {
            MovementInput();
            JumpInput();
        }
        else
        {
            HorizontalDirection = 0;
            VerticalDirection = 0;
        }
        AnimationHandeling();
        CameraDirectionChanges();
        ProjectToPlaneNormal();
        if (Direction.magnitude == 0)
        {
            maxSpeed = 1.0f;
        }
        else
        {
            maxSpeed = 8.0f;
        }
        if (Jumped)
        {
            JumpTimer += Time.deltaTime;
            if(JumpTimer >= 0.5f)
            {
                Jumped = false;
                JumpTimer = 0f;
            }
        }
        else
        {
        ControlDirection();
        GroundDistanceCheck();
        }
        ApplyGravity();
        Accelerate(Direction);
        base.Update();
        if (CurrentlyInteracting == false && PlayingCintematic == false)
        {
            Interact();
        }
        if (Grounded() == false)
            owner.TransitionTo<PlayerAirState>();
        else if (!Input.GetButton("Sprint") && Input.GetAxisRaw("Sprint") == 0)
        {
            owner.TransitionTo<PlayerGroundState>();
        }
    }

    private void ChangeGravityScale()
    {
        if (Grounded() == true)
        {
            gravity = 0;
        }
        else
        {
            gravity = 100f;
        }
    }

    /// <summary>
    /// Controlls the animation blending.
    /// </summary>
    private void AnimationHandeling()
    {
        //if (VerticalDirection > MovementTimer)
        //{
        //    VerticalDirection = MovementTimer;
        //}
        //if (HorizontalDirection > MovementTimer)
        //{
        //    HorizontalDirection = MovementTimer;
        //}

        if (Velocity.magnitude < 0.09f)
        {
            Anim.SetFloat("Movement", 0);
        }
        else
        {    
            Anim.SetFloat("Movement", Velocity.magnitude / topSpeed );

        }
        //Anim.SetFloat("Horizontal", HorizontalDirection * (Velocity.magnitude / topSpeed));
    }


    public override void Exit()
    {
        maxSpeed = 3f;
    }
}

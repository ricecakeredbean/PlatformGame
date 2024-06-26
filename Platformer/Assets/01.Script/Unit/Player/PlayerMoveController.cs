using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : UnitMoveController<PlayerController>
{
    private bool IsClim;

    private void Climwall()
    {
        Vector2 velocity = unit.UnitRigidibody.velocity;
        unit.UnitRigidibody.velocity = new Vector2(velocity.x, velocity.y * 0.8f);
    }

    private bool WallJump()
    {
        Vector2 jumpDir = new Vector2(unit.JumpPower * -MoveDIr.x, unit.UnitRigidibody.gravityScale * unit.JumpPower * 2f);
        unit.UnitRigidibody.AddForce(jumpDir, ForceMode2D.Impulse);
        return true;
    }

    private bool IsGround()
    {
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position, -transform.up * unit.UnitRigidibody.gravityScale, 1.5f, LayerMask.GetMask("Platform"));
        return hitGround;
    }

    private bool IsFrontWall()
    {
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, MoveDIr, 0.75f, LayerMask.GetMask("Platform"));
        return hitWall;
    }

    private void UnitJump()
    {
        unit.UnitRigidibody.AddForce(transform.up * unit.UnitRigidibody.gravityScale * unit.JumpPower, ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (InputManager.Instance.IsJump)
        {
            if (IsClim)
            {
                WallJump();
                return;
            }
            if (!IsGround())
            {
                return;
            }
            UnitJump();
            IsJumping = true;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        IsClim = !IsGround() && IsFrontWall();
        if (IsClim)
        {
            Climwall();
        }
        IsJumping = !IsGround() && !IsClim;
    }

    protected override void AnimationUpdate()
    {
        unit.AnimatorController.SetAnimation(PlayerAnimatorController.RunCode, IsCanMove());
        unit.AnimatorController.SetAnimation(PlayerAnimatorController.JumpCode, IsJumping);
        unit.AnimatorController.SetAnimation(PlayerAnimatorController.ClimCode, IsClim);
    }

    protected override Vector2 GetMoveDirection()
    {
        return InputManager.Instance.MoveVec;
    }

    protected override bool IsCanMove()
    {
        return MoveDIr != Vector2.zero;
    }

    protected override void UnitMove()
    {
        transform.Translate(MoveDIr * Time.deltaTime * unit.Speed);
    }
}


public readonly struct MovementState
{
    public readonly bool FacingRight;
    public readonly bool IsDashing;
    public readonly bool IsGrounded;

    public MovementState(bool facingRight, bool isDashing, bool isGrounded)
    {
        FacingRight = facingRight;
        IsDashing = isDashing;
        IsGrounded = isGrounded;
    }
}

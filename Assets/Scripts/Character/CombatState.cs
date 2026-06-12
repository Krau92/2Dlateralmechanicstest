
public readonly struct CombatState
{
    public readonly bool IsAttacking;
    public readonly float HorizontalDrag;
    public readonly float VerticalDrag;
    public readonly float FallGravityMultiplier;

    public CombatState(bool isAttacking, float horizontalDrag, float verticalDrag, float fallGravityMultiplier)
    {
        IsAttacking = isAttacking;
        HorizontalDrag = horizontalDrag;
        VerticalDrag = verticalDrag;
        FallGravityMultiplier = fallGravityMultiplier;
    }
}

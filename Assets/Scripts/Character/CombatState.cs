
public readonly struct CombatState
{
    public readonly CombatPhase Phase;
    public readonly float HorizontalDrag;
    public readonly float VerticalDrag;
    public readonly float FallGravityMultiplier;

    public CombatState(CombatPhase phase, float horizontalDrag, float verticalDrag, float fallGravityMultiplier)
    {
        Phase = phase;
        HorizontalDrag = horizontalDrag;
        VerticalDrag = verticalDrag;
        FallGravityMultiplier = fallGravityMultiplier;
    }
}

using Game.Core.World;

namespace Game.Core.TurnResolver;

public sealed class ExecutionResult
{
    public Outcome         Outcome;
    public IBlockCondition Condition;   // valorizzato se Outcome == Blocked
    public GameTime        NewEta;      // valorizzato se Outcome == NeedsRecalc
    public string          Reason;      // per il log (Failed, NeedsRecalc)
    public WorldDelta      Delta;       // effetti sul mondo (risorse, posizioni, danni, ...)
}
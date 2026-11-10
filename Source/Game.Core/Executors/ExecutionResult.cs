using System.Collections.Generic;
using Game.Core.TurnResolver;
using Game.Core.World;

namespace Game.Core.Executors;

public sealed class ExecutionResult
{
    public Outcome         Outcome;
    public IBlockCondition Condition;
    public GameTime        NewEta;
    public string          Reason;
    public WorldDelta      Delta;
    public List<GameEvent> GeneratedEvents;
}
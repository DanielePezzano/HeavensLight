using Game.Core.TurnResolver;
using Game.Core.World;

namespace Game.Core.Executors;

public interface ICombatExecutor
{
    ExecutionResult Execute(GameEvent evt, IWorldState world);
}
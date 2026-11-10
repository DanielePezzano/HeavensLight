using System;
using Game.Core.TurnResolver;
using Game.Core.World;

namespace Game.Core.Executors;

public interface IEventExecutor
{
    ExecutionResult Execute(GameEvent evt, IWorldState world);
}

public sealed class CompositeEventExecutor : IEventExecutor
{
    private readonly IMovementExecutor    movement;
    private readonly IProductionExecutor  production;
    private readonly ICombatExecutor      combat;
    private readonly IGrowthExecutor      growth;

    public ExecutionResult Execute(GameEvent evt, IWorldState world) => evt.Type switch
    {
        EventType.Movement   => movement.Execute(evt, world),
        EventType.Production => production.Execute(evt, world),
        EventType.Combat     => combat.Execute(evt, world),
        EventType.Growth     => growth.Execute(evt, world),
        _ => throw new ArgumentOutOfRangeException()
    };
}
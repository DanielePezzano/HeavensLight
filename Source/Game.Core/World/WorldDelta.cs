using System.Collections.Generic;

namespace Game.Core.World;

public sealed class WorldDelta
{
    public List<IWorldMutation> Mutations { get; } = new();

    public void Add(IWorldMutation m) => Mutations.Add(m);
    public bool IsEmpty => Mutations.Count == 0;
}
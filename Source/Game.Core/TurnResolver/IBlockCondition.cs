using Game.Core.World;

namespace Game.Core.TurnResolver;

public interface IBlockCondition
{
    bool IsSatisfied(WorldState world);
    string Describe();
}
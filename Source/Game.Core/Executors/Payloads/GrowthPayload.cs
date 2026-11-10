namespace Game.Core.Executors.Payloads;

public sealed class GrowthPayload : IEventPayload
{
    public int ColonyId;
    public GrowthKind Kind;           // Population, Stability
}

public enum GrowthKind
{
    Population,
    Stability
}
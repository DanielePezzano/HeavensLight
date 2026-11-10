using Game.Core.TurnResolver;

namespace Game.Core;

public sealed class GameEvent
{
    public int           OrderId;
    public int           PlayerId;
    public int           EventSeq;
    public EventType     Type;
    public OrderPriority Priority;
    public GameTime      Eta;
    public EventState    State = EventState.Pending;

    // Payload serializzato
    public byte          PayloadDiscriminator;
    public string        PayloadJson;

    // Blocco e ricalcolo
    public IBlockCondition BlockCondition;
    public int           RecalcCount;
    public int           GenerationDepth;       // 0 = da ingestione
    public int           CycleCount;            // per ordini continuativi

    // Determinismo
    public int           TiebreakSeed;
}
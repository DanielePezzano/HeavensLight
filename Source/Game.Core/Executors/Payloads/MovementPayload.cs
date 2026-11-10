namespace Game.Core.Executors.Payloads;

public sealed class MovementPayload : IEventPayload
{
    public int FleetId;
    public int OriginLocationId;      // può essere un punto interno a un sistema
    public int DestinationLocationId;
    public int? RouteId;              // null se movimento interno al sistema
    public int? PortalId;             // se il movimento attraversa un portale
    public int RequiredFuel;          // calcolato alla generazione dell'evento
    public MovementPhase Phase;       // Departure, Transit, Arrival, InternalMove
}

public enum MovementPhase
{
    Departure,
    Arrival,
    Transit,
    InternalMove
}
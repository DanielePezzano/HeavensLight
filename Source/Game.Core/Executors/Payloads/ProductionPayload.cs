namespace Game.Core.Executors.Payloads;

public sealed class ProductionPayload: IEventPayload
{
    public int FacilityId;
    public int DepartmentId;
    public int RecipeId;
    public ProductionKind Kind;       // Batch o Assembly
    public int? AssemblyPhase;        // 0,1,2 per le soglie 50/30/20; null per Batch
    public int CyclesRequested;       // per Batch: quanti cicli nel mese
}

public enum ProductionKind
{
    Batch,
    Assembly
}
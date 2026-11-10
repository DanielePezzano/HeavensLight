using System.Collections.Generic;

namespace Game.Core.Executors.Payloads;

public sealed class CombatPayload : IEventPayload
{
    public int EngagementId;          // identifica lo scontro tra più round
    public int RoundIndex;            // 0..2 nel mese
    public List<int> AttackerFleetIds;
    public List<int> DefenderFleetIds;
    public CombatDoctrine AttackerDoctrine;
    public CombatDoctrine DefenderDoctrine;
}

public enum CombatDoctrine;
public enum DefenderDoctrine;
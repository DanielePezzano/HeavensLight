using Game.Core.TurnResolver;

namespace Game.Core.Services;

public interface IIntelligenceService
{
    // Può il giocatore osservare questa località a questo istante?
    // Sensori passivi, sonde, reti di sorveglianza.
    bool CanObserveLocation(int playerId, int locationId, GameTime when);

    // Può il giocatore osservare questo evento specifico?
    // Usa CanObserveLocation + capacità specifiche (spionaggio su ordini, intercettazioni).
    bool CanObserveEvent(int playerId, GameEvent evt);

    // Filtra il payload di un evento per il destinatario.
    // Rimuove i campi non osservabili PRIMA che il log raggiunga il client.
    string SanitizePayloadFor(int playerId, GameEvent evt);
}
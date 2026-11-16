# Decision Records — HeavensLight

Questo documento raccoglie le decisioni progettuali confermate, con il relativo contesto, le alternative effettivamente considerate e le conseguenze. Gli identificativi ADR sono stabili e indipendenti dal versionamento del GDD.

---

## ADR-001 — Soglia minima dell'efficienza effettiva nel calcolo dei PC

- **Data:** 2026-11-14
- **Stato:** Confermata
- **Ambito:** Economia e produzione — calcolo del fabbisogno di Punti Capacità
- **Riferimento GDD:** §4.3.6, *Calcolo individuale del fabbisogno di PC*

### Contesto

Il fabbisogno individuale di Punti Capacità era definito dalla formula:

`PC_necessari = ceil(LR_residuo / (E_g × E_l))`

La formula non definiva il comportamento nel caso in cui il prodotto dei coefficienti di efficienza globale e locale fosse nullo o negativo. Con denominatore nullo il calcolo è indefinito; con denominatore negativo il risultato non rappresenta un fabbisogno valido. La lacuna era quindi un problema di correttezza della specifica, non un parametro di bilanciamento ancora da determinare.

### Decisione

Si introduce un'efficienza effettiva minima pari a `0.01`, applicata al prodotto dei coefficienti globale e locale:

`E_eff = max(E_g × E_l, 0.01)`

`PC_necessari = ceil(LR_residuo / E_eff)`

La soglia si applica al prodotto `E_g × E_l`, non ai singoli coefficienti separatamente. L'efficienza effettiva usata nel calcolo non può quindi essere inferiore a `0.01`, indipendentemente dai modificatori applicati.

### Alternative considerate

- **Efficienza nulla e ordine in `Waiting` con condizione `EfficiencyZero`:** alternativa scartata. Avrebbe introdotto uno stato speciale dell'ordine per gestire un caso limite matematico, anziché garantire la validità del denominatore nella formula.
- **Vincolo di positività sui coefficienti con soglia minima:** soluzione inizialmente considerata nella forma di un limite minimo applicato ai coefficienti. La specifica finale applica invece il limite al loro prodotto, cioè all'efficienza effettiva che compare realmente al denominatore.

### Conseguenze

#### Positive

- Il denominatore della formula è sempre strettamente positivo.
- Il fabbisogno di PC rimane definito anche quando il prodotto originario dei coefficienti è nullo o negativo.
- Non è necessario introdurre `EfficiencyZero` come condizione di attesa per questo caso.

#### Compromessi e implicazioni

- L'efficienza effettiva non può mai scendere sotto `0.01`; anche in presenza di modificatori estremamente penalizzanti, il calcolo mantiene questo limite.
- La soglia è un invariante della specifica e non un modificatore autonomo o un parametro da applicare separatamente a `E_g` e `E_l`.
- Quando il prodotto dei coefficienti è inferiore a `0.01`, il fabbisogno viene calcolato usando `0.01` come denominatore effettivo.

### Note

La decisione risolve la lacuna matematica individuata tra le questioni aperte del GDD. Non definisce né modifica i valori ordinari dei coefficienti di efficienza o i relativi criteri di bilanciamento.

---

## ADR-002 — Introduzione del backlog canonico delle questioni aperte

- **Data:** 2026-11-16
- **Stato:** Confermata
- **Ambito:** Organizzazione e gestione della documentazione di progetto
- **Riferimenti:** `OPEN_QUESTIONS.md`; GDD §8; Regola_organizzazione_documentazione_HeavensLight.md

### Contesto

Le questioni ancora da definire erano raccolte nella sezione 8 del GDD
insieme a decisioni già confermate, proposte e voci già risolte. Questa
commistione rendeva meno chiara la distinzione tra specifica corrente e
lavoro progettuale ancora da svolgere e rendeva più difficile gestire
priorità e dipendenze.

### Decisione

La documentazione canonica di HeavensLight viene organizzata in quattro
documenti complementari:

- `GDD.md`: descrive il modello di gioco corrente.
- `DECISIONS.md`: conserva le decisioni confermate e le relative motivazioni.
- `CHANGELOG.md`: registra cronologicamente le modifiche.
- `OPEN_QUESTIONS.md`: gestisce il backlog delle questioni non risolte,
  le proposte non approvate, le priorità, le dipendenze e gli aspetti
  bloccanti.

La sezione 8 del GDD viene mantenuta come breve punto di accesso a
`OPEN_QUESTIONS.md`; il backlog dettagliato viene trasferito nel nuovo
documento. Le voci già confermate o risolte non vengono migrate come
questioni aperte. Priorità e dipendenze non documentate restano da
valutare e non vengono assegnate automaticamente.

### Alternative considerate

- **Mantenere il backlog dettagliato nel GDD:** soluzione non adottata,
  perché continuava a mescolare la descrizione del gioco con la gestione
  del lavoro progettuale.
- **Separare il backlog in un documento autonomo:** soluzione adottata,
  così da rendere espliciti priorità, dipendenze, stato e responsabilità.

### Conseguenze

- Il GDD rimane la fonte canonica per il funzionamento corrente del gioco.
- `OPEN_QUESTIONS.md` diventa lo strumento operativo per selezionare e
  affrontare le questioni ancora da definire.
- Gli identificativi `OQ-XXX` restano stabili anche quando cambiano
  priorità e ordine di lavorazione.
- Le questioni in stato `Decisa` restano tracciate fino all’integrazione
  nel GDD e negli altri documenti pertinenti; successivamente passano
  all’archivio delle questioni risolte.
- La migrazione iniziale non assegna priorità o dipendenze non ancora
  analizzate e non trasforma proposte in decisioni approvate.

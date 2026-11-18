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

---

## ADR-003 — Sottoprodotti opzionali nelle ricette

- **Data:** 2026-11-16
- **Stato:** Confermata
- **Ambito:** Economia e produzione — formato delle ricette
- **Riferimento GDD:** §4.3.5; OQ-001

### Contesto

Nell'ambito della definizione del catalogo completo delle ricette
(OQ-001) occorreva stabilire se le ricette potessero produrre, oltre
all'output principale, anche sottoprodotti. Il GDD indicava gli
eventuali sottoprodotti come da definire.

### Decisione

Le ricette hanno un output principale e possono prevedere uno o più
sottoprodotti soltanto in ricette specificamente scelte; non tutte le
ricette li hanno. Il formato base resta di 2–4 input e un output
principale.

### Alternative considerate

- **A. Nessun sottoprodotto (un solo output per ricetta):** non adottata.
  Offriva semplicità e nessuna regola aggiuntiva di stoccaggio.
- **B. Sottoprodotti opzionali, solo in ricette scelte:** adottata.
  Consente profondità industriale dove serve senza gravare sulle altre
  ricette.
- **C. Sottoprodotti in tutte le raffinazioni:** non adottata. Avrebbe
  appesantito catalogo e logistica.

### Conseguenze

- Le ricette senza sottoprodotti restano invariate nel formato.
- Servono regole specifiche per le ricette con sottoprodotti.

### Note e questioni collegate

Restano aperti, nell'ambito di OQ-001: quali ricette prevedono
sottoprodotti; regole di stoccaggio, smaltimento e volume; tipologia e
trasportabilità dei sottoprodotti; il resto del catalogo.


---

## ADR-004 — Relazioni territoriali, gruppi demografici e infrastrutture orbitali

- **Data:** 2026-11-18
- **Stato:** Confermata
- **Ambito:** Modello del mondo, territorialità, colonie, popolazione e infrastrutture
- **Riferimenti GDD:** §5.12; §6.9–§6.11

### Contesto

La definizione delle entità del mondo richiedeva di distinguere la
proprietà delle entità, il loro utilizzo e il controllo politico-militare,
oltre a chiarire come rappresentare pianeti condivisi o contesi,
popolazioni autoctone e infrastrutture orbitali.

### Decisione

- Gli imperi possiedono direttamente flotte, navi e installazioni; il
  pianeta natale iniziale è proprietà diretta dell'impero.
- La relazione di utilizzo è distinta dalla proprietà. I pianeti
  colonizzati sono utilizzati in modo esclusivo da un solo impero alla
  volta; sistemi stellari e rotte possono essere utilizzati da più
  imperi contemporaneamente.
- L'utilizzo di un pianeta non attribuisce automaticamente diritti
  esclusivi sull'intero spazio orbitale o sulle installazioni orbitali.
- Proprietà, utilizzo e controllo sono relazioni distinte.
- Le popolazioni sono rappresentate da gruppi demografici autonomi. La
  popolazione totale della colonia deriva dalla somma dei gruppi; origine
  (autoctona/immigrata) e affiliazione politica (imperiale/indipendente)
  sono attributi, non classi di entità separate.
- L'occupazione militare non elimina automaticamente i gruppi presenti.
  L'espulsione e la riconversione degli slot sono operazioni esplicite.
- Gli insediamenti neutrali sono comunità indipendenti, ma non sono
  automaticamente imperi autonomi completi. I corpi disabitati restano
  distinti dalle popolazioni autoctone e dagli insediamenti neutrali.
- I pianeti hanno slot di sviluppo suddivisi tra abitazione, industria,
  ricerca, commercio e socialità. La capacità dipende dalla dimensione
  ed è modificata da tecnologie e fattori sociali globali.
- Le installazioni orbitali sono concettualmente unificate e
  specializzate almeno in stazioni scientifiche, di assemblaggio,
  logistiche civili e logistiche militari.

### Alternative considerate

Nel corso della discussione è stata valutata la possibilità che più
imperi utilizzassero lo stesso pianeta. È stata scelta l'esclusività
planetaria, mantenendo invece condivisibili sistemi stellari e rotte.
È stata inoltre preferita la rappresentazione delle popolazioni
autoctone tramite lo stesso tipo di entità dei gruppi demografici
immigrati, distinguendole tramite attributi.

### Conseguenze

- Il modello del mondo deve rappresentare separatamente entità e
  relazioni di proprietà, utilizzo e controllo.
- La conquista militare non comporta una sostituzione automatica della
  popolazione residente.
- Il totale demografico della colonia è un valore derivato.
- La capacità di sviluppo planetario può variare senza cancellare
  automaticamente le strutture e gli slot già occupati.
- Le infrastrutture orbitali non sono implicitamente incluse nei diritti
  di utilizzo del pianeta sottostante.

### Questioni collegate

Restano da definire il calcolo numerico degli slot e la gestione delle
variazioni di capacità (OQ-052), gli effetti operativi delle relazioni
territoriali e dell'accesso orbitale (OQ-053), gli attributi completi
dei gruppi e le procedure di espulsione/riconversione (OQ-056), nonché
frequenza e distribuzione delle popolazioni autoctone e neutrali
(OQ-028).

---

## ADR-005 — Modello delle informazioni e dell'intelligence

- **Data:** 2026-11-18
- **Stato:** Confermata
- **Ambito:** Informazioni, osservazione e intelligence
- **Riferimento GDD:** §1.3

### Contesto

Il modello di intelligence doveva chiarire quali informazioni sugli
altri imperi possono essere ottenute e come rappresentare la differenza
tra stato effettivo del mondo e conoscenza del singolo osservatore.

### Decisione

Lo stato reale del mondo è unico. Ogni impero dispone di una propria
rappresentazione della conoscenza, filtrata dalle capacità di
osservazione e intelligence. Le categorie informative comprendono
rotte utilizzate da altri imperi, composizione delle flotte e relativi
armamenti, stato delle colonie nemiche, tecnologie e posizione/funzione
delle installazioni.

La scala generale di intelligence è articolata in cinque livelli,
indicizzati da 0 a 4: nessuna osservazione utile; rilevazione di
presenza/attività; identificazione di tipologia/funzione; analisi di
composizione, stato coloniale o tecnologie pertinenti; dettaglio
approfondito, come armamenti e componenti. La scala è comune come
riferimento, ma le soglie effettive possono differire tra categorie.
Le informazioni osservate devono poter essere distinte in base alla
loro attualità.

### Conseguenze

- Non vengono create copie delle entità del mondo per ciascun
  osservatore: varia la conoscenza disponibile, non l'identità
  dell'entità reale.
- Le informazioni possono avere livelli di dettaglio differenti e
  riferibilità temporale.
- Le regole di condivisione tra alleati e le soglie specifiche per
  categoria devono essere definite separatamente.

### Questioni collegate

OQ-054 — soglie per categoria informativa, aggiornamento e decadimento
delle informazioni.

---

## ADR-006 — Autonomia delle navi e ammiraglia della flotta

- **Data:** 2026-11-18
- **Stato:** Confermata
- **Ambito:** Flotte e unità navali
- **Riferimento GDD:** §7.1

### Contesto

Occorreva stabilire se una nave potesse esistere al di fuori di una
flotta e quale fosse il comportamento della flotta in caso di perdita
della nave di riferimento.

### Decisione

Una nave può esistere e operare indipendentemente da una flotta. La
flotta è un raggruppamento di navi e, alla creazione, riceve
un'ammiraglia selezionata dal giocatore. Se l'ammiraglia viene perduta,
la flotta si dissolve e le navi superstiti tornano indipendenti.

### Conseguenze

L'appartenenza a una flotta deve essere modellata come relazione
modificabile e non come requisito di esistenza della nave. La perdita
dell'ammiraglia comporta la dissoluzione del raggruppamento, non la
perdita automatica delle altre navi.

### Questioni collegate

OQ-055 — comportamento in caso di distacco volontario, trasferimento o
sostituzione dell'ammiraglia.

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

---

## ADR-007 — Gestione temporale della conoscenza e osservazione mediante sonde

- **Data:** 2026-11-18
- **Stato:** Confermata
- **Ambito:** Intelligence, osservazione e aggiornamento simulativo dei sistemi
- **Riferimenti GDD:** §1.3; §3.6.2; §5.13
- **Decisione collegata:** estende il modello generale definito in ADR-005.

### Contesto

ADR-005 ha stabilito la distinzione tra stato reale del mondo e
conoscenza posseduta da ciascun impero, ma lasciava da definire il
comportamento temporale dei singoli dati, il modo in cui le osservazioni
vengono acquisite e aggiornate e il rapporto tra sonde, attività dei
sistemi e aggiornamento delle posizioni dei corpi celesti.

### Decisione

La conoscenza di un impero conserva le informazioni acquisite anche
quando non sono più attuali. Il trascorrere del tempo non cancella
automaticamente un dato. Le informazioni sono gestite per singolo
attributo e ciascun attributo mantiene un valore conosciuto, un livello
di dettaglio, una data di osservazione e una data di acquisizione.

La data di osservazione indica quando il dato è stato effettivamente
rilevato e determina la sua età; la data di acquisizione indica quando
è entrato nella conoscenza dell'impero. Un rapporto ricevuto in ritardo
non rende recente l'osservazione originaria.

Si distinguono tre comportamenti informativi:

- **Statico:** il dato non perde attualità per il solo trascorrere del
tempo; cambia nella conoscenza dell'impero soltanto a seguito di una
nuova osservazione del cambiamento reale.
- **Dinamico a variazione continua:** il valore reale varia nel tempo;
la conoscenza conserva il valore osservato, senza estrapolare
automaticamente un valore attuale.
- **Dinamico a variazione discreta:** il valore cambia in seguito a
eventi; un cambiamento non osservato non aggiorna la conoscenza.

Per ogni attributo viene conservato un solo valore conosciuto corrente.
Una nuova osservazione lo sostituisce soltanto se ha una data di
osservazione più recente e aggiorna esclusivamente gli attributi
realmente rilevati. Il criterio per osservazioni discordanti con la
stessa data resta aperto.

Il sistema non assegna punteggi di affidabilità alle fonti e non
classifica automaticamente la freschezza in stati discreti. Il
riferimento temporale è esposto tramite la data di osservazione e la
valutazione dell'attualità è lasciata al giocatore. La prima versione
non genera stime o estrapolazioni di valori attuali; tale capacità è
rinviata a uno sviluppo futuro.

Le sonde hanno una portata distinta dal livello di intelligence. La
portata e il dettaglio informativo sono capacità indipendenti e possono
essere sviluppati separatamente. La copertura è una sfera 3D quando la
sonda opera all'interno di un sistema stellare e un cerchio 2D sul piano
galattico quando viaggia tra sistemi; durante il viaggio il cerchio è
centrato sulla posizione corrente della sonda e si muove con essa.

Una sonda in movimento osserva le aree attraversate, senza continuare ad
aggiornare le aree lasciate alle spalle. Una sonda stazionaria continua
a osservare la propria area di copertura. Alla distruzione della sonda,
le informazioni raccolte sono conservate con la data dell'ultima
osservazione. Per i dati dinamici, il tempo trascorso permette al
giocatore di valutarne l'attualità; i valori a variazione continua
possono diventare progressivamente meno rappresentativi, mentre quelli
a variazione discreta possono non riflettere eventi non osservati.
Nessun dato viene cancellato automaticamente.

L'attività di un sistema è uno stato operativo distinto dalla sua
conoscenza da parte degli imperi. Un sistema è attivo in presenza di
almeno una nave, struttura/installazione o colonia di proprietà di un
impero giocante, oppure quando ricade entro il raggio operativo di una
sonda appartenente a un impero giocante. L'attività governa
l'aggiornamento mensile delle posizioni dei corpi celesti: nei sistemi
inattivi il calcolo è differito e viene recuperato dal riferimento
temporale conservato quando il sistema torna attivo o diventa oggetto
di un'azione. Le posizioni dei sistemi stellari sulla mappa galattica
sono aggiornate mensilmente.

### Conseguenze

- Il valore conosciuto di un attributo non coincide necessariamente con
  il suo valore reale corrente; il dato conserva la propria data di
  osservazione.
- Un aggiornamento parziale non rinnova la data di attributi che la
  nuova osservazione non ha effettivamente rilevato.
- La ricezione di un rapporto e il momento dell'osservazione restano
  distinguibili.
- L'attività simulativa non deve essere usata come indicatore di
  scoperta o come canale implicito di divulgazione delle informazioni.
- Il differimento degli aggiornamenti locali è un'ottimizzazione del
  calcolo e non modifica il comportamento temporale del modello.

### Aspetti ancora da definire

Restano aperte le soglie di rilevamento per categoria informativa, le
regole di condivisione tra alleati, la frequenza precisa delle
osservazioni delle sonde stazionarie, gli attributi rilevabili dalle
diverse capacità di sensori e il trattamento di osservazioni
conflittuali con la stessa data. Questi aspetti sono tracciati in
OQ-054.



---

## ADR-008 — Entità radice `World` e avanzamento temporale

- **Data:** 2026-11-19
- **Stato:** Confermata
- **Ambito:** Modello del mondo e tempo di simulazione
- **Riferimento GDD:** §1.2.1; §2.1

### Contesto

La definizione dell'entità radice dell'universo simulato richiede di
distinguere lo stato del mondo dall'orchestrazione della partita e di
stabilire chi possiede il tempo di simulazione e come viene richiesto
un suo avanzamento. È inoltre necessario garantire che il mondo non
esponga come definitivo uno stato aggiornato soltanto in parte.

### Decisione

`World` rappresenta esclusivamente l'universo simulato ed è il
contenitore autorevole del suo stato. Comprende la struttura galattica,
tutti gli imperi esistenti e le entità che popolano l'universo. Le
relazioni tra queste entità non sono ricondotte necessariamente a una
gerarchia rigida: proprietà, controllo, appartenenza, utilizzo e
posizione restano relazioni distinte.

`World` possiede il tempo della simulazione, espresso in tick assoluti
interi. Il contesto della partita mantiene invece la responsabilità di
giocatori, turni, fasi di pianificazione e sincronizzazione; decide
quando richiedere l'avanzamento, senza trasferire a `World` la gestione
del ciclo della partita.

L'avanzamento utilizza un istante assoluto di destinazione, secondo
l'interfaccia concettuale `World.advanceTo(targetTick)`. `World`
determina l'intervallo da elaborare a partire dal proprio tempo
corrente e rifiuta una destinazione precedente.

L'avanzamento deve rispettare atomicità logica e coerenza temporale: gli
stati intermedi non sono esposti come stati autorevoli definitivi; il
tempo di `World` viene aggiornato soltanto al completamento di tutti
gli aggiornamenti previsti; un errore o un'interruzione non deve
lasciare `World` in uno stato autorevole parzialmente aggiornato.
L'implementazione può usare più fasi di elaborazione, purché garantisca
queste proprietà.

### Conseguenze

- Il tempo di simulazione appartiene a `World`; il tempo e il flusso dei
  turni appartengono al contesto della partita.
- Il chiamante specifica un istante assoluto, evitando ambiguità tra
  data di destinazione e durata relativa.
- Il modello deve impedire l'esposizione di stati intermedi come stato
  autorevole definitivo.
- I dettagli tecnici di staging, commit, recupero e persistenza
  dell'avanzamento non sono definiti da questa decisione.

---

## ADR-009 — Struttura del catalogo delle ricette

- **Data:** 2026-11-25
- **Stato:** Confermata
- **Ambito:** Economia e produzione — catalogo delle ricette
- **Riferimento GDD:** §4.3.4; §4.3.5

### Contesto

La questione OQ-001 (Catalogo completo delle ricette) chiedeva di
definire il catalogo delle ricette. Il suo contenuto è stato affrontato
per parti, registrate nel GDD come proposte provvisorie (v0.64–v0.67):
tipi di input delle ricette di raffinazione, dei componenti e degli
assemblaggi, assegnazione dei sottoprodotti e livello tecnologico
richiesto. Occorreva stabilire quale parte costituisse il catalogo vero
e proprio e chiudere la questione, separando ciò che resta da definire.

### Decisione

La struttura del catalogo è l'elenco di tutte le ricette con i relativi
input e output per tipo. Quantità, tempi, LR e bilanciamento economico
ne sono esclusi. Sono confermati, senza modifiche sostanziali rispetto
alle proposte provvisorie:

- le tabelle degli input di raffinazione (7 famiglie), dei componenti
  (6 categorie funzionali) e degli assemblaggi (5 scafi, moduli di
  stazione orbitale, infrastrutture);
- l'assegnazione dei sottoprodotti: Metalli strutturali → Composti
  chimici; Composti chimici → Combustibili raffinati; Combustibili
  raffinati → Composti chimici; nessun sottoprodotto per le altre
  ricette;
- i livelli tecnologici richiesti (L1–L5), senza nominare tecnologie
  specifiche, inclusa l'equivalenza tra livello del cantiere e livello
  tecnologico per gli scafi.

Le ricette restano modificabili in futuro tramite una nuova decisione.
L'elenco dei materiali individuali resta illustrativo e non confermato.

### Alternative considerate

- **Sottoprodotti:** nessun sottoprodotto, sottoprodotti opzionali in
  ricette scelte o sottoprodotti in tutte le raffinazioni; la scelta
  (opzionali) è documentata in ADR-003, di cui questa decisione conferma
  l'assegnazione per ricetta.
- **Ricette delle navi:** ricette fisse per classe oppure ricetta
  generata dal progetto (componenti negli slot più scafo). È stata
  adottata la seconda; nel catalogo restano cinque ricette di scafo, una
  per taglia.

### Conseguenze

- Il catalogo ha una struttura stabile su cui definire quantità, tempi e
  LR.
- Le ricette restano modificabili soltanto tramite una nuova decisione.
- Restano aperti l'elenco e le fonti dei materiali individuali, le
  regole sui sottoprodotti e l'albero tecnologico.

### Note e questioni collegate

OQ-001 è chiusa e archiviata. Collegate: OQ-052 e OQ-012 (quantità e
bilanciamento; valori di LR, come indicato nel GDD); OQ-057 (elenco e
fonti dei materiali individuali); OQ-058 (stoccaggio, smaltimento,
volume e trasportabilità dei sottoprodotti, ADR-003); OQ-059 (albero
tecnologico e tecnologie che sbloccano i livelli, collegata a OQ-039 e
OQ-046).

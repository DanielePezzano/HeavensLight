# Game Design Document — HeavensLight

**Last updated:** 20 novembre 2026  
**Versione:** 0.65  
**Stato:** documento di progettazione corrente

Questo documento descrive il modello di gioco corrente. Le decisioni confermate, le proposte e le questioni aperte sono indicate nelle sezioni pertinenti. Le questioni ancora aperte sono gestite in `OPEN_QUESTIONS.md`; la cronologia delle modifiche è mantenuta separatamente in `CHANGELOG.md`.

## Indice

1. [Visione e pilastri del gioco](#1-visione-e-pilastri-del-gioco)
2. [Capitolo 1 --- Tempo e sistema degli ordini](#2-capitolo-1-tempo-e-sistema-degli-ordini)
3. [Capitolo 2 --- Galassia e generazione della mappa](#3-capitolo-2-galassia-e-generazione-della-mappa)
4. [Capitolo 3 --- Risorse ed economia](#4-capitolo-3-risorse-ed-economia)
5. [Capitolo 4 --- Sistemi stellari e corpi celesti](#5-capitolo-4-sistemi-stellari-e-corpi-celesti)
6. [Capitolo 5 --- Colonie e popolazione](#6-capitolo-5-colonie-e-popolazione)
7. [Capitolo 6 --- Flotte e combattimento](#7-capitolo-6-flotte-e-combattimento)
8. [Gestione delle questioni aperte](#8-gestione-delle-questioni-aperte)
9. [Principi di progettazione da preservare](#9-principi-di-progettazione-da-preservare)
10. [Registro sintetico delle decisioni confermate](#10-registro-sintetico-delle-decisioni-confermate)
11. [Architettura tecnica del TurnResolver](#11-architettura-tecnica-del-turnresolver)

---

## 1. Visione e pilastri del gioco

Il progetto è un gioco strategico spaziale 4X, con una propria identità
e non una semplice replica di altri titoli del genere.

### 1.1 Modalità di gioco

-   Il gioco deve supportare sia il single-player sia il multiplayer.
-   Le partite possono includere giocatori umani e imperi controllati
    dall'intelligenza artificiale.

### 1.2 Struttura temporale e simultaneità

-   Il gioco è a turni.
-   Le azioni vengono pianificate dai giocatori e poi risolte dal
    **TurnResolver** (§2.3).
-   Gli eventi non sono mai realmente simultanei: sono ordinati in modo
    deterministico secondo parametri espliciti, a partire dall'ETA.
-   Un giocatore non può vedere le mosse pianificate dagli avversari.
-   La risoluzione avviene **dopo una barriera di sincronizzazione**:
    il TurnResolver non inizia finché **tutti** i giocatori attivi (umani
    e AI) non hanno confermato il proprio piano annuale.
-   Il TurnResolver **non legge le pianificazioni durante la
    risoluzione**. Le converte in eventi in coda una sola volta, in fase
    di ingestione (§2.3.4), all'inizio dell'anno. Da quel momento la
    coda persistente è l'unica fonte di verità.
-   La risoluzione dell'anno N è funzione deterministica di: stato del
    mondo a inizio anno N, coda persistente a inizio anno N, seed della
    partita. È riproducibile bit-per-bit.

### 1.2.1 Entità radice `World` e avanzamento della simulazione

`World` rappresenta esclusivamente l'universo simulato. È il contenitore
autorevole dello stato dell'universo e comprende la struttura galattica
(`Galaxy`), tutti gli imperi esistenti (`Empires`) e le entità che
popolano l'universo, tra cui sistemi stellari, corpi celesti, navi,
flotte, colonie, popolazioni e installazioni. L'appartenenza a `World`
non implica necessariamente una gerarchia rigida di contenimento: le
relazioni di proprietà, controllo, appartenenza, utilizzo e posizione
restano concetti distinti.

`World` possiede il riferimento temporale della simulazione, espresso
in tick assoluti interi secondo §2.1. Non gestisce giocatori, turni,
fasi di pianificazione o sincronizzazione multiplayer: questi aspetti
appartengono al contesto della partita. Quest'ultimo decide quando
richiedere un avanzamento; `World` esegue gli aggiornamenti necessari
per raggiungere l'istante richiesto.

L'interfaccia concettuale di avanzamento è basata su un istante assoluto
di destinazione:

`World.advanceTo(targetTick)`

`targetTick` identifica l'istante che `World` deve raggiungere. `World`
determina l'intervallo da elaborare a partire dal proprio tempo
corrente. Una destinazione precedente al tempo corrente viene
rifiutata.

L'avanzamento rispetta un requisito di **atomicità logica e coerenza
temporale**: gli stati intermedi di elaborazione non sono esposti come
stati autorevoli definitivi; il riferimento temporale di `World` viene
aggiornato soltanto quando tutti gli aggiornamenti previsti sono
completati. In caso di errore o interruzione, `World` non deve restare
in uno stato autorevole parzialmente aggiornato. L'implementazione può
articolare l'elaborazione in più fasi, purché garantisca questo
comportamento.

### 1.3 Informazioni e intelligence

La segretezza delle mosse avversarie è una regola generale, ma non
assoluta: tecnologie, spionaggio e reti di sorveglianza possono
consentire di ottenere informazioni sugli altri imperi, entro i limiti
delle capacità effettivamente sviluppate.

Il principio vale anche per il **log del TurnResolver** (§11.6): ogni
giocatore riceve un log già filtrato per le proprie capacità di
osservazione. Il log completo è disponibile solo in build di sviluppo e
non attraversa mai il confine di rete in multiplayer.

Il modello informativo distingue lo **stato reale del mondo** dalla
conoscenza posseduta da ciascun impero. Le entità del mondo esistono una
sola volta nello stato autorevole; ogni impero mantiene una propria
rappresentazione delle informazioni effettivamente acquisite, filtrata
dalle capacità di osservazione e intelligence. L'attività del sistema
di simulazione e la conoscenza del sistema da parte dell'impero sono
concetti distinti: l'aggiornamento interno del mondo non rende
automaticamente note le informazioni al giocatore.

Le informazioni ottenibili riguardano almeno:

- **Rotte utilizzate da altri imperi**, alleati o avversari: l'intelligence
  aumenta il dettaglio sulle attività rilevate.
- **Composizione delle flotte**: il livello di intelligence determina
  quali dettagli delle navi e dei loro armamenti sono visibili.
- **Stato delle colonie nemiche**: le informazioni sulle colonie nemiche
  richiedono capacità di intelligence; le regole di condivisione delle
  informazioni tra alleati restano da precisare.
- **Tecnologie** degli altri imperi.
- **Posizione e funzione delle installazioni**: il dettaglio conosciuto
  dipende dal livello di intelligence.

Scala generale di riferimento:

| Livello | Informazione ottenibile |
|---|---|
| 0 | Nessuna osservazione utile |
| 1 | Rilevazione di presenza o attività |
| 2 | Identificazione della tipologia e della funzione |
| 3 | Analisi della composizione, dello stato della colonia o delle tecnologie pertinenti |
| 4 | Dettaglio approfondito, per esempio armamenti e componenti |

La scala è un riferimento comune, non una soglia uniforme: i requisiti
specifici possono variare in base alla categoria informativa.

#### 1.3.1 Struttura temporale delle informazioni conosciute

La conoscenza di un impero conserva i dati acquisiti anche quando non
sono più attuali. Il trascorrere del tempo, da solo, non cancella un dato né ne modifica
il valore registrato. Per le informazioni dinamiche, l'età
dell'osservazione permette al giocatore di valutarne l'attualità: nei
dati a variazione continua il valore osservato può diventare
progressivamente meno rappresentativo; nei dati a variazione discreta
il valore resta quello osservato finché una nuova osservazione non
rileva un evento che lo ha modificato. Le informazioni statiche non
perdono attualità per il solo trascorrere del tempo.

Ogni informazione è gestita al livello del singolo **attributo** e
mantiene separatamente:

- il valore conosciuto;
- il livello di dettaglio acquisito;
- la **data di osservazione**, cioè quando il dato è stato effettivamente
  rilevato dalla fonte;
- la **data di acquisizione**, cioè quando il dato è entrato nella
  conoscenza dell'impero.

La data di osservazione determina l'età dell'informazione. La data di
acquisizione registra quando l'impero ne è venuto a conoscenza: la
ricezione ritardata di un rapporto non rende recente un'osservazione
avvenuta in precedenza.

Le informazioni sono distinte, ai fini del loro comportamento, in:

- **Statiche:** il dato non perde attualità per il solo trascorrere del
tempo. Può cambiare soltanto se cambia la caratteristica reale; se il
cambiamento non viene osservato, la conoscenza dell'impero conserva il
valore precedente.
- **Dinamiche a variazione continua:** il valore reale può variare
continuamente nel tempo, come una posizione orbitale o la posizione di
una flotta. La conoscenza conserva il valore osservato e la relativa
data, senza estrapolare automaticamente una posizione attuale.
- **Dinamiche a variazione discreta:** il valore resta invariato fino a
un evento che lo modifica, come un cambio di proprietà, composizione,
controllo, popolazione o stato di un'installazione. Un cambiamento non
osservato non aggiorna la conoscenza dell'impero.

Ogni attributo conserva un solo valore conosciuto corrente. Una nuova
osservazione sostituisce il valore precedente soltanto se la sua data di
osservazione è più recente. L'aggiornamento riguarda esclusivamente gli
attributi effettivamente rilevati: un'osservazione che aggiorna la
presenza di una flotta, ma non ne rileva la composizione, non rinnova la
data del dato relativo alla composizione. La gestione di osservazioni
con date uguali e valori discordanti resta da definire.

Il modello non assegna un punteggio di affidabilità alle fonti e non
classifica automaticamente le informazioni in stati discreti di
freschezza. Il giocatore valuta l'attualità del dato sulla base della
data di osservazione. Nella prima versione non vengono prodotti valori
stimati o posizioni attuali estrapolate da osservazioni precedenti; tale
funzionalità è rinviata a uno sviluppo futuro.

#### 1.3.2 Osservazione mediante sonde

Le sonde rilevano informazioni entro un raggio che dipende dal tipo di
sonda e dalle tecnologie sviluppate. **Portata** e **livello di
intelligence** sono capacità indipendenti, sviluppate attraverso
ricerche distinte: aumentare la portata non implica aumentare il
dettaglio delle informazioni rilevate, e viceversa.

La copertura dipende dalla modalità di operazione:

- **All'interno di un sistema stellare:** il raggio di rilevamento è una
  sfera tridimensionale.
- **Durante il viaggio tra sistemi:** il raggio di rilevamento è un
  cerchio bidimensionale sul piano galattico, centrato sulla posizione
  corrente della sonda e quindi mobile insieme ad essa.

Le prestazioni di rilevamento nelle due modalità possono essere
sviluppate separatamente. Sono quindi distinte le capacità tecnologiche
relative alla portata e all'intelligence in ambito locale/tridimensionale
e quelle relative alla portata e all'intelligence in ambito galattico/
bidimensionale.

Una sonda in movimento osserva le aree attraversate; le informazioni
acquisite restano nella conoscenza dell'impero con la data della
rispettiva osservazione, ma le aree già lasciate alle spalle non
continuano a essere aggiornate dalla sonda. Una sonda stazionaria
continua invece a osservare la propria area di copertura. Se viene
distrutta, le informazioni già raccolte sono conservate e la loro data
di ultima osservazione resta quella dell'ultimo rilevamento, coincidente
con il momento della distruzione. Da quel momento i dati restano conservati con la data dell'ultima
osservazione. Per i dati dinamici, il tempo trascorso permette al
giocatore di valutare l'attualità dell'informazione; i valori a
variazione continua possono diventare progressivamente meno
rappresentativi, mentre quelli a variazione discreta possono non
riflettere eventi non osservati. Nessun dato viene cancellato
automaticamente.

La frequenza precisa delle osservazioni effettuate da una sonda
stazionaria e le soglie di rilevamento per le singole categorie restano
da definire (OQ-054).
### 1.4 Logistica militare

La logistica deve avere un peso sostanziale. Le flotte non devono poter
combattere indefinitamente senza supporto: composizione delle flotte,
navi ausiliarie, rifornimenti e catene logistiche devono influire sulle
operazioni militari.

------------------------------------------------------------------------

## 2. Capitolo 1 --- Tempo e sistema degli ordini

### 2.1 Unità temporale

-   Un turno di gioco corrisponde a **un anno**.
-   Ogni anno è suddiviso in **12 mesi**.
-   Ogni mese è suddiviso in **1000 tick** interi. Un anno vale quindi
    **12.000 tick**.
-   La simulazione procede mese per mese: un mese viene risolto prima di
    passare al successivo.
-   Il tempo è rappresentato come **tick assoluti interi**:
    `tick = (anno × 12 + mese) × 1000 + sotto_istante`, con
    `sotto_istante` in `[0, 999]`. I confronti tra istanti sono confronti
    interi esatti, senza aritmetica floating-point.

### 2.2 Pianificazione annuale

-   Durante la fase di pianificazione, il giocatore organizza in
    anticipo le proprie azioni per tutti i 12 mesi dell'anno.
-   Gli ordini possono essere distribuiti liberamente nei diversi mesi.
-   Un mese può non contenere alcun ordine.
-   Gli ordini possono durare più mesi e proseguire oltre la fine
    dell'anno.
-   Prima della risoluzione, il piano annuale viene confermato e gli
    ordini diventano bloccati per l'anno in corso.
-   La conferma è soggetta a **barriera**: il TurnResolver attende che
    tutti i giocatori attivi abbiano confermato. Nessun giocatore vede i
    piani altrui prima della barriera.
-   Una volta superata la barriera, il piano è **immutabile** per l'anno
    in corso. Le uniche modifiche ammesse sono quelle previste per gli
    ordini persistenti (§2.10) e avvengono nella **fase di pianificazione
    successiva**, non durante la risoluzione.

### 2.3 Risoluzione del turno: TurnResolver [CONFERMATO]

Il turno è risolto da un componente dedicato, il **TurnResolver**.

-   Ogni azione messa a piano dai giocatori ha un **ETA**
    (tempo stimato di completamento o di arrivo), espresso in tick
    assoluti (§2.1).
-   Le azioni che non si completano nel turno corrente vengono inserite
    in una **coda persistente**. La coda è consultata **a ogni mese**
    (Fase B, §2.3.3), ma è **popolata** una sola volta all'anno, in fase
    di ingestione (§2.3.4).
-   Gli eventi non sono mai realmente simultanei: vengono **ordinati**
    secondo parametri precisi, a partire dall'ETA.
-   La risoluzione procede per mesi, completando la simulazione di
    ciascun mese prima di avviare quella del mese successivo.
-   Eventi con lo stesso ETA (stesso tick assoluto) sono considerati
    **concorrenti** e vengono ordinati con uno **spareggio
    deterministico** (§2.3.1). Il motore non deve avvantaggiare un
    giocatore per l'ordine interno di elaborazione.
-   I giocatori non vedono gli ordini degli avversari.

#### 2.3.1 Criteri di spareggio [CONFERMATO]

A parità di ETA (stesso tick assoluto) gli eventi sono ordinati
secondo questi criteri, applicati in sequenza come confronto
lessicografico totale:

1.  **EtaTick** crescente.
2.  **Tipo di evento**, nell'ordine: movimento → combattimento →
    produzione/costruzione → crescita.
3.  **Priorità dell'ordine** (§2.6), con la regola del flag di
    precedenza assoluta già definita.
4.  **TiebreakSeed**, intero derivato dal seed della partita e
    dall'identità dell'evento: `Hash(seed_partita, order_id, event_seq)`.
    Calcolato una sola volta alla creazione dell'evento e mai modificato.

La funzione di confronto è **totale**: due eventi distinti hanno sempre
un ordine definito, anche a parità di ETA, tipo e priorità. Il tipo di
evento è un **tie-breaker**, non un criterio primario: un movimento a
priorità Minima precede una produzione a priorità Critica **solo se
hanno lo stesso tick assoluto**. Se la produzione ha ETA precedente,
viene eseguita prima.

#### 2.3.2 Blocco persistente e ricalcolo ETA

Il modello distingue nettamente due situazioni, che la versione
precedente mescolava:

**Blocco persistente (flag di blocco).** Quando un evento non può essere
eseguito per una condizione esterna, viene marcato `Blocked` con un
riferimento alla condizione bloccante. L'evento **non viene ricalcolato**
e il suo ETA **resta invariato**. La condizione viene rivalutata
**all'inizio di ogni mese** (Fase A, §2.3.3) per tutti gli eventi
bloccati, indipendentemente dal mese del loro ETA. Quando la condizione
cade, l'evento torna `Pending` con l'ETA originale e rientra
nell'ordinamento cronologico senza ulteriori ricalcoli.

**Ricalcolo ETA.** Quando la condizione fisica dell'azione cambia in modo
che l'ETA originale non è più valido (rotta distrutta, percorso
alternativo, bersaglio spostato), l'evento **non va in `Blocked`**: viene
**ricalcolato** e riceve un nuovo `EtaTick` esplicito. Il giocatore è
notificato tramite il log (§11.6).

La distinzione è netta:

| Situazione | Stato | ETA | Ripresa | Notifica |
|---|---|---|---|---|
| Condizione esterna temporanea | `Blocked` | invariato | automatica a inizio mese | log `Unblocked` |
| Condizione interna (risorse) | `Waiting` | invariato | automatica a inizio mese | log `Unblocked` |
| Modifica fisica del percorso | `NeedsRecalc` | nuovo | immediata, con nuovo ETA | log `EtaRecalculated` |

**Limite di ricalcoli.** Ogni evento ha un contatore `RecalcCount`.
Superata la soglia configurabile (default: **3**), l'evento passa a
`Failed` con motivazione `RepeatedRecalc` ed è segnalato al giocatore.

#### 2.3.3 Loop mensile

Ogni mese è risolto in quattro fasi in sequenza rigida:

-   **Fase A — Rivalutazione condizioni.** Tutti gli eventi `Blocked` e
    `Waiting` vengono rivalutati, indipendentemente dal mese del loro
    ETA. Quelli la cui condizione è soddisfatta tornano `Pending` con
    ETA invariato e rientrano nella coda ordinata.
-   **Fase B — Estrazione eventi del mese.** Dalla coda persistente
    vengono estratti gli eventi `Pending` con ETA nel mese corrente e
    inseriti in una struttura ordinata secondo §2.3.1.
-   **Fase C — Esecuzione.** Gli eventi del mese vengono eseguiti in
    ordine, uno alla volta. L'esecuzione può modificare lo stato del
    mondo e quindi influenzare gli eventi successivi. Un evento che ne
    blocca un altro dello stesso mese causa il passaggio a `Blocked` del
    secondo, che non viene eseguito questo mese.
-   **Fase D — Chiusura.** Consolidamento degli stati `Done`/`Failed`/
    `Cancelled`, emissione del riepilogo mensile per ciascun giocatore
    (log filtrato, §11.6), avanzamento al mese successivo.

#### 2.3.4 Ingestione annuale

Prima della Fase A del primo mese dell'anno, il TurnResolver esegue
un'**ingestione atomica** una sola volta:

1.  **Merge dei template degli ordini continuativi** (§2.10, tipo B):
    modifiche, revoche e nuovi template decisi nella pianificazione.
2.  **Merge degli ordini incompleti** (§2.10, tipo A): modifiche e
    annullamenti decisi nella pianificazione.
3.  **Conversione dei nuovi ordini in `GameEvent`**, con calcolo del
    `TiebreakSeed` e inserimento nella coda persistente.

Da questo momento la coda è l'unica fonte di verità per la risoluzione
dell'anno. La coda **può crescere durante la Fase C** per effetto di
eventi figli generati dagli executor (round di combattimento, cicli di
ordini continuativi). Questo è compatibile con il determinismo, perché
la generazione dipende solo dallo stato e dai template, entrambi noti.

**Formato del merge [CONFERMATO].** Il merge è un elenco ordinato di
comandi `(order_id, operazione, payload)`, applicati per `order_id`
crescente. Operazioni: `Create`, `Modify`, `Cancel`, `Revoke`.
L'applicazione è atomica per comando: un comando non valido viene
scartato e registrato nel log, senza influire sugli altri. In caso di
più comandi sullo stesso ordine vince l'ultimo.

**Limiti anti-loop [CONFERMATO].**

-   `MaxChildrenPerEvent` (default 64) e `MaxEventsPerMonth` globale
    (configurabile).
-   Un ordine continuativo genera al massimo **1 ciclo per mese**
    (`CycleCount`). Se il ciclo successivo ricadrebbe nello stesso mese,
    viene posticipato al primo tick utile del mese seguente.
-   Il superamento dei limiti porta l'ordine a `Failed` con motivazione
    `LoopLimit`, con notifica al giocatore.

#### 2.3.5 Carburante richiesto dal movimento [CONFERMATO]

-   `RequiredFuel` è calcolato alla **generazione** dell'evento ed è
    usato per prenotazione ed ETA.
-   All'esecuzione viene **riverificata la disponibilità**: se il
    carburante manca, l'evento passa a `Waiting`.
-   Il costo non viene ricalcolato, salvo in caso di `NeedsRecalc`.

### 2.4 Tipologie e struttura degli ordini

Il sistema deve poter rappresentare ordini di diversa natura, tra cui:

-   **Puntuali:** eseguono un'azione singola.
-   **Continuativi:** rimangono attivi fino al completamento o alla
    revoca. **Generano cicli successivi al completamento del precedente**
    (§2.10, tipo B).
-   **Temporizzati:** sono programmati per iniziare in un determinato
    mese.
-   **Condizionali:** vengono eseguiti quando si verifica una
    condizione.
-   **Dipendenti da eventi:** si attivano in seguito al completamento di
    un altro ordine o al verificarsi di un evento.
-   **Piani concatenati:** uniscono più ordini in una sequenza
    operativa.

Un piano può includere istruzioni condizionali. Per esempio, una flotta
può ricevere un itinerario e istruzioni su come comportarsi se incontra
forze ostili.

Le reazioni delle unità devono basarsi esclusivamente sulle informazioni
che esse possono effettivamente conoscere. Una flotta potrebbe rilevare
la presenza di navi nemiche senza necessariamente conoscerne
composizione, forza o intenzioni.

### 2.5 Ordini temporaneamente impossibili

-   Se un ordine non può essere eseguito temporaneamente, non viene
    eliminato automaticamente.
-   Rimane in attesa (`Waiting`).
-   Le condizioni di esecuzione vengono rivalutate ogni mese (Fase A,
    §2.3.3).
-   Quando le condizioni tornano valide, l'ordine può riprendere secondo
    le proprie istruzioni e priorità, con ETA invariato.

**Distinzione `Waiting` vs `Blocked`:** `Waiting` è una sospensione
**interna** all'ordine (risorse insufficienti, capacità produttiva
saturo, spazio deposito pieno). `Blocked` è una sospensione **esterna**
(rotta chiusa, portale distrutto, flotta intercettata, vincolo logistico
sul complesso). Entrambi hanno ETA invariato e ripresa automatica a
inizio mese; differiscono solo per la natura della condizione.

**Impossibilità permanente.** Un ordine è permanentemente impossibile
(`Failed`) solo quando nessuna condizione futura può renderlo
eseguibile. Nel modello attuale i casi sono: destinazione distrutta,
tecnologia rimossa, `RepeatedRecalc` superato. Tutti gli altri casi
sono `Waiting` o `Blocked`.

### 2.6 Priorità degli ordini

Il giocatore può assegnare una priorità agli ordini. I livelli previsti
sono:

1.  Critica
2.  Alta
3.  Normale
4.  Bassa
5.  Minima

Quando le risorse effettive non bastano a soddisfare tutti gli impegni,
la priorità concorre a determinare quali ordini vengono serviti per
primi.

La priorità non può sottrarre risorse già prenotate da un altro ordine
valido. Non determina inoltre l'iniziativa militare o l'ordine di
risoluzione dei combattimenti.

**Regola confermata per la precedenza in caso di deficit di
risorse:** 1. prevale l'ordine con priorità più alta; 2. a parità di
priorità, prevale l'ordine con il mese di esecuzione previsto più
vicino; 3. gli ordini che non ottengono le risorse necessarie restano in
attesa e vengono rivalutati.

**Ulteriori spareggi:** se priorità e mese coincidono, si applica il
`TiebreakSeed` (§2.3.1).

### 2.7 Prenotazione, previsione e consumo delle risorse

#### Prenotazione sulla disponibilità prevista

Durante la pianificazione annuale, il sistema consente di prenotare sia
le risorse già disponibili sia quelle che si prevede di produrre prima
del momento in cui saranno necessarie.

-   Il sistema deve calcolare disponibilità attuali, entrate previste e
    impegni già assunti.
-   Una stessa risorsa futura non può essere prenotata da più ordini.
-   Le prenotazioni devono essere compatibili con il calendario previsto
    di produzione e consumo.
-   Se la produzione effettiva risulta inferiore alle previsioni, gli
    ordini possono restare in attesa per mancanza delle risorse
    necessarie.
-   Il deficit viene gestito applicando priorità e mese di esecuzione
    previsto, nell'ordine indicato nella sezione 2.6.

La prenotazione è distinta dal consumo: impegnare una risorsa non
significa consumarla immediatamente.

#### Consumo progressivo in tre fasi

Per le attività con durata e costo in risorse, il consumo è ripartito in
tre fasi:

  Fase             Quota del costo totale
  -------------- ------------------------
  Prima fase                          50%
  Seconda fase                        30%
  Terza fase                          20%
  **Totale**                     **100%**

Le tre fasi corrispondono, come principio generale, a tre parti della
durata prevista dell'attività. La quota viene consumata quando
l'attività raggiunge la relativa fase.

**Esempio:** una nave costa 300 minerali e richiede 12 mesi: - prima
fase: 150 minerali; - seconda fase: 90 minerali; - terza fase: 60
minerali.

Le quote non ancora consumate rimangono prenotate per l'ordine.

**Arrotondamento [CONFERMATO].** I costi sono unità intere. Le
quote della prima e della seconda fase sono arrotondate **per difetto**;
la terza fase assorbe il resto, così che il totale sia sempre esatto.
Le attività di durata inferiore a 3 mesi hanno un'unica fase al 100%.

### 2.8 Annullamento degli ordini e rilascio delle risorse

Quando un ordine viene annullato:

-   le risorse prenotate ma non ancora consumate vengono liberate
    immediatamente;
-   le risorse liberate tornano disponibili anche per altri ordini già
    pianificati nello stesso anno;
-   le risorse già consumate non vengono restituite automaticamente;
-   gli ordini che dipendono dall'ordine annullato vengono rivalutati e
    possono proseguire, restare in attesa oppure diventare non validi,
    secondo le condizioni aggiornate.

**Dipendenze [CONFERMATO].** Alla modifica o all'annullamento di un
ordine padre:

-   i dipendenti passano a `Waiting` con motivo `DependencyCancelled`;
-   se nella pianificazione successiva il giocatore non li riassegna o
    li annulla, diventano `Failed`;
-   una modifica che cambia l'ETA del padre rivaluta i dipendenti senza
    ricalcolarne l'ETA (la gestione resta quella di §2.3.2).

### 2.9 Interruzione e ripresa delle attività

È necessario distinguere chiaramente **interruzione** e
**annullamento**.

**Interruzione** - È una sospensione non definitiva. - L'ordine rimane
attivo. - Il progresso già raggiunto viene conservato. - Le risorse già
consumate rimangono contabilizzate. - Le risorse prenotate e non ancora
consumate restano riservate all'attività. - Alla ripresa, l'attività
continua dalla fase raggiunta, senza ripetere i consumi precedenti.

**Annullamento** - È la cessazione definitiva dell'ordine. - Le risorse
prenotate ma non ancora consumate vengono liberate immediatamente. - Le
risorse già consumate non vengono restituite automaticamente.

**Durata dell'interruzione** - Non è prevista una scadenza automatica. -
Un'attività può restare interrotta senza limite temporale. - Rimane in
tale stato finché il giocatore non la riprende, la modifica o la
annulla. - Le risorse non ancora consumate restano prenotate durante
l'interruzione.

### 2.10 Persistenza degli ordini tra anni

La versione precedente confondeva due nozioni distinte, che vanno
separate nel modello dati e nel comportamento.

#### Tipo A — Ordini incompleti che attraversano l'anno

Un ordine a durata lunga (es. assemblaggio di una nave da 12 mesi) che
non si conclude entro l'anno **continua** nella coda persistente,
mantenendo il proprio stato e il progresso raggiunto. Non si
"reinstanzia": prosegue.

-   Gli ordini incompleti vengono trasferiti automaticamente all'anno
    successivo tramite la coda persistente.
-   Mantengono il proprio stato e il progresso già raggiunto.
-   Le prenotazioni valide e non ancora consumate restano associate agli
    ordini.
-   Durante la nuova fase di pianificazione annuale, prima del blocco
    del piano, il giocatore può intervenire sugli ordini persistenti.

Per ciascun ordine di tipo A, il giocatore può: - lasciarlo invariato; -
modificarne i parametri consentiti dal tipo di ordine; - cambiarne la
priorità; - modificarne le condizioni operative; - annullarlo,
liberando immediatamente le risorse prenotate e non ancora consumate.

Le modifiche diventano effettive con la conferma del nuovo piano
annuale, tramite il merge di ingestione (§2.3.4).

#### Tipo B — Ordini continuativi auto-reistanzianti

Un ordine continuativo (es. una rotta permanente di rifornimento,
§4.6.5) è un **template** che genera cicli successivi. Al completamento
di un ciclo, il TurnResolver genera automaticamente il ciclo successivo,
finché il template resta attivo. **Non è un evento che dura: è un
template che produce eventi.**

-   Il template non è mai esso stesso nella coda: nella coda stanno solo
    gli eventi dei cicli concreti, ciascuno con proprio `EventSeq` e
    proprio `TiebreakSeed`.
-   Il ciclo successivo viene generato **al completamento del
    precedente**, non a cadenza fissa. Un ordine continuativo non è un
    ordine "a orologio".
-   La revoca dell'ordine (`IsContinuative = false`) **non interrompe il
    ciclo in corso**, che si completa normalmente. Impedisce solo la
    generazione del ciclo successivo.
-   Nella fase di pianificazione annuale, il giocatore può modificare il
    template. I cicli **già generati e in corso** restano come sono; i
    **cicli futuri** usano il nuovo template. La modifica non è mai
    retroattiva.

#### 2.10.1 Modifiche ammesse per categoria [CONFERMATO]

Le modifiche avvengono solo in pianificazione e hanno effetto tramite il
merge di ingestione (§2.3.4).

| Categoria | Modificabile | Non modificabile |
|---|---|---|
| Puntuale | priorità, annullamento | destinazione/oggetto dopo l'avvio |
| Incompleto (tipo A) | priorità, condizioni operative, annullamento; destinazione solo con ricalcolo ETA (`RecalcCount`+1) | risorse già consumate |
| Continuativo (tipo B) | intero template, con effetto sui soli cicli futuri | cicli già generati |
| Temporizzato | mese di avvio, se non ancora iniziato | |
| Condizionale / dipendente | condizione e priorità | |

**Nota:** un eventuale ordine "a orologio" (es. "ogni 1° del mese parte
un convoglio, indipendentemente dal rientro del precedente") sarebbe un
**terzo sottotipo**, non una variante del tipo B. Per ora non è
previsto.

------------------------------------------------------------------------

## 3. Capitolo 2 --- Galassia e generazione della mappa

I parametri ancora da definire per geometria, distribuzione regionale,
generazione dei sistemi, rotte, popolazioni autoctone ed esplorazione
sono tracciati in `OPEN_QUESTIONS.md` (OQ-022–OQ-029).

### 3.1 Morfologia generale

-   La galassia generata deve avere sempre una **struttura a spirale**.
-   In questa fase progettuale non sono previste altre morfologie
    galattiche, come galassie ellittiche o irregolari.
-   Il numero dei bracci è variabile e viene determinato casualmente
    nell'intervallo compreso tra **2 e 4**.
-   La forma concreta della spirale varia da una partita all'altra,
    pur mantenendo sempre riconoscibile la morfologia a spirale.

### 3.2 Generazione geometrica e distributiva

La generazione procedurale deve randomizzare entrambi i seguenti aspetti:

-   **Casualità geometrica:** variazioni nella configurazione della
    spirale, nella geometria e nella disposizione dei bracci, nonché
    nella posizione e nelle dimensioni degli ammassi.
-   **Casualità distributiva:** variazioni nella distribuzione dei
    sistemi stellari, nella dimensione e densità degli ammassi e nella
    presenza di regioni più rarefatte o di sistemi isolati.

La casualità deve produrre configurazioni differenti tra le partite,
senza compromettere la riconoscibilità della struttura a spirale.

Gli ammassi che, nel loro insieme, compongono i bracci non devono essere
distribuiti uniformemente. La loro posizione, dimensione e densità
possono variare da una regione all'altra. Possono quindi esistere
addensamenti locali, zone scarsamente popolate e sistemi isolati.

#### 3.2.1 Morfologia e distribuzione dei bracci

Le decisioni confermate per la morfologia dei bracci sono:

-   I bracci devono essere relativamente stretti e ben riconoscibili.
-   La densità stellare aumenta procedendo verso il nucleo galattico.
-   L'irregolarità della distribuzione stellare aumenta procedendo verso
    le regioni esterne.
-   La curvatura deve corrispondere a una spirale intermedia: i bracci
    sono chiaramente curvi, ma presentano un'apertura relativamente
    ampia. È ammessa una certa variazione casuale tra galassie.
-   I bracci non devono apparire regolari, continui o compatti.
-   Sono ammesse interruzioni anche ampie, biforcazioni e diramazioni
    locali. Un braccio principale può apparire composto da più segmenti
    stellari distinti.

Queste irregolarità locali non devono compromettere la leggibilità
dell'intera galassia: osservata a una scala sufficientemente ampia, la
struttura a spirale e la presenza di 2–4 bracci principali devono
rimanere chiaramente riconoscibili.

È quindi necessario distinguere due scale:

-   **Scala globale:** la geometria complessiva deve rendere evidente
    la struttura a spirale e i suoi bracci principali.
-   **Scala locale:** la distribuzione può essere discontinua e
    irregolare, con segmenti, vuoti, addensamenti, biforcazioni e
    diramazioni.

#### 3.2.2 Regioni inter-braccio e addensamenti secondari

Le regioni comprese tra i bracci principali non devono essere
semplicemente vuote o scarsamente popolate: devono mantenere una densità
stellare significativa. Occasionalmente possono essere presenti anche
sistemi isolati al di fuori dei bracci.

Nelle regioni inter-braccio possono comparire addensamenti secondari,
generati liberamente e non necessariamente collegati ai bracci
principali né collocati nelle loro immediate vicinanze. Tali
addensamenti:

-   devono poter essere riconosciuti come strutture autonome;
-   possono raggiungere dimensioni considerevoli ed essere ben visibili
    sulla mappa;
-   non devono tuttavia diventare così grandi o densi da competere
    visivamente con i segmenti dei bracci principali, che devono restare
    dominanti;
-   possono avere dimensioni piccole, medie o grandi, con differenze
    visibili ma non estreme;
-   devono avere forme prevalentemente irregolari e frammentate, non
    generalmente sferiche o compatte.

La frammentazione può manifestarsi attraverso distribuzioni
discontinue, contorni irregolari e concentrazioni di sistemi separate
da zone meno dense. Sono ammesse porzioni localmente più compatte o
tondeggianti, purché non costituiscano la forma dominante degli
addensamenti secondari.

Non è stata stabilita una proporzione numerica rigida tra addensamenti
piccoli, medi e grandi.

### 3.3 Numero complessivo di sistemi stellari

-   La dimensione prevista per una galassia è compresa tra **1.000 e
    2.000 sistemi stellari**.

Questo intervallo costituisce il riferimento progettuale attuale per il
numero complessivo di sistemi generati. Non implica che tutti i sistemi
siano abitabili, colonizzabili o già abitati.

### 3.4 Nucleo galattico

Il centro della galassia rappresenta la regione più antica e presenta
caratteristiche astronomiche specifiche:

-   elevata densità di sistemi stellari;
-   maggiore concentrazione di stelle molto antiche;
-   maggiore concentrazione di buchi neri rispetto alle altre regioni.

L'estensione del nucleo è compresa tra il **15% e il 20% del diametro
complessivo della galassia**.

Il nucleo deve essere nettamente più denso delle regioni circostanti.
L'estensione percentuale si riferisce al diametro della regione centrale
rispetto al diametro galattico, non alla percentuale della superficie
complessiva.

#### 3.4.1 Distribuzione dei sistemi tra le regioni [CONFERMATO]

Distribuzione indicativa dei sistemi stellari:

| Regione | Quota dei sistemi |
|---|---:|
| Nucleo galattico | ~50% |
| Regioni intermedie | ~35% |
| Regioni periferiche | ~15% |

-   Le percentuali sono valori di riferimento per la generazione, non
    quote rigide: la componente casuale può produrre scostamenti.
-   La **transizione tra nucleo e regioni intermedie è graduale**: la
    densità cala in modo continuo, senza un confine netto.

### 3.5 Gradiente radiale dell'età stellare

La composizione della popolazione stellare varia progressivamente
procedendo dal centro verso la periferia della galassia.

| Regione | Caratteristiche prevalenti |
|---|---|
| Nucleo galattico | Stelle molto antiche, elevata densità stellare e maggiore concentrazione di buchi neri |
| Regioni intermedie | Popolazione stellare mista, con stelle di età differenti |
| Regioni periferiche | Maggiore presenza relativa di stelle giovani e sistemi di formazione stellare |

Il gradiente radiale è una regola generale della generazione, non una
separazione assoluta. Sono ammesse eccezioni locali prodotte dalla
componente casuale della generazione.

La composizione astronomica e il valore economico o strategico dei
sistemi sono parametri distinti. L'età di una stella o la presenza di un
buco nero non determinano automaticamente la ricchezza economica o il
valore strategico del sistema.

#### 3.5.1 Effetti di gameplay delle regioni [CONFERMATO]

Le caratteristiche astronomiche delle regioni influenzano il gameplay
come **tendenze di generazione**, non come bonus o malus diretti:

| Regione | Tendenze |
|---|---|
| Nucleo | Ricco di buchi neri e materiali rari; pericoloso; pochi pianeti abitabili |
| Regioni intermedie | Condizioni miste |
| Periferia | Più pianeti abitabili e stelle giovani; meno risorse rare |

Restano valide le eccezioni locali (§3.5) e la distinzione tra
composizione astronomica e valore economico o strategico.

### 3.6 Distinzione tra composizione astronomica e colonizzabilità

Il numero complessivo dei sistemi stellari non coincide necessariamente
con il numero di sistemi utilizzabili dal giocatore.

Devono rimanere concettualmente distinti:
-   sistemi stellari esistenti;
-   sistemi esplorabili;
-   sistemi colonizzabili;
-   sistemi effettivamente abitati.

Non è stabilito che ogni sistema stellare debba contenere pianeti
abitabili o opportunità di colonizzazione.

#### 3.6.1 Stati dei sistemi [CONFERMATO]

-   **Esplorabile:** tutti i sistemi sono potenzialmente esplorabili, ma
    devono essere **scoperti con sonde o flotte**. All'inizio della
    partita ogni giocatore conosce solo il proprio sistema di partenza e
    i sistemi vicini.
-   **Colonizzabile:** un sistema è colonizzabile se contiene almeno un
    corpo celeste con i **requisiti minimi** per la colonizzazione
    (abitabile, oppure sfruttabile con la tecnologia disponibile). La
    colonizzabilità **dipende dalla tecnologia del giocatore**: lo
    stesso sistema può diventare colonizzabile con nuove tecnologie.
-   **Abitato:** un sistema è abitato se ospita una **colonia o un
    insediamento di qualsiasi impero**. Si distingue tra sistemi abitati
    da **giocatori** e da **popolazioni autoctone o neutrali**.

Gli stati non sono mutuamente esclusivi: un sistema può essere
contemporaneamente esplorabile, colonizzabile e abitato.

#### 3.6.2 Attività simulativa dei sistemi stellari

L'**attività** di un sistema stellare è uno stato operativo del motore
di simulazione e non una proprietà astronomica né un indicatore della
conoscenza che un impero possiede del sistema.

Un sistema è attivo se è soddisfatta almeno una delle seguenti
condizioni:

- è presente almeno una nave di proprietà di un impero giocante;
- è presente almeno una struttura o installazione di proprietà di un
  impero giocante;
- è presente almeno una colonia appartenente a un impero giocante;
- il sistema ricade entro il raggio operativo di una sonda appartenente
  a un impero giocante.

Quando nessuna di queste condizioni è soddisfatta, il sistema è
inattivo. L'attività determina la frequenza con cui vengono aggiornate
le posizioni dei corpi celesti al suo interno; non determina quali dati
siano conosciuti da un impero.

Un sistema può quindi essere attivo senza essere stato scoperto o senza
che un impero ne conosca la composizione; viceversa, può essere noto a
un impero e rimanere inattivo. L'attivazione dovuta alla copertura di
una sonda non comporta di per sé che tutte le informazioni del sistema
siano rilevate: la conoscenza dipende dalle capacità di osservazione e
intelligence (§1.3).

### 3.7 Movimento e collegamenti tra sistemi

Il movimento segue un **modello ibrido** su due scale.

#### 3.7.1 Movimento interno al sistema

-   All'interno di un sistema stellare le flotte si muovono
    **liberamente** tra i corpi celesti e le località del sistema.
-   I tempi sono espressi in **tick** (frazioni di mese): gli
    spostamenti interni si concludono spesso entro il mese.
-   Il consumo di carburante dipende dalla distanza percorsa e dal
    carico (§4.6.3).

#### 3.7.2 Rotte tra sistemi e portali

-   Oltre il confine del sistema si viaggia solo lungo **rotte** che
    collegano sistemi **vicini** (entro una distanza massima).
-   Una rotta può essere **aperta o chiusa** esclusivamente tramite
    **portali**.
-   Un giocatore può usare una rotta in uscita da un sistema solo se
    possiede lì un **portale**. Non serve un portale nel sistema di
    arrivo.
-   La **posizione del portale** nel sistema è irrilevante per la
    durata del viaggio tra sistemi, ma incide sul **consumo di
    carburante** necessario per raggiungerlo dal punto di partenza.
-   Il tempo di viaggio tra sistemi dipende dalla **lunghezza della
    rotta** ed è espresso in **mesi interi** (indicativamente da 1 a 6).

#### 3.7.3 Portali

-   Il portale è un **impianto costruito da un impero**.
-   È utilizzabile solo dal **proprietario** e dai suoi **alleati**.
-   Può essere **distrutto o conquistato**: la perdita del portale
    chiude la rotta per il giocatore che lo possedeva.
-   Ha **livelli di capacità da 1 a 5**: il livello determina quante
    navi (o quale tonnellaggio) possono transitare **ogni mese**. Il
    costo di costruzione cresce con il livello.

#### 3.7.4 Distanza delle rotte e visibilità

-   La **distanza massima delle rotte** è un **raggio fisso**, uguale in
    tutta la galassia. Di conseguenza il nucleo, più denso, offre molte
    rotte per sistema, mentre la periferia ne offre poche e più lunghe.
-   Le rotte sono **note solo se scoperte** (§3.6.1).
-   I portali avversari e i transiti sono visibili solo tramite
    **intelligence o sensori** (§1.3).

#### 3.7.5 Stati di una rotta

Per il modello del TurnResolver (§2.3.2) è necessario distinguere:

-   **Aperta:** percorribile dal proprietario del portale e dagli
    alleati.
-   **Chiusa:** il portale è spento, di livello insufficiente per la
    finestra di transito corrente, o conquistato da un terzo. La rotta
    **tornerà percorribile** quando la condizione cade. Gli eventi di
    movimento su rotta chiusa vanno in `Blocked` con ETA invariato.
-   **Distrutta:** la rotta non esiste più (portale distrutto
    irreparabilmente, sistema di partenza o arrivo distrutto). Gli
    eventi di movimento vanno in `NeedsRecalc` se esiste un percorso
    alternativo, altrimenti in `Failed`.

**Da definire:** valori numerici (raggio, costi dei portali, capacità
per livello, tempi di viaggio, soglie di distruzione delle rotte).

------------------------------------------------------------------------

## 4. Capitolo 3 --- Risorse ed economia

Questo capitolo raccoglie le regole confermate sulle risorse. I dettagli non ancora decisi sono tracciati in `OPEN_QUESTIONS.md` (in
particolare OQ-001–OQ-021).

### 4.1 Set di risorse

Il gioco utilizza **cinque risorse fisiche**:

-   Minerali
-   Energia
-   Cibo
-   Carburante
-   Materiali avanzati

### 4.2 Localizzazione fisica delle risorse

-   Le risorse sono **localizzate fisicamente**: ogni pianeta, stazione,
    deposito o nave cargo possiede le proprie scorte.
-   Non esiste un pool imperiale unico.
-   Per usare una risorsa in un luogo diverso da quello in cui si trova,
    deve essere trasportata.
-   Le prenotazioni descritte nella sezione 2.7 devono quindi riferirsi
    a risorse presenti, o previste, in una località determinata.

### 4.3 Filiera produttiva e trasformazione industriale

**Principio confermato:** l'economia deve essere organizzata come una
catena produttiva e logistica fisica. Le risorse e i materiali intermedi
non sono semplici valori astratti del bilancio imperiale: sono beni
localizzati, prodotti, immagazzinati, prenotati, trasportati e consumati
in località determinate.

La filiera produttiva viene articolata concettualmente in più livelli:

1. **Estrazione o raccolta:** acquisizione di risorse grezze da depositi
   naturali o fonti ambientali.
2. **Lavorazione e raffinazione:** trasformazione delle risorse grezze
   in materiali raffinati e prodotti intermedi.
3. **Produzione di componenti:** trasformazione dei materiali in
   componenti industriali specializzati.
4. **Assemblaggio e produzione finale:** utilizzo dei materiali e dei
   componenti per realizzare navi, armi, motori, moduli, strutture,
   infrastrutture e altri prodotti finiti.

I livelli rappresentano le fasi logiche della filiera. Non è ancora
stabilito se ogni fase debba sempre corrispondere a un edificio
indipendente o se alcune possano essere integrate nello stesso
complesso industriale.

**Principio confermato:** le cinque risorse fondamentali già definite
(Minerali, Energia, Cibo, Carburante e Materiali avanzati) non
costituiscono necessariamente l'intero inventario dei beni gestiti dal
gioco. La catena industriale può comprendere materiali raffinati,
semilavorati e componenti intermedi che sono a loro volta risorse
fisiche, stoccabili, prenotabili e trasportabili.

**Principio confermato:** il livello di dettaglio dei materiali segue
un approccio misto. Alcuni materiali possono essere rappresentati come
risorse individuali; altri possono essere raggruppati in famiglie
industriali. La distinzione deve essere mantenuta quando produce
conseguenze strategiche, tecnologiche, produttive o logistiche
significative, evitando dettagli che non aggiungono decisioni
interessanti al giocatore.

#### 4.3.1 Estrazione e fonti naturali [IMPOSTAZIONE DA SVILUPPARE]

L'estrazione richiede una combinazione coerente di deposito o fonte
naturale, strutture estrattive e tecnologia appropriata. L'impostazione
proposta distingue:

- **Deposito o fonte:** determina quali materiali possono essere
  ottenuti e in quale ambiente si trovano.
- **Miniera o installazione di raccolta:** consente di accedere al
  deposito e sfruttarlo.
- **Impianto e processo estrattivo:** permettono di recuperare il
  materiale dalla fonte.
- **Tecnologia estrattiva:** abilita lo sfruttamento di determinati
  ambienti, depositi o classi di risorse.

Le possibili localizzazioni considerate comprendono superfici di
pianeti, lune e asteroidi, oltre a installazioni o attività di raccolta
all'interno delle nebulose. L'estrazione sui giganti gassosi richiede
una tecnologia e impianti specifici, distinti dalle tecnologie minerarie
convenzionali.

L'accessibilità delle risorse dipende dalla tecnologia: tecnologie di
base consentono di estrarre risorse comuni da depositi superficiali
accessibili, mentre risorse più rare o ambienti più difficili possono
richiedere tecnologie estrattive più avanzate. L'estrazione in ambienti
particolari, come giganti gassosi e nebulose, non è automaticamente
abilitata dalla disponibilità di una miniera convenzionale.

Questa articolazione è una proposta di progettazione; devono ancora
essere definiti i tipi precisi di deposito, le strutture necessarie e la
progressione tecnologica.

#### 4.3.2 Raffinazione e materiali intermedi [IMPOSTAZIONE DA SVILUPPARE]

Le risorse grezze possono essere trasformate in materiali raffinati e
prodotti intermedi attraverso impianti industriali dedicati. I processi
richiedono materiali in ingresso, capacità produttiva e tecnologie
adeguate.

La raffinazione non coincide con l'estrazione: una tecnologia che
consente di raggiungere un deposito non abilita automaticamente tutti
i processi necessari a trasformarne il contenuto in materiali
utilizzabili.

Le tecnologie di lavorazione possono sbloccare progressivamente
processi più complessi, nuove famiglie di materiali e prodotti
intermedi più sofisticati. Questi ultimi possono diventare a loro volta
input di ulteriori processi industriali.

#### 4.3.3 Componenti e prodotti finiti [IMPOSTAZIONE DA SVILUPPARE]

I materiali raffinati e i prodotti intermedi vengono utilizzati per
produrre componenti industriali e, successivamente, prodotti finiti.
Tra gli esempi rientrano componenti meccanici, elettronici,
strutturali, sistemi energetici, armamenti, motori, moduli per stazioni
e astronavi complete.

La produzione di un bene dovrebbe essere descritta attraverso una
ricetta o un processo industriale che specifichi almeno:

- impianto o reparto richiesto;
- tecnologia necessaria;
- materiali e quantità in ingresso;
- tempo di lavorazione;
- capacità produttiva richiesta;
- prodotti in uscita ed eventuali sottoprodotti (previsti solo in
  ricette specificamente scelte, §4.3.5).

Questi elementi costituiscono una struttura di riferimento proposta;
non sono ancora stati definiti valori numerici, ricette o regole
complete.

#### 4.3.4 Inventario dei materiali e dei componenti [CONFERMATO]

**Famiglie di materiali raffinati (7):**

| Famiglia | Note |
|---|---|
| Metalli strutturali | Derivati prevalentemente dai Minerali |
| Metalli rari | Derivati da Minerali e Materiali avanzati |
| Composti chimici | Base per processi industriali e carburanti |
| Cristalli e semiconduttori | Base per elettronica e sensori |
| Combustibili raffinati | Derivati dal Carburante grezzo |
| Materiali energetici | Celle, reattori e sistemi di accumulo |
| Biomateriali | Derivati dal Cibo e dalla biosfera |

Le famiglie sono gruppi industriali: all'interno di una famiglia i
materiali possono essere trattati in modo aggregato. Le ricette
specifiche e le eventuali eccezioni individuali sono da definire.

Per impostazione predefinita le sette famiglie restano aggregate. Un
materiale è rappresentato individualmente soltanto se ha conseguenze
strategiche, tecnologiche, produttive o logistiche; la scelta conferma
l'approccio misto già descritto in §4.3. L'elenco definitivo dei
materiali individuali, le loro ricette, quantità, tempi, fonti di
approvvigionamento ed effetti meccanici sono da definire (OQ-001).

**[ILLUSTRATIVO, NON VINCOLANTE: OQ-001]** La tabella seguente descrive
a titolo illustrativo che cosa comprende ciascuna famiglia. Non è una
regola meccanica: in gioco resta un solo materiale aggregato per
famiglia; i materiali citati non diventano risorse distinte e non hanno
ricette, valori o stock propri.

| Famiglia | Comprende, a titolo illustrativo |
|---|---|
| Metalli strutturali | Ferro, alluminio, titanio e leghe da costruzione |
| Metalli rari | Platino, iridio, terre rare e metalli pregiati |
| Composti chimici | Acidi, polimeri, solventi e reagenti industriali |
| Cristalli e semiconduttori | Silicio, cristalli sintetici e substrati elettronici |
| Combustibili raffinati | Idrogeno e idrocarburi raffinati, propellenti |
| Materiali energetici | Celle, isotopi e materiali per accumulo e reattori |
| Biomateriali | Fibre, tessuti e composti organici di derivazione biologica |

**[ESEMPI ILLUSTRATIVI, ELENCO DA DEFINIRE: OQ-001]** I materiali
seguenti illustrano il tipo di eccezione individuale; non costituiscono
un elenco confermato, non hanno regole, valori o ricette associati e
sono al massimo dieci.

| Famiglia | Materiale | Ruolo indicativo | Provenienza |
|---|---|---|---|
| Combustibili raffinati | Celle di antimateria | Combustibile avanzato | Esempio dell'utente |
| Metalli rari | Vanadium-234 | Costruzione di armature | Esempio dell'utente |
| Biomateriali | Cellule estratte da specifici animali (specie da definire) | Rigenerazione di strutture biomeccaniche | Esempio dell'utente |
| Metalli strutturali | Neutronio compresso | Corazzature estreme | Proposta, non approvata |
| Cristalli e semiconduttori | Cristalli di risonanza | Sensori avanzati | Proposta, non approvata |
| Materiali energetici | Nuclei di fusione stabilizzati | Da definire | Proposta, non approvata |
| Composti chimici | Catalizzatore esotico | Da definire | Proposta, non approvata |
| Combustibili raffinati | Deuterio arricchito | Propulsione | Proposta, non approvata |
| Biomateriali | Fibre di biosintesi neurale | Supporto vitale | Proposta, non approvata |
| Cristalli e semiconduttori | Wafer quantistici | Da definire | Proposta, non approvata |

**Categorie funzionali di componenti (6):**

-   strutturali;
-   propulsione;
-   energia;
-   elettronica e sensori;
-   armamenti;
-   supporto vitale.

**Tipi di impianto (7):**

| Impianto | Funzione |
|---|---|
| Miniera | Estrazione e raccolta di risorse grezze |
| Raffineria | Trasformazione in materiali raffinati |
| Fabbrica di componenti | Produzione dei componenti funzionali |
| Cantiere/Assemblaggio | Produzione di navi, strutture e prodotti finiti |
| Centrale energetica | Produzione di Energia |
| Deposito/Magazzino | Stoccaggio delle risorse |
| Spazioporto | Interfaccia logistica con le flotte di trasporto |

**Potenziamento:** ogni impianto ha **livelli da 1 a 5**. Ogni livello
aumenta la capacità produttiva, con costo crescente.

**Vincoli condivisi:** gli impianti di una località sono limitati da
**energia** disponibile e **personale/popolazione locale**.

Valori numerici, ricette, costi dei livelli e consumi sono da definire.

#### 4.3.5 Ricette: formato e regole [FORMATO CONFERMATO, VALORI PROVVISORI]

Ogni ricetta è semplice e strutturata: da **2 a 4 input**, un output
principale, un impianto richiesto e una tecnologia richiesta.

**Sottoprodotti** ([ADR-003](DECISIONS.md#adr-003--sottoprodotti-opzionali-nelle-ricette))
**[CONFERMATO]**: le ricette hanno un output principale e possono
prevedere uno o più sottoprodotti soltanto in ricette specificamente
scelte; non tutte le ricette li hanno. Il formato base resta di 2–4
input e un output principale. Restano da definire (OQ-001): quali
ricette prevedono sottoprodotti, le regole di stoccaggio, smaltimento e
volume, la tipologia e la trasportabilità dei sottoprodotti.

**[PROPOSTA PROVVISORIA, NON VINCOLANTE: OQ-001]** Struttura degli input
delle ricette di raffinazione. Le ricette di raffinazione sono prodotte
nella Raffineria, hanno da 2 a 4 input e un output principale. La
tabella indica soltanto i tipi di input per famiglia: non definisce
quantità, tempi, LR, tecnologie richieste né sottoprodotti.

| Famiglia (output principale) | Tipi di input | Provenienza |
|---|---|---|
| Metalli strutturali | Minerali, Energia | Derivazione già indicata in §4.3.4 |
| Metalli rari | Minerali, Materiali avanzati, Energia | Derivazione già indicata in §4.3.4 |
| Combustibili raffinati | Carburante, Energia | Derivazione già indicata in §4.3.4 |
| Biomateriali | Cibo, Energia | Derivazione già indicata in §4.3.4 |
| Composti chimici | Minerali, Carburante, Energia | Ipotesi di lavoro, approvata come provvisoria |
| Cristalli e semiconduttori | Minerali, Materiali avanzati, Energia | Ipotesi di lavoro, approvata come provvisoria |
| Materiali energetici | Carburante, Materiali avanzati, Energia | Ipotesi di lavoro, approvata come provvisoria |

Restano da definire: tecnologie richieste, eventuali sottoprodotti per
ricetta, ricette di livello superiore (assemblaggi) e fonti dei
materiali individuali (OQ-001). Quantità di input e output e
bilanciamento economico delle ricette sono tracciati in OQ-052; i
valori di LR per ricetta in OQ-012.

**[PROPOSTA PROVVISORIA, NON VINCOLANTE: OQ-001]** Struttura degli input
delle ricette dei componenti. Le ricette dei componenti sono prodotte
nella Fabbrica di componenti, hanno da 2 a 4 input e un output
principale. La tabella indica soltanto le famiglie di input per
categoria funzionale: non definisce quantità, tempi, LR, tecnologie
richieste né sottoprodotti.

| Categoria funzionale (output principale) | Famiglie di input | Provenienza |
|---|---|---|
| Strutturali | Metalli strutturali, Composti chimici | Già nell'esempio illustrativo di §4.3.5 |
| Propulsione | Metalli strutturali, Materiali energetici, Metalli rari | Già nell'esempio illustrativo di §4.3.5 |
| Energia | Materiali energetici, Cristalli e semiconduttori, Metalli strutturali | Ipotesi di lavoro dell'assistente, approvata come provvisoria |
| Elettronica e sensori | Cristalli e semiconduttori, Metalli rari, Composti chimici | Ipotesi di lavoro dell'assistente, approvata come provvisoria |
| Armamenti | Metalli strutturali, Materiali energetici, Metalli rari | Ipotesi di lavoro dell'assistente, approvata come provvisoria |
| Supporto vitale | Biomateriali, Composti chimici, Metalli strutturali | Ipotesi di lavoro dell'assistente, approvata come provvisoria |

**[PROPOSTA PROVVISORIA, NON VINCOLANTE: OQ-001]** Struttura degli input
delle ricette degli assemblaggi. Le ricette degli assemblaggi sono
prodotte nel Cantiere/Assemblaggio. Le **navi non hanno ricette fisse
per classe**: la ricetta di una nave è generata dal progetto, cioè dai
componenti scelti negli slot più lo scafo (§7.4; §7.5). Nel catalogo
restano quindi cinque ricette di scafo, una per taglia. Strutture e
prodotti finiti del Cantiere comprendono soltanto i moduli di stazione
orbitale e le infrastrutture (impianti, avamposti), senza altri prodotti
finiti. La tabella indica soltanto i tipi di input: non definisce
quantità, tempi, LR, tecnologie richieste né sottoprodotti.

| Assemblaggio (output principale) | Tipi di input | Provenienza |
|---|---|---|
| Scafo Piccolo | Componenti strutturali, Metalli strutturali | Ipotesi di lavoro dell'assistente, approvata come provvisoria |
| Scafo Medio | Componenti strutturali, Metalli strutturali | Ipotesi di lavoro dell'assistente, approvata come provvisoria |
| Scafo Grande | Componenti strutturali, Metalli strutturali, Composti chimici | Ipotesi di lavoro dell'assistente, approvata come provvisoria |
| Scafo Capitale | Componenti strutturali, Metalli strutturali, Composti chimici, Metalli rari | Ipotesi di lavoro dell'assistente, approvata come provvisoria |
| Scafo Colosso | Componenti strutturali, Metalli strutturali, Composti chimici, Metalli rari | Ipotesi di lavoro dell'assistente, approvata come provvisoria |
| Modulo di stazione orbitale | Componenti strutturali, Componenti di energia, Componenti di supporto vitale | Ipotesi di lavoro dell'assistente, approvata come provvisoria |
| Infrastruttura (impianto o avamposto) | Componenti strutturali, Componenti di energia, Componenti di elettronica e sensori, Metalli strutturali | Ipotesi di lavoro dell'assistente, approvata come provvisoria |

Restano da definire (OQ-001): eventuali sottoprodotti per ricetta,
tecnologie richieste e verifica della completezza dell'elenco delle
strutture. Quantità, tempi e LR non fanno parte di OQ-001 (OQ-052;
OQ-012).

**Tipi di lavorazione**

Le lavorazioni possono produrre beni in lotti discreti oppure realizzare
assemblaggi articolati in più fasi. Entrambe utilizzano lo stesso
modello di avanzamento basato sulle UDL (§4.3.6).

1. **Lavorazioni a lotto** (materiali raffinati, componenti e altri beni):
   - ogni lotto ha un output definito e un proprio **Lavoro Richiesto
     (LR)**, espresso in UDL;
   - il lotto accumula UDL nel tempo fino al raggiungimento del LR;
   - l'output viene generato soltanto al completamento del lotto;
   - se il lavoro disponibile nel mese non basta, il progresso residuo
     viene conservato per i mesi successivi;
   - tutti gli input necessari al lotto vengono consumati integralmente
     al suo avvio. Se il lotto viene sospeso, il lavoro accumulato e gli
     input già consumati restano contabilizzati. In caso di annullamento,
     gli input già consumati non vengono restituiti automaticamente.
2. **Lavorazioni di assemblaggio** (navi, strutture e prodotti finiti):
   - richiedono materiali e componenti in ingresso;
   - sono definite da un LR complessivo, non da una durata rigida;
   - accumulano UDL secondo le stesse regole delle altre lavorazioni;
   - il consumo degli input segue le tre fasi 50% / 30% / 20% della
     sezione 2.7, al raggiungimento delle rispettive soglie di
     avanzamento.

La durata indicata negli esempi è quindi una stima derivata dal LR e
dalle UDL effettivamente generate, non una regola autonoma di
avanzamento. Tutti i tipi di lavorazione utilizzano il medesimo motore
di progresso; cambiano soltanto LR, input, output e modalità di consumo
degli input.

Gli input devono trovarsi nella località dell'impianto (o essere
trasportati lì, §4.2 e §4.5) e possono essere prenotati secondo la
sezione 2.7.

**Esempi illustrativi [valori provvisori, solo per mostrare il formato]:**

| Ricetta | Impianto | Input (per ciclo) | Output (per ciclo) | Durata |
|---|---|---|---|---|
| Metalli strutturali | Raffineria | 20 Minerali, 5 Energia | 10 Metalli strutturali | 1 mese |
| Combustibili raffinati | Raffineria | 20 Carburante, 5 Energia | 10 Combustibili raffinati | 1 mese |
| Componenti di propulsione | Fabbrica di componenti | 8 Metalli strutturali, 4 Materiali energetici, 2 Metalli rari | 2 Componenti di propulsione | 1 mese |
| Componenti strutturali | Fabbrica di componenti | 10 Metalli strutturali, 2 Composti chimici | 4 Componenti strutturali | 1 mese |

| Assemblaggio | Impianto | Input totali | Durata | Consumo |
|---|---|---|---|---|
| Nave leggera | Cantiere | 10 Componenti strutturali, 4 Componenti di propulsione, 3 Componenti di energia, 2 Componenti di elettronica | 12 mesi | 50% / 30% / 20% |

Le quantità dell'esempio sono segnaposto: il bilanciamento è da
definire. Le ricette complete saranno raccolte in un catalogo
separato.

#### 4.3.6 Capacità produttiva, Punti Capacità e modello di lavoro [PRINCIPI CONFERMATI, PARAMETRI IN CORSO DI DEFINIZIONE]

**Capacità nominale e Punti Capacità**

La capacità produttiva nominale degli impianti è misurata in **Punti
Capacità (PC)**. La scala dei PC è comune a tutte le tipologie di
impianto: un PC rappresenta la stessa unità di capacità
indipendentemente dal tipo di struttura.

Ogni tipologia di impianto dispone di un proprio valore di capacità
base, espresso in PC, al livello 1. È confermata la conversione diretta
della scala relativa: **un punto dell'indice equivale a un PC**. I valori
base in PC coincidono quindi con gli indici approvati riportati più
avanti. La dimensione o classe dell'impianto non costituisce una
variabile aggiuntiva: per ciascuna tipologia è previsto un solo valore
base.

**Progressione dei livelli**

Il livello dell'impianto principale determina la capacità nominale
complessiva disponibile. La progressione percentuale è uniforme per
tutte le tipologie:

| Livello | Capacità rispetto al livello 1 |
|---:|---:|
| 1 | 100% |
| 2 | 150% |
| 3 | 200% |
| 4 | 250% |
| 5 | 300% |

La formula di riferimento è:

`PC_nominali = PC_base × moltiplicatore del livello`

Il livello massimo raggiungibile dipende dallo sblocco di una
specifica ricerca tecnologica. Il potenziamento dell'impianto principale
consente di sostenere più reparti oppure di aumentare la capacità
destinabile alle lavorazioni.

**Budget condiviso tra reparti e autonomia degli impianti principali**

I PC costituiscono un budget complessivo condiviso dai reparti e dalle
lavorazioni attive appartenenti allo stesso impianto principale. La
somma dei PC assegnati nell'impianto non può superare la sua capacità
effettivamente disponibile. I reparti possono lavorare in parallelo,
ma ciascun reparto è soggetto a un proprio limite massimo di PC
utilizzabili. Il limite del reparto non costituisce capacità aggiuntiva:
la somma dei PC assegnati a tutti i reparti resta comunque vincolata al
budget complessivo dell'impianto.

**Calcolo del limite massimo di PC per reparto [CONFERMATO]**

Per il modello iniziale, tutti i reparti utilizzano una percentuale base
uniforme pari al **50% della capacità base dell'impianto principale al
livello 1**. Non sono quindi introdotte, in questa fase, percentuali
base differenti per tipologia di reparto.

Il limite massimo di ciascun reparto cresce in funzione del livello
dell'impianto principale e può essere ulteriormente incrementato dalle
tecnologie sviluppate. Sono previsti due ambiti di applicazione
tecnologica:

- **Tecnologie generali:** modificano il limite massimo di tutti i
  reparti a cui la tecnologia si applica;
- **Tecnologie specifiche:** modificano il limite massimo soltanto
  delle tipologie di reparto interessate.

Tutti i modificatori percentuali sono applicati con criterio
moltiplicativo. Il limite risultante è arrotondato per difetto, poiché
i PC sono unità indivisibili.

La formula di riferimento è:

`PC_max_reparto = floor(PC_base_impianto × 0,50 × M_livello × prodotto(M_tech_applicabili))`

Dove:
- `PC_base_impianto` è la capacità base dell'impianto principale al
  livello 1;
- `0,50` è la percentuale base uniforme del reparto;
- `M_livello` è il moltiplicatore del livello dell'impianto principale;
- `M_tech_applicabili` sono i moltiplicatori delle tecnologie generali
  e specifiche applicabili a quel reparto;
- `floor` indica l'arrotondamento per difetto al PC intero.

I valori base risultanti al livello 1, prima dei modificatori
tecnologici, sono:

| Tipologia di impianto principale | Capacità base impianto | Limite base di ciascun reparto |
|---|---:|---:|
| Miniera | 100 PC | 50 PC |
| Centrale energetica | 90 PC | 45 PC |
| Raffineria | 80 PC | 40 PC |
| Fabbrica di componenti | 70 PC | 35 PC |
| Cantiere/Assemblaggio | 60 PC | 30 PC |
| Deposito/Magazzino | 50 PC | 25 PC |
| Spazioporto | 40 PC | 20 PC |

Il limite è riferito a ciascun singolo reparto: più reparti della stessa
tipologia possono quindi avere ciascuno il proprio limite, ma non
possono superare complessivamente il budget PC disponibile
nell'impianto principale. In fase di distribuzione mensile, ogni ordine
è vincolato alla capacità residua del proprio reparto e il complesso
degli ordini resta vincolato alla capacità residua complessiva
dell'impianto. Le regole di priorità, ripartizione e precedenza tra
ordini restano quelle definite in questa sezione, applicate nel rispetto
di entrambi i limiti.

I valori numerici dei bonus tecnologici e le singole tecnologie che li
forniscono restano da definire.

**Associazione degli ordini ai reparti [CONFERMATA]**

Ogni ricetta specifica la tipologia di reparto compatibile con la
lavorazione. Quando crea un ordine, il giocatore seleziona l'impianto
principale presso il quale intende eseguirlo. Se nell'impianto è
presente un solo reparto compatibile, l'ordine viene assegnato
automaticamente a quel reparto; se sono presenti più reparti
compatibili, il giocatore seleziona il reparto specifico.

L'ordine resta associato al reparto selezionato e non viene trasferito
automaticamente verso altri reparti, neppure quando questi dispongono
di capacità PC libera. Se il reparto non può assegnare ulteriori PC,
l'ordine resta in attesa della disponibilità di capacità nel reparto a
cui è associato.

Il giocatore può trasferire manualmente un ordine a un altro reparto,
purché il nuovo reparto sia compatibile con la ricetta e appartenga allo
stesso impianto principale. Il trasferimento non crea un nuovo ordine:
identità, anzianità, priorità e progresso accumulato vengono conservati.
Gli input già consumati non vengono restituiti né consumati nuovamente.
Ai fini della distribuzione dei PC, l'ordine concorre nel nuovo reparto
a partire dal mese successivo, secondo le normali regole di
assegnazione. Non è consentito trasferire l'ordine a un reparto di un
impianto principale diverso.

Ogni impianto principale mantiene un budget autonomo: gli impianti
principali distinti non condividono né trasferiscono automaticamente
i propri PC.

Ogni ordine produttivo è associato a un solo impianto principale e
riceve PC esclusivamente dal budget di quell'impianto. Un singolo
ordine non può quindi attingere contemporaneamente a più impianti
principali. Le diverse fasi di una filiera produttiva sono rappresentate
da ordini distinti: l'output di un ordine può diventare l'input di un
ordine successivo, associato al proprio impianto principale.

**Lavoro Richiesto e Unità di Lavoro**

Ogni ricetta o processo produttivo definisce un **Lavoro Richiesto
(LR)**, espresso in **Unità di Lavoro (UDL)**. Il LR indica la quantità
totale di lavoro necessaria per completare una singola lavorazione o
un singolo lotto.

La conversione di riferimento è:

`1 PC assegnato = 1 UDL`

Questa equivalenza è universale e costituisce il valore di base per
qualsiasi tipologia di impianto e lavorazione. Il lavoro effettivamente
generato può essere modificato dai coefficienti di efficienza globali
e locali descritti più avanti.

Le UDL generate vengono accumulate sul processo. Quando il totale
accumulato raggiunge il LR, la lavorazione è completata e viene
generato l'output previsto. Se il lavoro generato nel mese non è
sufficiente, il progresso viene conservato e la lavorazione prosegue
nei mesi successivi. Se un processo viene sospeso, conserva il
progresso già accumulato; alla ripresa continua dal punto raggiunto.

Le lavorazioni a lotto generano l'intero output soltanto al
completamento del lotto. Gli input del lotto vengono consumati
integralmente all'avvio. Per gli assemblaggi resta invece valida la
ripartizione del consumo in tre fasi 50% / 30% / 20% (§2.7).

**Calcolo dell'efficienza produttiva**

Il valore di riferimento è 1 UDL per PC assegnato. L'efficienza
effettiva può essere modificata da fattori globali e locali. I due
livelli vengono calcolati separatamente:

- `E_g`: coefficiente globale, determinato dai modificatori
  tecnologici e dai modificatori sociali legati alla specie/razza e
  all'organizzazione sociale del giocatore;
- `E_l`: coefficiente locale, determinato dai fattori contingenti
  presenti nella località dell'impianto.

La formula è:

`UDL_generate = PC_assegnati × E_g × E_l`

Ogni modificatore percentuale viene applicato sequenzialmente come
fattore moltiplicativo. Per esempio, un modificatore di +20% equivale
a moltiplicare per 1,20; un modificatore di −15% equivale a
moltiplicare per 0,85. L'ordine di applicazione dei modificatori
all'interno dello stesso livello è indifferente, perché la
moltiplicazione è commutativa.

I modificatori globali agiscono su tutti gli impianti dell'impero:
- **tecnologici**, derivanti dalle scoperte e dai progressi scientifici;
- **sociali**, derivanti dalla specie/razza e dall'organizzazione
  sociale del giocatore.

I modificatori locali agiscono su tutti gli impianti presenti nella
località interessata. Possono essere di natura sociale, economica,
bellica o di altra natura contingente. Esempi includono rivolte,
scioperi, crisi economiche locali, blocchi nemici, bombardamenti e
sabotaggi. Il livello di applicazione dipende dalla località in cui
l'effetto si manifesta, non dalla sua natura.

I modificatori locali sono associati alla **località**, non ai singoli
impianti: un evento locale viene registrato una sola volta e influenza
tutti gli impianti interessati presenti in quel luogo, inclusi quelli
costruiti mentre l'evento è attivo. Quando l'evento termina, il relativo
modificatore cessa di applicarsi.

I modificatori di efficienza non devono essere confusi con vincoli che
riducono direttamente la capacità disponibile o impediscono una
lavorazione. Per esempio, una carenza di personale può ridurre i PC
effettivamente disponibili; un blocco logistico può impedire l'avvio o
la prosecuzione per mancanza di input; un danno agli impianti può
ridurre l'efficienza o la capacità, secondo la natura dell'effetto.
La classificazione puntuale dei singoli eventi resta da definire.

**Calcolo della capacità effettivamente disponibile**

La capacità nominale è determinata dal valore base dell'impianto e dal
suo livello. Le limitazioni operative che riducono la capacità vengono
applicate separatamente dai modificatori di efficienza.

Le riduzioni percentuali della capacità sono rappresentate da fattori
moltiplicativi, applicati sequenzialmente. Il loro prodotto costituisce
il coefficiente di disponibilità `C`. Le perdite fisse vengono invece
espresse direttamente in PC (`PC_persi`).

La capacità effettivamente disponibile è calcolata come segue:

`PC_disponibili = max(0, floor(PC_nominali × C − PC_persi))`

Dove:
- `PC_nominali` è la capacità nominale dell'impianto;
- `C` è il prodotto dei modificatori percentuali di disponibilità;
- `PC_persi` è la quantità fissa di capacità perduta, espressa in PC;
- `floor` indica l'arrotondamento per difetto.

Il risultato non può essere negativo. Le riduzioni percentuali e le
perdite fisse non modificano direttamente i coefficienti di efficienza
`E_g` ed `E_l`: incidono invece sul numero di PC distribuibili.

**Calcolo individuale del fabbisogno di PC**

All'inizio della distribuzione mensile viene determinata una situazione
di riferimento comune, comprendente i modificatori globali e locali
attivi in quel momento. Il TurnResolver calcola poi individualmente,
per ciascun ordine produttivo operativo che richiede PC, i coefficienti
di efficienza specifici dell'ordine, in base ai modificatori applicabili
alla sua località e al suo contesto.

Il fabbisogno di PC viene calcolato in funzione del lavoro residuo e
dell'efficienza risultante. Per evitare che il denominatore sia nullo
o negativo, l'efficienza effettiva utilizzata nel calcolo è definita
applicando una soglia minima al prodotto dei coefficienti globale e
locale:

`E_eff = max(E_g × E_l, 0.01)`

`PC_necessari = ceil(LR_residuo / E_eff)`

Dove:
- `LR_residuo` è il Lavoro Richiesto ancora da completare, espresso in
  UDL;
- `E_g` è il coefficiente globale applicabile all'ordine;
- `E_l` è il coefficiente locale applicabile all'ordine;
- `E_eff` è l'efficienza effettiva impiegata nel calcolo, pari al
  prodotto `E_g × E_l` ma mai inferiore a `0.01`;
- `max` seleziona il maggiore tra il prodotto dei coefficienti e la
  soglia minima;
- `ceil` indica l'arrotondamento all'intero superiore, necessario
  perché i PC sono unità indivisibili.

La soglia minima di `0.01` si applica al prodotto `E_g × E_l`, non ai
singoli coefficienti separatamente. Costituisce un invariante del
sistema: nessuna combinazione di modificatori può ridurre l'efficienza
effettiva sotto tale valore. Il denominatore del calcolo è quindi
sempre strettamente positivo e il fabbisogno non presenta il caso
indefinito dovuto a un'efficienza nulla o negativa.

Il fabbisogno così calcolato indica il numero massimo di PC che
l'ordine può assorbire per completare il lavoro residuo nel mese,
secondo i coefficienti di riferimento. Non è una quota prenotata o
persistente dell'ordine.

**Distribuzione collettiva dopo il calcolo individuale**

Il calcolo dei coefficienti e del fabbisogno è individuale per ciascun
ordine; l'assegnazione dei PC è invece collettiva. Il sistema determina
prima i fabbisogni di tutti gli ordini operativi che utilizzano lo
stesso impianto principale e poi distribuisce il budget disponibile
applicando priorità, ripartizione equa, anzianità e limiti di fabbisogno
descritti in questa sezione.

L'ordine con cui il TurnResolver esamina gli ordini non deve quindi
determinare un vantaggio nell'assegnazione: nessun ordine riceve PC
prima che siano stati considerati i fabbisogni degli altri ordini
concorrenti dello stesso impianto.

I coefficienti sono calcolati individualmente, ma si riferiscono alla
situazione comune fotografata all'inizio della distribuzione mensile.
Gli eventi che modificano la capacità o l'efficienza dopo tale momento
non alterano la distribuzione già effettuata e vengono considerati
nella distribuzione del mese successivo.

**Ricalcolo mensile del fabbisogno**

Il fabbisogno di PC non è un valore persistente dell'ordine. Viene
ricalcolato a ogni distribuzione mensile sulla base del lavoro residuo
aggiornato e dei coefficienti di efficienza validi per quel mese.
Il progresso accumulato resta conservato; eventuali variazioni
dell'efficienza possono quindi aumentare o diminuire il fabbisogno
mensile. La priorità e la sequenza cronologica dell'ordine restano
distinte dal fabbisogno e non vengono modificate dal suo ricalcolo.

**Distribuzione automatica dei PC in base alla priorità**

Ogni ordine produttivo eredita la priorità assegnata dal giocatore
all'ordine stesso (§2.6). All'inizio di ogni mese il sistema distribuisce
automaticamente i PC disponibili, servendo prima gli ordini con
priorità più alta. La distribuzione è effettuata una sola volta all'inizio
del mese: i PC liberati dal completamento di un ordine durante il mese
non vengono redistribuiti nello stesso mese e diventano disponibili per
la distribuzione del mese successivo.

**Ripartizione tra ordini della stessa priorità**

Gli ordini con la stessa priorità ricevono una ripartizione il più
possibile equa dei PC disponibili. I PC sono **unità indivisibili**:
non vengono assegnate frazioni di PC. Quando la divisione lascia uno o
più PC residui, questi vengono assegnati singolarmente seguendo la
sequenza di precedenza degli ordini.

La sequenza di precedenza è determinata dall'anzianità dell'ordine:
tra ordini con la stessa priorità, quelli inseriti prima precedono quelli
inseriti successivamente. Di conseguenza, quando un nuovo ordine entra
nella distribuzione, gli ordini già presenti con priorità uguale o
superiore mantengono la precedenza. La priorità resta comunque il
criterio principale: un ordine nuovo con priorità superiore precede un
ordine più vecchio con priorità inferiore.

Non viene utilizzato un criterio rotativo per l'assegnazione dei PC
residui. La sequenza cronologica costituisce il criterio deterministico
di spareggio e deve essere conservata nei salvataggi, così che la
simulazione resti riproducibile dopo un caricamento.

**Limite al fabbisogno e redistribuzione nella stessa priorità**

La quota assegnata a ciascun ordine è limitata al lavoro residuo che può
ancora svolgere. Un ordine non riceve PC oltre il fabbisogno necessario
per completarlo. I PC che non può utilizzare vengono redistribuiti tra
gli altri ordini della stessa priorità, mantenendo la ripartizione il
più possibile equa e applicando la precedenza cronologica agli eventuali
residui. Il sistema ripete la ripartizione finché gli ordini di quella
priorità sono soddisfatti oppure non rimangono PC disponibili. Soltanto
quando il fabbisogno degli ordini di una priorità è stato soddisfatto, i
PC ancora disponibili passano agli ordini della priorità immediatamente
inferiore.

**Validità mensile dei PC**

I PC rappresentano una disponibilità riferita al mese. I PC non
utilizzati entro la fine del mese **scadono** e non vengono accumulati
né trasferiti al mese successivo. All'inizio di ogni mese la
distribuzione viene ricalcolata sulla base della capacità disponibile,
degli ordini produttivi attivi e operativi, delle rispettive priorità,
della loro sequenza di precedenza e del lavoro ancora necessario.

La produttività effettiva può essere frazionaria anche se i PC sono
indivisibili: i coefficienti di efficienza possono far sì che un PC
assegnato generi, per esempio, 1,15 UDL.

**Valori base in PC al livello 1**

È confermata la conversione diretta: **1 punto dell'indice relativo
equivale a 1 PC**. La scala approvata diventa pertanto la seguente:

| Tipologia di impianto | Capacità base al livello 1 |
|---|---:|
| Miniera | 100 PC |
| Centrale energetica | 90 PC |
| Raffineria | 80 PC |
| Fabbrica di componenti | 70 PC |
| Cantiere/Assemblaggio | 60 PC |
| Deposito/Magazzino | 50 PC |
| Spazioporto | 40 PC |

I valori sono capacità base di riferimento, prima dell'applicazione
degli eventuali modificatori. Per il Deposito/Magazzino, la capacità
rappresenta la capacità operativa di stoccaggio e movimentazione, non
il volume massimo di risorse immagazzinabili. Per lo Spazioporto,
rappresenta la capacità operativa di gestione del traffico spaziale e
delle operazioni con le navi, non una capacità produttiva industriale.

**Distinzione tra capacità e rendimento**

I valori base e i moltiplicatori di livello determinano la capacità
nominale espressa in PC. I modificatori tecnologici e sociali globali
e i modificatori contingenti locali descritti sopra agiscono invece
sul rendimento con cui i PC assegnati vengono convertiti in UDL.
Eventuali effetti che riducono direttamente la disponibilità di PC
devono essere definiti come variazioni della capacità disponibile e
non come modificatori di efficienza. I valori numerici dei singoli
modificatori e gli eventuali limiti del coefficiente risultante restano
da bilanciare.

Per le centrali energetiche, la produzione effettiva dipende anche
dalla tecnologia di generazione sbloccata e dalle condizioni ambientali
e locali. Il valore base della tipologia e i modificatori tecnologici
e ambientali devono restare concettualmente distinti.

**Vincoli operativi e logistici**

La capacità base e la capacità effettiva non coincidono con la
possibilità concreta di eseguire ogni processo. La disponibilità di
materiali in ingresso, di spazio nei depositi, di collegamenti logistici
e delle altre condizioni necessarie può impedire l'esecuzione di un
processo anche quando l'impianto dispone di PC sufficienti.

Un vincolo operativo o logistico può sospendere il processo
interessato. La sospensione non comporta la perdita del progresso già
accumulato né degli input già consumati, ma **non mantiene riservata una
quota di PC**. I PC sono una capacità mensile e non una risorsa
materiale prenotabile: il processo sospeso viene escluso dalla
distribuzione dei PC finché non torna operativo.

La sospensione può riguardare un singolo processo oppure l'intera
attività produttiva del complesso, in funzione della natura e
dell'estensione del vincolo. Per esempio, se una flotta nemica blocca
l'asteroide sul quale si trovano un complesso produttivo, un deposito e
uno spazioporto, la produzione di tutti i reparti del complesso si
arresta. Nella prima distribuzione mensile successiva alla sospensione,
i PC non assegnati ai processi sospesi vengono distribuiti agli altri
processi operativi secondo le normali regole di priorità. Se il processo
è già sospeso all'inizio del mese, viene escluso direttamente dalla
distribuzione di quel mese.

**Ripresa dei processi sospesi per vincoli operativi o logistici**

Quando viene meno la condizione che ha causato la sospensione, il
processo può essere riattivato manualmente dal giocatore prima della
riattivazione automatica. In assenza di intervento, il sistema lo
riavvia automaticamente all'inizio del mese successivo, purché tutte
le condizioni operative necessarie siano nuovamente soddisfatte.
Anche la riattivazione manuale è subordinata alla verifica di tali
condizioni: il giocatore non può forzare l'esecuzione di un processo
che rimane impossibile. Durante la sospensione il progresso già
raggiunto viene conservato e gli input già consumati restano
contabilizzati; alla ripresa, la lavorazione continua dal punto
raggiunto.

Ai fini della distribuzione dei PC, un processo riattivato viene
trattato come un nuovo ordine: conserva il proprio progresso e gli input
già consumati, ma riceve una nuova posizione nella sequenza di
precedenza, successiva agli ordini già presenti. Rientra nella
distribuzione mensile soltanto quando è nuovamente operativo.

#### 4.3.7 Integrità strutturale, danni, riparazioni, demolizione e distruzione degli impianti [REGOLE CONFERMATE, PARAMETRI DA DEFINIRE: OQ-006–OQ-008]

**Punti Struttura (PS)**

Ogni reparto dispone di propri **Punti Struttura (PS)**. Il valore
complessivo dei Punti Struttura di un impianto principale è pari alla
somma dei Punti Struttura massimi o correnti dei suoi reparti, secondo
il contesto del calcolo:

`PS_impianto = somma(PS_reparto_i)`

**Calcolo dei PS massimi [CONFERMATO]**

Tutti i reparti condividono lo stesso valore base di PS massimi,
indipendentemente dalla tipologia e dal livello del reparto. Il valore
numerico del parametro base resta da stabilire durante il bilanciamento.

Il valore base viene modificato dai seguenti fattori:

- **Fattori locali:** condizioni ambientali e condizioni sociali della
  località in cui si trova il reparto.
- **Fattori globali:** tecnologie specifiche applicabili, caratteristiche
  generali del governo e caratteristiche intrinseche della razza.

Tutti i modificatori applicabili si combinano in modo moltiplicativo.
Un modificatore non applicabile vale 1,00. Il risultato finale viene
arrotondato per difetto, poiché i PS massimi sono un valore intero:

`PS_massimi = floor(PS_base × M_ambiente × M_sociale_locale × M_tecnologia × M_governo × M_razza)`

**Ricalcolo dei PS massimi e aggiornamento dei PS correnti [CONFERMATO]**

Quando un evento modifica uno o più fattori ambientali, sociali,
tecnologici, governativi o razziali applicabili al reparto, i suoi PS
massimi vengono ricalcolati immediatamente, senza attendere il turno o
il mese successivo.

I PS correnti sono gestiti con precisione decimale interna e non vengono
arrotondati durante i calcoli. L'interfaccia può mostrarli come valori
interi.

- Se il nuovo valore massimo è inferiore ai PS correnti, i PS correnti
  vengono ridotti immediatamente al nuovo massimo. La differenza viene
  persa e non può essere recuperata tramite riparazione.
- Se il nuovo valore massimo è superiore ai PS correnti, i PS correnti
  non aumentano automaticamente. Il recupero dei PS mancanti avviene
  tramite riparazione.

Il totale corrente dell'impianto principale continua a essere
determinato dalla somma dei PS correnti dei suoi reparti; il totale
massimo è determinato dalla somma dei rispettivi PS massimi.

**Categorie di danno e mitigazione**

Gli impianti e i reparti possono subire danni riconducibili alle seguenti
categorie:

- **Bombardamenti:** danni causati da bombardamenti diretti sulle
  strutture.
- **Attacchi militari:** danni derivanti da azioni offensive militari
  diverse dai bombardamenti, secondo le meccaniche di combattimento
  previste.
- **Eventi ambientali:** danni provocati da fenomeni naturali o dalle
  condizioni ambientali della località.
- **Incidenti industriali:** danni provocati da malfunzionamenti,
  errori operativi o incidenti connessi alle attività industriali.

L'impatto dei danni può essere mitigato da:

- tecnologie sviluppate nell'albero tecnologico;
- fattori sociali e governativi, inclusa l'organizzazione sociale;
- caratteristiche intrinseche della specie, che possono conferire
  bonus o malus.

I modificatori di mitigazione si applicano **moltiplicativamente**,
secondo la regola generale adottata per gli altri modificatori del
sistema. Le diverse categorie di danno possono essere influenzate da
modificatori differenti: non è necessario che ogni tecnologia o fattore
sociale/razziale protegga da tutte le categorie.

I valori iniziali dei danni e dei singoli modificatori non sono ancora
definiti.

**Applicazione dei danni ai reparti e selezione dei bersagli**

Il danno strutturale viene applicato ai PS dei singoli reparti, non
indistintamente al totale dell'impianto principale. La selezione dei
reparti interessati distingue tra attacchi mirati e danni non mirati.

- **Attacchi mirati:** l'attaccante può selezionare uno specifico reparto
  come bersaglio, se dispone delle informazioni e delle capacità
  necessarie. La selezione non garantisce il colpo: la probabilità di
  colpire effettivamente il reparto dipende dalla precisione dell'arma,
  dalle difese del bersaglio e dalle tecnologie disponibili.
- **Attacco mirato mancato:** il colpo può mancare completamente,
  senza causare danni, oppure può colpire accidentalmente un altro
  reparto. Le probabilità e le modalità di deviazione restano da
  bilanciare.
- **Danni non mirati:** il sistema seleziona casualmente i reparti
  coinvolti, usando probabilità ponderate in base all'esposizione dei
  reparti e alle caratteristiche dell'attacco. Le protezioni e gli
  eventuali modificatori applicabili concorrono alla selezione secondo
  le regole specifiche dell'evento.
- **Numero di reparti coinvolti:** un singolo attacco o evento può
  interessare uno o più reparti. Il numero dipende dalla natura
  dell'evento e dalle caratteristiche dell'arma, per esempio dalla
  capacità di concentrare il danno o di colpire un'area.

Il danno diretto viene applicato al reparto individuato dalle regole di
selezione. L'eventuale danno eccedente i PS residui del reparto viene
perso e non si trasferisce automaticamente ad altri reparti.

Alcune armi o eventi possono inoltre generare **danni secondari**,
come esplosioni, reazioni a catena o danni collaterali. Ogni danno
secondario è trattato come un nuovo evento di danno, con una propria
selezione dei bersagli e un proprio calcolo. Anche il danno secondario
non può superare i PS residui del reparto colpito: l'eccedenza viene
persa, salvo che generi a sua volta un ulteriore evento secondario
esplicitamente previsto.

Il totale dei PS dell'impianto principale continua a essere determinato
dalla somma dei PS dei suoi reparti. Non esistono PS separati per la
struttura principale: l'impianto è completamente inutilizzabile quando
tutti i reparti sono distrutti, condizione equivalente all'esaurimento
dei PS complessivi dell'impianto (`PS_impianto_residui = 0`).

**Effetto del danno sulla capacità produttiva**

Quando un reparto subisce una perdita di PS, riceve un fattore
moltiplicativo temporaneo che riduce la sua capacità produttiva. Per la
prima versione il fattore è direttamente proporzionale ai PS residui:

`M_danno = PS_attuali / PS_massimi`

Il fattore viene applicato al limite di PC utilizzabili dal reparto,
secondo le regole già definite per i modificatori percentuali della
capacità (§4.3.6). Il budget complessivo di PC dell'impianto principale
resta regolato dalle normali regole di distribuzione e non viene
incrementato dal fattore di danno. In caso di perdita del 25% dei PS,
per esempio, il reparto conserva il 75% della propria capacità massima
utilizzabile.

Il fattore è temporaneo: il suo valore viene aggiornato in funzione dei
PS correnti e scompare quando il reparto recupera tutti i PS. La
riduzione della capacità produttiva è distinta dalla perdita dei PS,
ma deriva direttamente da essa.

**Ordini di riparazione**

La riparazione costituisce una nuova tipologia di ordine. Può essere
avviata manualmente dal giocatore oppure generata automaticamente dal
sistema quando la struttura subisce un danno, se è attivo l'apposito
flag di riparazione automatica. Il flag può essere configurato per
l'impianto o per il singolo reparto.

Ogni ordine di riparazione è associato alla struttura danneggiata e
utilizza la capacità di riparazione dell'impianto principale a cui essa
appartiene. Le riparazioni non utilizzano il budget di PC destinato alla
produzione ordinaria.

**Priorità e distribuzione dei PC di riparazione [CONFERMATO]**

La capacità autonoma di riparazione di ciascun impianto viene
distribuita tra gli ordini di riparazione relativi ai reparti dello
stesso impianto. La distribuzione segue lo stesso sistema adottato per
gli ordini produttivi: priorità dell'ordine, ripartizione tra ordini
della stessa priorità e precedenza temporale secondo le regole di
distribuzione dei PC definite in §4.3.6. La capacità di riparazione
resta separata da quella produttiva: i due gruppi di ordini non
competono tra loro.

Gli ordini di riparazione generati automaticamente ricevono la
priorità predefinita prevista per gli altri ordini. Il giocatore può
modificare manualmente tale priorità.

**Costo delle riparazioni**

Il costo di riparazione è proporzionale alla quota di PS persi rispetto
ai PS massimi della struttura. Per ciascuna risorsa impiegata nella
costruzione originaria, il costo della riparazione completa del danno
considerato è pari alla stessa percentuale della quantità richiesta
per la costruzione:

`Quota_danno = (PS_massimi - PS_attuali) / PS_massimi`

`Costo_riparazione_risorsa = Costo_costruzione_risorsa × Quota_danno`

La proporzione viene applicata separatamente a ciascuna risorsa
necessaria alla costruzione della struttura. I valori assoluti dipendono
quindi dai costi di costruzione definiti per il reparto o l'impianto.

**Capacità di riparazione**

Ogni impianto principale dispone di una propria capacità di riparazione,
espressa in Punti Capacità (PC) di riparazione. Questa capacità è
indipendente dal budget di PC destinato alla produzione ordinaria: le
riparazioni non sottraggono PC alla produzione e la produzione non
sottrae PC alle riparazioni.

La capacità effettiva di riparazione dipende da:

- capacità base di riparazione dell'impianto;
- tecnologie;
- fattori ambientali locali;
- fattori sociali e governativi;
- caratteristiche intrinseche della specie, inclusi eventuali bonus
o malus razziali.

Tutti i modificatori si applicano moltiplicativamente. La capacità
risultante è espressa in PC interi e viene arrotondata per difetto:

`PC_riparazione = floor(PC_riparazione_base × M_tech × M_ambiente × M_sociale × M_razza)`

I valori base e l'entità dei modificatori restano da definire.

**Avanzamento e consumo delle risorse**

Gli ordini di riparazione seguono il normale sistema di avanzamento dei
processi: richiedono Lavoro Richiesto (LR), avanzano tramite Unità di
Lavoro (UDL) e utilizzano i coefficienti di efficienza globale e locale
previsti per gli altri processi. I PC impiegati provengono
esclusivamente dalla capacità di riparazione dell'impianto.

Le risorse necessarie vengono consumate progressivamente secondo il
modello misto già adottato per gli ordini di assemblaggio:

- al raggiungimento del 50% del lavoro richiesto viene consumato il 50%
delle risorse previste;
- al raggiungimento dell'80% del lavoro richiesto viene consumato un
ulteriore 30%;
- al completamento del lavoro viene consumato il restante 20%.

Se l'ordine viene interrotto, il progresso accumulato e le risorse già
consumate vengono conservati. Alla ripresa, l'ordine continua dal punto
raggiunto, senza consumare nuovamente le risorse già contabilizzate.

**Ripristino progressivo dei PS**

I PS vengono ripristinati progressivamente in proporzione al lavoro di
riparazione completato, senza attendere la conclusione dell'intero
ordine. Il recupero è proporzionale alla quota di UDL accumulate rispetto
alle UDL richieste per la riparazione. I PS da ripristinare e le UDL
richieste costituiscono i valori di riferimento dell'ordine al momento
della sua creazione:

`PS_recuperati = PS_da_ripristinare × (UDL_accumulate / UDL_richieste)`

Il ripristino non può superare i PS massimi della struttura. Il sistema
mantiene precisione interna nel calcolo del progresso e dei PS, così da
non perdere lavoro a causa degli arrotondamenti di visualizzazione.

**Nuovi danni durante una riparazione [CONFERMATO]**

Se il reparto subisce ulteriori danni mentre è già attivo un ordine di
riparazione, il danno viene applicato immediatamente ai PS correnti.
L'ordine di riparazione rimane attivo e conserva il progresso già
accumulato. Il lavoro e i costi ancora necessari vengono ricalcolati
sulla base dei nuovi PS mancanti; non viene creato un nuovo ordine e
il lavoro già svolto non viene perso.

**Distruzione del reparto a 0 PS e ricostruzione [CONFERMATO]**

Quando i PS residui di un reparto raggiungono zero, il reparto è
distrutto e non può più essere riparato. Per ripristinarlo è necessario
avviare un nuovo ordine di costruzione, con i normali costi e tempi di
costruzione. La ricostruzione crea un nuovo reparto: non ripristina gli
ordini produttivi o di altro tipo che erano stati cancellati con la
distruzione del reparto.

Il nuovo reparto mantiene le impostazioni personalizzate proprie
del reparto precedente, incluse le impostazioni strutturali, il flag di
riparazione automatica e la priorità predefinita degli ordini generati
automaticamente. Le priorità personalizzate assegnate ai singoli ordini
non vengono recuperate, poiché tali ordini sono stati cancellati con la
distruzione. Non vengono inoltre recuperate impostazioni che dipendono
specificamente dagli ordini cancellati.

**Indisponibilità e danni**

Si distinguono tre condizioni operative:

- **Indisponibilità temporanea:** il reparto o l'impianto non può
  lavorare temporaneamente. Gli ordini produttivi interessati vengono
  sospesi; progresso e input già consumati sono conservati. Alla
  riattivazione si applicano le regole di ripresa e di nuova precedenza
  per la distribuzione dei PC definite in §4.3.6.
- **Indisponibilità parziale:** il reparto o l'impianto continua a
  funzionare con capacità ridotta. Gli ordini restano attivi e possono
  ricevere i PC effettivamente disponibili, nel rispetto dei limiti del
  reparto e del budget complessivo dell'impianto. Il danno strutturale
  produce la riduzione descritta sopra; altri fattori possono produrre
  ulteriori riduzioni indipendenti.
- **Distruzione:** un reparto è distrutto quando i suoi PS residui
  raggiungono zero; l'impianto principale è completamente distrutto e
  inutilizzabile quando tutti i suoi reparti hanno zero PS (equivalente
  a `PS_impianto_residui = 0`). Si applicano le regole di cancellazione
  descritte di seguito.

**Demolizione volontaria e distruzione**

La demolizione volontaria segue le stesse regole della distruzione.
La distruzione di una struttura comporta la cancellazione degli ordini
direttamente o indirettamente collegati alla struttura non più
disponibile, secondo l'ambito indicato di seguito.

Quando un singolo reparto viene demolito o distrutto:

- vengono cancellati tutti gli ordini produttivi associati a quel
  reparto;
- vengono cancellati tutti gli ordini di riparazione associati al
  reparto;
- vengono cancellati gli ordini logistici e di rifornimento diretti al
  reparto, sia continuativi sia non continuativi, inclusi gli ordini
  dei convogli non ancora partiti;
- vengono cancellati gli eventuali ordini dipendenti o comunque
  direttamente o indirettamente collegati al reparto distrutto;
- gli altri reparti dello stesso impianto principale non vengono
  coinvolti e possono continuare a operare, se le altre condizioni lo
  consentono.

Quando viene demolito o distrutto l'intero impianto principale:

- vengono cancellati tutti gli ordini produttivi e di riparazione
  associati a qualsiasi reparto dell'impianto;
- vengono cancellati tutti gli ordini logistici, di rifornimento e
  continuativi diretti all'impianto;
- vengono cancellati anche gli ordini dei convogli non ancora partiti
  e tutti gli altri ordini direttamente o indirettamente collegati
  all'impianto.

Gli ordini cancellati restano registrati nella cronologia. Il
progresso accumulato viene perso e le risorse già consumate non
vengono restituite; le risorse prenotate ma non ancora consumate
seguono le normali regole di rilascio previste per l'annullamento.
Nessun ordine viene trasferito automaticamente verso un'altra
struttura.

**Convogli di rifornimento in movimento**

Se, al momento della demolizione o distruzione, uno o più convogli sono
ancora in viaggio verso il reparto o l'impianto interessato, interrompono
il viaggio di andata e invertono la rotta verso il rispettivo punto di
partenza. La regola si applica sia ai convogli associati a ordini
continuativi sia a quelli associati a ordini non continuativi.
Gli ordini continuativi diretti alla struttura vengono cancellati e
non generano ulteriori cicli; sono cancellati anche gli ordini dei
convogli non ancora partiti.

Il convoglio conserva integralmente il carico: non scarica risorse
presso la struttura demolita o distrutta e le riporta al punto di
partenza. Le risorse trasportate tornano disponibili nel deposito di
origine soltanto quando il convoglio vi fa effettivamente ritorno. Il
relativo ordine di rifornimento viene rimosso.

### 4.4 Impianti modulari e integrazione della produzione [PRINCIPIO CONFERMATO, DETTAGLI DA DEFINIRE]

Le strutture industriali non devono essere necessariamente semplici
punti di assemblaggio. Un impianto principale può integrare reparti o
officine interne, capaci di produrre localmente alcuni materiali o
componenti necessari alla propria attività.

Per esempio, un cantiere navale può ricevere dall'esterno componenti
base e materie prime, ma disporre anche di officine proprie per
realizzare componenti specializzati destinati alle navi che costruisce.
Questo consente di ridurre la dipendenza da fornitori esterni per
componenti particolarmente sensibili, strategici o specifici di una
classe di unità.

Il giocatore può quindi scegliere, nei limiti delle tecnologie e degli
impianti disponibili, quali componenti produrre localmente e quali
approvvigionare dall'esterno. La produzione integrata richiede comunque
materiali in ingresso, capacità industriale e strutture adeguate: non
rende il complesso autosufficiente per definizione.

L'organizzazione sociale dell'impero può influenzare i costi, i
vantaggi e la convenienza delle diverse configurazioni industriali,
senza determinare rigidamente quali produzioni siano consentite. La
tecnologia stabilisce quali processi sono possibili; gli impianti
stabiliscono quali processi possono essere eseguiti in una località;
l'organizzazione sociale può modificarne l'efficienza o la
convenienza.

**Principio confermato:** i reparti possono lavorare in parallelo,
ma la capacità loro assegnata proviene da un budget complessivo di PC
condiviso dall'impianto principale (§4.3.6). Ogni reparto dispone di
un limite massimo specifico, determinato dalla capacità base
dell'impianto, dal suo livello e dai modificatori tecnologici
applicabili. Tale limite non si aggiunge al budget condiviso: ogni
reparto deve rispettare il proprio tetto e la somma delle assegnazioni
resta entro la capacità effettivamente disponibile dell'impianto.

La disponibilità di PC non implica autonomia materiale: ogni reparto
deve comunque disporre dei materiali in ingresso, delle tecnologie e
degli impianti necessari. Restano da definire i valori numerici dei
fabbisogni di PC dei singoli processi, i modificatori sociali,
ambientali e tecnologici specifici e gli eventuali parametri dei
vincoli operativi di personale, energia o infrastrutture condivise.

### 4.5 Distribuzione geografica della filiera

La filiera può essere distribuita tra località differenti. Una località
può specializzarsi nell'estrazione, un'altra nella raffinazione e
un'altra ancora nella produzione di componenti o nell'assemblaggio
finale.

Ogni passaggio tra località richiede il trasferimento fisico dei
materiali o dei componenti. Le interruzioni dei collegamenti logistici
possono quindi ridurre o fermare la produzione a valle, anche quando
l'impero possiede complessivamente le risorse necessarie in altre
località.

La produzione integrata all'interno di un complesso industriale può
ridurre alcuni trasferimenti e i relativi rischi o ritardi, ma richiede
investimenti e capacità produttiva locali. La scelta tra specializzazione
geografica e integrazione locale costituisce una decisione economica e
logistica del giocatore.

### 4.6 Trasporto e navi cargo [CONFERMATO]

#### 4.6.1 Modello di trasporto

-   Le risorse vengono trasferite tra località mediante **navi cargo
    fisiche**, dotate di capacità, velocità e consumo di carburante.
-   Il trasferimento avviene attraverso **ordini di trasporto** tra
    località, soggetti alle regole sugli ordini del capitolo 1.
-   Un ordine di trasporto prenota i beni nella località di origine
    (§2.7) e li rende disponibili alla destinazione all'arrivo.

#### 4.6.2 Tipi di nave cargo

-   Tre modelli di cargo generico: **leggero**, **medio** e
    **pesante**.
-   **Cisterne** per fluidi e carburante.
-   **Convogli scortati**: gruppi di cargo accompagnati da navi
    militari.

Capacità, velocità e costi dei diversi modelli sono da definire.

#### 4.6.3 Carburante e autonomia

-   Ogni viaggio consuma **carburante**, in misura **proporzionale a
    distanza e carico**.
-   Una nave può partire solo se dispone dell'**autonomia** necessaria
    per il viaggio.

#### 4.6.4 Carichi in transito

-   Un carico in transito appartiene al convoglio che lo trasporta.
-   Le navi cargo possono essere **intercettate, ritardate o
    distrutte**.
-   Il carico perso con una nave distrutta è **perso definitivamente**.
-   Ai cargo possono essere **assegnate scorte** militari, in
    coerenza con la logistica militare del §1.4.
-   Se la destinazione viene demolita o distrutta mentre il convoglio è
    ancora in viaggio, il convoglio inverte la rotta e riporta
    integralmente il carico al punto di partenza. Le risorse tornano
    disponibili nel deposito di origine al rientro effettivo; il
    relativo ordine di rifornimento viene rimosso (§4.3.7).

#### 4.6.5 Rotte permanenti

-   Il giocatore può programmare **rotte permanenti**: trasporti
    periodici (per esempio mensili) tra due località.
-   Una rotta è un **ordine continuativo** (§2.4): resta attivo fino a
    revoca, è soggetto a priorità, attese e persistenza tra anni.
-   Le risorse da trasportare sono prenotate a ogni ciclo secondo il
    §2.7. Se i beni o l'autonomia mancano, il ciclo resta in attesa
    secondo il §2.5.

Tempi di viaggio, distanze e collegamenti sono definiti nel §3.7.

------------------------------------------------------------------------

## 5. Capitolo 4 --- Sistemi stellari e corpi celesti

Questo capitolo raccoglie le prime regole confermate sul contenuto dei
singoli sistemi stellari. I parametri non ancora definiti sono tracciati in
`OPEN_QUESTIONS.md` (OQ-030–OQ-034).

### 5.1 Contenuto dei sistemi

Un sistema stellare può contenere:

-   una o più **stelle**, di classi (tipi spettrali) diverse;
-   **pianeti**;
-   **lune**;
-   **cinture di asteroidi**;
-   **anomalie**.

### 5.2 Numero di stelle

-   La maggior parte dei sistemi ha **una sola stella**.
-   I sistemi **binari o multipli** sono rari: circa il **5%** dei
    sistemi (valore indicativo, da verificare in fase di bilanciamento).
-   Un sistema può avere **fino a 3 stelle**.
-   I pianeti possono orbitare attorno a **una sola stella** oppure
    attorno a **più stelle**.

### 5.3 Numero di pianeti

-   I sistemi hanno **pochi pianeti** (intervallo indicativo: da 0 a 6).
-   Molti sistemi sono vuoti o poveri di risorse utili.

### 5.4 Anomalie

-   Le anomalie (per esempio nebulose, campi di asteroidi densi, buchi
    neri) sono **poche** e hanno **effetti di gameplay**.
-   I buchi neri sono più concentrati nel nucleo galattico (§3.4).

### 5.5 Colonizzabilità dei corpi celesti

-   Solo **pochi pianeti sono abitabili**.
-   Gli altri corpi celesti sono colonizzabili solo con tecnologie
    adeguate o come **avamposti e miniere**.
-   Resta valida la distinzione tra sistemi esistenti, esplorabili,
    colonizzabili e abitati (§3.6).

### 5.6 Classi stellari [OQ-030]

-   Le stelle appartengono alle classi spettrali **O, B, A, F, G, K, M**.
-   Esistono inoltre tre **oggetti compatti**: **nana bianca**,
    **stella di neutroni/pulsar** e **buco nero**.
-   La frequenza segue un **gradiente realistico**: le nane rosse (M)
    sono molto comuni, mentre le stelle calde (O, B) sono rare.
-   La classe stellare ha **effetti indiretti** sul gameplay: influenza
    la fascia di abitabilità, la produzione di energia e i tipi di
    pianeti presenti. Non produce bonus o malus diretti su
    movimento o sensori.
-   La distribuzione resta compatibile con il gradiente radiale
    dell'età stellare (§3.5), con concentrazione di buchi neri nel
    nucleo.

Frequenze [CONFERMATO : gradiente realistico; VALORI DA
BILANCIARE] sul totale delle stelle:

| Classe | Frequenza [PROPOSTA] |
|---|---|
| M | ~74% |
| K | ~12% |
| G | ~7% |
| F | ~3% |
| A | ~0,6% |
| B | ~0,15% |
| O | ~0,03% |
| Nana bianca | ~3% |
| Neutroni/pulsar | ~0,1% |
| Buco nero | ~0,05% (concentrato nel nucleo, §3.4) |

Le frequenze delle classi spettrali e dei compatti si intendono
normalizzate a 100% in fase di generazione.

### 5.7 Tipi di pianeta

Esistono sette tipi:

-   rocciosi;
-   oceanici;
-   desertici;
-   ghiacciati;
-   gassosi;
-   giganti di ghiaccio;
-   lavici/infernali.

### 5.8 Abitabilità [OQ-031]

L'abitabilità di un pianeta dipende da:

-   la **fascia abitabile** della stella;
-   il **tipo di pianeta**;
-   **atmosfera e temperatura**.

Modello [CONFERMATO]:

-   La **fascia abitabile** deriva dalla luminosità della stella: stretta
    e vicina per le nane rosse, ampia e lontana per le stelle calde.
    Nei sistemi multipli vale §5.8.1.
-   Ogni pianeta ha **atmosfera** e **temperatura** derivate da tipo e
    posizione orbitale.
-   Ogni specie ha **soglie** di atmosfera e temperatura tollerate
    (§6.2): un pianeta è abitabile per una specie se rientra nelle sue
    soglie. L'abitabilità è quindi relativa alla specie.
-   La **terraformazione non è inclusa** nella prima versione; gli altri
    corpi restano colonizzabili solo come avamposti e miniere (§5.5).
-   Valori numerici delle soglie: da definire con le specie.

#### 5.8.1 Sistemi multipli [CONFERMATO]

-   **Orbite di tipo S**: ogni pianeta orbita attorno a una singola
    stella, oppure attorno al baricentro di una **coppia stretta** di
    stelle (trattata come unica sorgente con luminosità somma).
-   La **fascia abitabile** di un pianeta si calcola sulla luminosità
    della **stella locale** attorno a cui orbita.
-   Se una stella compagna è **vicina** (separazione sotto una soglia
    da definire), il pianeta subisce un **malus** di abitabilità
    (temperatura instabile): ampiezza e soglia da definire
    [PROPOSTA: −1 livello di idoneità].
-   Un sistema con 3 stelle è una coppia stretta più una compagna
    lontana, oppure tre stelle indipendenti; la configurazione è
    scelta in fase di generazione.

### 5.9 Anomalie [OQ-032]

Sono previste cinque categorie di anomalie, poche e con effetti di
gameplay:

-   nebulose;
-   campi di asteroidi densi;
-   buchi neri;
-   pulsar e aree di radiazioni;
-   tempeste di plasma.

Effetti confermati delle anomalie:

| Anomalia | Effetti |
|---|---|
| Nebulose | Riducono sensori e movimento; possono contenere risorse rare |
| Campi di asteroidi densi | Rallentano e danneggiano le flotte; ricchi di minerali |
| Buchi neri | Pericolosi per la navigazione (possono distruggere navi); forniscono energia alle stazioni di ricerca |
| Pulsar e aree di radiazioni | Danneggiano gli equipaggi e disturbano i sistemi elettronici; impediscono colonie abitate |
| Tempeste di plasma | Temporanee e mobili; danneggiano gli scudi e interrompono movimento e comunicazioni |

I valori numerici sono definiti come segue [CONFERMATO: il modello; VALORI DA BILANCIARE]:

-   Ogni anomalia agisce **solo nel sistema che la contiene**; non
    influenza i sistemi vicini.
-   Il **raggio** è espresso in unità di sistema (distanza orbitale):
    l'anomalia occupa una zona del sistema (per esempio una fascia o
    l'intero sistema).
-   Ogni anomalia ha una **intensità da 1 a 3** che scala gli effetti.

| Anomalia | Intensità 1 / 2 / 3 [PROPOSTA] |
|---|---|
| Nebulose | Sensori e movimento −20% / −40% / −60% |
| Campi di asteroidi | Velocità −10% / −25% / −40%; danno per mese 1% / 3% / 5% dello scafo |
| Buchi neri | Rischio di distruzione per transito 1% / 3% / 6% per nave; energia alle stazioni ×1,5 / ×2 / ×3 |
| Pulsar e radiazioni | Danno agli equipaggi e −15% / −30% / −50% efficienza elettronica; colonie abitate impossibili da intensità 2 |
| Tempeste di plasma | Scudi −25% / −50% / −75%; durata 1--3 mesi, posizione mobile |

Le regole di rischio dettagliate restano da affinare in bilanciamento.

### 5.10 Lune

-   Le lune sono **poche** [CONFERMATO]: circa il **15%** dei
    pianeti ha almeno una luna [PROPOSTA]; massimo **2 lune** per
    pianeta, **4** per i pianeti gassosi e i giganti di ghiaccio.
-   Possono orbitare attorno a **qualsiasi tipo di pianeta**, ma la
    probabilità è **maggiore per i pianeti giganti**.
-   Il loro ruolo principale è quello di **avamposti o miniere**.
-   Una luna può essere **abitabile solo molto raramente** e in
    condizioni particolari, per esempio una luna di un gigante situato
    nella fascia abitabile della stella.
-   Condizioni precise [CONFERMATO]: una luna è candidata
    all'abitabilità solo se orbita attorno a un pianeta **gassoso o
    gigante di ghiaccio** situato nella **fascia abitabile** della
    stella locale (§5.8). Ogni luna candidata è abitabile con
    probabilità di circa **5%** [PROPOSTA]; se lo è, soglie di
    atmosfera e temperatura seguono §5.8 e la capacità demografica ha un
    moltiplicatore **×0,3** (§6.1) [PROPOSTA].

### 5.11 Origine delle risorse nei corpi celesti [OQ-033]

-   Ogni corpo celeste ha una **ricchezza per ciascuna risorsa**, legata
    al proprio tipo.
-   Associazioni principali:

| Risorsa | Origine prevalente |
|---|---|
| Minerali | Pianeti rocciosi e lavici, asteroidi |
| Carburante | Pianeti gassosi e giganti di ghiaccio |
| Materiali avanzati | Rari: pianeti lavici, asteroidi, nebulose |
| Cibo | Pianeti oceanici e rocciosi abitabili |
| Energia | Stella del sistema e anomalie (buchi neri, pulsar) |

-   Le associazioni indicano le fonti prevalenti, non esclusive.
-   [CONFERMATO] Ogni corpo ha, **per ciascuna risorsa**, un
    livello di ricchezza a 5 gradi: **assente, povera, media, ricca,
    eccezionale**, estratto con probabilità dipendenti dal tipo di
    corpo.

| Risorsa / corpo | Probabilità [PROPOSTA] (assente/povera/media/ricca/eccez.) |
|---|---|
| Minerali, roccioso/lavico | 5 / 20 / 40 / 28 / 7 |
| Minerali, asteroide | 10 / 25 / 35 / 25 / 5 |
| Carburante, gassoso/gigante di ghiaccio | 0 / 15 / 40 / 35 / 10 |
| Materiali avanzati, lavico/asteroide/nebulosa | 55 / 25 / 12 / 6 / 2 |
| Cibo, oceanico/roccioso abitabile | 10 / 25 / 40 / 20 / 5 |

I corpi non elencati hanno ricchezza assente per quella risorsa, salvo
eccezioni. Il moltiplicatore numerico di ogni livello è da definire con
le ricette (OQ-001) [PROPOSTA: ×0 / ×0,5 / ×1 / ×1,5 / ×2,5].

### 5.12 Relazioni territoriali e utilizzo dei corpi celesti [CONFERMATO]

Il modello distingue tre relazioni, che non devono essere considerate
sinonimi:

- **Proprietà:** titolarità diretta di entità appartenenti a un impero.
  Gli imperi possiedono direttamente le proprie flotte, navi e
  installazioni. Il pianeta natale iniziale è proprietà diretta
  dell'impero.
- **Utilizzo:** diritto/relazione con cui un impero utilizza un corpo
  celeste o un collegamento per colonizzare, insediare strutture o
  svolgere attività. I pianeti colonizzati e le rotte scoperte sono
  normalmente rappresentati tramite questa relazione, non come
  proprietà diretta dell'intero corpo o collegamento.
- **Controllo:** relazione politico-militare che descrive chi esercita
  il controllo effettivo. Non coincide automaticamente con la
  popolazione presente né con la proprietà delle singole installazioni.

Regole territoriali:

- Un **pianeta può essere utilizzato da un solo impero alla volta**.
  L'utilizzo planetario è quindi esclusivo.
- Un **sistema stellare può essere utilizzato da più imperi**
  contemporaneamente.
- Una **rotta può essere utilizzata da più imperi** contemporaneamente,
  nel rispetto delle regole di accesso ai portali (§3.7.3).
- L'utilizzo esclusivo di un pianeta non attribuisce automaticamente
  diritti esclusivi sull'intero spazio orbitale né sulle installazioni
  orbitali: queste mantengono una propria identità e una propria
  relazione di proprietà/utilizzo.

La distinzione tra proprietà, utilizzo e controllo deve essere mantenuta
anche in caso di occupazione militare, conquista o cambiamento della
situazione politica. Le modalità operative di trasferimento, perdita e
contestazione di tali relazioni restano da definire (OQ-053).


### 5.13 Posizioni e aggiornamento dei corpi celesti

Le posizioni dei sistemi stellari nella galassia e quelle dei corpi
celesti all'interno dei sistemi appartengono a due scale spaziali
indipendenti. Le posizioni dei sistemi stellari sulla mappa galattica
vengono aggiornate ogni mese durante la risoluzione del turno.

Anche le posizioni dei corpi celesti all'interno dei sistemi seguono
l'avanzamento mensile della simulazione, ma il ricalcolo è ottimizzato
in base allo stato di attività del sistema (§3.6.2):

- nei sistemi attivi, le posizioni dei corpi celesti vengono aggiornate
  mensilmente;
- nei sistemi inattivi, il motore conserva il riferimento temporale
  dell'ultimo aggiornamento e differisce il ricalcolo;
- quando un sistema inattivo diventa oggetto di un'azione o torna attivo,
  le posizioni dei corpi celesti vengono calcolate dal riferimento
  temporale conservato fino al tempo corrente della simulazione,
  prima di risolvere le operazioni che ne dipendono.

Il differimento del calcolo nei sistemi inattivi è un'ottimizzazione
simulativa: non modifica le regole del moto né trasforma la posizione
calcolata in un'informazione automaticamente conosciuta dagli imperi.
La conoscenza delle posizioni resta regolata dall'osservazione e dal
modello informativo (§1.3).

------------------------------------------------------------------------

## 6. Capitolo 5 --- Colonie e popolazione

Le questioni relative a specie, bilanciamento dei parametri, navi
coloniali e organizzazione sociale sono tracciate in
`OPEN_QUESTIONS.md` (OQ-035–OQ-038).

### 6.1 Popolazione e crescita demografica [CONFERMATO: modello logistico, VALORI DA BILANCIARE: OQ-036]

-   Ogni colonia ha una **popolazione complessiva**, espressa in
    **milioni** e calcolata come somma delle popolazioni dei gruppi
    demografici presenti (§6.9).
-   La popolazione dei gruppi **cresce nel tempo** in base a fattori
    **di specie, sociali e ambientali**, entro la capacità massima
    dipendente dal corpo celeste e dalla specie (modello logistico).
-   La crescita è modellata come un **evento mensile** per colonia
    (tipo `Growth`, §11.5). Un evento annuale è stato scartato: il
    mensile è coerente con la granularità della simulazione (§2.1) e
    permette di reagire a carestie e guerre con la giusta reattività.
-   La popolazione **fornisce il personale** necessario al
    funzionamento degli impianti (§4.3.4).

Formula dell'evento mensile `Growth`:

```
cap  = capacità_corpo × fattore_ambiente(specie) × (1 + bonus_tecnologici)
Δpop = pop × r × crescita_specie × mod_cibo × mod_stabilità × (1 − pop / cap)
```

-   `r` base [PROPOSTA]: 0,4% mensile (circa 5% annuo).
-   `mod_cibo`: 1,0 se il cibo copre il fabbisogno; scende linearmente a
    0 con copertura del 50%; sotto il 50% scatta la carestia (§6.4).
-   `mod_stabilità`: 1,0 con stabilità ≥ 50; scende a 0,5 a stabilità
    15.
-   Se `pop > cap` (per esempio per riduzione della capacità): calo
    dell'1% mensile verso `cap`.
-   Popolazione iniziale di una colonia [PROPOSTA]: 0,5 M (una nave
    coloniale base).
-   La popolazione è memorizzata in milioni con 3 decimali (migliaia);
    il calcolo usa virgola fissa per garantire il determinismo (§2.3).

Capacità per tipo di corpo, in milioni, prima dei moltiplicatori
[PROPOSTA]: piccolo 50, medio 150, grande 400, luna 20. Avamposti e
corpi non abitabili: nessuna crescita, popolazione fissa minima (§6.6).

### 6.2 Specie [CONFERMATO: 4-6 specie con tratti marcati]

-   Esistono **specie giocabili** (da 4 a 6) con tratti marcati e un
    ambiente preferito.
-   Ogni impero ha una **specie principale**; sono possibili
    **minoranze** di altre specie.

Tratti di ogni specie:

| Tratto | Descrizione |
|---|---|
| Ambiente preferito | Tipo di pianeta/atmosfera con capacità massima (§5.7, §5.8) |
| Tolleranza | Moltiplicatore di capacità su ambienti non preferiti (0,3--0,8) [PROPOSTA] |
| Crescita | Moltiplicatore del tasso di crescita (0,8--1,3) [PROPOSTA] |
| Consumo | Cibo/energia per milione di abitanti, relativo alla base |
| Bonus/malus | Un bonus e un malus specifici (per esempio industria, ricerca, stabilità) |

Archetipi [PROPOSTA, nomi provvisori]:

1.  **Umani** --- adattabili: tolleranza alta, nessun estremo.
2.  **Silicoidi** --- ambienti aridi/vulcanici, crescita lenta, consumo
    di cibo basso, bonus industria.
3.  **Acquatici** --- pianeti oceanici, crescita alta, tolleranza
    bassa, bonus cibo.
4.  **Gregari** --- crescita molto alta, stabilità bassa, bonus
    militare.
5.  **Contemplativi** --- stabilità alta, crescita bassa, bonus
    ricerca.
6.  *(opzionale)* **Criofili** --- pianeti freddi, consumo di energia
    alto.

**Gruppi di specie minoritarie** [PROPOSTA]: una colonia può ospitare
gruppi demografici appartenenti a specie diverse da quella principale.
Ogni gruppo minoritario che raggiunge almeno il 10% della popolazione
complessiva applica i propri tratti di stabilità (malus se la specie
principale gli è ostile) in proporzione alla propria quota. La quota è
calcolata sulla popolazione del gruppo rispetto al totale della colonia
(§6.9).

### 6.3 Stabilità sociale [CONFERMATO: scala 0--100 con soglie, VALORI DA BILANCIARE]

-   Ogni colonia ha una **stabilità** da 0 a 100, aggiornata ogni mese
    verso un **target**.
-   È influenzata da **organizzazione sociale** dell'impero,
    **condizioni** della colonia, **guerra** e **minoranze**.
-   Influisce su **efficienza** produttiva e **rivolte**.
-   L'organizzazione sociale è lo stesso fattore che incide
    sull'efficienza industriale (§4.4).

```
target = 50 + organizzazione + condizioni + guerra + minoranze
stabilità += clamp(target − stabilità, −3, +3)   // inerzia mensile
```

| Fascia | Effetto [PROPOSTA] |
|---|---|
| ≥ 70 | +10% efficienza produttiva |
| 30--69 | Nessun effetto |
| 15--29 | −25% efficienza; crescita ridotta |
| < 15 | Rivolta: produzione sospesa; dopo 3 mesi consecutivi la colonia diventa ribelle/perduta |

Fattori del target [PROPOSTA]:

-   **Organizzazione sociale**: ±10 (forme da definire; OQ-038).
-   **Condizioni**: carestia −20; sovraffollamento (pop > 90% cap) −5;
    energia mancante −10.
-   **Guerra**: −5 per colonia attaccata o bloccata; −10 per colonia
    bombardata.
-   **Minoranze**: −5 per ciascuna minoranza ostile ≥ 10%.

Gli effetti sull'efficienza si applicano come moltiplicatore alla
capacità effettiva degli impianti (§4.4).

### 6.4 Sostentamento e carestia [VALORI DA BILANCIARE]

-   La popolazione richiede **cibo**; i servizi richiedono **energia**.
-   Consumo base [PROPOSTA]: 1 unità di cibo e 0,5 di energia per
    milione di abitanti al mese, modulato dalla specie.
-   Copertura = disponibile / richiesto.
-   Copertura 50--100%: solo crescita ridotta (`mod_cibo`).
-   **Carestia** (copertura < 50%) [PROPOSTA]: popolazione −2% mensile,
    target di stabilità −20; dopo 3 mesi consecutivi scattano gli
    effetti di rivolta (§6.3).
-   Cibo ed energia sono risorse localizzate (§4.2): devono essere
    prodotte o trasportate nella colonia.

### 6.5 Fondazione delle colonie

-   Le colonie si fondano con **navi coloniali**, costruite nei
    cantieri (§4.3.4).
-   La fondazione avviene con un **ordine di colonizzazione**, soggetto
    alle regole del capitolo 1.
-   La colonia **parte piccola e cresce**.
-   Il corpo celeste deve rispettare i requisiti di colonizzabilità
    (§3.6.1, §5.5).
-   Nave coloniale base [PROPOSTA]: capacità 0,5 M, velocità come cargo
    di taglia media (§4.6); costi in PC e risorse da definire con le
    ricette (OQ-001).

### 6.6 Colonie, impianti e avamposti

-   Gli impianti richiedono **personale locale**.
-   Gli **avamposti e le miniere** su corpi non abitabili hanno
    personale **ridotto e importato** e una popolazione minima.
-   Valori [PROPOSTA]: impianto base 0,05 M di personale per livello,
    con efficienza proporzionale se il personale è insufficiente;
    avamposti 0,01 M importati.

### 6.7 Organizzazione sociale [DA DECIDERE: OQ-038]

Proposta di 4 forme, ciascuna con un compromesso: **Industriale**
(+efficienza, −stabilità), **Corporativa** (+commercio, −ricerca),
**Collettivista** (+stabilità, −crescita), **Militarista** (+flotte,
−stabilità civile).

### 6.8 Ricerca e sviluppo tecnologico [REGOLE CONFERMATE, PARAMETRI DA DEFINIRE: OQ-042–OQ-051]

#### 6.8.1 Generazione dei Punti Ricerca

-   I **Punti Ricerca (PR)** sono generati da due fonti:
    -   **Popolazione:** ogni milione di unità di popolazione genera
        **1 PR per turno**.
    -   **Installazioni scientifiche:** laboratori planetari,
        università/istituti di ricerca e stazioni scientifiche orbitali
        generano PR aggiuntivi secondo una produzione base specifica.
-   La produzione da popolazione non è modificata da moltiplicatori.
-   La produzione delle installazioni è modificata dai fattori
    **ambientali, sociali e contingenti** applicabili.
-   La produzione totale dell'impero è la somma delle due fonti:
    `PR_totali = PR_popolazione + PR_installazioni`.
-   I PR possono essere **frazionari**. Le quantità sono conservate con
    precisione decimale interna, senza arrotondamenti intermedi.

#### 6.8.2 Serbatoio imperiale e avvio degli studi

-   Tutti i PR prodotti confluiscono in un **serbatoio di ricerca
    centralizzato a livello imperiale**.
-   I PR non utilizzati restano nel serbatoio e si accumulano tra i
    turni.
-   Ogni tecnologia definisce un **costo in PR per avviare lo studio**.
    Il costo viene sottratto dal serbatoio al momento dell'avvio.
-   Il costo in PR è quindi un requisito di avvio e non rappresenta,
    di per sé, il tempo necessario a completare lo sviluppo.
-   L'impero può mantenere al massimo **tre ricerche attive
    contemporaneamente**.
-   I PR non assegnati a uno studio restano nel serbatoio; non vengono
    distribuiti automaticamente.

#### 6.8.3 Tempo di sviluppo

-   Ogni tecnologia definisce anche un **tempo di sviluppo** necessario
    per completare lo studio, distinto dal costo in PR di avvio.
-   Dopo l'avvio, la ricerca avanza nel tempo fino al completamento.
    Non è sufficiente disporre dei PR: occorre attendere il tempo di
    sviluppo effettivo.
-   Il tempo di sviluppo può essere modificato da fattori **ambientali,
    sociali e contingenti**.
-   I modificatori applicabili e la relativa formula numerica sono da
    definire.

#### 6.8.4 Categorie e specializzazioni delle installazioni scientifiche

Il sistema prevede tre categorie distinte di installazioni scientifiche:

-   **Laboratori planetari:** strutture dedicate alla ricerca
    sperimentale e applicata. Ogni laboratorio viene associato a uno
    specifico ramo della TechTree; il giocatore sceglie la
    specializzazione al momento della costruzione.
-   **Università e istituti di ricerca:** strutture dedicate alla ricerca
    teorica e multidisciplinare. Non ricevono bonus specifici legati a un
    singolo ramo tecnologico.
-   **Stazioni scientifiche orbitali:** installazioni dedicate alla
    ricerca scientifica e spaziale avanzata. Ricevono il bonus di
    specializzazione alla produzione di PR per le ricerche dei rami
    **D. Scienza e Intelligence** e **C. Navale e Propulsione**. Non
    ricevono il bonus per il ramo **E. Colonizzazione e Biologico**.

La specializzazione non limita le tecnologie che una struttura può
ospitare: ogni installazione scientifica idonea può essere assegnata a
una ricerca di qualsiasi ramo. Il bonus di specializzazione si applica
alla produzione di PR dell'installazione quando questa contribuisce a una
ricerca coerente con il proprio ambito; non riduce il costo in PR della
tecnologia.

**Produzione base per tipologia.** La produzione base per turno di
un'installazione di livello 1 è:

| Installazione | Produzione base L1 |
|---|---:|
| Laboratorio planetario | 1 PR/turno |
| Università / istituto di ricerca | 2 PR/turno |
| Stazione scientifica orbitale | 3 PR/turno |

La gerarchia risultante è quindi
`PR_base_laboratorio < PR_base_università/istituto < PR_base_stazione_orbitale`.
La produzione base cresce inoltre con il livello dell'installazione,
utilizzando la seguente progressione comune:

| Livello | Moltiplicatore della produzione base PR |
|---|---:|
| L1 | ×1,00 |
| L2 | ×1,50 |
| L3 | ×2,00 |
| L4 | ×2,50 |
| L5 | ×3,00 |

La progressione per livello è un parametro iniziale di bilanciamento e
potrà essere rivista. I valori base e i moltiplicatori determinano la
produzione teorica prima dell'applicazione dei requisiti operativi e dei
modificatori.

**Bonus di specializzazione.** Un'installazione specializzata riceve un
bonus del **+25%** alla propria produzione di PR quando contribuisce a
una ricerca coerente con la propria specializzazione. Il moltiplicatore
è `M_specializzazione = 1,25` in caso di compatibilità e `1,00` negli
altri casi. Il bonus si applica una sola volta per installazione,
indipendentemente dal numero di caratteristiche che rendono compatibile
la ricerca. Installazioni diverse applicano ciascuna il proprio bonus
alla propria produzione; i bonus non si sommano tra installazioni.
Università e istituti non ricevono bonus specifici per ramo. I laboratori
planetari ricevono il bonus per il ramo a cui sono specializzati. Le
stazioni orbitali ricevono il bonus nei soli rami C e D; il ramo E è
escluso anche nel caso in cui la stazione disponga di laboratori dedicati
a tale ambito.

La produzione teorica di un'installazione, prima dei modificatori
ambientali, sociali e contingenti, è quindi calcolata come:

`PR_teorici = PR_base_tipologia × M_livello × M_specializzazione`.

I modificatori ambientali, sociali e contingenti si applicano secondo le
regole generali. La produzione effettiva è inoltre vincolata ai requisiti
operativi e alla disponibilità del personale scientifico (§6.8.4.1).

La specializzazione di un laboratorio planetario può essere modificata
anche dopo la costruzione, mediante un processo di **riconversione che
richiede tempo e risorse**. I relativi costi e tempi sono da definire.
Durante la riconversione il laboratorio è temporaneamente inoperativo:
tutte le ricerche assegnate vengono sospese automaticamente e conservano
integralmente il progresso accumulato. Al termine della riconversione,
le ricerche riprendono automaticamente. Il giocatore può, in alternativa,
trasferire preventivamente un progetto a un'altra installazione
compatibile; il trasferimento facoltativo segue le regole generali di
trasferimento, inclusa la sospensione per un turno.

##### 6.8.4.1 Requisiti operativi, personale e formazione scientifica

**Requisiti operativi delle installazioni.** Per produrre PR,
un'installazione scientifica deve soddisfare le condizioni operative
minime applicabili:

-   integrità strutturale sufficiente a mantenere la struttura operativa;
-   disponibilità di energia;
-   presenza di personale scientifico;
-   per le stazioni orbitali, collegamenti logistici necessari al
    sostentamento della struttura.

Il mancato rispetto di un requisito operativo essenziale può rendere
l'installazione inoperativa e interromperne la produzione di PR. Le soglie
strutturali, i fabbisogni energetici e i requisiti logistici quantitativi
restano da definire.

**Categorie professionali scientifiche.** Il personale scientifico è
suddiviso in tre categorie:

1. **Ricercatori:** svolgono l'attività scientifica e contribuiscono allo
   sviluppo della ricerca.
2. **Tecnici scientifici:** gestiscono strumenti, laboratori,
   apparecchiature e attività sperimentali.
3. **Personale di supporto scientifico:** assicura il funzionamento
   organizzativo e operativo delle installazioni scientifiche.

**Disponibilità e produzione di PR.** La produzione effettiva di PR
dipende dal personale scientifico disponibile presso l'installazione.
Una disponibilità di personale inferiore al fabbisogno riduce
progressivamente la produzione, senza rendere automaticamente inattiva
la struttura. In assenza totale di personale scientifico la produzione
di PR è nulla. Il moltiplicatore
applicato alla produzione teorica è:

`M_personale = min(1, Personale_disponibile / Personale_richiesto)`.

Il personale disponibile oltre il fabbisogno richiesto non genera un
bonus ulteriore. Il fabbisogno richiesto cresce con il livello
dell'installazione secondo gli stessi moltiplicatori utilizzati per la
produzione base:

| Livello | Moltiplicatore del personale richiesto |
|---|---:|
| L1 | ×1,00 |
| L2 | ×1,50 |
| L3 | ×2,00 |
| L4 | ×2,50 |
| L5 | ×3,00 |

Il fabbisogno assoluto di personale e la sua ripartizione tra le tre
categorie professionali devono essere definiti per ciascuna tipologia di
installazione. La formula esprime il rapporto tra personale disponibile
e richiesto. La ripartizione del fabbisogno tra le tre categorie deve
comunque essere rispettata: la presenza di personale in una categoria
non compensa automaticamente l'assenza di personale in un'altra.

La produzione effettiva di PR, prima degli altri modificatori generali,
è pertanto:

`PR_effettivi = PR_teorici × M_personale`.

**Formazione annuale nelle università e negli istituti.** Ogni
università/istituto di ricerca produce annualmente personale qualificato
in tutte e tre le categorie professionali. La produzione base è uguale
per ciascuna categoria: in condizioni neutre, il numero di ricercatori,
tecnici scientifici e addetti al supporto scientifico formati ogni anno
è lo stesso.

La quantità effettivamente formata è influenzata dai consueti fattori
applicabili alla produzione: fattori tecnologici globali, fattori sociali
globali, fattori sociali specifici, fattori ambientali locali e fattori
contingenti. I fattori sociali globali, tra cui l'impostazione di governo,
possono modificare la distribuzione della formazione tra le tre
categorie, favorendone alcune rispetto alle altre. La parità riguarda
quindi la produzione base in assenza di modificatori che alterino la
composizione.

Non sono ancora definiti i valori assoluti della formazione annuale per
livello e tipologia di università/istituto, né le formule numeriche dei
singoli modificatori.

#### 6.8.5 Sede della ricerca

-   Ogni progetto deve essere assegnato a uno specifico **laboratorio
    planetario, università/istituto di ricerca o stazione scientifica
    orbitale** idoneo.
-   La sede assegnata è la struttura presso cui si svolge lo sviluppo
    tecnologico e determina i modificatori locali applicabili.
-   Le installazioni scientifiche sono quindi infrastrutture strategiche
    e possibili obiettivi militari per gli imperi avversari.
-   La distribuzione dei progetti tra più sedi può ridurre il rischio
    strategico derivante dalla concentrazione della ricerca in un'unica
    struttura.

#### 6.8.6 Danni, indisponibilità e distruzione della sede

-   **Struttura danneggiata ma operativa:** la ricerca continua;
    eventuali penalità alla velocità di sviluppo dipendono dalle
    condizioni della struttura e dai modificatori applicabili.
-   **Struttura temporaneamente inoperativa:** la ricerca viene sospesa
    e conserva integralmente il progresso accumulato.
-   **Struttura completamente distrutta:** il progetto non viene
    annullato, ma subisce una perdita parziale del progresso. La perdita
    percentuale è calcolata secondo la formula:

    `Perdita_% = (T_totale − T_trascorso) × 100 / T_totale`

    dove `T_totale` è il tempo di sviluppo effettivo previsto al momento
    dell'avvio della ricerca, dopo l'applicazione dei modificatori allora
    vigenti, e `T_trascorso` è il tempo trascorso dall'avvio della ricerca
    fino alla distruzione della sede. La perdita è proporzionale al
    **tempo residuo previsto rispetto a `T_totale`**, non al progresso
    effettivamente accumulato: la formula misura quindi il tempo trascorso
    rispetto alla durata di riferimento congelata, non la percentuale di
    sviluppo già completata. Il progresso residuo è conservato e può
    essere trasferito a un'altra installazione compatibile.
-   `T_totale` viene **congelato al momento dell'avvio** e resta il
    riferimento per il calcolo della perdita per tutta la durata del
    progetto. I modificatori successivi possono cambiare la velocità
    effettiva di sviluppo, ma non modificano retroattivamente questo
    valore di riferimento.
-   La perdita di progresso si applica **esclusivamente alla
    distruzione completa** della struttura; non si applica alla semplice
    indisponibilità temporanea.

#### 6.8.7 Trasferimento di un progetto

-   Un progetto può essere trasferito a un'altra installazione
    compatibile.
-   Il trasferimento non comporta ulteriori perdite del progresso
    accumulato, oltre all'eventuale perdita già applicata in seguito
    alla distruzione completa della sede precedente.
-   Durante il trasferimento la ricerca resta sospesa per **un turno**.
-   La nuova sede può determinare una velocità di sviluppo diversa,
    in funzione dei propri modificatori locali.

### 6.9 Gruppi demografici autonomi [CONFERMATO]

La popolazione di una colonia è articolata in **gruppi demografici
autonomi**, anziché essere trattata soltanto come un totale aggregato
con quote di specie. Ogni gruppo è un'entità distinta e mantiene almeno
la propria specie, la propria consistenza demografica e le proprie
relazioni con gli slot occupati.

- La popolazione complessiva della colonia è un valore derivato dalla
  somma delle popolazioni dei gruppi presenti.
- L'origine del gruppo distingue almeno popolazioni **autoctone** e
  **immigrate**.
- L'affiliazione politica distingue gruppi appartenenti a un impero e
  gruppi indipendenti. Un gruppo indipendente non costituisce, per
  questo solo fatto, un impero controllato dall'IA.
- Una popolazione autoctona usa lo stesso modello di gruppo
  demografico delle popolazioni immigrate: l'origine è un attributo,
  non una classe di entità separata.
- La conquista o l'occupazione militare di un pianeta non elimina e non
  trasferisce automaticamente i gruppi demografici già presenti.
- Un nuovo insediamento può utilizzare slot liberi oppure richiedere
  l'espulsione degli occupanti e la riconversione degli slot. Espulsione
  e riconversione sono operazioni esplicite e non avvengono
  istantaneamente per il solo arrivo dei nuovi coloni.

Si distinguono inoltre tre condizioni del corpo celeste:

1. **Popolazioni autoctone:** gruppi demografici presenti dalla
   generazione del mondo.
2. **Insediamenti neutrali:** comunità organizzate indipendenti dagli
   imperi giocabili, potenzialmente dotate di strutture e attività
   locali; non sono automaticamente imperi autonomi completi.
3. **Corpi celesti disabitati:** privi di popolazione o insediamento
   permanente, pur potendo contenere risorse e anomalie.

Gli attributi completi dei gruppi, le procedure e i tempi di espulsione
e riconversione e gli effetti conseguenti restano da definire (OQ-056;
OQ-028).

### 6.10 Slot di sviluppo planetario [CONFERMATO: PRINCIPIO E CATEGORIE]

Ogni pianeta dispone di un numero totale di **slot di sviluppo
planetario**. Il numero base dipende dalla dimensione del pianeta ed è
modificato dalle tecnologie e dai fattori sociali globali applicabili.

Gli slot sono assegnati a una delle seguenti funzioni:

- abitazione;
- industria;
- ricerca;
- commercio;
- socialità.

Uno slot rappresenta un'unità/area di sviluppo e non implica
necessariamente una corrispondenza rigida di un solo edificio per slot.
Ogni slot mantiene una propria identità, funzione, eventuale gruppo
demografico associato quando pertinente, struttura presente e stato.
Gli stati comprendono almeno: libero, occupato, in costruzione, in
riconversione e non disponibile.

La diminuzione della capacità massima calcolata non cancella
automaticamente gli slot già occupati: gli slot e le strutture esistenti
vengono conservati; finché il numero effettivo di slot non rientra nel
nuovo limite, non è possibile aggiungere nuovi slot/insediamenti oltre
la capacità consentita. Le regole numeriche di calcolo, la gestione
puntuale delle variazioni e l'associazione dei gruppi alle diverse
funzioni sono da definire (OQ-052).

### 6.11 Installazioni orbitali [CONFERMATO: CATEGORIE]

Le installazioni orbitali condividono la natura di infrastrutture
costruite nello spazio orbitale, ma sono specializzate per funzione.
Sono previste almeno quattro categorie:

- **Stazioni scientifiche:** ricerca e attività scientifiche, secondo
  le regole del §6.8.
- **Stazioni di assemblaggio:** costruzione e assemblaggio di navi o
  altri prodotti compatibili.
- **Stazioni logistiche civili:** supporto al trasporto, allo stoccaggio
  e alla logistica civile.
- **Stazioni logistiche militari:** supporto logistico alle flotte e
  alle operazioni militari.

Le categorie condividono il concetto generale di installazione
orbitale, ma non si presume che abbiano identici requisiti, capacità,
processi o regole operative. Le specifiche funzionali e i rapporti con
l'utilizzo del pianeta sottostante sono da definire (OQ-053).

------------------------------------------------------------------------

## 7. Capitolo 6 --- Flotte e combattimento

Il completamento del catalogo dei componenti, i costi di costruzione
dell’Accademia e il bilanciamento dei parametri di combattimento sono
tracciati in `OPEN_QUESTIONS.md` (OQ-039–OQ-041).
Questo capitolo descrive navi, classi, progettazione, componenti e
combattimento (valori provvisori da bilanciare, §7.9--7.12). Gli eventi di
movimento e combattimento sono ordinati dal TurnResolver (§2.3).

### 7.1 Flotte e navi [CONFERMATO]

-   Una **nave** può esistere e operare indipendentemente da una flotta;
    l'appartenenza a una flotta è una relazione di raggruppamento, non
    un requisito per l'esistenza della nave.
-   Una **flotta** è composta da una o più navi e dispone di una nave
    designata come **ammiraglia**.
-   Quando una flotta viene creata, il giocatore ne seleziona
    l'ammiraglia.
-   Se l'ammiraglia viene distrutta o altrimenti perduta, la flotta si
    dissolve e le navi superstiti tornano a essere unità indipendenti.
-   Le navi appartengono a **classi** diverse.
-   Alcune classi si sbloccano solo avanzando nella **tecnologia**.

Il comportamento in caso di distacco volontario, trasferimento o
sostituzione dell'ammiraglia è da definire (OQ-055).

### 7.2 Categorie di navi [CONFERMATO]

-   **Civili:** scopi logistici (trasporto, cargo, colonizzazione;
    §4.6, §6).
-   **Militari:** spionaggio e combattimento.

### 7.3 Progettazione delle classi [CONFERMATO]

-   Il giocatore può **creare la propria classe**, civile o militare,
    scegliendo ogni sottocomponente.
-   In alternativa può usare la **configurazione automatica**, che
    tenta di installare per ogni slot il componente con il più alto
    grado tecnologico disponibile.

### 7.4 Scafi e slot [CONFERMATO]

-   Ogni classe è basata su uno **scafo** di una di **5 taglie** (per
    esempio Piccolo, Medio, Grande, Capitale, Colosso), con slot
    crescenti. I nomi sono provvisori.
-   Le taglie si sbloccano con la tecnologia.
-   Gli **slot sono generici**: ognuno accetta qualunque tipo di
    componente.
-   Ogni progetto deve includere **almeno un componente di
    propulsione**; senza propulsione la nave non si muove.

| Taglia | Slot (valori provvisori) | Disponibilità | Cantiere minimo (provvisorio) |
|---|---|---|---|
| Piccolo | 4 | Da inizio partita | L1 |
| Medio | 8 | Da inizio partita | L1 |
| Grande | 14 | Tecnologia | L2--L3 |
| Capitale | 22 | Tecnologia | L4 |
| Colosso | 32 | Tecnologia | L5 |

-   Per costruire una nave servono sia la **tecnologia** sia un
    **Cantiere** del livello minimo richiesto dalla taglia (§4.3.6).

### 7.5 Componenti [CONFERMATO: CATALOGO E TECNOLOGIE DA COMPLETARE, OQ-039–OQ-040]

Tipi di componente installabili:

| Tipo | Note |
|---|---|
| Armamenti | Includono le contromisure elettroniche |
| Spionaggio | |
| Ricerca | |
| Colonizzazione | |
| Propulsione | Determina velocità di crociera e di combattimento |

-   Ogni tipo ha **componenti specifici** che dipendono dalla
    tecnologia, ciascuno con il proprio **costo in risorse**.
-   Ogni tipo di componente ha **5 livelli tecnologici**. I livelli
    più alti costano più risorse (anche Materiali avanzati) e più
    personale.
-   I componenti sono prodotti dalla **Fabbrica** come componenti
    funzionali (§4.3.4) e usati dal **Cantiere** nella costruzione. Il
    costo di un progetto è la somma dei componenti più lo scafo.

#### 7.5.1 Propulsione [CONFERMATO]

-   La propulsione ha **più livelli tecnologici** (provvisoriamente 5).
-   Ogni livello ha **velocità di crociera**, **velocità di
    combattimento** e **consumo di carburante** crescenti (§4.6).
-   Se sono installati più componenti di propulsione, la velocità
    della nave è quella del **componente più lento**.

#### 7.5.2 Personale qualificato [CONFERMATO]

-   Le navi richiedono **personale**. Il personale qualificato è una
    **risorsa specializzata**, di un solo tipo generico.
-   **Origine:** deriva dalla popolazione dei pianeti (§6). Diventa
    qualificato dopo l'addestramento in un'**Accademia / Centro di
    addestramento**, un edificio unico con livelli e PC propri, che
    deve trovarsi sul pianeta o sulla stazione spaziale. Consuma
    popolazione, cibo ed energia.
-   **Trasferimento:** il personale va trasferito al cantiere come una
    risorsa fisica, con le regole di trasporto di §4.6.
-   **Fabbisogno:** ogni componente richiede un numero di personale. Il
    totale della nave è la somma dei componenti più una **quota base**
    per taglia di scafo.
-   **Consumo:** il personale è consumato alla costruzione (regola
    50/30/20, §2.7) e resta a bordo. Se la nave è distrutta, il
    personale è perso.

**Valori [CONFERMATO, DA BILANCIARE].**

Quota base per taglia di scafo:

| Taglia | Quota base |
|---|---|
| Piccolo | 5 |
| Medio | 15 |
| Grande | 40 |
| Capitale | 100 |
| Colosso | 250 |

Personale per componente di livello 1 (i livelli 2--5 usano i
moltiplicatori ×1, ×1,6, ×2,5, ×4, ×6 di §7.9, con arrotondamento per
eccesso a interi):

| Tipo | Personale (L1) |
|---|---|
| Propulsione | 1 |
| Armamenti offensivi | 2 |
| Scudi / armatura / contromisure | 1 |
| Spionaggio | 2 |
| Ricerca | 3 |
| Colonizzazione | 4 |

**Accademia.**

-   Livelli 1--5, un solo edificio per pianeta o stazione.
-   Capacità di addestramento (personale/mese a PC pieni): L1 10, L2 25,
    L3 60, L4 120, L5 250.
-   Costo per persona: 1 migliaio di abitanti, 1 cibo, 1 energia.
-   Ciclo di addestramento di 2 mesi, con consumo 50/30/20 (§2.7). Il
    personale resta nel deposito dell'Accademia fino al trasferimento
    (§4.6).
-   La popolazione addestrata esce dalla colonia. Se scende sotto il
    minimo, l'Accademia passa a `Waiting` (condizione interna, §2.5).
-   Costi di costruzione e potenziamento: progressivi e legati ai PC,
    da definire con le ricette (OQ-001; OQ-040).

### 7.6 Svolgimento del combattimento

-   Il combattimento è **automatico**: il giocatore non interviene
    durante la risoluzione, che è affidata al TurnResolver (§2.3).
-   Uno scontro è composto da **round**: un **numero fisso di round per
    mese** (valore provvisorio: 3), ciascuno con il proprio
    sotto-istante (es. tick 0, 333, 666 del mese).
-   Lo scontro è modellato come **entità persistente** (`Engagement`),
    che sopravvive ai round e ai mesi. Ogni round è un evento figlio
    generato dal TurnResolver durante la Fase C (§2.3.3).
-   Se lo scontro non si risolve entro il mese, **prosegue nei mesi
    successivi** con nuovi round generati all'inizio del mese.
-   Il giocatore influenza le sue flotte assegnando una **dottrina /
    ordine di battaglia** (per esempio aggressiva, difensiva, ritirata
    a una soglia di danno), che guida le scelte automatiche.

**Round per mese [CONFERMATO]:** 3 round al mese (tick 0, 333,
666). I valori di danno, scudi e armatura sono in §7.9; le riparazioni
in §7.11.

### 7.7 Dottrine e fine dello scontro [CONFERMATO]

-   **Dottrine base:** Aggressiva, Difensiva, Ritirata a soglia di
    danno.
-   **Fine dello scontro:** una parte è distrutta, si ritira con
    successo, oppure non restano parti ostili nel sistema.
-   **Ritirata:** avviene al round successivo al raggiungimento della
    soglia, verso il sistema di provenienza lungo una rotta valida
    (§3.7.4). È possibile l'**inseguimento**, in base alla velocità di
    combattimento (propulsione).

### 7.8 Danno, scudi e contromisure [CONFERMATO]

-   **Scudi:** assorbono danno e si **rigenerano a ogni round**.
-   **Scafo/armatura:** subisce **danno permanente** fino alla
    riparazione.
-   **Scudi e armatura** sono **componenti** della categoria
    Armamenti (difensivi) e occupano slot, come le contromisure
    elettroniche.
-   **Tipi di danno** (per esempio energia, cinetico, missili): le
    difese hanno efficacia diversa per tipo.
-   **Contromisure elettroniche:** riducono la precisione di missili e
    bersagliamento nemico.
-   **Bersagli:** scelti in modo **casuale ponderato** (seed della
    partita) tra le navi nemiche presenti; la dottrina può dare
    priorità (per esempio l'Aggressiva colpisce le navi più
    pericolose).

**Da definire:** lista dei componenti specifici per tipo e costi,
valori di velocità, struttura dell'albero tecnologico, livello esatto
di Cantiere per la taglia Grande.

### 7.9 Statistiche e risoluzione del danno [CONFERMATO, VALORI DA BILANCIARE: OQ-041]

**Struttura (HP scafo) per taglia:** Piccolo 100, Medio 300, Grande 800,
Capitale 2000, Colosso 5000.

**Difese:**

-   **Armatura:** componente difensivo che aggiunge HP di armatura,
    assorbiti dopo gli scudi.
-   **Scudi:** componente con una capacità; si rigenerano a fine round
    del 25% della capacità.
-   Moltiplicatore dei valori per livello tecnologico (1--5):
    ×1, ×1,6, ×2,5, ×4, ×6.

**Tipi di danno:**

| Tipo | Contro scudi | Contro armatura | Note |
|---|---|---|---|
| Energia | 100% | 50% | precisione alta |
| Cinetico | 50% | 100% | non ridotto dalle contromisure |
| Missili | 75% | 75% | precisione ridotta dalle contromisure |

**Sequenza per colpo:**

1.  Tiro di precisione (seed): base 90%, ridotta dalle contromisure del
    bersaglio di 5% per livello (massimo 40%), solo per missili e
    bersagliamento.
2.  Danno × moltiplicatore del tipo.
3.  Il danno colpisce prima gli scudi, poi l'armatura, poi la struttura.
4.  Struttura a 0: nave distrutta (personale perso, §7.5.2).
5.  Gli scudi si rigenerano a fine round.

**Round simultanei:** i danni di un round sono calcolati sullo stato a
inizio round e applicati insieme a fine round, senza vantaggio di
iniziativa (coerente con §2.6).

### 7.10 Ritirata [CONFERMATO]

-   La soglia di ritirata è una percentuale della struttura totale
    della flotta (default 50%).
-   La ritirata riesce se la velocità di combattimento media della
    flotta è maggiore o uguale a quella dell'inseguitore; altrimenti
    l'inseguitore ottiene un round di fuoco libero.

### 7.11 Riparazioni [CONFERMATO, VALORI DA BILANCIARE: OQ-041]

-   Possibili solo in un sistema con Cantiere o stazione propria.
-   Costo: 30% del costo in risorse della nave per riparazione completa,
    in proporzione al danno.
-   Ritmo: proporzionale ai PC del Cantiere (§4.3.6); è un ordine con
    consumo 50/30/20.
-   Nessuna riparazione in volo, salvo navi ausiliarie (§1.4), a metà
    ritmo.

### 7.12 Spionaggio, ricerca in nave e costruzione [CONFERMATO]

**Spionaggio.**

-   I componenti di spionaggio danno punti di **Intelligence** per nave.
-   Una flotta con spionaggio nel sistema soddisfa `CanObserveLocation`
    (§11.7).
-   Se il livello di spionaggio è maggiore o uguale a quello di
    contromisure/occultamento dell'avversario, la composizione è
    osservata per intero; altrimenti solo presenza e numero.

**Ricerca in nave.** Bonus mensile ai punti ricerca del giocatore, solo
se la nave è ferma in un sistema con anomalia o corpo da studiare.

**Cantieri e PC (§4.3.6).**

| Taglia | Costo in PC | Durata minima |
|---|---|---|
| Piccolo | 100 | 3 mesi |
| Medio | 300 | 4 mesi |
| Grande | 800 | 6 mesi |
| Capitale | 2000 | 9 mesi |
| Colosso | 5000 | 12 mesi |

I componenti costano PC in Fabbrica (§4.3.4).

------------------------------------------------------------------------

## 8. Gestione delle questioni aperte

Le questioni progettuali ancora da definire, le relative proposte, le
dipendenze, le priorità e gli aspetti che bloccano sono gestiti in
`OPEN_QUESTIONS.md`. Il documento costituisce il backlog operativo del
processo di game design; gli identificativi `OQ-XXX` sono stabili e
possono essere richiamati nelle sezioni pertinenti del GDD.

Le voci presenti in `OPEN_QUESTIONS.md` non costituiscono regole
approvate. Una questione passa allo stato `Decisa` soltanto dopo
l'approvazione esplicita dell'utente; la relativa specifica viene quindi
incorporata nel GDD e registrata negli altri documenti pertinenti.

## 9. Principi di progettazione da preservare

1.  **Pianificazione significativa:** il giocatore deve poter
    programmare attività su un orizzonte annuale, considerando anche la
    produzione futura.
2.  **Equità della risoluzione:** l'ordine degli eventi è deterministico
    e dichiarato (ETA, poi spareggio esplicito); l'ordine interno di
    elaborazione del motore non deve creare vantaggi arbitrari.
3.  **Informazione limitata:** le unità reagiscono soltanto a ciò che
    possono conoscere.
4.  **Conseguenze economiche:** risorse, prenotazioni, produzione e
    logistica devono essere coerenti tra loro.
5.  **Continuità degli ordini:** attese, interruzioni e passaggi tra
    anni non devono cancellare arbitrariamente il lavoro già svolto.
6.  **Controllo del giocatore:** il giocatore può rivedere gli ordini
    persistenti nella nuova fase di pianificazione, prima di bloccare il
    piano.
7.  **Generazione galattica coerente:** la casualità deve produrre
    galassie differenti, mantenendo i vincoli morfologici e i gradienti
    astronomici concordati.

------------------------------------------------------------------------

## 10. Registro sintetico delle decisioni confermate

| Tema | Decisione |
|---|---|
| Modello territoriale e demografico ([ADR-004](DECISIONS.md#adr-004--relazioni-territoriali-gruppi-demografici-e-infrastrutture-orbitali)) | Distinzione tra proprietà, utilizzo e controllo; pianeti a utilizzo esclusivo, sistemi e rotte condivisibili; gruppi demografici autonomi; slot di sviluppo planetario e categorie di installazioni orbitali (§5.12; §6.9–§6.11) |
| Intelligence ([ADR-005](DECISIONS.md#adr-005--modello-delle-informazioni-e-dellintelligence)) | Stato reale unico e conoscenza per impero; categorie informative e scala generale di intelligence 0–4 (§1.3) |
| Entità radice `World` e avanzamento temporale ([ADR-008](DECISIONS.md#adr-008--entità-radice-world-e-avanzamento-temporale)) | `World` contiene lo stato autorevole dell'universo, possiede il tempo di simulazione e avanza verso un tick assoluto tramite `advanceTo`; atomicità logica e coerenza temporale (§1.2.1) |
| Gestione temporale della conoscenza e osservazione mediante sonde ([ADR-007](DECISIONS.md#adr-007--gestione-temporale-della-conoscenza-e-osservazione-mediante-sonde)) | Informazioni per attributo con date di osservazione/acquisizione; distinzione statiche/dinamiche; aggiornamento tramite sonde e separazione tra attività del sistema e conoscenza (§1.3; §3.6.2; §5.13) |
| Navi e flotte ([ADR-006](DECISIONS.md#adr-006--autonomia-delle-navi-e-ammiraglia-della-flotta)) | Navi indipendenti o raggruppate in flotte; ogni flotta ha un'ammiraglia e si dissolve se questa viene perduta (§7.1) |
| Organizzazione documentale | Quattro documenti canonici distinti: GDD, DECISIONS, CHANGELOG e OPEN_QUESTIONS (ADR-002) |
| Sottoprodotti nelle ricette ([ADR-003](DECISIONS.md#adr-003--sottoprodotti-opzionali-nelle-ricette)) | Output principale più eventuali sottoprodotti, presenti solo in ricette specificamente scelte; non tutte le ricette li hanno; formato base invariato: 2–4 input e un output principale (§4.3.5) |
| Modalità | Single-player e multiplayer |
| Struttura | Gioco a turni; risoluzione affidata al TurnResolver, con eventi ordinati (mai realmente simultanei) |
| TurnResolver | Azioni con ETA; quelle non completate entro il turno vanno in una coda persistente consultata a inizio elaborazione |
| ETA | In mesi con sotto-istante (frazione di mese); spareggio per eventi con stesso ETA: tipo di evento (movimento → combattimento → produzione → crescita), priorità, sorteggio con seed; ricalcolo automatico dell'ETA se cambiano le condizioni, l'azione resta in coda |
| Informazioni | Mosse avversarie nascoste, salvo capacità di intelligence |
| Logistica | Elemento centrale, soprattutto in ambito militare |
| Durata del turno | Un anno |
| Suddivisione | 12 mesi di simulazione |
| Pianificazione | Programmazione anticipata dell'intero anno |
| Risorse future | Prenotazione sulla disponibilità prevista |
| Set di risorse | Cinque: Minerali, Energia, Cibo, Carburante, Materiali avanzati |
| Localizzazione risorse | Fisica: scorte per pianeta, stazione, deposito o nave; nessun pool imperiale; il trasferimento richiede trasporto |
| Materiali raffinati | 7 famiglie: metalli strutturali, metalli rari, composti chimici, cristalli/semiconduttori, combustibili raffinati, materiali energetici, biomateriali |
| Componenti | 6 categorie: strutturali, propulsione, energia, elettronica/sensori, armamenti, supporto vitale |
| Impianti | 7 tipi: miniera, raffineria, fabbrica di componenti, cantiere/assemblaggio, centrale energetica, deposito, spazioporto |
| Potenziamento impianti | Livelli 1-5; progressione uniforme della capacità: 100%, 150%, 200%, 250%, 300%; livello massimo subordinato a ricerca tecnologica |
| Capacità produttiva | Unità comune: Punti Capacità (PC); capacità base unica per tipologia; budget condiviso tra i reparti dello stesso impianto principale; budget indipendenti tra impianti principali; limite massimo specifico per ciascun reparto |
| Assegnazione PC | Automatica all'inizio del mese; priorità prima, ripartizione equa a parità di priorità; PC indivisibili; PC residui assegnati secondo anzianità dell'ordine; nessuna redistribuzione durante il mese; PC non utilizzati scadono |
| Capacità effettivamente disponibile | Capacità nominale modificata da fattori percentuali moltiplicativi e perdite fisse in PC; `PC_disponibili = max(0, floor(PC_nominali × C − PC_persi))` |
| Fabbisogno di PC ([ADR-001](DECISIONS.md#adr-001--soglia-minima-dellefficienza-effettiva-nel-calcolo-dei-pc)) | Calcolato individualmente per ordine in base a LR residuo ed efficienza effettiva: `E_eff = max(E_g × E_l, 0.01)` e `ceil(LR_residuo / E_eff)`; ricalcolato a ogni distribuzione mensile (§4.3.6) |
| Coefficienti di efficienza | Calcolo specifico per ordine sulla situazione comune fotografata all'inizio della distribuzione mensile; eventi successivi incidono dal mese seguente |
| Ordini, reparti e impianti | Ogni ordine produttivo è associato a un solo impianto principale e a uno specifico reparto compatibile; la ricetta determina la tipologia di reparto, il giocatore sceglie l'impianto e, se necessario, il reparto; assegnazione automatica se esiste un solo reparto compatibile; trasferimento solo manuale tra reparti compatibili dello stesso impianto, con conservazione di identità, anzianità, priorità e progresso e applicazione dal mese successivo; le fasi di una filiera sono ordini distinti; ciascun impianto principale ha un budget PC autonomo |
| Capacità base al livello 1 | Miniera 100 PC; Centrale energetica 90 PC; Raffineria 80 PC; Fabbrica di componenti 70 PC; Cantiere/Assemblaggio 60 PC; Deposito/Magazzino 50 PC; Spazioporto 40 PC |
| Limite massimo dei reparti | Per ogni reparto: 50% della capacità base dell'impianto al livello 1, moltiplicato per il coefficiente del livello dell'impianto e per i modificatori tecnologici generali e specifici applicabili; modificatori moltiplicativi; arrotondamento per difetto; il limite del reparto non aumenta il budget condiviso dell'impianto |
| Indisponibilità dei reparti e degli impianti | Indisponibilità temporanea: ordini sospesi, con progresso e input consumati conservati; indisponibilità parziale: ordini attivi con capacità ridotta; distruzione: ordini di produzione e rifornimento riferiti alla struttura rimossi/cancellati |
| Punti Struttura | Tutti i reparti condividono un valore base di PS massimi, indipendente da tipologia e livello; modificatori locali ambientali e sociali e modificatori globali tecnologici specifici, governativi e razziali si applicano moltiplicativamente; modificatori non applicabili = 1,00; arrotondamento finale per difetto; valore base numerico da bilanciare. I PS massimi vengono ricalcolati immediatamente quando cambia un fattore applicabile; i PS correnti mantengono precisione decimale interna, sono limitati immediatamente al nuovo massimo se questo diminuisce e non aumentano automaticamente se il massimo cresce; il recupero avviene tramite riparazione |
| Danni strutturali | Danni da bombardamenti, attacchi militari, eventi ambientali e incidenti industriali; mitigazione tramite tecnologie, fattori sociali/governativi e caratteristiche razziali con modificatori moltiplicativi. Attacchi mirati: il giocatore seleziona il reparto, ma il colpo dipende da precisione dell'arma, difese e tecnologie; un mancato può mancare del tutto o colpire accidentalmente un altro reparto. Danni non mirati: selezione casuale ponderata per esposizione e caratteristiche dell'attacco. Un attacco può coinvolgere uno o più reparti; il danno eccedente i PS residui viene perso e non si trasferisce. Danni secondari trattati come eventi distinti, con propria selezione e calcolo. Nessun PS separato per l'impianto: è distrutto quando tutti i reparti sono a 0 PS, equivalente a PS complessivi residui pari a zero |
| Capacità produttiva dei reparti danneggiati | Fattore temporaneo direttamente proporzionale ai PS residui: `M_danno = PS_attuali / PS_massimi`; applicato al limite di PC del reparto secondo le regole dei modificatori di capacità |
| Ordini di riparazione | Nuova tipologia di ordine, manuale o automatica tramite flag per impianto/reparto; associata alla struttura danneggiata; usa PC di riparazione dedicati e non il budget PC produttivo; ordini automatici con priorità predefinita modificabile manualmente; distribuzione dei PC di riparazione tra gli ordini dello stesso impianto secondo priorità e precedenza temporale, con regole analoghe alla produzione |
| Capacità di riparazione | Ogni impianto ha PC di riparazione autonomi; modificatori tecnologici, ambientali, sociali/governativi e razziali moltiplicativi; valori base e modificatori da bilanciare |
| Costi e avanzamento delle riparazioni | Costo proporzionale alla quota di PS persi e alle risorse originarie di costruzione; avanzamento con LR/UDL ed efficienza globale/locale; consumo risorse 50%/30%/20%; progresso e risorse consumate conservati in caso di interruzione; PS ripristinati progressivamente in proporzione alle UDL; nuovi danni durante la riparazione aggiornano PS mancanti, lavoro e costi residui senza annullare l'ordine o perdere il progresso |
| Demolizione e distruzione | La demolizione volontaria segue le stesse regole della distruzione; a 0 PS il reparto è distrutto e non riparabile: serve un nuovo ordine di costruzione, con nuovi costi e tempi; la distruzione di un reparto o impianto cancella tutti gli ordini direttamente o indirettamente collegati nel relativo ambito, inclusi produzione, riparazione, logistica, ordini continuativi e convogli non partiti; gli ordini cancellati restano in cronologia, il progresso è perso e le risorse già consumate non sono restituite; gli ordini cancellati non vengono ripristinati con la ricostruzione; il reparto ricostruito mantiene le proprie impostazioni personalizzate (incluse impostazioni strutturali, riparazione automatica e priorità predefinita degli ordini automatici), ma non le priorità dei singoli ordini cancellati né le impostazioni dipendenti da tali ordini |
| Convogli verso strutture distrutte/demolite | Gli ordini continuativi diretti alla struttura e gli ordini dei convogli non ancora partiti vengono cancellati; i convogli già in viaggio invertono la rotta, conservano il carico e lo riportano al punto di partenza; le risorse tornano disponibili all'arrivo |
| Vincoli di località | Energia e personale/popolazione locale |
| Ricette | 2-4 input; tutte le lavorazioni avanzano tramite LR e UDL; lotti con input consumati all'avvio e output al completamento; assemblaggi con consumo 50/30/20 |
| Trasporto | Navi cargo fisiche (leggero, medio, pesante, cisterne, convogli scortati); consumo di carburante proporzionale a distanza e carico; carichi intercettabili e perdibili; scorte assegnabili |
| Rotte permanenti | Ordini continuativi di trasporto periodico tra due località |
| Movimento nel sistema | Libero, in frazioni di mese |
| Movimento tra sistemi | Solo su rotte tra sistemi vicini; tempi in mesi interi (indicativamente 1-6) |
| Portali | Impianti che aprono/chiudono le rotte; serve solo nel sistema di partenza; usabili da proprietario e alleati; distruggibili/conquistabili; con livelli; posizione influisce solo sul carburante |
| Stati dei sistemi | Esplorabile (scoperta con sonde/flotte; si parte conoscendo sistema proprio e vicini); colonizzabile (corpo con requisiti minimi, dipende dalla tecnologia); abitato (colonia di qualsiasi impero, distinguendo giocatori e popolazioni autoctone/neutrali) |
| Filiera produttiva | Distinzione tra estrazione, raffinazione, produzione di componenti e assemblaggio finale; sviluppo dei dettagli ancora in corso |
| Materiali intermedi | Catena industriale articolata; materiali fisici stoccabili e trasportabili, con granularità mista tra risorse individuali e famiglie |
| Impianti industriali | Possibilità di integrare reparti produttivi interni; ogni reparto riceve una quota distinta del budget condiviso di PC e può lavorare in parallelo con gli altri |
| Modificatori produttivi | Efficienza globale da tecnologia e fattori sociali della civiltà; efficienza locale da fattori contingenti della località; applicazione sequenziale moltiplicativa |
| Lavoro produttivo | Ogni processo ha un Lavoro Richiesto (LR) in UDL; conversione base 1 PC = 1 UDL; progresso cumulativo conservato tra i mesi |
| Capacità e durata dei processi | Le UDL effettive derivano da PC assegnati × efficienza globale × efficienza locale; la durata dipende dal LR e dal lavoro mensile generato |
| Priorità produttive | Gli ordini produttivi usano la priorità dell'ordine; il sistema assegna prima ai livelli più alti e ripartisce equamente a parità di priorità; a parità di priorità gli ordini più vecchi precedono i nuovi |
| Redistribuzione PC | I PC liberati da un completamento sono redistribuibili dal mese successivo; divisione equa tra ordini della stessa priorità, con PC residui assegnati in ordine cronologico; redistribuzione dei PC non utilizzabili tra ordini della stessa priorità prima di passare a quella inferiore |
| Vincoli operativi e logistici | Possono sospendere un processo o l'intera produzione del complesso; i processi sospesi sono esclusi dalla distribuzione dei PC e non mantengono capacità riservata |
| Ripresa dei processi sospesi | Progresso e input consumati sono conservati; alla riattivazione il processo rientra come nuovo ordine nella sequenza di precedenza per la distribuzione dei PC |
| Organizzazione sociale e industria | Può influenzare costi, efficienza e convenienza delle configurazioni industriali, senza vincolare rigidamente i processi possibili |
| Priorità in caso di deficit | Priorità dell'ordine, poi mese di esecuzione |
| Annullamento | Rilascio immediato delle risorse non consumate |
| Consumo | Progressivo in tre fasi: 50%, 30%, 20% |
| Interruzione | Conserva progresso e prenotazioni |
| Ripresa | Continua dalla fase raggiunta |
| Interruzione prolungata | Nessuna scadenza automatica |
| Persistenza | Gli ordini incompleti passano all'anno successivo tramite la coda del TurnResolver |
| Revisione annuale | Gli ordini persistenti possono essere modificati o annullati prima del blocco |
| Morfologia galattica | Sempre a spirale |
| Numero di bracci | Variabile casualmente da 2 a 4 |
| Generazione della spirale | Casualità geometrica e distributiva |
| Bracci principali | Relativamente stretti, curvi e riconoscibili; irregolarità locale ammessa senza perdere la leggibilità globale |
| Irregolarità dei bracci | Interruzioni, segmenti distinti, biforcazioni e diramazioni locali ammesse |
| Regioni inter-braccio | Densità stellare significativa; possibili sistemi isolati e addensamenti secondari autonomi |
| Addensamenti secondari | Dimensioni eterogenee ma non estreme; forme prevalentemente irregolari e frammentate |
| Gerarchia visiva | I bracci principali restano dominanti rispetto agli addensamenti secondari |
| Sistemi stellari | Da 1.000 a 2.000 per galassia |
| Nucleo galattico | Estensione pari al 15–20% del diametro galattico |
| Nucleo: composizione | Alta densità, stelle molto antiche e maggiore concentrazione di buchi neri |
| Distribuzione dei sistemi | Indicativamente nucleo ~50%, regioni intermedie ~35%, periferia ~15%; transizione nucleo-intermedie graduale |
| Effetti delle regioni | Tendenze di generazione: nucleo ricco di buchi neri e materiali rari, pericoloso, pochi pianeti abitabili; periferia più pianeti abitabili e stelle giovani, meno risorse rare |
| Rotte: raggio | Raggio massimo fisso in tutta la galassia: nucleo molte rotte, periferia poche e lunghe |
| Rotte e portali: visibilità | Rotte note solo se scoperte; portali avversari e transiti visibili solo con intelligence/sensori |
| Portali: livelli | 1-5; determinano navi/tonnellaggio al mese; costo crescente |
| Popolazione | Numerica per colonia, in milioni; cresce in base a specie, fattori sociali e ambientali; fornisce personale |
| Generazione PR | 1 PR per turno ogni milione di abitanti, senza moltiplicatori; installazioni scientifiche generano PR aggiuntivi secondo produzione base per tipologia e livello, bonus di specializzazione, requisiti operativi, disponibilità di personale e modificatori ambientali, sociali e contingenti |
| Accumulo PR | Serbatoio di ricerca centralizzato per impero; PR frazionari conservati con precisione decimale interna; i PR non spesi si accumulano |
| Avvio e parallelismo della ricerca | Ogni tecnologia ha un costo in PR per avviare lo studio; massimo tre ricerche attive per impero; i PR non assegnati restano nel serbatoio |
| Sviluppo tecnologico | Ogni tecnologia ha un tempo di sviluppo distinto dal costo in PR; il tempo è modificabile da fattori ambientali, sociali e contingenti |
| Installazioni scientifiche | Tre categorie: laboratori planetari (ricerca sperimentale e applicata), università/istituti (ricerca teorica e multidisciplinare) e stazioni scientifiche orbitali (ricerca scientifica e spaziale avanzata); produzione base L1: 1, 2 e 3 PR/turno rispettivamente |
| Produzione PR per livello | Moltiplicatori provvisori comuni: L1 ×1,00; L2 ×1,50; L3 ×2,00; L4 ×2,50; L5 ×3,00; valori soggetti a bilanciamento |
| Bonus di specializzazione scientifica | +25% alla produzione PR dell'installazione quando contribuisce a una ricerca compatibile; bonus applicato una sola volta per installazione, anche in presenza di più condizioni di compatibilità; installazioni diverse applicano ciascuna il proprio bonus; nessuna riduzione del costo PR della tecnologia |
| Specializzazione dei laboratori planetari | Ogni laboratorio è associato a uno specifico ramo TechTree, scelto dal giocatore al momento della costruzione; può ospitare ricerche di qualsiasi ramo, ma riceve +25% PR solo sulle ricerche del ramo corrispondente; specializzazione modificabile tramite riconversione con costi e tempi da definire; durante la riconversione il laboratorio diventa inoperativo, le ricerche assegnate sono sospese automaticamente con progresso integralmente conservato e riprendono automaticamente al termine; è possibile trasferirle preventivamente, facoltativamente |
| Specializzazione delle stazioni orbitali | Bonus +25% alla produzione di PR nei soli rami D. Scienza e Intelligence e C. Navale e Propulsione; ramo E. Colonizzazione e Biologico escluso anche in presenza di laboratori dedicati |
| Requisiti operativi delle installazioni scientifiche | Necessari integrità strutturale sufficiente, energia e presenza di personale scientifico; la carenza parziale riduce la produzione senza rendere automaticamente inattiva la struttura; per le stazioni orbitali servono anche i collegamenti logistici necessari al sostentamento; soglie e quantità da definire |
| Personale scientifico | Tre categorie: ricercatori, tecnici scientifici e personale di supporto scientifico; la carenza riduce linearmente la produzione PR, il personale oltre il fabbisogno non dà bonus e l'assenza totale azzera la produzione; fabbisogni richiesti scalano con il livello secondo ×1,00/×1,50/×2,00/×2,50/×3,00 |
| Formazione scientifica | Ogni università/istituto forma annualmente tutte e tre le categorie; in condizioni neutre la produzione base è uguale per categoria; modificatori tecnologici, sociali globali e specifici, ambientali locali e contingenti possono modificarne quantità e distribuzione; i fattori sociali globali, inclusa l'impostazione di governo, possono favorire categorie differenti |
| Sede della ricerca | Ogni progetto è assegnato a uno specifico laboratorio planetario, università/istituto o stazione scientifica orbitale; la sede determina i modificatori locali ed è un obiettivo strategico |
| Interruzione della ricerca | Sede danneggiata ma operativa: ricerca continua; sede temporaneamente inoperativa: ricerca sospesa e progresso conservato; sede distrutta: perdita parziale del progresso secondo `(T_totale − T_trascorso) × 100 / T_totale`, applicata solo alla distruzione completa; `T_totale` è congelato al momento dell'avvio, dopo i modificatori allora vigenti |
| Trasferimento della ricerca | Progetto trasferibile a un'altra installazione compatibile; nessuna ulteriore perdita di progresso; ricerca sospesa per un turno e velocità aggiornata in base alla nuova sede |
| Specie | Specie giocabili con tratti propri; una specie principale per impero, minoranze possibili |
| Stabilità | Felicità/stabilità per colonia, da organizzazione sociale, condizioni e guerra; influisce su efficienza e rivolte |
| Sostentamento | Cibo per la popolazione, energia per i servizi; carenza di cibo riduce crescita e stabilità fino alla carestia |
| Fondazione colonie | Navi coloniali dai cantieri, ordine di colonizzazione; la colonia parte piccola e cresce |
| Flotte e navi | Flotte composte da navi di classi diverse, alcune sbloccate dalla tecnologia; categorie civili (logistica) e militari (spionaggio, combattimento) |
| Progettazione navi | Classi personalizzabili componente per componente, oppure configurazione automatica con il componente a più alta tecnologia |
| Scafi | 5 taglie, slot provvisori 4/8/14/22/32; slot generici; propulsione obbligatoria; Piccolo e Medio da subito, le altre via tecnologia; livello minimo di Cantiere per taglia (provvisorio L1/L1/L2-3/L4/L5) |
| Componenti | Armamenti (con contromisure elettroniche), spionaggio, ricerca, colonizzazione, propulsione; 5 livelli tecnologici per tipo; prodotti in Fabbrica, usati dal Cantiere; costo progetto = componenti + scafo |
| Propulsione | Livelli con velocità di crociera, di combattimento e consumo di carburante crescenti; velocità nave = componente più lento |
| Personale qualificato | Risorsa specializzata unica, da popolazione addestrata in Accademia (pianeta o stazione), trasferita al cantiere come risorsa; richiesto da componenti più quota base per scafo; consumato alla costruzione (50/30/20), perso se la nave è distrutta |
| Combattimento | Automatico, risolto dal TurnResolver; round (provvisorio: 3 per mese) con sotto-istante; prosegue nei mesi successivi se non risolto |
| Dottrina di battaglia | Aggressiva, Difensiva, Ritirata a soglia di danno; guidano le scelte automatiche |
| Fine scontro | Una parte distrutta, ritirata riuscita o nessuna parte ostile nel sistema |
| Ritirata | Al round successivo alla soglia, verso il sistema di provenienza lungo una rotta valida; inseguimento in base alla velocità di combattimento |
| Danno e difese | Scudi assorbono e si rigenerano a ogni round; scafo/armatura con danno permanente fino a riparazione; scudi e armatura sono componenti Armamenti difensivi |
| Tipi di danno | Per esempio energia, cinetico, missili; efficacia delle difese diversa per tipo |
| Contromisure | Riducono precisione di missili e bersagliamento nemico |
| Bersagli | Casuale ponderato (seed) tra le navi nemiche; la dottrina può dare priorità |
| Avamposti | Personale ridotto e importato, popolazione minima; gli impianti richiedono personale locale |
| Età stellare | Gradiente radiale dal centro alla periferia, con eccezioni locali |
| Regioni intermedie | Popolazione stellare mista |
| Regioni periferiche | Maggiore presenza relativa di stelle giovani e sistemi di formazione stellare |
| Valore dei sistemi | Composizione astronomica e valore economico/strategico restano distinti |
| Contenuto dei sistemi | Stelle di classi diverse, pianeti, lune, cinture di asteroidi, anomalie |
| Numero di stelle | Per lo più singole; binari e multipli circa 5%, fino a 3 stelle; pianeti attorno a una o più stelle |
| Numero di pianeti | Pochi (indicativamente 0--6); molti sistemi vuoti o poveri |
| Anomalie | Poche, con effetti di gameplay |
| Colonizzabilità | Pochi pianeti abitabili; gli altri corpi solo con tecnologia o come avamposti/miniere |
| Classi stellari | O, B, A, F, G, K, M più nana bianca, pulsar/stella di neutroni, buco nero |
| Frequenza stellare | Gradiente realistico: M molto comuni, O/B rare |
| Effetti della classe stellare | Indiretti: abitabilità, energia, tipi di pianeti |
| Tipi di pianeta | Rocciosi, oceanici, desertici, ghiacciati, gassosi, giganti di ghiaccio, lavici/infernali |
| Abitabilità | Fascia abitabile, tipo di pianeta, atmosfera e temperatura |
| Anomalie | Nebulose, campi di asteroidi densi, buchi neri, pulsar/radiazioni, tempeste di plasma |
| Lune | Poche; possibili attorno a ogni tipo di pianeta, più probabili attorno ai giganti; ruolo di avamposto/miniera; abitabili molto raramente |
| Effetti delle anomalie | Nebulose: sensori e movimento ridotti, risorse rare. Asteroidi densi: rallentano e danneggiano, ricchi di minerali. Buchi neri: distruggono navi, energia per ricerca. Pulsar/radiazioni: danni a equipaggi ed elettronica, niente colonie abitate. Tempeste di plasma: temporanee e mobili, danni agli scudi, interrompono movimento e comunicazioni |
| Risorse nei corpi celesti | Ricchezza per ciascuna risorsa legata al tipo di corpo; minerali da rocciosi/lavici/asteroidi, carburante da gassosi/giganti di ghiaccio, materiali avanzati rari, cibo da oceanici/rocciosi abitabili, energia da stella e anomalie |
| Rappresentazione del tempo | Tick interi assoluti; 1000 tick per mese, 12.000 per anno; nessun floating-point |
| Sotto-istante | Discreto, intero in [0, 999], non continuo |
| Ordinamento eventi | Confronto lessicografico totale: EtaTick → Tipo → Priorità → TiebreakSeed |
| TiebreakSeed | `Hash(seed_partita, order_id, event_seq)`, calcolato una volta e mai modificato |
| Blocco persistente | Flag `Blocked` con condizione; ETA invariato; rivalutazione a inizio mese; ripresa automatica |
| Ricalcolo ETA | Solo per modifiche fisiche del percorso (`NeedsRecalc`); nuovo ETA esplicito; notifica al giocatore |
| Limite ricalcoli | Default 3, poi `Failed` con `RepeatedRecalc` |
| Fasi del mese | A: rivalutazione condizioni (tutti i bloccati/waiting, anche se ETA futuro); B: estrazione eventi del mese; C: esecuzione sequenziale; D: chiusura |
| Barriera di pianificazione | Il TurnResolver parte solo quando tutti i giocatori attivi hanno confermato il piano |
| Immutabilità del piano | Una volta superata la barriera, il piano è immutabile per l'anno; modifiche solo nella pianificazione successiva |
| Ingestione annuale | Una sola volta, atomica, prima del primo mese; merge template + merge ordini incompleti + conversione nuovi ordini |
| Coda persistente | Fonte di verità durante la risoluzione; può crescere per eventi figli generati in Fase C |
| Ordini incompleti (tipo A) | Continuano nella coda, mantengono progresso, modificabili in pianificazione |
| Ordini continuativi (tipo B) | Template che genera cicli al completamento del precedente; revoca = stop generazione, ciclo in corso si completa; modifiche non retroattive |
| Crescita | Evento `Growth` mensile per colonia |
| Ripresa manuale §4.3.6 | Azione di pianificazione, non evento in coda |
| Formato del merge | Comandi (order_id, operazione, payload) ordinati per order_id; operazioni Create/Modify/Cancel/Revoke; atomico per comando, ultimo comando vince |
| RequiredFuel | Calcolato alla generazione dell'evento, riverificato all'esecuzione (Waiting se manca); non ricalcolato salvo NeedsRecalc |
| Modifiche per categoria di ordine | Tabella §2.10.1: puntuali, tipo A, tipo B, temporizzati, condizionali |
| Dipendenze all'annullamento | Dipendenti in Waiting (DependencyCancelled), poi Failed se non riassegnati |
| Arrotondamento 50/30/20 | Interi; fasi 1-2 per difetto, fase 3 il resto; sotto i 3 mesi fase unica |
| Tempo residuo e deterioramento | Residuo = durata - progresso (per eccesso); nessun deterioramento in v1 |
| Payload degli eventi | Registro separato con `PayloadId`, immutabile |
| Engagement | Entità persistente in `MutableWorldState`; gli eventi portano solo `EngagementId` |
| Capacità portale | `PortalCapacityLedger` mensile; esaurita = `Blocked` |
| IIntelligenceService | Visibilità per posizione; dettaglio per capacità; puro e deterministico |
| Outcome | Completed, Blocked, Waiting, Failed, NeedsRecalc; Blocked/Waiting scartano il delta |
| Struttura per taglia | 100 / 300 / 800 / 2000 / 5000 (valori provvisori); livelli tecnologici ×1, ×1,6, ×2,5, ×4, ×6 |
| Tipi di danno | Energia, cinetico, missili con moltiplicatori diversi su scudi e armatura (§7.9) |
| Sequenza del danno | Precisione (90% meno contromisure) → scudi → armatura → struttura; scudi rigenerati 25% a fine round |
| Round | 3 al mese, simultanei: danni calcolati a inizio round e applicati a fine round |
| Soglia di ritirata | 50% della struttura della flotta (default); riesce se velocità di combattimento ≥ inseguitore |
| Riparazioni | Solo con Cantiere/stazione; 30% del costo per riparazione completa; ritmo legato ai PC; ausiliarie in volo a metà ritmo |
| Spionaggio | Punti Intelligence; dettaglio osservato in base al confronto con contromisure |
| Costo in PC delle navi | 100 / 300 / 800 / 2000 / 5000, durata minima 3 / 4 / 6 / 9 / 12 mesi |
| Personale per nave | Quota base 5 / 15 / 40 / 100 / 250 per taglia più personale dei componenti (L1: propulsione 1, armi 2, difese 1, spionaggio 2, ricerca 3, colonizzazione 4; livelli con moltiplicatori §7.9) |
| Accademia | Livelli 1-5; capacità 10 / 25 / 60 / 120 / 250 al mese; ciclo di 2 mesi; costo per persona 1 migliaio di abitanti, 1 cibo, 1 energia; `Waiting` sotto la popolazione minima |
| Rotte | Stati: Aperta / Chiusa (`Blocked`, ETA invariato) / Distrutta (`NeedsRecalc` o `Failed`) |
| Log del TurnResolver | Strutturato, filtrato per destinatario prima dell'emissione; log completo solo in build di sviluppo |
| Eventi figli in Fase C | Ammessi (round di combattimento, cicli continuativi); determinismo garantito dallo stato; limite anti-loop: `MaxChildrenPerEvent` (64), `MaxEventsPerMonth`, 1 ciclo/mese per ordine continuativo, poi `Failed` `LoopLimit` |
| `Engagement` | Entità persistente dello scontro, sopravvive a round e mesi |

------------------------------------------------------------------------

## 11. Architettura tecnica del TurnResolver

Questo capitolo raccoglie l'abbozzo tecnico discusso nella progettazione
dei Blocchi 1–3. Non è ancora codice definitivo, ma fissa le interfacce
e le scelte architetturali.

### 11.1 Struttura dei componenti
TurnResolver
├─ persistentQueue : SortedSet<GameEvent> // coda persistente
├─ blockedOrWaiting : List<GameEvent> // eventi sospesi
├─ IWorldState world // facciata read-only
├─ MutableWorldState mutableWorld // stato mutabile
├─ IEventExecutor executor // dispatcher
│ ├─ IMovementExecutor
│ ├─ IProductionExecutor
│ ├─ ICombatExecutor
│ └─ IGrowthExecutor
└─ ILogDispatcher log // log per-utente

### 11.2 Contratto di `IEventExecutor`

Un executor:

-   legge da `IWorldState`;
-   **non** modifica direttamente il mondo;
-   restituisce un `ExecutionResult` contenente `Outcome`, eventuale
    `Condition` (se `Blocked`), eventuale `NewEta` (se `NeedsRecalc`),
    eventuale `WorldDelta` (effetti da applicare), ed eventuali
    `GeneratedEvents` (eventi figli).

Il TurnResolver applica il `WorldDelta` **solo** se l'`Outcome` è
`Completed` o `NeedsRecalc` con effetti parziali. Un evento `Blocked` o
`Waiting` scarta il delta: il blocco è atomico, o si applica tutto o niente.

### 11.3 `ExecutionResult`

```csharp
public enum Outcome { Completed, Blocked, Waiting, Failed, NeedsRecalc }

public sealed class ExecutionResult
{
    public Outcome            Outcome;
    public IBlockCondition    Condition;
    public GameTime           NewEta;
    public string             Reason;
    public WorldDelta         Delta;
    public List<GameEvent>    GeneratedEvents;
}
```

### 11.4 WorldDelta e mutazioni
```csharp
public sealed class WorldDelta
{
    public List<IWorldMutation> Mutations { get; } = new();
    public bool IsEmpty => Mutations.Count == 0;
}

public interface IWorldMutation
{
    void Apply(MutableWorldState world);
    string Describe(int recipientPlayerId);
}

```

Mutazioni tipiche: MoveFleet, ConsumeResource, ProduceResource,
ReserveResource, ReleaseReservation, AdvanceProductionPhase,
ApplyDamage, DestroyShip, SetFleetDoctrine, GrowPopulation,
ChangeStability.

### 11.5 Payload degli eventi
Ogni GameEvent porta un payload tipizzato per EventType:

MovementPayload (FleetId, Origin, Destination, RouteId, PortalId,
RequiredFuel, Phase);

ProductionPayload (FacilityId, DepartmentId, RecipeId, Kind,
AssemblyPhase, CyclesRequested);

CombatPayload (EngagementId, RoundIndex, AttackerFleetIds,
DefenderFleetIds, Doctrines);

GrowthPayload (ColonyId, Kind).

**Scelta [CONFERMATO]:** registro separato con `PayloadId`. Il registro è serializzabile nativamente in Unity, senza converter custom. Il payload è immutabile dopo la creazione dell'evento.

### 11.6 Log per-utente
Il TurnResolver emette LogEntry con destinatario esplicito:

Eventi propri: sempre visibili, in forma completa.

Eventi avversari: visibili solo se IIntelligenceService. CanObserveEvent(playerId, evt) è vero.

Eventi globali: visibili a tutti, marcati esplicitamente.

Eventi neutrali/autoctoni: visibili solo se scoperti.

Il filtro non è di visualizzazione: il payload viene sanitizzato prima
dell'emissione, e il log completo è disponibile solo in build di
sviluppo, mai nel client di produzione.

### 11.7 Decisioni sui punti dell'architettura [CONFERMATO]

1.  **Payload:** registro separato con `PayloadId` (§11.5).
2.  **Engagement:** entità persistente in `MutableWorldState`
    (`Engagements[EngagementId]`) con sistema, partecipanti, dottrine,
    `RoundIndex` e stato. Creata dal primo evento di combattimento e
    rimossa a fine scontro (§7). Gli eventi `Combat` portano solo
    `EngagementId`, così gli scontri multi-mese sopravvivono ai
    salvataggi.
3.  **Anti-loop:** vedi §2.3.4.
4.  **Capacità portale mensile:** `PortalCapacityLedger[PortalId, mese]`
    in `MutableWorldState`. La prenotazione avviene con la mutazione
    `ReservePortalCapacity` nel delta del movimento, nell'ordine di
    §2.3.1 (deterministico). Capacità esaurita: il movimento va in
    `Blocked` e riprova a inizio mese. Il registro si azzera a inizio
    mese.
5.  **`IIntelligenceService`:** `CanObserveLocation(playerId, systemId)`
    è vero se il giocatore ha sensore, colonia o flotta nel sistema, o
    una rete di sorveglianza attiva che lo copre.
    `CanObserveEvent(playerId, evt)` è vero se `CanObserveLocation` è
    vero per la posizione dell'evento; il livello di dettaglio dipende
    dalla capacità di intelligence e il payload viene sanitizzato di
    conseguenza (§1.3). Il servizio è puro e deterministico e legge lo
    stato a inizio mese.
6.  **`RequiredFuel`:** vedi §2.3.5.

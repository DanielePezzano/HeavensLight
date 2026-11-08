# Documento di Game Design --- Bozza iniziale

**Progetto:** Test di Codice\
**Versione:** 0.29 --- bozza di lavoro\
**Data:** 6 novembre 2026\
**Stato:** consolidamento delle decisioni emerse nella progettazione

> Questo documento raccoglie le regole concordate finora. Le decisioni
> confermate sono distinte dalle proposte e dalle questioni ancora
> aperte. Le parti non ancora definite non devono essere considerate
> regole definitive.

------------------------------------------------------------------------

## 1. Visione e pilastri del gioco

Il progetto è un gioco strategico spaziale 4X, con una propria identità
e non una semplice replica di altri titoli del genere.

### 1.1 Modalità di gioco

-   Il gioco deve supportare sia il single-player sia il multiplayer.
-   Le partite possono includere giocatori umani e imperi controllati
    dall'intelligenza artificiale.

### 1.2 Struttura temporale e simultaneità

-   Il gioco è a turni.
-   Le azioni vengono pianificate dai giocatori e poi risolte
    simultaneamente.
-   Un giocatore non può vedere le mosse pianificate dagli avversari.
-   La risoluzione simultanea avviene dopo che tutti i giocatori hanno
    concluso la fase di pianificazione prevista.

### 1.3 Informazioni e intelligence

La segretezza delle mosse avversarie è una regola generale, ma non
assoluta: tecnologie, spionaggio e reti di sorveglianza possono
consentire di ottenere informazioni sugli altri imperi, entro i limiti
delle capacità effettivamente sviluppate.

### 1.4 Logistica militare

La logistica deve avere un peso sostanziale. Le flotte non devono poter
combattere indefinitamente senza supporto: composizione delle flotte,
navi ausiliarie, rifornimenti e catene logistiche devono influire sulle
operazioni militari.

------------------------------------------------------------------------

## 2. Capitolo 1 --- Tempo e sistema degli ordini

### 2.1 Unità temporale

-   Un turno di gioco corrisponde a **un anno**.
-   Ogni anno è suddiviso in **12 mesi**, che costituiscono le unità
    temporali della simulazione.
-   La simulazione procede mese per mese: un mese viene risolto prima di
    passare al successivo.

### 2.2 Pianificazione annuale

-   Durante la fase di pianificazione, il giocatore organizza in
    anticipo le proprie azioni per tutti i 12 mesi dell'anno.
-   Gli ordini possono essere distribuiti liberamente nei diversi mesi.
-   Un mese può non contenere alcun ordine.
-   Gli ordini possono durare più mesi e proseguire oltre la fine
    dell'anno.
-   Prima della risoluzione, il piano annuale viene confermato e gli
    ordini diventano bloccati per l'anno in corso.

### 2.3 Risoluzione simultanea

-   Gli ordini dei diversi giocatori vengono risolti nello stesso
    contesto temporale.
-   Il motore di simulazione non deve avvantaggiare un giocatore
    soltanto perché le sue azioni vengono elaborate prima internamente.
-   La risoluzione procede per mesi, completando la simulazione di
    ciascun mese prima di avviare quella del mese successivo.
-   I giocatori non vedono gli ordini degli avversari.

**Da definire:** algoritmo tecnico per risolvere eventi simultanei,
conflitti di movimento, incontri tra flotte, combattimenti e altri
eventi concorrenti.

### 2.4 Tipologie e struttura degli ordini

Il sistema deve poter rappresentare ordini di diversa natura, tra cui:

-   **Puntuali:** eseguono un'azione singola.
-   **Continuativi:** rimangono attivi fino al completamento o alla
    revoca.
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
-   Rimane in attesa.
-   Le condizioni di esecuzione vengono rivalutate ogni mese.
-   Quando le condizioni tornano valide, l'ordine può riprendere secondo
    le proprie istruzioni e priorità.

**Da definire:** criteri per distinguere in modo formale un impedimento
temporaneo da un'impossibilità permanente e comportamento degli ordini
divenuti definitivamente non validi.

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

**Da definire:** eventuali ulteriori criteri di spareggio se priorità e
mese di esecuzione coincidono.

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

**Da definire:** regole di arrotondamento per costi non divisibili e
comportamento per attività di durata molto breve o non suddivisibile in
tre intervalli interi.

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

-   Gli ordini incompleti vengono trasferiti automaticamente all'anno
    successivo.
-   Mantengono il proprio stato e il progresso già raggiunto.
-   Le prenotazioni valide e non ancora consumate restano associate agli
    ordini.
-   Durante la nuova fase di pianificazione annuale, prima del blocco
    del piano, il giocatore può intervenire sugli ordini persistenti.

Per ciascun ordine persistente, il giocatore può: - lasciarlo
invariato; - modificarne i parametri consentiti dal tipo di ordine; -
cambiarne la priorità; - modificarne le condizioni operative; -
annullarlo, liberando immediatamente le risorse prenotate e non ancora
consumate.

Le modifiche diventano effettive con la conferma del nuovo piano
annuale.

------------------------------------------------------------------------

## 3. Capitolo 2 --- Galassia e generazione della mappa

Questo capitolo raccoglie le prime regole concordate per la struttura
generale della galassia e per la generazione procedurale dei sistemi
stellari. I parametri non ancora definiti sono riportati tra le questioni
aperte e non costituiscono regole definitive.

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

### 3.7 Movimento e collegamenti tra sistemi [CONFERMATO]

Il movimento segue un **modello ibrido** su due scale.

#### 3.7.1 Movimento interno al sistema

-   All'interno di un sistema stellare le flotte si muovono
    **liberamente** tra i corpi celesti e le località del sistema.
-   I tempi sono espressi in **frazioni di mese**: gli spostamenti
    interni si concludono spesso entro il mese.
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

Valori numerici (raggio, costi dei portali, capacità per livello, scala
dei tempi) sono da definire.

-----------------------------------------------------------------------

## 4. Capitolo 3 --- Risorse ed economia

Questo capitolo raccoglie le regole confermate sulle risorse. I dettagli
non ancora decisi sono riportati nelle questioni aperte (§7.1).

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
- prodotti in uscita ed eventuali sottoprodotti.

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
principale, un impianto richiesto e una tecnologia richiesta. Gli
eventuali sottoprodotti sono da definire.

**Due tipi di ricetta:**

1. **Ricette a lotto mensile** (materiali raffinati, componenti):
   - la ricetta produce una **quantità fissa per ciclo di 1 mese**;
   - la capacità assegnata al reparto, espressa in Punti Capacità (PC),
     determina quanti cicli possono essere eseguiti in parallelo nello
     stesso mese, in base al fabbisogno di PC della ricetta (§4.3.6);
   - gli input vengono consumati all'avvio di ciascun ciclo.
2. **Ricette di assemblaggio** (navi, strutture, prodotti finiti):
   - richiedono componenti e materiali in ingresso;
   - hanno una **durata in mesi**;
   - il consumo segue la regola **50/30/20** della sezione 2.7, con
     prenotazione, interruzione e annullamento come già definiti.

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

#### 4.3.6 Capacità produttiva, Punti Capacità e assegnazione ai reparti [PRINCIPI CONFERMATI, PARAMETRI IN CORSO DI DEFINIZIONE]

**Unità comune di capacità**

La capacità produttiva degli impianti è misurata in **Punti Capacità
(PC)**. La scala dei PC è comune a tutte le tipologie di impianto: un
PC rappresenta la stessa unità di capacità indipendentemente dal tipo
di struttura.

Ogni tipologia di impianto dispone di un proprio valore di capacità
base, espresso in PC, al livello 1. È confermata la conversione diretta
della scala relativa: **un punto dell'indice equivale a un PC**. I valori
base in PC coincidono quindi con gli indici approvati riportati più
avanti. La dimensione o classe dell'impianto non costituisce una
variabile aggiuntiva: per ciascuna tipologia è previsto un solo valore
base.

**Progressione dei livelli**

Il livello dell'impianto principale determina la capacità complessiva
disponibile. La progressione percentuale è uniforme per tutte le
tipologie:

| Livello | Capacità rispetto al livello 1 |
|---:|---:|
| 1 | 100% |
| 2 | 150% |
| 3 | 200% |
| 4 | 250% |
| 5 | 300% |

La formula di riferimento è:

`PC_totali = PC_base × moltiplicatore del livello`

Il livello massimo raggiungibile dipende dallo sblocco di una
specifica ricerca tecnologica. Il potenziamento dell'impianto principale
consente di sostenere più reparti oppure di aumentare il ritmo
produttivo dei reparti esistenti.

**Budget condiviso tra reparti**

I PC costituiscono un budget complessivo condiviso dall'impianto
principale. Il giocatore assegna ai singoli reparti una quota di tale
capacità. I reparti possono lavorare in parallelo, ma la somma dei PC
assegnati non può superare la capacità totale disponibile. Non esiste
una capacità indipendente e illimitata per ciascun reparto e non sono
previsti livelli di potenziamento separati per i singoli reparti.

Ogni ricetta o processo industriale richiede un fabbisogno di PC
specifico, indicato nella relativa ricetta o scheda del processo. Per i
processi articolati su più mesi, il fabbisogno rappresenta la capacità
necessaria per ciascun ciclo mensile di lavorazione: la durata effettiva
dipende quindi dai PC che il reparto può mettere a disposizione ogni
mese.

**Configurazione e programmazione dell'assegnazione**

Il giocatore può assegnare manualmente i PC ai reparti. È prevista
anche una modalità automatica basata su modelli di assegnazione
prestabiliti; i modelli concreti e i loro criteri sono ancora da
definire.

La configurazione dell'assegnazione è persistente: una volta applicata,
resta valida nei mesi successivi fino a una nuova modifica. Il
giocatore può programmare la data di decorrenza della nuova
configurazione, indicando mese e anno. Fino a tale data rimane attiva
la configurazione precedente; dalla data stabilita entra in vigore la
nuova configurazione, che resta valida fino alla successiva modifica.

La configurazione comprende sia la modalità di assegnazione (manuale
o automatica) sia la distribuzione dei PC tra i reparti.

**Validità mensile dei PC assegnati**

I PC assegnati a un reparto costituiscono la sua disponibilità per il
mese di riferimento. I PC non utilizzati entro la fine del mese
**scadono** e non vengono accumulati né trasferiti al mese successivo.

All'inizio di ogni nuovo mese il reparto riparte dalla configurazione di
assegnazione attiva in quel momento. La quota mensile viene quindi
rinnovata secondo tale configurazione, senza riportare eventuali PC
inutilizzati nei mesi precedenti.

Esempio: un reparto al quale sono assegnati 25 PC, ma che nel mese
riesce a utilizzarne soltanto 18, perde i 7 PC residui alla fine del
mese. Nel mese successivo dispone nuovamente dei 25 PC previsti dalla
configurazione attiva, non di 32 PC né di una quota ridotta.

**Assorbimento dei PC da parte dei processi**

Ogni ricetta o processo produttivo specifica il proprio fabbisogno di
PC per ciclo produttivo. I PC vengono assorbiti dal reparto durante
l'esecuzione del ciclo; il numero di cicli eseguibili nello stesso mese
dipende dalla quota di PC assegnata al reparto e dai fabbisogni dei
processi attivi.

Esempio: se un processo richiede 10 PC per ciclo e il reparto dispone
di 60 PC mensili, il reparto può completare fino a 6 cicli nel mese,
purché siano disponibili tutti i materiali in ingresso e siano
soddisfatte le altre condizioni operative.

Per i processi che richiedono più capacità di quella che il complesso
può fornire in un singolo mese, la lavorazione si estende su più mesi.
Per esempio, un processo con fabbisogno complessivo di 20 PC, eseguito
da un complesso che può dedicargli una capacità effettiva di 5 PC al
mese, richiede 4 mesi di lavorazione, in assenza di interruzioni.

La capacità effettiva mensile può essere inferiore a quella nominale
per effetto di fattori esterni, inclusi fattori sociali, ambientali e
altri modificatori già previsti. Il tempo di lavorazione si allunga in
proporzione alla capacità effettivamente disponibile; i valori e le
formule dei singoli modificatori restano da definire.

**Priorità dei processi produttivi**

Il giocatore può stabilire manualmente le priorità dei processi
produttivi. In alternativa, in fase di creazione o configurazione del
processo produttivo, può selezionare un **modello produttivo** che
imposta una configurazione predefinita di priorità.

Il modello produttivo non si limita ai processi già presenti: le sue
regole si applicano anche ai processi inseriti successivamente, finché
il modello rimane la configurazione attiva. I nuovi processi
rispettano quindi la configurazione vigente nel momento in cui vengono
creati, sia essa automatica (modello) o manuale.

Il giocatore può modificare manualmente le priorità indicando il mese
e l'anno a partire dai quali la modifica deve diventare effettiva. Da
quel momento la nuova configurazione resta attiva anche nei mesi
successivi, fino a un ulteriore intervento. Le modifiche non alterano
retroattivamente i mesi precedenti.

In caso di parità nel livello di priorità, il giocatore può attivare un
**flag di precedenza assoluta** per indicare quale processo debba essere
servito prima. Tale flag prevale sugli altri criteri di spareggio.
La data del processo può essere utilizzata come ulteriore criterio
quando il flag non risolve la parità; il criterio cronologico preciso
è da formalizzare.

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

**Modificatori della capacità effettiva**

La capacità effettivamente disponibile può variare rispetto al valore
base per effetto di fattori sociali, ambientali e tecnologici. È
confermato il principio della loro influenza, ma fattori specifici,
entità e formule di applicazione restano da definire.

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
interessato senza comportare la perdita della capacità assegnata. La
quota di PC riservata al processo sospeso rimane impegnata e non può
essere utilizzata da altri processi durante il blocco.

La sospensione può riguardare un singolo processo oppure l'intera
attività produttiva del complesso, in funzione della natura e
dell'estensione del vincolo. Per esempio, se una flotta nemica blocca
l'asteroide sul quale si trovano un complesso produttivo, un deposito e
uno spazioporto, la produzione di tutti i reparti del complesso si
arresta. Le rispettive quote di capacità restano riservate durante il
blocco.

**Ripresa dei processi sospesi per vincoli operativi o logistici**

Quando viene meno la condizione che ha causato la sospensione, il
processo può essere riattivato manualmente dal giocatore prima della
riattivazione automatica. In assenza di intervento, il sistema lo
riavvia automaticamente all'inizio del mese successivo, purché tutte
le condizioni operative necessarie siano nuovamente soddisfatte.
Anche la riattivazione manuale è subordinata alla verifica di tali
condizioni: il giocatore non può forzare l'esecuzione di un processo
che rimane impossibile. Durante la sospensione il progresso già
raggiunto viene conservato e la capacità assegnata resta riservata;
alla ripresa, la lavorazione continua dal punto raggiunto.

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
condiviso dall'impianto principale (§4.3.6). Ogni reparto dispone quindi
di una quota distinta e assegnata, non di una capacità autonoma che si
aggiunge senza limiti a quella degli altri reparti.

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
singoli sistemi stellari. I parametri non ancora definiti sono riportati
nelle questioni aperte (§7.5).

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

### 5.6 Classi stellari

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

### 5.7 Tipi di pianeta

Esistono sette tipi:

-   rocciosi;
-   oceanici;
-   desertici;
-   ghiacciati;
-   gassosi;
-   giganti di ghiaccio;
-   lavici/infernali.

### 5.8 Abitabilità

L'abitabilità di un pianeta dipende da:

-   la **fascia abitabile** della stella;
-   il **tipo di pianeta**;
-   **atmosfera e temperatura**.

### 5.9 Anomalie

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

I valori numerici, i raggi d'azione e le regole di rischio sono da
definire.

### 5.10 Lune

-   Le lune sono **poche**.
-   Possono orbitare attorno a **qualsiasi tipo di pianeta**, ma la
    probabilità è **maggiore per i pianeti giganti**.
-   Il loro ruolo principale è quello di **avamposti o miniere**.
-   Una luna può essere **abitabile solo molto raramente** e in
    condizioni particolari, per esempio una luna di un gigante situato
    nella fascia abitabile della stella.

### 5.11 Origine delle risorse nei corpi celesti

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

-   Le associazioni indicano le fonti prevalenti, non esclusive: i
    valori effettivi di ciascun corpo sono da definire.

------------------------------------------------------------------------

## 6. Capitolo 5 --- Colonie e popolazione

Questo capitolo raccoglie le prime regole confermate su colonie e
popolazione. I parametri non ancora definiti sono riportati nelle
questioni aperte (§7.6).

### 6.1 Popolazione

-   Ogni colonia ha una **popolazione numerica**, espressa in
    **milioni**.
-   La popolazione **cresce nel tempo** in base a fattori **di specie,
    sociali e ambientali**.
-   La popolazione **fornisce il personale** necessario al
    funzionamento degli impianti (§4.3.4).

### 6.2 Specie

-   Esistono **specie giocabili** con tratti propri, per esempio
    crescita, ambienti preferiti e bonus.
-   Ogni impero ha una **specie principale**; sono possibili
    **minoranze** di altre specie.

### 6.3 Stabilità sociale

-   Ogni colonia ha una **felicità/stabilità**.
-   È influenzata da **organizzazione sociale** dell'impero,
    **condizioni** della colonia e **guerra**.
-   Influisce su **efficienza** produttiva e **rivolte**.
-   L'organizzazione sociale è lo stesso fattore che incide
    sull'efficienza industriale (§4.4).

### 6.4 Sostentamento

-   La popolazione richiede **cibo**; i servizi richiedono **energia**.
-   La carenza di cibo riduce **crescita e stabilità**, fino alla
    **carestia**.
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

### 6.6 Colonie, impianti e avamposti

-   Gli impianti richiedono **personale locale**.
-   Gli **avamposti e le miniere** su corpi non abitabili hanno
    personale **ridotto e importato** e una popolazione minima.

------------------------------------------------------------------------

## 7. Questioni aperte e decisioni da affrontare

Le seguenti questioni non sono ancora state definite in modo conclusivo
e non devono essere considerate regole approvate.

### 7.1 Risorse, produzione e logistica

-   Catalogo completo delle ricette: materiali di ciascuna famiglia,
    quantità, tempi, sottoprodotti e bilanciamento dei valori.
-   Tipi di depositi naturali, distribuzione delle risorse e relazione
    tra depositi, corpi celesti e fonti ambientali.
-   Strutture e tecnologie richieste per estrarre risorse da pianeti,
    lune, asteroidi, giganti gassosi e nebulose.
-   Processi di raffinazione, ricette industriali, materiali in
    ingresso, prodotti in uscita ed eventuali sottoprodotti.
-   Reparti integrati di ciascun impianto, costi di potenziamento e
    tempi di lavorazione.
-   Quantità di energia e personale richiesti da ciascun impianto.
-   Ruolo specifico di ciascuna risorsa, capacità di stoccaggio,
    deperimento e consumi di mantenimento.
-   Valori numerici dei fabbisogni di PC delle singole ricette e dei
    processi industriali, e relativi criteri di bilanciamento.
-   Modelli produttivi concreti e relativi criteri di assegnazione delle
    priorità; regole di spareggio cronologico quando il flag di
    precedenza assoluta non risolve una parità.
-   Fattori sociali, ambientali e tecnologici che modificano la capacità
    effettiva degli impianti, relativa entità e formule di applicazione.
-   Regole e parametri per i vincoli operativi e logistici che possono
    sospendere singoli processi; eventuali condizioni specifiche per
    ciascuna tipologia di impianto.
-   Costi di potenziamento e condizioni tecnologiche dettagliate per
    ciascun livello dell'impianto.
-   Effetti concreti dell'organizzazione sociale dell'impero su
    integrazione, specializzazione ed efficienza industriale.
-   Capacità, velocità e costi dei cargo; regole di intercettazione e
    di scorta; tempi di viaggio.
-   Esistono commercio tra imperi e una valuta?
-   Come vengono calcolate e aggiornate le previsioni di produzione?
-   Come vengono trattate le risorse prenotate se un pianeta viene
    conquistato, bloccato o perde capacità produttiva?
-   Come si gestiscono arrotondamenti e frazioni nelle quote 50/30/20?

### 7.2 Ordini e simulazione

-   Quali sono le regole per identificare un ordine permanentemente
    impossibile?
-   Quali criteri di spareggio si applicano agli ordini generali quando
    due ordini hanno la stessa priorità e lo stesso mese previsto?
-   Come vengono risolti gli eventi simultanei e i conflitti tra ordini?
-   Quali modifiche sono ammesse per ciascuna categoria di ordine?
-   Come vengono gestite le dipendenze quando un ordine viene modificato
    o annullato?

### 7.3 Attività e avanzamento

-   Come si calcola il tempo residuo dopo un'interruzione?
-   Come si applicano le tre fasi a costruzioni, ricerca, riparazioni,
    progetti civili e attività senza un costo o una durata standard?
-   Un'attività interrotta può subire deterioramento, obsolescenza o
    costi di mantenimento? Non è stata ancora presa una decisione.

### 7.4 Galassia e generazione della mappa

-   Quali saranno i valori quantitativi precisi della larghezza e
    dell'estensione dei bracci e come verranno parametrizzate le
    variazioni locali della loro geometria?
-   Quali saranno le proporzioni numeriche tra addensamenti
    secondari piccoli, medi e grandi?
-   Come varierà la densità all'interno dei singoli addensamenti
    secondari?
-   Valori numerici di quota dei sistemi per regione e raggio della
    regione di transizione tra nucleo e regioni intermedie.
-   Quali saranno le regole di generazione dei singoli sistemi stellari,
    dei corpi celesti e delle loro caratteristiche?
-   Raggio massimo delle rotte, costi e capacità dei livelli dei
    portali, tempi di viaggio numerici.
-   Frequenza e ruolo delle popolazioni autoctone o neutrali, soglie
    tecnologiche di colonizzabilità, meccaniche di scoperta (portata e
    costo di sonde e flotte) e posizioni di partenza dei giocatori.
-   Intensità numerica delle tendenze di generazione per regione.

### 7.5 Sistemi stellari e corpi celesti

-   Quali valori numerici hanno le frequenze delle classi stellari?
-   Quanto sono poche le lune e quante ne può avere al massimo un
    pianeta?
-   Quali condizioni precise rendono abitabile una luna?
-   Quali valori numerici hanno raggi e intensità degli effetti delle
    anomalie?
-   Come orbitano i pianeti nei sistemi multipli e come ne risente la
    fascia abitabile?
-   Quali soglie di atmosfera e temperatura rendono abitabile un
    pianeta e quali tecnologie permettono di colonizzare gli altri
    corpi?
-   Quali valori numerici di ricchezza di risorse hanno i singoli corpi
    celesti?

### 7.6 Colonie e popolazione

-   Quali specie esistono, con quali tratti e come funzionano le
    minoranze?
-   Formule di crescita demografica, popolazione iniziale e massima per
    corpo celeste, e consumo di cibo ed energia per milione di abitanti.
-   Come si calcolano stabilità, soglie di rivolta e relativi effetti?
-   Cosa comporta la carestia (calo della popolazione, rivolte)?
-   Costo, capacità e tempi delle navi coloniali.
-   Personale richiesto dai diversi impianti e dagli avamposti.
-   Organizzazione sociale dell'impero: quali forme esistono?

-----------------------------------------------------------------------

## 8. Principi di progettazione da preservare

1.  **Pianificazione significativa:** il giocatore deve poter
    programmare attività su un orizzonte annuale, considerando anche la
    produzione futura.
2.  **Simultaneità reale:** l'ordine interno di elaborazione del motore
    non deve creare vantaggi arbitrari.
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

-----------------------------------------------------------------------

## 9. Registro sintetico delle decisioni confermate

| Tema | Decisione |
|---|---|
| Modalità | Single-player e multiplayer |
| Struttura | Gioco a turni con risoluzione simultanea |
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
| Capacità produttiva | Unità comune: Punti Capacità (PC); conversione diretta 1 indice = 1 PC; capacità base unica per tipologia; budget complessivo condiviso tra i reparti |
| Assegnazione PC | Manuale o automatica tramite modelli; configurazione persistente e programmabile per mese/anno; PC non utilizzati in un mese scadono senza accumulo |
| Capacità base al livello 1 | Miniera 100 PC; Centrale energetica 90 PC; Raffineria 80 PC; Fabbrica di componenti 70 PC; Cantiere/Assemblaggio 60 PC; Deposito/Magazzino 50 PC; Spazioporto 40 PC |
| Vincoli di località | Energia e personale/popolazione locale |
| Ricette | 2-4 input; materiali e componenti a lotto mensile scalato dalla capacità; navi e strutture con durata in mesi e consumo 50/30/20 |
| Trasporto | Navi cargo fisiche (leggero, medio, pesante, cisterne, convogli scortati); consumo di carburante proporzionale a distanza e carico; carichi intercettabili e perdibili; scorte assegnabili |
| Rotte permanenti | Ordini continuativi di trasporto periodico tra due località |
| Movimento nel sistema | Libero, in frazioni di mese |
| Movimento tra sistemi | Solo su rotte tra sistemi vicini; tempi in mesi interi (indicativamente 1-6) |
| Portali | Impianti che aprono/chiudono le rotte; serve solo nel sistema di partenza; usabili da proprietario e alleati; distruggibili/conquistabili; con livelli; posizione influisce solo sul carburante |
| Stati dei sistemi | Esplorabile (scoperta con sonde/flotte; si parte conoscendo sistema proprio e vicini); colonizzabile (corpo con requisiti minimi, dipende dalla tecnologia); abitato (colonia di qualsiasi impero, distinguendo giocatori e popolazioni autoctone/neutrali) |
| Filiera produttiva | Distinzione tra estrazione, raffinazione, produzione di componenti e assemblaggio finale; sviluppo dei dettagli ancora in corso |
| Materiali intermedi | Catena industriale articolata; materiali fisici stoccabili e trasportabili, con granularità mista tra risorse individuali e famiglie |
| Impianti industriali | Possibilità di integrare reparti produttivi interni; ogni reparto riceve una quota distinta del budget condiviso di PC e può lavorare in parallelo con gli altri |
| Modificatori della capacità | Fattori sociali, ambientali e tecnologici possono modificare la capacità effettiva; fattori specifici e formule da definire |
| Consumo dei PC | Ogni processo ha un fabbisogno di PC per ciclo; la quota è mensile e i PC inutilizzati scadono a fine mese, senza accumulo |
| Capacità e durata dei processi | La capacità effettiva mensile determina il ritmo di lavorazione; se insufficiente, il processo prosegue su più mesi |
| Priorità produttive | Gestione manuale o tramite modelli produttivi; la configurazione attiva si applica anche ai processi creati successivamente e le modifiche decorrono dal mese/anno indicato |
| Spareggio delle priorità produttive | Flag di precedenza assoluta in caso di parità; la data può costituire un ulteriore criterio da formalizzare |
| Vincoli operativi e logistici | Possono sospendere un processo o l'intera produzione del complesso; la capacità assegnata ai processi sospesi resta riservata fino alla ripresa |
| Ripresa dei processi sospesi | Alla rimozione del vincolo, ripresa manuale anticipata possibile previa verifica delle condizioni; altrimenti riavvio automatico all'inizio del mese successivo, se le condizioni sono soddisfatte |
| Organizzazione sociale e industria | Può influenzare costi, efficienza e convenienza delle configurazioni industriali, senza vincolare rigidamente i processi possibili |
| Priorità in caso di deficit | Priorità dell'ordine, poi mese di esecuzione |
| Annullamento | Rilascio immediato delle risorse non consumate |
| Consumo | Progressivo in tre fasi: 50%, 30%, 20% |
| Interruzione | Conserva progresso e prenotazioni |
| Ripresa | Continua dalla fase raggiunta |
| Interruzione prolungata | Nessuna scadenza automatica |
| Persistenza | Gli ordini incompleti passano all'anno successivo |
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
| Specie | Specie giocabili con tratti propri; una specie principale per impero, minoranze possibili |
| Stabilità | Felicità/stabilità per colonia, da organizzazione sociale, condizioni e guerra; influisce su efficienza e rivolte |
| Sostentamento | Cibo per la popolazione, energia per i servizi; carenza di cibo riduce crescita e stabilità fino alla carestia |
| Fondazione colonie | Navi coloniali dai cantieri, ordine di colonizzazione; la colonia parte piccola e cresce |
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

-----------------------------------------------------------------------

**Nota di manutenzione:** questa è una bozza di lavoro consolidata,
non ancora il GDD completo. Quando una questione aperta viene decisa,
la regola corrispondente dovrà essere aggiornata nel capitolo pertinente
e nel registro sintetico.
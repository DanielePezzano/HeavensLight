# Changelog — HeavensLight

Cronologia delle modifiche al documento di game design. Le voci sono riportate dalle versioni precedenti e ordinate dalla più recente alla più vecchia.

## Novità v0.56

Riorganizzata la documentazione di progetto introducendo
`OPEN_QUESTIONS.md` come backlog canonico delle questioni ancora da
definire. Trasferite nel nuovo documento le questioni aperte della
precedente §8 del GDD, assegnando identificativi stabili `OQ-001`–
`OQ-051`; le voci già confermate o risolte sono state escluse dal
backlog attivo; anche l’arrotondamento 50/30/20 è stato escluso perché
già definito in §2.7. Priorità e dipendenze non documentate restano da
valutare. La §8 del GDD è stata sostituita da un punto di accesso al
backlog. Aggiornata la regola di organizzazione documentale e
registrata la decisione in `DECISIONS.md` con ADR-002.


## Novità v0.55

Precisata in §6.8.6 la natura della perdita di progresso di un progetto
di ricerca in caso di distruzione completa della sede: la perdita è
proporzionale al tempo residuo previsto rispetto a `T_totale`, congelato
all'avvio, e non al progresso effettivamente accumulato. La formula
esistente resta invariata; viene esplicitato il criterio temporale per
evitare interpretazioni basate sulla percentuale di sviluppo completata.

## Novità v0.54

Corretto il calcolo individuale del fabbisogno di Punti Capacità
(§4.3.6): introdotta l'efficienza effettiva `E_eff = max(E_g × E_l,
0.01)`, applicando la soglia minima al prodotto dei coefficienti e
impedendo denominatori nulli o negativi. Rimossa dalle questioni aperte
la relativa lacuna di specifica. Introdotto `DECISIONS.md` con il record
ADR-001, che documenta contesto, alternative e conseguenze della scelta;
aggiornato il registro sintetico delle decisioni nel GDD.

## Novità v0.53

Completato il primo consolidamento dei parametri delle installazioni scientifiche (§6.8): produzione base L1 pari a 1 PR/turno per i laboratori planetari, 2 per università/istituti e 3 per stazioni orbitali; definiti i requisiti operativi e l’effetto progressivo della disponibilità di personale. Introdotte le tre categorie professionali scientifiche e confermato che ogni università/istituto forma annualmente ricercatori, tecnici scientifici e personale di supporto in quantità base uguali, modificabili dai fattori applicabili e in particolare dai fattori sociali globali. Confermato il bonus orbitale del +25% per i rami C e D e la sua esclusione dal ramo E. Aggiornati il registro delle decisioni e le questioni aperte.

## Novità v0.50

Introdotto il modello di ricerca e sviluppo tecnologico (§6.8): generazione dei Punti Ricerca dalla popolazione e dalle installazioni scientifiche; distinzione tra costo in PR per avviare uno studio e tempo di sviluppo; serbatoio imperiale centralizzato, valori frazionari e massimo di tre ricerche parallele. Ogni progetto è assegnato a una specifica installazione, che costituisce anche un obiettivo strategico; definite le conseguenze di danni, indisponibilità e distruzione della sede, nonché il trasferimento del progetto. Aggiornato il registro sintetico delle decisioni.

## Novità v0.49

Definiti il calcolo dei PS massimi dei reparti, il valore base comune e i modificatori locali e globali moltiplicativi. Precisati l'arrotondamento per difetto, il ricalcolo immediato dei PS massimi al variare dei fattori applicabili e il comportamento dei PS correnti quando il massimo cambia. Definite le impostazioni personalizzate conservate dopo la ricostruzione di un reparto e quelle non recuperate. Aggiornato il registro sintetico delle decisioni.

## Novità v0.48

Completate le regole operative di riparazione e distruzione (§4.3.7): un reparto a 0 PS è distrutto e non può essere riparato, ma deve essere ricostruito con un nuovo ordine, costi e tempi di costruzione; la ricostruzione non ripristina gli ordini precedentemente cancellati e conserva l'impostazione di riparazione automatica. Definita la gestione di nuovi danni durante una riparazione, la distribuzione autonoma dei PC di riparazione secondo priorità e anzianità e la priorità predefinita degli ordini automatici. Precisate le cancellazioni degli ordini collegati alla distruzione di un reparto o impianto e il rientro dei convogli già in viaggio. Aggiornato il registro sintetico delle decisioni.

## Novità v0.47

Completate le regole di selezione dei reparti colpiti
(§4.3.7): distinzione tra attacchi mirati e danni non mirati; per gli
attacchi mirati la probabilità di colpire il reparto selezionato dipende
da precisione dell'arma, difese del bersaglio e tecnologie disponibili.
Un attacco mancato può non causare danni oppure colpire accidentalmente
un altro reparto. I danni non mirati selezionano i reparti in modo
casuale ponderato; un attacco può coinvolgere uno o più reparti in base
all'arma o all'evento. Definiti inoltre il limite del danno ai PS residui
del reparto, la perdita del danno eccedente, i danni secondari come eventi
distinti e l'assenza di PS separati per l'impianto principale. Aggiornato
il registro sintetico delle decisioni.

## Novità v0.46

Estese le regole di integrità strutturale (§4.3.7)
introducendo le categorie di danno e i relativi modificatori
moltiplicativi, gli ordini di riparazione manuali o automatici, una
capacità di riparazione autonoma per ogni impianto e il ripristino
progressivo dei Punti Struttura. Definita inoltre la riduzione
direttamente proporzionale della capacità produttiva del reparto in
funzione dei PS persi. Aggiornato il registro sintetico delle decisioni.

## Novità v0.45

Definita l'associazione degli ordini produttivi ai
reparti: la ricetta determina la tipologia compatibile, il giocatore
seleziona l'impianto principale e, se sono presenti più reparti
compatibili, sceglie quello specifico; con un solo reparto compatibile
l'assegnazione è automatica. Gli ordini non cambiano reparto
automaticamente; il trasferimento manuale è consentito tra reparti
compatibili dello stesso impianto, conservando identità, anzianità,
priorità e progresso, e applicando la nuova distribuzione PC dal mese
successivo. Definite le conseguenze dell'indisponibilità temporanea,
parziale e permanente dei reparti e degli impianti. Introdotti i Punti
Struttura (PS): il totale dell'impianto è la somma dei PS dei reparti,
modificabile da tecnologie e fattori sociali. Demolizione volontaria e
distruzione hanno gli stessi effetti: gli ordini produttivi e di
rifornimento diretti alla struttura vengono rimossi; i convogli già
in movimento invertono la rotta e riportano il carico al punto di
partenza, dove le risorse tornano disponibili all'arrivo. Aggiornato
il registro sintetico delle decisioni.

## Novità v0.44

Definiti i limiti massimi di PC utilizzabili dai
singoli reparti (§4.3.6). Ogni reparto ha un limite proprio, calcolato
a partire dal 50% della capacità base dell'impianto principale al
livello 1, moltiplicato per il coefficiente del livello dell'impianto
e per i modificatori tecnologici generali e specifici applicabili.
I modificatori sono moltiplicativi e il risultato è arrotondato per
difetto. Precisato che i limiti dei reparti non aumentano il budget
complessivo dell'impianto e che la somma dei PC assegnati resta
vincolata sia al budget disponibile sia ai limiti dei singoli reparti.
Aggiornato il registro sintetico delle decisioni.

## Novità v0.43

Integrate in §4.3.6 le decisioni sul calcolo
del fabbisogno di PC, sulla determinazione mensile dei coefficienti
di efficienza e sulla capacità effettivamente disponibile. Precisata
l'autonomia dei budget tra impianti principali e stabilito che ogni
ordine produttivo è associato a un solo impianto principale; le fasi
di una filiera sono rappresentate da ordini distinti. Aggiornato il
registro sintetico delle decisioni. Restano da definire alcuni casi
limite del calcolo.

## Novità v0.42

Consolidate in §4.3.6 le regole di distribuzione
automatica dei Punti Capacità (PC): assegnazione mensile, mancata
redistribuzione durante il mese, ripartizione equa tra ordini della
stessa priorità, gestione dei PC indivisibili e dei residui secondo
anzianità dell'ordine. Precisato inoltre che i processi sospesi non
mantengono una quota di PC riservata e che, alla riattivazione, un
ordine rientra nella sequenza di precedenza come nuovo ordine ai fini
della distribuzione. Aggiornato il registro sintetico delle decisioni.

## Novità v0.40

Aggiornato il documento per allineare intestazione e
riepilogo editoriale alla versione corrente. Sono state integrate in
§5 le decisioni e le proposte relative a classi stellari, sistemi
multipli, lune, anomalie e ricchezza delle risorse dei corpi celesti.
In §6 sono state sviluppate le sezioni su crescita demografica,
specie, stabilità sociale e sostentamento. I valori indicati come
proposte o da bilanciare restano provvisori e non costituiscono
parametri definitivi.

## Novità v0.38

Confermate le regole su merge di ingestione, limiti
anti-loop, `RequiredFuel`, modifiche ammesse per categoria di ordine,
dipendenze, arrotondamenti e interruzioni (§2.3.4, §2.3.5, §2.7,
§2.8, §2.10.1).

## Novità v0.37

Il capitolo 1 (§2) è stato riscritto per integrare il
modello a tick, il flag di blocco persistente, la distinzione tra
ordini incompleti e ordini continuativi, e la barriera di
pianificazione. È stato aggiunto il capitolo 11 con l'architettura
tecnica del TurnResolver.

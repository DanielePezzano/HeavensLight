# Regola di progetto — Organizzazione e gestione della documentazione

## 1. Struttura documentale canonica

Il progetto HeavensLight utilizza quattro documenti canonici, distinti per responsabilità e complementari tra loro:

- **`GDD.md` — Game Design Document:** descrive lo stato attuale e consolidato del gioco. Contiene le regole, i sistemi, le meccaniche, i parametri e le specifiche che definiscono come funziona HeavensLight.
- **`DECISIONS.md` — Decision Records:** documenta le decisioni progettuali confermate e le relative motivazioni. Spiega perché una determinata soluzione è stata scelta, quale problema affrontava, quali alternative sono state effettivamente considerate e quali conseguenze comporta.
- **`CHANGELOG.md` — Registro delle modifiche:** registra cronologicamente le modifiche apportate al GDD e agli altri documenti di progetto, indicando la versione e sintetizzando che cosa è cambiato.
- **`OPEN_QUESTIONS.md` — Backlog delle questioni aperte:** gestisce le questioni progettuali non ancora risolte, le proposte non approvate, le priorità, le dipendenze, gli aspetti bloccanti e i responsabili della decisione e dell’analisi.

I quattro documenti rispondono a domande diverse:

| Documento | Domanda di riferimento |
|---|---|
| `GDD.md` | Come funziona attualmente HeavensLight? |
| `DECISIONS.md` | Perché abbiamo deciso che funzioni così? |
| `CHANGELOG.md` | Che cosa è cambiato, e quando? |
| `OPEN_QUESTIONS.md` | Che cosa resta da decidere e in quale ordine affrontarlo? |

Non duplicare estesamente le stesse informazioni nei quattro documenti: ciascuno deve mantenere la propria funzione e rimandare agli altri quando necessario.

## 2. Regole per `GDD.md`

1. Il GDD è la fonte canonica per la descrizione dello stato attuale del game design.
2. Deve contenere le specifiche operative e le decisioni consolidate, non la ricostruzione estesa delle discussioni che le hanno prodotte.
3. Le proposte non confermate e le questioni ancora aperte devono rimanere chiaramente distinte dalle regole confermate.
4. Quando una regola viene modificata, il GDD deve riflettere lo stato aggiornato senza perdere chiarezza o coerenza con il resto del documento.
5. Il registro sintetico delle decisioni nel GDD deve funzionare come indice delle decisioni documentate in `DECISIONS.md`, riportandone identificativo, descrizione breve e riferimento alle sezioni pertinenti del GDD.
6. Il GDD non deve contenere il backlog dettagliato: deve rimandare a `OPEN_QUESTIONS.md` e mantenere, dove necessario, riferimenti agli ID `OQ-XXX` accanto a proposte o parametri non definitivi.


## 3. Regole per `DECISIONS.md`

### 3.1 Ambito

`DECISIONS.md` raccoglie le decisioni progettuali effettivamente confermate. È un registro di Decision Records in stile ADR (*Architecture Decision Records*), applicato al game design e all'architettura tecnica di HeavensLight.

Ogni decisione deve avere un identificativo stabile e univoco, ad esempio `ADR-001`, `ADR-002` e così via. L'identificativo non deve dipendere dalla versione del GDD.

### 3.2 Contenuto di un Decision Record

Ogni record deve documentare, quando le informazioni sono disponibili:

- **Identificativo e titolo:** codice stabile e descrizione sintetica della decisione.
- **Data:** data in cui la decisione è stata confermata.
- **Stato:** per esempio `Confermata`, `Superata` (*Superseded*) o altro stato esplicitamente definito.
- **Ambito:** sistema, meccanica o area progettuale interessata.
- **Riferimenti al GDD:** capitolo e sezione in cui la decisione è applicata.
- **Contesto:** problema, esigenza o vincolo che ha reso necessaria la decisione.
- **Decisione:** formulazione precisa della soluzione adottata.
- **Alternative considerate:** alternative realmente discusse e motivi documentati per cui sono state scartate.
- **Conseguenze:** effetti positivi, compromessi, costi, vincoli e implicazioni progettuali derivanti dalla decisione.
- **Note e questioni collegate:** dipendenze, precisazioni o aspetti ancora da definire, se pertinenti.

Non è obbligatorio inventare o compilare artificialmente informazioni mancanti. Se un campo non è documentabile, va omesso oppure indicato esplicitamente come non disponibile.

### 3.3 Integrità storica

1. Non attribuire retroattivamente al processo decisionale alternative, motivazioni o valutazioni che non risultino dalle fonti disponibili.
2. Distinguere tra alternative effettivamente valutate, semplici possibilità menzionate e alternative mai discusse.
3. Ricostruire la data dalla conversazione o dalla documentazione storica quando possibile. Se non è determinabile con sufficiente certezza, indicarla come approssimativa o non disponibile; non inventarla.
4. Una decisione confermata non deve essere riscritta retroattivamente per farla apparire diversa da quella originaria.
5. Se una decisione viene modificata o sostituita, conservarne il record storico, aggiornarne lo stato a `Superata` (*Superseded*) e creare un nuovo ADR collegato al precedente. Il nuovo record deve spiegare che cosa cambia e perché.
6. Una proposta non ancora confermata non deve essere registrata come decisione confermata. Può essere tenuta separata in una sezione chiaramente identificata come proposta o questione aperta, se necessario.

## 4. Regole per `OPEN_QUESTIONS.md`

### 4.1 Ambito

`OPEN_QUESTIONS.md` è il backlog operativo del lavoro di game design.
Raccoglie problemi da definire, proposte non approvate e questioni ancora
aperte, senza sostituire il GDD come fonte canonica del modello corrente.

### 4.2 Identificativi e campi

1. Ogni questione riceve un identificativo stabile e univoco (`OQ-001`,
   `OQ-002`, ecc.), indipendente dalla priorità e dall’ordine di lavoro.
2. Il backlog generale riporta almeno: ID, priorità, area, titolo, stato,
   dipendenze e riferimento al GDD.
3. La scheda di dettaglio riporta problema, obiettivo, cosa blocca,
   dipendenze, responsabile della decisione, responsabile della
   preparazione dell’analisi, stato, proposta corrente e riferimenti.
4. Priorità e dipendenze sono concetti distinti: la prima esprime
   importanza/urgenza, le seconde prerequisiti effettivi. Non vanno
   assegnate per semplice somiglianza tematica.

### 4.3 Priorità e stati

Le priorità sono `Critica`, `Alta`, `Media` e `Bassa`. La priorità va
assegnata dopo una valutazione condivisa; non deve essere inventata
durante una migrazione documentale.

Gli stati sono:
- `Aperta`: problema identificato, senza soluzione candidata definita.
- `Proposta`: esiste una soluzione candidata, non ancora approvata.
- `Decisa`: l’utente ha approvato esplicitamente la soluzione;
  l’integrazione documentale può essere ancora da completare.

Il responsabile della decisione è l’utente. L’assistente prepara analisi,
alternative e conseguenze, ma non approva autonomamente le soluzioni.

### 4.4 Chiusura e tracciabilità

Una questione in stato `Decisa` resta nel backlog fino al completamento
dell’integrazione documentale. Dopo l’aggiornamento del GDD e, quando
pertinente, di `DECISIONS.md` e `CHANGELOG.md`, viene trasferita
all’archivio delle questioni risolte, conservando l’ID e i riferimenti.

Il GDD deve richiamare gli ID `OQ-XXX` accanto ai punti provvisori
quando il collegamento è utile. Il backlog non deve duplicare intere
sezioni del GDD: descrive il lavoro ancora da svolgere e rimanda alla
specifica corrente.


## 5. Regole per `CHANGELOG.md`

1. Il changelog conserva la cronologia delle modifiche documentali e progettuali secondo la convenzione di versionamento già adottata dal progetto.
2. Ogni aggiornamento del GDD deve essere accompagnato da una voce corrispondente nel changelog, con la versione pertinente e una descrizione sintetica delle modifiche effettive.
3. L'introduzione, la riorganizzazione o l'aggiornamento sostanziale di `DECISIONS.md` o `OPEN_QUESTIONS.md` deve essere registrato nel changelog quando comporta una modifica significativa alla documentazione del progetto.
4. Le voci storiche esistenti devono essere preservate. Non eliminare o riscrivere la cronologia senza una ragione esplicita e documentata.
5. Il changelog non sostituisce i Decision Records: registra che cosa è cambiato, mentre `DECISIONS.md` conserva il contesto e le motivazioni delle decisioni.

## 6. Coerenza tra i documenti

Quando una decisione progettuale viene confermata e deve essere incorporata nella documentazione:

1. Aggiornare `GDD.md` affinché descriva correttamente la regola o il sistema risultante.
2. Creare o aggiornare il relativo record in `DECISIONS.md`, senza trasformare in fatti le informazioni non documentate.
3. Aggiornare il registro sintetico delle decisioni nel GDD, mantenendolo coerente con gli identificativi ADR.
4. Aggiornare `CHANGELOG.md` con una voce che descriva le modifiche effettivamente apportate e la versione pertinente.
5. Aggiornare `OPEN_QUESTIONS.md` dopo l’approvazione e trasferire le voci nell’archivio soltanto dopo l’integrazione documentale.
6. Verificare la coerenza dei riferimenti incrociati, della terminologia e dello stato delle decisioni.

Non modificare automaticamente tutti e quattro i documenti per ogni intervento: applicare i passaggi pertinenti alla natura della modifica. Tuttavia, ogni modifica al GDD deve sempre avere la corrispondente registrazione nel changelog.

## 7. Metodo di lavoro e tutela delle decisioni

1. Considerare come confermata soltanto una decisione che l'utente ha esplicitamente approvato.
2. Mantenere sempre separate decisioni confermate, proposte, ipotesi di lavoro e questioni aperte.
3. Non introdurre modifiche non richieste e non risolvere autonomamente ambiguità sostanziali.
4. Prima di modificare una regola già documentata, verificare i relativi riferimenti nel GDD e in `DECISIONS.md`, così da tenere conto delle motivazioni e dei compromessi già concordati.
5. Preservare struttura, terminologia, livello di dettaglio e convenzioni già adottati nei documenti.
6. In caso di conflitto tra documenti, non scegliere silenziosamente quale versione considerare corretta: segnalare la discrepanza e chiedere o proporre una risoluzione esplicita. Il GDD descrive lo stato attuale, `DECISIONS.md` documenta la storia e la motivazione delle decisioni, `CHANGELOG.md` registra le modifiche e `OPEN_QUESTIONS.md` gestisce il lavoro ancora da decidere.

## 8. Principio guida

La documentazione di HeavensLight deve consentire di ricostruire sia **lo stato attuale del progetto** sia **il percorso razionale che ha portato alle scelte progettuali**, senza confondere specifiche, motivazioni e cronologia.

In sintesi:

- `GDD.md` è la descrizione del gioco.
- `DECISIONS.md` è la memoria motivazionale delle decisioni progettuali.
- `CHANGELOG.md` è la cronologia delle modifiche.
- `OPEN_QUESTIONS.md` è il backlog delle questioni ancora da risolvere.

Questa separazione è uno standard permanente di organizzazione e manutenzione della documentazione di HeavensLight e deve essere rispettata in ogni successivo intervento sui documenti del progetto.

# Open Questions — HeavensLight

Backlog operativo delle questioni progettuali non ancora definite o approvate.

Questo documento accompagna `GDD.md`: raccoglie ciò che resta da decidere,
mentre il GDD descrive il modello corrente. Gli identificativi `OQ-XXX`
sono stabili e non cambiano quando le priorità o l’ordine di lavoro vengono
modificati.

## 1. Regole di gestione

- **Priorità:** `Critica`, `Alta`, `Media`, `Bassa`. Non è stata assegnata
  automaticamente durante la migrazione; tutte le voci iniziano come
  `Non assegnata` e saranno ordinate dopo una valutazione condivisa.
- **Stato:** `Aperta` = problema identificato senza soluzione candidata;
  `Proposta` = esiste una soluzione o un modello candidato non approvato;
  `Decisa` = soluzione approvata esplicitamente dall’utente, anche se
  l’integrazione documentale deve ancora essere completata.
- **Dipendenze:** indicano prerequisiti effettivi, non semplice affinità
  tematica. I collegamenti non ancora analizzati restano da valutare.
- **Cosa blocca:** va specificato durante l’analisi della singola questione;
  non viene dedotto dalla sola vicinanza tematica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente, che propone alternative e
  conseguenze senza approvarle al posto dell’utente.
- Una questione `Decisa` viene rimossa dal backlog attivo e trasferita
  nell’archivio delle questioni risolte dopo l’integrazione nel GDD e,
  quando pertinente, in `DECISIONS.md` e `CHANGELOG.md`. L’identificativo
  OQ viene conservato nell’archivio e nei riferimenti incrociati.

## 2. Backlog generale

| ID | Priorità | Area | Questione | Stato | Dipendenze | Riferimento GDD |
|---|---|---|---|---|---|---|
| OQ-001 | Non assegnata | Economia e produzione | Catalogo completo delle ricette | Aperta | Da valutare | §4.3–§4.4 |
| OQ-002 | Non assegnata | Risorse e galassia | Depositi naturali e distribuzione delle risorse | Aperta | Da valutare | §4.2; §5.11 |
| OQ-003 | Non assegnata | Produzione | Estrazione delle risorse | Aperta | Da valutare | §4.3; §5.11 |
| OQ-004 | Non assegnata | Produzione | Raffinazione e ricette industriali | Aperta | Da valutare | §4.3–§4.4 |
| OQ-005 | Non assegnata | Produzione e infrastrutture | Reparti integrati e lavorazioni degli impianti | Aperta | Da valutare | §4.3 |
| OQ-006 | Non assegnata | Integrità strutturale | Punti Struttura e modificatori strutturali | Proposta | Da valutare | §4.3.7 |
| OQ-007 | Non assegnata | Combattimento e integrità strutturale | Parametri numerici del modello dei danni | Proposta | Da valutare | §4.3.7 |
| OQ-008 | Non assegnata | Riparazioni | Capacità e ordini di riparazione | Proposta | Da valutare | §4.3.7; §7.11 |
| OQ-009 | Non assegnata | Economia e produzione | Costi di costruzione delle strutture | Aperta | Da valutare | §4.3; §4.3.7 |
| OQ-010 | Non assegnata | Infrastrutture e colonie | Fabbisogni di energia e personale degli impianti | Aperta | Da valutare | §4.3; §6.6 |
| OQ-011 | Non assegnata | Economia | Ruolo e gestione delle risorse | Aperta | Da valutare | §4.1–§4.2 |
| OQ-012 | Non assegnata | Produzione | Fabbisogni di PC delle ricette e dei processi | Aperta | Da valutare | §4.3.6; §4.4 |
| OQ-013 | Non assegnata | Produzione | Modelli produttivi, priorità e spareggi | Aperta | Da valutare | §4.3.6; §4.4 |
| OQ-014 | Non assegnata | Produzione e bilanciamento | Modificatori della capacità effettiva degli impianti | Proposta | Da valutare | §4.3.6; §4.4 |
| OQ-015 | Non assegnata | Produzione e logistica | Vincoli operativi e logistici dei processi | Aperta | Da valutare | §4.3; §4.4 |
| OQ-016 | Non assegnata | Infrastrutture e tecnologia | Potenziamento degli impianti | Aperta | Da valutare | §4.3 |
| OQ-017 | Non assegnata | Organizzazione sociale ed economia | Effetti dell’organizzazione sociale sull’industria | Aperta | Da valutare | §4.4; §6.7 |
| OQ-018 | Non assegnata | Logistica | Specifiche delle navi cargo e del trasporto | Aperta | Da valutare | §4.6; §3.7 |
| OQ-019 | Non assegnata | Economia diplomatica | Commercio tra imperi e valuta | Aperta | Da valutare | §4 |
| OQ-020 | Non assegnata | Economia e interfaccia | Previsioni di produzione | Aperta | Da valutare | §4.3–§4.4 |
| OQ-021 | Non assegnata | Ordini e logistica | Risorse prenotate in caso di cambiamento della località | Aperta | Da valutare | §2.7; §4.2; §4.3 |
| OQ-022 | Non assegnata | Generazione galattica | Geometria e dimensioni dei bracci galattici | Aperta | Da valutare | §3 |
| OQ-023 | Non assegnata | Generazione galattica | Proporzioni degli addensamenti secondari | Aperta | Da valutare | §3 |
| OQ-024 | Non assegnata | Generazione galattica | Densità degli addensamenti secondari | Aperta | Da valutare | §3 |
| OQ-025 | Non assegnata | Generazione galattica | Quote regionali e transizione tra regioni galattiche | Aperta | Da valutare | §3 |
| OQ-026 | Non assegnata | Generazione galattica | Generazione dei sistemi stellari e dei corpi celesti | Aperta | Da valutare | §3; §5 |
| OQ-027 | Non assegnata | Movimento e logistica | Parametri di rotte e portali | Aperta | Da valutare | §3.7 |
| OQ-028 | Non assegnata | Esplorazione e colonizzazione | Popolazioni autoctone, scoperta e posizioni iniziali | Aperta | Da valutare | §3.6; §5; §6.9; ADR-004 |
| OQ-029 | Non assegnata | Generazione galattica | Intensità delle tendenze regionali di generazione | Aperta | Da valutare | §3 |
| OQ-030 | Non assegnata | Sistemi stellari | Frequenze delle classi stellari | Proposta | Da valutare | §5.6 |
| OQ-031 | Non assegnata | Abitabilità | Soglie di abitabilità per specie | Proposta | Da valutare | §5.8; §5.10 |
| OQ-032 | Non assegnata | Anomalie | Valori e raggi delle anomalie | Proposta | Da valutare | §5.9 |
| OQ-033 | Non assegnata | Risorse e corpi celesti | Valori della ricchezza delle risorse dei corpi celesti | Proposta | Da valutare | §5.11 |
| OQ-034 | Non assegnata | Coerenza del GDD | Unificazione della descrizione delle anomalie | Aperta | Da valutare | §5.4; §5.9 |
| OQ-035 | Non assegnata | Specie e popolazione | Specie definitive e regole delle minoranze | Aperta | Da valutare | §6.1–§6.2 |
| OQ-036 | Non assegnata | Colonie e popolazione | Bilanciamento dei parametri di colonie e popolazione | Proposta | Da valutare | §6.1–§6.6 |
| OQ-037 | Non assegnata | Colonizzazione e produzione | Costi e tempi delle navi coloniali | Aperta | Da valutare | §6.5; OQ-001 |
| OQ-038 | Non assegnata | Organizzazione sociale | Forme ed effetti dell’organizzazione sociale | Aperta | Da valutare | §6.7; §4.4 |
| OQ-039 | Non assegnata | Flotte e tecnologia | Componenti delle navi e tecnologie associate | Aperta | Da valutare | §7.5 |
| OQ-040 | Non assegnata | Flotte e personale | Costi di costruzione e potenziamento dell’Accademia | Aperta | Da valutare | §7.5.2; §4.3; OQ-001 |
| OQ-041 | Non assegnata | Flotte e combattimento | Bilanciamento di danni, scudi, armatura e riparazioni delle navi | Proposta | Da valutare | §7.9; §7.11 |
| OQ-042 | Non assegnata | Ricerca e sviluppo | Varianti e compatibilità delle installazioni scientifiche | Aperta | Da valutare | §6.8 |
| OQ-043 | Non assegnata | Ricerca e sviluppo | Fabbisogni di personale delle installazioni scientifiche | Aperta | Da valutare | §6.8 |
| OQ-044 | Non assegnata | Ricerca e sviluppo | Requisiti operativi delle installazioni scientifiche | Aperta | Da valutare | §6.8; §4.3.7; OQ-010 |
| OQ-045 | Non assegnata | Ricerca e sviluppo | Formazione del personale scientifico e relativi modificatori | Aperta | Da valutare | §6.8; §6.7; OQ-038 |
| OQ-046 | Non assegnata | Ricerca e sviluppo | Costi in PR e tempi di sviluppo delle tecnologie | Aperta | Da valutare | §6.8 |
| OQ-047 | Non assegnata | Ricerca e sviluppo | Modificatori della generazione dei PR e del tempo di sviluppo | Aperta | Da valutare | §6.8; OQ-038 |
| OQ-048 | Non assegnata | Ricerca e sviluppo | Effetti dei danni sulle sedi scientifiche | Aperta | Da valutare | §6.8; §4.3.7 |
| OQ-049 | Non assegnata | Ricerca e sviluppo | Riconversione dei laboratori planetari | Aperta | Da valutare | §6.8 |
| OQ-050 | Non assegnata | Ricerca e sviluppo | Trasferimento dei progetti di ricerca | Aperta | Da valutare | §6.8 |
| OQ-051 | Non assegnata | Ricerca e sviluppo | Bonus di ricerca in nave e modello generale dei PR | Aperta | Da valutare | §7.12; §6.8 |
| OQ-052 | Non assegnata | Sviluppo planetario | Calcolo degli slot planetari e gestione delle variazioni di capacità | Aperta | Da valutare | §6.10 |
| OQ-053 | Non assegnata | Territorialità e infrastrutture orbitali | Regole operative di utilizzo, controllo e accesso orbitale | Aperta | Da valutare | §5.12; §6.11 |
| OQ-054 | Non assegnata | Intelligence | Soglie informative, aggiornamento e decadimento della conoscenza | Aperta | Da valutare | §1.3; §7.12 |
| OQ-055 | Non assegnata | Flotte e navi | Distacco, trasferimento e sostituzione dell'ammiraglia | Aperta | Da valutare | §7.1 |
| OQ-056 | Non assegnata | Colonie e popolazione | Attributi dei gruppi demografici ed effetti di espulsione/riconversione | Aperta | Da valutare | §6.9–§6.10 |

## 3. Schede delle questioni

Le schede riportano il problema da affrontare e il punto di partenza
ricavato dal GDD. Priorità, impatto bloccante e dipendenze operative
restano da determinare durante la presa in carico: non sono stati dedotti
automaticamente in fase di migrazione.

### OQ-001 — Catalogo completo delle ricette

- **Area:** Economia e produzione.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire materiali per famiglia, quantità, tempi, sottoprodotti e bilanciamento dei valori delle ricette.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata come decisione. In §4.3.5 sono presenti proposte provvisorie e non vincolanti sui tipi di input delle ricette di raffinazione, dei componenti e degli assemblaggi. Assemblaggi (v0.65): le navi non hanno ricette fisse per classe ma derivano dal progetto (componenti negli slot più scafo); nel catalogo restano cinque ricette di scafo, una per taglia; strutture e prodotti finiti del Cantiere comprendono solo moduli di stazione orbitale e infrastrutture (impianti, avamposti). Gli input di scafi, moduli e infrastrutture sono ipotesi dell'assistente approvate come provvisorie. Sottoprodotti (v0.66): proposta provvisoria, ipotesi di lavoro dell'assistente approvata come provvisoria, in applicazione di ADR-003; un sottoprodotto è sempre una famiglia di materiali già esistente, previsto solo nelle ricette di raffinazione: Metalli strutturali → Composti chimici; Composti chimici → Combustibili raffinati; Combustibili raffinati → Composti chimici; nessun sottoprodotto per le altre ricette di raffinazione (Metalli rari, Cristalli e semiconduttori, Materiali energetici, Biomateriali), per i componenti e per gli assemblaggi.
- **Resta da definire:** regole di stoccaggio, smaltimento e volume dei sottoprodotti, trasportabilità, tecnologie richieste, verifica della completezza dell'elenco delle strutture, fonti dei materiali individuali. Quantità (compresi i sottoprodotti), tempi e bilanciamento sono in OQ-052; i valori di LR in OQ-012.
- **Riferimenti:** §4.3–§4.4; §4.3.5.

### OQ-002 — Depositi naturali e distribuzione delle risorse

- **Area:** Risorse e galassia.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire tipi di depositi naturali, distribuzione delle risorse e relazione con corpi celesti e fonti ambientali.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.2; §5.11.

### OQ-003 — Estrazione delle risorse

- **Area:** Produzione.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire strutture e tecnologie necessarie per estrarre risorse da pianeti, lune, asteroidi, giganti gassosi e nebulose.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.3; §5.11.

### OQ-004 — Raffinazione e ricette industriali

- **Area:** Produzione.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire processi di raffinazione, materiali in ingresso, prodotti in uscita ed eventuali sottoprodotti.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.3–§4.4.

### OQ-005 — Reparti integrati e lavorazioni degli impianti

- **Area:** Produzione e infrastrutture.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire reparti integrati per ciascun impianto, relativi costi di potenziamento e tempi di lavorazione.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.3.

### OQ-006 — Punti Struttura e modificatori strutturali

- **Area:** Integrità strutturale.
- **Stato:** Proposta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Determinare valori base dei Punti Struttura dei reparti, modificatori tecnologici e sociali e formule numeriche di applicazione.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.3.7.

### OQ-007 — Parametri numerici del modello dei danni

- **Area:** Combattimento e integrità strutturale.
- **Stato:** Proposta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Determinare valori di danno per categoria, probabilità di successo degli attacchi mirati, probabilità di impatto accidentale e pesi di selezione dei bersagli non mirati; includere numero di reparti coinvolgibili e danni secondari.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.3.7.

### OQ-008 — Capacità e ordini di riparazione

- **Area:** Riparazioni.
- **Stato:** Proposta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire PC base di riparazione per tipologia e livello di impianto, modificatori tecnologici, ambientali, sociali, governativi e razziali, mitigazione dei danni, LR e limiti dei coefficienti; chiarire distruzione della struttura durante la riparazione e nuovi danni durante l’intervento.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.3.7; §7.11.

### OQ-009 — Costi di costruzione delle strutture

- **Area:** Economia e produzione.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire i costi dettagliati di costruzione delle strutture, da cui derivano proporzionalmente i costi di riparazione.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.3; §4.3.7.

### OQ-010 — Fabbisogni di energia e personale degli impianti

- **Area:** Infrastrutture e colonie.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Determinare quantità di energia e personale richiesti da ciascuna tipologia e livello di impianto.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.3; §6.6.

### OQ-011 — Ruolo e gestione delle risorse

- **Area:** Economia.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire il ruolo specifico di ciascuna risorsa, capacità di stoccaggio, deperimento e consumi di mantenimento.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.1–§4.2.

### OQ-012 — Fabbisogni di PC delle ricette e dei processi

- **Area:** Produzione.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire valori numerici dei fabbisogni di Punti Capacità delle singole ricette e dei processi industriali e i relativi criteri di bilanciamento.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.3.6; §4.4.

### OQ-013 — Modelli produttivi, priorità e spareggi

- **Area:** Produzione.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire modelli produttivi concreti, criteri di assegnazione delle priorità e spareggi cronologici quando il flag di precedenza assoluta non risolve una parità.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.3.6; §4.4.

### OQ-014 — Modificatori della capacità effettiva degli impianti

- **Area:** Produzione e bilanciamento.
- **Stato:** Proposta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Determinare fattori sociali, ambientali e tecnologici, relativa entità e formule che modificano la capacità effettiva degli impianti.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.3.6; §4.4.

### OQ-015 — Vincoli operativi e logistici dei processi

- **Area:** Produzione e logistica.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire regole e parametri dei vincoli operativi e logistici che possono sospendere singoli processi, incluse eventuali condizioni specifiche per tipologia di impianto.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.3; §4.4.

### OQ-016 — Potenziamento degli impianti

- **Area:** Infrastrutture e tecnologia.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire costi di potenziamento e condizioni tecnologiche dettagliate per ciascun livello degli impianti.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.3.

### OQ-017 — Effetti dell’organizzazione sociale sull’industria

- **Area:** Organizzazione sociale ed economia.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire gli effetti concreti dell’organizzazione sociale dell’impero su integrazione, specializzazione ed efficienza industriale.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.4; §6.7.

### OQ-018 — Specifiche delle navi cargo e del trasporto

- **Area:** Logistica.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire capacità, velocità e costi dei cargo, regole di intercettazione e scorta e tempi di viaggio.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.6; §3.7.

### OQ-019 — Commercio tra imperi e valuta

- **Area:** Economia diplomatica.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Stabilire se il gioco prevede commercio tra imperi e una valuta.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.

### OQ-020 — Previsioni di produzione

- **Area:** Economia e interfaccia.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire come vengono calcolate e aggiornate le previsioni di produzione.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §4.3–§4.4.

### OQ-021 — Risorse prenotate in caso di cambiamento della località

- **Area:** Ordini e logistica.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire il trattamento delle risorse prenotate se un pianeta viene conquistato, bloccato o perde capacità produttiva.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §2.7; §4.2; §4.3.

### OQ-022 — Geometria e dimensioni dei bracci galattici

- **Area:** Generazione galattica.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire valori quantitativi di larghezza ed estensione dei bracci e parametrizzazione delle variazioni locali della geometria.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §3.

### OQ-023 — Proporzioni degli addensamenti secondari

- **Area:** Generazione galattica.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire le proporzioni numeriche tra addensamenti secondari piccoli, medi e grandi.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §3.

### OQ-024 — Densità degli addensamenti secondari

- **Area:** Generazione galattica.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire come varia la densità all’interno dei singoli addensamenti secondari.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §3.

### OQ-025 — Quote regionali e transizione tra regioni galattiche

- **Area:** Generazione galattica.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire quote numeriche dei sistemi per regione e raggio della regione di transizione tra nucleo e regioni intermedie.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §3.

### OQ-026 — Generazione dei sistemi stellari e dei corpi celesti

- **Area:** Generazione galattica.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire le regole di generazione dei singoli sistemi stellari, dei corpi celesti e delle loro caratteristiche.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §3; §5.

### OQ-027 — Parametri di rotte e portali

- **Area:** Movimento e logistica.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire raggio massimo delle rotte, costi e capacità dei livelli dei portali e tempi di viaggio numerici.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §3.7.

### OQ-028 — Popolazioni autoctone, scoperta e posizioni iniziali

- **Area:** Esplorazione e colonizzazione.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire frequenza e ruolo delle popolazioni autoctone o neutrali, soglie tecnologiche di colonizzabilità, portata e costo di sonde e flotte e posizioni di partenza dei giocatori.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** sono confermate le distinzioni tra gruppi demografici autoctoni, insediamenti neutrali e corpi celesti disabitati. Le popolazioni autoctone utilizzano lo stesso modello dei gruppi demografici immigrati, distinguendosi tramite l'attributo di origine; gli insediamenti neutrali non costituiscono automaticamente imperi autonomi completi. Restano da definire frequenze, distribuzione, contenuti e comportamento degli insediamenti, nonché scoperta e posizioni iniziali dei giocatori.
- **Riferimenti:** §3.6; §5; §6.9; ADR-004.

### OQ-029 — Intensità delle tendenze regionali di generazione

- **Area:** Generazione galattica.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Determinare l’intensità numerica delle tendenze di generazione per regione.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §3.

### OQ-030 — Frequenze delle classi stellari

- **Area:** Sistemi stellari.
- **Stato:** Proposta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Bilanciare i valori delle frequenze delle classi stellari secondo il modello già descritto.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §5.6.

### OQ-031 — Soglie di abitabilità per specie

- **Area:** Abitabilità.
- **Stato:** Proposta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire le soglie numeriche di abitabilità per ciascuna specie, mantenendo il modello descritto nel GDD.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §5.8; §5.10.

### OQ-032 — Valori e raggi delle anomalie

- **Area:** Anomalie.
- **Stato:** Proposta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Bilanciare valori e raggi delle anomalie secondo il modello esistente.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §5.9.

### OQ-033 — Valori della ricchezza delle risorse dei corpi celesti

- **Area:** Risorse e corpi celesti.
- **Stato:** Proposta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Bilanciare i valori del modello di ricchezza delle risorse dei corpi celesti.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §5.11.

### OQ-034 — Unificazione della descrizione delle anomalie

- **Area:** Coerenza del GDD.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Valutare l’unificazione delle descrizioni delle anomalie attualmente presenti in due sezioni del GDD.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §5.4; §5.9.

### OQ-035 — Specie definitive e regole delle minoranze

- **Area:** Specie e popolazione.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire nomi e tratti esatti delle specie e le regole delle minoranze.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §6.1–§6.2.

### OQ-036 — Bilanciamento dei parametri di colonie e popolazione

- **Area:** Colonie e popolazione.
- **Stato:** Proposta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Bilanciare i valori numerici del modello di popolazione, crescita, stabilità, sostentamento, carestia e personale attualmente impostato come proposta.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §6.1–§6.6.

### OQ-037 — Costi e tempi delle navi coloniali

- **Area:** Colonizzazione e produzione.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire costi e tempi delle navi coloniali, in coordinamento con il catalogo delle ricette.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §6.5; OQ-001.

### OQ-038 — Forme ed effetti dell’organizzazione sociale

- **Area:** Organizzazione sociale.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire le forme definitive di organizzazione sociale dell’impero e i relativi effetti.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §6.7; §4.4.

### OQ-039 — Componenti delle navi e tecnologie associate

- **Area:** Flotte e tecnologia.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire quali componenti esistono per ciascun tipo e a quali tecnologie sono collegati; il GDD indica già una progressione di livelli per tipo, ma non il catalogo completo.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §7.5.

### OQ-040 — Costi di costruzione e potenziamento dell’Accademia

- **Area:** Flotte e personale.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire i costi di costruzione e potenziamento dell’Accademia. I fabbisogni di personale per scafo e componenti, le capacità di addestramento, il costo per persona e il ciclo di addestramento sono già descritti in §7.5.2 e non vengono riaperti da questa voce.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §7.5.2; §4.3; OQ-001.

### OQ-041 — Bilanciamento di danni, scudi, armatura e riparazioni delle navi

- **Area:** Flotte e combattimento.
- **Stato:** Proposta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Bilanciare i valori numerici dei modelli di danno, scudi, armatura e riparazioni già descritti.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §7.9; §7.11.

### OQ-042 — Varianti e compatibilità delle installazioni scientifiche

- **Area:** Ricerca e sviluppo.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire varianti delle installazioni scientifiche planetarie e orbitali e compatibilità con i progetti di ricerca.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §6.8.

### OQ-043 — Fabbisogni di personale delle installazioni scientifiche

- **Area:** Ricerca e sviluppo.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire valori assoluti dei fabbisogni di personale per tipologia e livello di installazione scientifica, inclusa la ripartizione tra ricercatori, tecnici scientifici e personale di supporto.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §6.8.

### OQ-044 — Requisiti operativi delle installazioni scientifiche

- **Area:** Ricerca e sviluppo.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire soglie minime di integrità strutturale, fabbisogni energetici e requisiti logistici quantitativi per mantenere operative le installazioni scientifiche.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §6.8; §4.3.7; OQ-010.

### OQ-045 — Formazione del personale scientifico e relativi modificatori

- **Area:** Ricerca e sviluppo.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire quantità annuali di personale formato da università e istituti per livello e tipologia, formule e intensità dei modificatori tecnologici, sociali globali e specifici, ambientali locali e contingenti, inclusi gli effetti dell’impostazione di governo sulla distribuzione professionale.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §6.8; §6.7; OQ-038.

### OQ-046 — Costi in PR e tempi di sviluppo delle tecnologie

- **Area:** Ricerca e sviluppo.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire costi in Punti Ricerca e tempi di sviluppo per ciascuna tecnologia e livello.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §6.8.

### OQ-047 — Modificatori della generazione dei PR e del tempo di sviluppo

- **Area:** Ricerca e sviluppo.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire valori e formule dei modificatori ambientali, sociali e contingenti applicati alla generazione dei PR delle installazioni e al tempo di sviluppo.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §6.8; OQ-038.

### OQ-048 — Effetti dei danni sulle sedi scientifiche

- **Area:** Ricerca e sviluppo.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire eventuali condizioni operative aggiuntive e conseguenze specifiche dei diversi livelli di danneggiamento delle sedi scientifiche.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §6.8; §4.3.7.

### OQ-049 — Riconversione dei laboratori planetari

- **Area:** Ricerca e sviluppo.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire costi e tempi della riconversione della specializzazione dei laboratori planetari.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §6.8.

### OQ-050 — Trasferimento dei progetti di ricerca

- **Area:** Ricerca e sviluppo.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire regole di compatibilità tra progetto e installazione per il trasferimento e durata puntuale del turno di trasferimento in relazione alla scansione mensile.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §6.8.

### OQ-051 — Bonus di ricerca in nave e modello generale dei PR

- **Area:** Ricerca e sviluppo.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Coordinare il bonus mensile ai PR della ricerca in nave con il modello generale di generazione e accumulo dei Punti Ricerca.
- **Obiettivo:** definire una specifica verificabile e coerente con i sistemi già documentati.
- **Cosa blocca:** da valutare nell’analisi; non ancora determinato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell’analisi:** assistente.
- **Proposta corrente:** nessuna proposta approvata. Eventuali valori o modelli già presenti nel GDD sono indicati come provvisori e non diventano automaticamente decisioni.
- **Riferimenti:** §7.12; §6.8.

### OQ-052 — Calcolo degli slot planetari e gestione delle variazioni di capacità

- **Area:** Sviluppo planetario.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire il numero base di slot per dimensione planetaria, i modificatori tecnologici e sociali, il ricalcolo della capacità e il comportamento quando il limite diminuisce o aumenta.
- **Obiettivo:** ottenere un modello deterministico che preservi gli slot e le strutture già occupati senza consentire nuove assegnazioni oltre la capacità effettiva.
- **Cosa blocca:** il dimensionamento numerico e le regole complete di assegnazione degli slot.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell'analisi:** assistente.
- **Proposta corrente:** sono confermate le cinque funzioni (abitazione, industria, ricerca, commercio, socialità), la dipendenza dalla dimensione e i modificatori tecnologici/sociali. Resta da definire l'associazione dei gruppi demografici alle funzioni non residenziali e sociali.
- **Riferimenti:** §6.10; ADR-004.

### OQ-053 — Regole operative di utilizzo, controllo e accesso orbitale

- **Area:** Territorialità e infrastrutture orbitali.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Specificare come si acquisiscono, trasferiscono o perdono le relazioni di utilizzo e controllo e quali diritti derivano dall'utilizzo di un pianeta rispetto allo spazio orbitale e alle installazioni presenti.
- **Obiettivo:** rendere coerenti colonizzazione, occupazione, conquista, accesso alle installazioni orbitali e utilizzo condiviso di sistemi e rotte.
- **Cosa blocca:** le regole operative di colonizzazione/occupazione e interazione con infrastrutture orbitali.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell'analisi:** assistente.
- **Proposta corrente:** sono confermate l'esclusività dell'utilizzo planetario e la possibilità di utilizzo condiviso di sistemi e rotte. L'utilizzo del pianeta non attribuisce automaticamente diritti esclusivi sull'orbita o sulle installazioni orbitali.
- **Riferimenti:** §5.12; §6.11; ADR-004.

### OQ-054 — Soglie informative, aggiornamento e decadimento della conoscenza

- **Area:** Intelligence.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire soglie specifiche per categoria informativa, condivisione tra alleati, frequenza di aggiornamento e decadimento delle informazioni non più osservate.
- **Obiettivo:** garantire una rappresentazione coerente e temporalmente corretta della conoscenza di ciascun impero.
- **Cosa blocca:** il dettaglio operativo delle informazioni disponibili nell'interfaccia e nei sistemi decisionali.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell'analisi:** assistente.
- **Proposta corrente:** è confermata la scala generale 0–4 e le categorie informative riportate in §1.3. Le soglie puntuali possono differire tra categorie.
- **Riferimenti:** §1.3; §7.12; ADR-005.

### OQ-055 — Distacco, trasferimento e sostituzione dell'ammiraglia

- **Area:** Flotte e navi.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire cosa accade se l'ammiraglia viene distaccata volontariamente, trasferita o sostituita prima di essere perduta.
- **Obiettivo:** completare le regole di ciclo di vita della flotta senza contraddire la dissoluzione automatica in caso di perdita dell'ammiraglia.
- **Cosa blocca:** le operazioni di riorganizzazione e trasferimento delle navi tra flotte.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell'analisi:** assistente.
- **Proposta corrente:** è confermato che la perdita dell'ammiraglia dissolve la flotta e rende indipendenti le navi superstiti. Non è ancora definito il caso di distacco volontario o sostituzione.
- **Riferimenti:** §7.1; ADR-006.

### OQ-056 — Attributi dei gruppi demografici ed effetti di espulsione/riconversione

- **Area:** Colonie e popolazione.
- **Stato:** Aperta.
- **Priorità:** Non assegnata.
- **Problema da affrontare:** Definire gli attributi completi dei gruppi demografici, le relazioni con gli slot e le procedure, i tempi e gli effetti dell'espulsione e della riconversione degli slot occupati.
- **Obiettivo:** rendere simulabili migrazione, coesistenza, insediamento e rimozione di gruppi senza cancellazioni automatiche dovute alla conquista.
- **Cosa blocca:** la definizione completa della gestione demografica e delle operazioni di insediamento forzato.
- **Dipendenze:** da valutare; nessun prerequisito viene assunto senza verifica.
- **Responsabile della decisione:** utente.
- **Preparazione dell'analisi:** assistente.
- **Proposta corrente:** sono confermati gruppi autonomi con specie, popolazione, origine e affiliazione politica; la popolazione totale della colonia è derivata dalla somma dei gruppi. L'occupazione militare non elimina automaticamente i gruppi esistenti; espulsione e riconversione sono operazioni esplicite.
- **Riferimenti:** §6.9–§6.10; ADR-004.

## 4. Questioni risolte e archivio

Le questioni risolte saranno trasferite qui dopo l’approvazione e
l’integrazione documentale. Per ciascuna saranno mantenuti ID, decisione
finale e riferimenti al GDD e all’eventuale ADR.

### Elementi già confermati o risolti prima della migrazione

Le seguenti voci della precedente §8 del GDD non sono state inserite nel
backlog attivo perché risultavano già definite nel GDD:

- **Attività e avanzamento (§2.7):** calcolo del tempo residuo dopo
  un’interruzione; durata minima di tre mesi per applicare il consumo in
  tre fasi; assenza di deterioramento e costi di mantenimento durante
  l’interruzione nella prima versione.
- **Sistemi stellari e corpi celesti:** numero massimo di lune e
  condizioni di abitabilità delle lune (§5.10); orbite nei sistemi
  multipli e fascia abitabile (§5.8.1).
- **Flotte e combattimento:** numero di slot per taglia di scafo (§7.4);
  modello di spionaggio, contromisure e ricerca in nave e collegamento
  tra navi e cantieri tramite PC (§7.12).
- **Personale delle navi e Accademia (§7.5.2):** quote base per taglia,
  fabbisogni dei componenti, capacità di addestramento, costo per persona
  e durata del ciclo di addestramento. Restano aperti soltanto i costi di
  costruzione e potenziamento dell’Accademia (OQ-040).
- **Arrotondamento del consumo 50/30/20 (§2.7):** costi in unità intere,
  prime due quote arrotondate per difetto e terza quota a conguaglio;
  sotto i tre mesi si applica una fase unica.

Queste voci restano consultabili nelle rispettive sezioni del GDD; non
ricevono un ID OQ attivo perché non richiedono una nuova decisione.

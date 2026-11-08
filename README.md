# Heaven's Light

Heaven's Light è un gioco strategico spaziale 4X in fase di progettazione. Questo repository raccoglie la documentazione e, progressivamente, il codice sorgente del progetto.

## Stato del progetto

Il progetto è nella fase di preparazione. Il Game Design Document (GDD) è in lavorazione e definisce le regole, i sistemi e le decisioni di design.

La libreria C# e i progetti Unity non sono ancora stati creati.

## Architettura prevista

Il codice sarà organizzato per separare le regole di simulazione dall'interfaccia e dal motore grafico:

- **Game.Core** — modelli di dominio e regole fondamentali, indipendenti da Unity.
- **Game.Application** — casi d'uso e coordinamento delle operazioni di gioco.
- **Game.Unity** — integrazione con Unity, interfaccia, visualizzazione e input.

La simulazione sarà progettata per poter essere verificata indipendentemente dalla presentazione grafica. Il primo obiettivo è un prototipo single-player; il multiplayer potrà essere affrontato in una fase successiva.

## Struttura prevista

```text
Documentation/
  GDD/
  TechnicalDesign/
  Decisions/
Source/
  Game.Core/
  Game.Application/
  Game.Unity/
Tests/
  Core.UnitTests/
  Simulation.Tests/
  Integration.Tests/
Assets/
```

La struttura è indicativa e verrà aggiornata man mano che i contenuti vengono aggiunti al repository.

## Tecnologie previste

- C#
- Unity
- Git e GitHub
- NUnit per i test automatici
- JSON per i dati iniziali di gioco

## Documentazione

Il GDD contiene le regole di gioco concordate e le questioni ancora aperte. Le proposte non ancora approvate non devono essere considerate decisioni definitive.

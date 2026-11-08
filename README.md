# Heaven's Light

Heaven's Light è un gioco strategico spaziale 4X in fase di progettazione. Il repository contiene la documentazione e la struttura iniziale del codice sorgente.

## Stato del progetto

La fase zero, dedicata alla preparazione della struttura e degli strumenti di sviluppo, è completata sul branch `Fase-zero`. Il gameplay non è ancora stato implementato.

## Architettura

- **Game.Core** — modelli e regole fondamentali della simulazione, indipendenti da Unity.
- **Game.Application** — casi d'uso e coordinamento della logica di gioco; dipende da `Game.Core`.
- **Game.Unity** — progetto Unity per la presentazione, l'interfaccia, l'input e l'integrazione con la simulazione.

Le librerie `Game.Core` e `Game.Application` supportano i target `.NET 10` e `.NET Standard 2.1`. Il progetto Unity usa Unity `6000.6.8f1`.

## Struttura attuale

```text
HeavensLight/
├── HeavensLight.sln
├── README.md
├── .gitignore
├── Source/
│   ├── Game.Core/
│   │   └── Game.Core.csproj
│   ├── Game.Application/
│   │   └── Game.Application.csproj
│   └── Game.Unity/
│       ├── Assets/
│       ├── Packages/
│       └── ProjectSettings/
└── Tests/
    ├── Game.Core.Tests/
    │   └── Game.Core.Tests.csproj
    └── Game.Application.Tests/
        └── Game.Application.Tests.csproj
```

La solution .NET contiene le due librerie e i relativi progetti di test. Il progetto Unity viene aperto tramite Unity Hub ed è configurato per usare Rider come editor esterno.

## Test

- **Game.Core.Tests** — include il test iniziale che verifica i limiti validi dei mesi di gioco (da 1 a 12).
- **Game.Application.Tests** — progetto di test predisposto; i test dei casi d'uso verranno aggiunti quando sarà implementata la relativa logica.

I test automatici usano NUnit.

## Versionamento

Il repository usa Git. Sono versionati i file necessari per ricostruire il progetto Unity, inclusi `Assets`, `Packages` e `ProjectSettings`; le cartelle temporanee e gli artefatti generati sono esclusi tramite `.gitignore`.

# DatingApp

DatingApp è un'applicazione web full-stack di incontri (dating app) composta da un backend **ASP.NET Core Web API** e un frontend **Angular**. Permette agli utenti di registrarsi, creare un profilo con foto, sfogliare altri utenti, mettere "like", scambiare messaggi in tempo reale e gestire lo stato online/offline tramite SignalR.

## Indice

- [Funzionalità principali](#funzionalità-principali)
- [Architettura](#architettura)
- [Stack tecnologico](#stack-tecnologico)
- [Struttura del progetto](#struttura-del-progetto)
- [Prerequisiti](#prerequisiti)
- [Configurazione](#configurazione)
- [Avvio del backend](#avvio-del-backend)
- [Avvio del frontend](#avvio-del-frontend)
- [API principali](#api-principali)
- [Test](#test)
- [Sicurezza](#sicurezza)

## Funzionalità principali

- **Autenticazione e registrazione** utenti con password hashing e JWT (JSON Web Token).
- **Gestione profilo**: informazioni personali, città, interessi, data di nascita, genere.
- **Upload e gestione foto** tramite integrazione con [Cloudinary](https://cloudinary.com/), inclusa la possibilità di impostare la foto principale ed eliminare foto.
- **Ricerca e filtro utenti** con paginazione, filtro per genere ed età, ordinamento per ultimo accesso o data di iscrizione.
- **Sistema di "Like"**: possibilità di mettere/rimuovere like ad altri utenti e visualizzare chi ti ha messo like o chi hai messo like.
- **Messaggistica privata** tra utenti, con cronologia dei messaggi, conteggio messaggi non letti ed eliminazione "soft" dei messaggi (per mittente/destinatario).
- **Comunicazione in tempo reale** tramite **SignalR**:
  - `PresenceHub`: notifica quando un utente è online/offline.
  - `MessageHub`: invio e ricezione di messaggi in tempo reale, aggiornamento "letto/non letto".
- **Pannello di amministrazione** per la gestione dei ruoli utente e la moderazione delle foto (approvazione/rifiuto).
- **Gestione ruoli** basata su ASP.NET Core Identity (es. `Admin`, `Moderator`, `Member`).
- **Gestione centralizzata degli errori** tramite middleware dedicato, con risposte di errore consistenti verso il client.

## Architettura

L'applicazione è divisa in due macro-componenti che comunicano tramite API REST e WebSocket (SignalR):

```
┌─────────────────────┐        HTTPS / REST        ┌──────────────────────┐
│   Angular Frontend   │  ─────────────────────────▶ │   ASP.NET Core API   │
│   (client/DatingAppFE) │ ◀───────────────────────── │      (WebApi)        │
└─────────────────────┘        WebSocket (SignalR)   └──────────┬───────────┘
                                                                  │
                                                                  ▼
                                                        ┌───────────────────┐
                                                        │  SQLite Database  │
                                                        │  (DatingApp.db)   │
                                                        └───────────────────┘
                                                                  │
                                                                  ▼
                                                        ┌───────────────────┐
                                                        │     Cloudinary    │
                                                        │ (storage immagini)│
                                                        └───────────────────┘
```

## Stack tecnologico

### Backend (`WebApi`)

- **.NET 8** / ASP.NET Core Web API
- **Entity Framework Core** con provider **SQLite**
- **ASP.NET Core Identity** per gestione utenti e ruoli
- **Autenticazione JWT Bearer** (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- **AutoMapper** per il mapping tra entità e DTO
- **SignalR** per comunicazione real-time (presenza utenti e messaggistica)
- **CloudinaryDotNet** per l'upload e la gestione delle foto

### Frontend (`client/DatingAppFE`)

- **Angular 17** (standalone components)
- **Bootstrap 5** e **Bootswatch** per lo stile
- **ngx-toastr** per le notifiche
- **ngx-spinner** per gli indicatori di caricamento
- **ngx-bootstrap** per componenti UI (es. date picker, tabs)
- **ng2-file-upload** per l'upload delle foto
- **ng-gallery** per la galleria immagini del profilo
- **@microsoft/signalr** per il client SignalR (chat e presenza in tempo reale)
- **ngx-timeago** per la formattazione "tempo fa" dei messaggi

## Struttura del progetto

```
DatingApp/
├── DatingApp.sln                # Solution file .NET
├── WebApi/                      # Backend ASP.NET Core
│   ├── Controllers/             # Endpoint API (Account, Users, Likes, Messages, Admin, ...)
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Data/                    # DbContext, repository, seed dati di test
│   ├── Entities/                # Modelli di dominio (User, Message, Photo, UserLike, ...)
│   ├── Errors/                  # Modelli per la gestione errori
│   ├── Extensions/              # Metodi di estensione per la configurazione servizi
│   ├── Helpers/                 # Paginazione, mapping profiles, parametri di query
│   ├── Interfaces/              # Contratti dei repository/servizi
│   ├── Middleware/              # Middleware personalizzati (gestione eccezioni)
│   ├── Services/                # Implementazioni servizi (Token, Photo)
│   ├── SignalR/                 # Hub SignalR (Presence, Message) e tracker connessioni
│   ├── Program.cs               # Entry point, configurazione pipeline HTTP
│   └── appsettings.json         # Configurazione applicazione
└── client/
    └── DatingAppFE/              # Frontend Angular
        └── src/app/
            ├── _directives/      # Direttive custom
            ├── _forms/           # Componenti/utility per i form
            ├── _guards/          # Route guard (auth, admin, ...)
            ├── _interceptors/    # HTTP interceptor (JWT, error, loading)
            ├── _models/          # Modelli TypeScript
            ├── _resolvers/       # Resolver per il precaricamento dati nelle rotte
            ├── _services/        # Servizi (account, members, likes, messages, presence, ...)
            ├── admin/            # Componenti pannello amministrazione
            ├── errors/           # Pagine di errore
            ├── home/             # Home page e landing
            ├── lists/            # Liste utenti (like ricevuti/inviati)
            ├── members/          # Profilo utente, dettaglio, modifica, foto
            ├── messages/         # Componenti chat/messaggistica
            ├── modals/           # Finestre modali
            ├── nav-bar/          # Barra di navigazione
            └── register/         # Registrazione utente
```

## Prerequisiti

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (versione compatibile con Angular 17, es. Node 18+) e npm
- [Angular CLI](https://angular.io/cli) (`npm install -g @angular/cli`)
- Un account [Cloudinary](https://cloudinary.com/) (per l'upload delle foto)

## Configurazione

Il backend richiede alcune impostazioni in `WebApi/appsettings.json` o, preferibilmente, tramite [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) in locale:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DatingApp.db"
  },
  "JsonTokenKey": "<una chiave segreta lunga e casuale per firmare i JWT>",
  "CloudinarySettings": {
    "CloudName": "<il tuo cloud name Cloudinary>",
    "ApiKey": "<la tua API key Cloudinary>",
    "ApiSecret": "<il tuo API secret Cloudinary>"
  }
}
```

> ⚠️ **Non committare mai chiavi/segreti reali nel repository.** Utilizzare `dotnet user-secrets` o variabili d'ambiente per i valori sensibili in ambienti di sviluppo/produzione.

## Avvio del backend

```bash
cd WebApi
dotnet restore
dotnet ef database update   # applica le migrazioni ed eventualmente crea il database SQLite
dotnet run
```

All'avvio, l'applicazione:
1. Applica automaticamente le migrazioni del database (`context.Database.MigrateAsync()`).
2. Ripulisce la tabella delle connessioni SignalR residue.
3. Esegue il seed di utenti di prova (`Seed.SeedUsers`) se il database è vuoto.

L'API sarà disponibile su `https://localhost:7246` (verificare la porta effettiva in `Properties/launchSettings.json`).

## Avvio del frontend

```bash
cd client/DatingAppFE
npm install
ng serve
```

L'applicazione sarà disponibile su `http://localhost:4200`. Il frontend è configurato (vedi `src/environments`) per comunicare con l'API su `https://localhost:7246/api/` e con gli hub SignalR su `https://localhost:7246/hubs/`.

## API principali

| Controller | Descrizione |
|---|---|
| `AccountController` | Registrazione e login utenti, generazione JWT |
| `UsersController` | Recupero, filtro, paginazione e aggiornamento profili utente; gestione foto |
| `LikesController` | Aggiunta/rimozione like, elenco utenti a cui è stato messo like o che hanno messo like |
| `MessagesController` | Invio, lettura ed eliminazione dei messaggi tra utenti |
| `AdminController` | Gestione ruoli utente e moderazione foto (solo per amministratori/moderatori) |

Gli endpoint sono protetti tramite autenticazione JWT, tranne gli endpoint di login e registrazione; inoltre CORS è configurato per accettare richieste dal frontend Angular in esecuzione su `localhost:4200`.

## Test

- **Backend**: al momento non sono presenti progetti di test automatizzati dedicati; eventuali test possono essere aggiunti come progetti xUnit/NUnit collegati alla solution.
- **Frontend**: `ng test` esegue gli unit test tramite Karma/Jasmine.

```bash
cd client/DatingAppFE
ng test
```

## Sicurezza

- Le password sono gestite tramite ASP.NET Core Identity (hashing sicuro).
- L'autenticazione avviene tramite JWT firmati con una chiave segreta configurabile.
- Le foto vengono caricate ed erogate tramite Cloudinary, con possibilità di moderazione da parte degli amministratori prima della pubblicazione.
- Si raccomanda di non esporre mai in chiaro, nei file di configurazione versionati, le chiavi JWT o le credenziali Cloudinary: utilizzare variabili d'ambiente o secret manager negli ambienti reali.

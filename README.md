# MCP Workshop

## Introduksjon

Vi skal legge til MCP på et eksisterende todo-API. API-et lar en vise, opprette, redigere og slette oppgaver. Idéen er at vi skal gjøre det mulig for f.eks. GitHub Copilot å jobbe med oppgavene på tvers av sessions.

Dersom du sitter fast kan du ta en titt på commit-historikken i [solution-branchen](https://github.com/tmaregge/MCP-workshop/tree/solution).

## Oppgave -1: Grunnleggende oppsett

1. Klon repo: `git clone https://github.com/tmaregge/MCP-workshop.git`
2. Installer pakker:
    ```bash
    dotnet add package ModelContextProtocol --prerelease
    dotnet add package ModelContextProtocol.AspNetCore --prerelease
    dotnet add package Microsoft.Extensions.Hosting
    ```
3. Bygg løsning og kjør
4. Opprett noen todos

## Oppgave 0: Sett opp MCP

### ⚙️ Konfigurer prosjekt

Legg til følgende i `builder` i `Program.cs`:

```csharp
builder.Services
    .AddMcpServer()
    .HttpTransport() 
    .WithToolsFromAssembly();
```

`.HttpTransport()` spesifiserer at MCP skal tilgjengeliggjøres på et eget HTTP-endepunkt. `.WithToolsFromAssembly()` gjør så verktøy vi definerer i prosjektet automatisk blir tilgjengelige. Alternativt kan man spesifisere konkrete verktøy man vil tilgjengeliggjøre med `.WithTools<IFooTool>()`.

For at MCP-endepunktet skal være tilgjengelig må vi mappe det:
```csharp
app.MapMcp("/mcp");
```

---

### 📁 Mappestruktur

Opprett følgende struktur i prosjektet:

```
/MCP
    /Tools
        TodoTool.cs
```

---

### 🧩 `TodoTool.cs`

Opprett filen `TodoTool.cs` med følgende innhold:

```csharp
using System.ComponentModel;
using ModelContextProtocol.Server;

namespace TodoApi.MCP.Tools;

[McpServerToolType]
public class TodoTool
{
    [McpServerTool, Description("Test")]
    public static string Echo(string message) => $"hello {message}";
}
```

### 🧪 Test med MCP Inspector

MCP Inspector er et nyttig verktøy for å feilsøke MCP. Det kan hende du må installere `npm`/`npx`, hvis du ikke har det fra før.

Start programmet ved å kjøre følgende kommando fra prosjektets rotmappe:

```bash
npx @modelcontextprotocol/inspector dotnet run
```

Velg deretter:

* **Transport type:** `Streamable HTTP`
* **URL:** `http://localhost:5056/mcp`

Trykk **Connect**.
Du kan nå liste opp tools og kalle dem. Prøv å kjøre `Echo`-toolet for å verifisere at alt fungerer.

---

### 🧠 Koble MCP-serveren til IDE

For å bruke MCP-verktøyene våre via GitHub Copilot i IDE-en, må den konfigureres. Dette er litt ulikt basert på IDE-en, men det innebærer å opprette en `mcp.json`-fil som sier hvordan MCP-serveren kan nås. Her konfigureres også eksterne MCP-servere (f.eks GitHub MCP).

VS Code

1. Trykk `CTRL + Shift + P`
2. Velg **MCP: Add Server**
3. Velg **HTTP**
4. Skriv inn `http://localhost:5056/mcp`

---

JetBrains Rider

1. Åpne **Copilot**
2. Aktiver **Agent Mode**
3. Trykk på **verktøysymbolet** → **Add More Tools**

Dette åpner `intellij/mcp.json` (i AppData).
Lim inn følgende konfigurasjon (samme som for VS Code):

```json
{
  "inputs": [],
  "servers": {
    "TodoMcpServer": {
      "url": "http://localhost:5056/mcp"
    }
  }
}
```

### 🚀 Kjør prosjektet

Bygg og start prosjektet:

```bash
dotnet build
dotnet run
```

Når prosjektet kjører:

* Trykk på **verktøysymbolet** i Copilot Chat-vinduet og finn `Echo`-toolet. Huk av boksen.
* I Rider: Trykk på **Configure Model Access** for å sikre at modellen har tilgang til toolet.
* Be Copilot kalle toolet med en vilkårlig input 
* Profit

## Oppgave 1: Lag tools for todo-API

I denne oppgaven skal du legge til tools for å gjøre det mulig å interagere med todo-API-et.

Bruk metodene fra `ITodoRepository` for å implementere tools for å gjøre følgende:
1. Vise todos for en gitt bruker
2. Opprette todo
3. Slette todo
4. Oppdatere en todo med ny info

Hold gjerne funksjonaliteten enkel til å begynne med. Mer avansert filtrering og lignende kan eventuelt legges på etter hvert. Utforsk hva agenten er i stand til å gjøre med relativt enkle tools!

Når du er ferdig med å lage tools kan du teste følgende:
* Bryt ned en feature i konkrete oppgaver
* Lag et sammendrag av de viktigste oppgavene
* Omprioriter oppgaver basert
* List opp todos relatert til et gitt tema/tag
* Slett todos som er utført


💡 Tips:
* Test tools med MCP Inspector før du prøver med Copilot
* Bruk grensesnittet til todo-API-et for å opprette data og verifisere at ting funker
* Kast `McpException` når du vil at en feilmelding skal vises til brukeren
* Hvis et tool ikke dukker i Copilot hender det at man må restarte MCP-koblingen. I VS Code kan man gjøre dette i `mcp.json`-filen

## Oppgave 2: Lag prompts

MCP-serveren kan definere vilkårlige `Prompts` som man kan ta i bruk via agenter. Disse brukes ved å skrive `/` etterfulgt av prompt-navnet, f.eks. `/list` for å få et prompt som kan brukes for å liste todos. Prompts kan brukes til å definere vanlige arbeidsflyter, redusere mengden tekst man trenger å skrive, fortelle agenten hvordan den skal bruke tools i kombinasjon, etc.

Opprett filen `MCP/Prompts/TodoPrompts.cs` og lag ulike prompts. Her er et eksempel fra [dokumentasjonen](https://github.com/modelcontextprotocol/csharp-sdk):
```csharp
[McpServerPromptType]
public static class MyPrompts
{
    [McpServerPrompt, Description("Creates a prompt to summarize the provided message.")]
    public static ChatMessage Summarize([Description("The content to summarize")] string content) =>
        new(ChatRole.User, $"Please summarize this content into a single sentence: {content}");
}
```

Funksjonen returnerer et `ChatMessage`-objekt med en melding som pastes i chatboksen til Copilot når du skriver `/summarize`. Man kan returnere en liste med `ChatMessage` for å simulere en samtale. Da kan man f.eks legge inn et systemprompt ved å bruke `ChatRole.System`.

Her er noen eksempler på ting man kan lage prompts for:
* Lag en konsis oppsummering av relevante todos (kun tittel, feks)
* Lag en detaljert rapport av ukens arbeid (hva som ble utført og når ting ble gjort)
* Sorter todos basert på prioritet, og bruk emojis for å representere de ulike nivåene
* Hva enn du kommer på. Kun fantasien setter grenser ✋🌈🤚

💡 Tips:
* For at Prompts skal eksponeres må man kalle `.WithPromptsFromAssembly()` i service builder-en 
* Som med Tools kan det hende du må restarte MCP-koblingen for at de skal dukke opp
    
## Oppgave 3: Elicitation

Elicitation lar MCP-serveren be brukeren om ekstra input. Dersom Elicitation trigges vil brukeren få en popup med informasjonen de må fylle ut.

For legge til et Elicitation-steg i et gitt tool må man gjøre følgende:
* Legg til `McpServer` som funksjonsparameter via dependency injection
* Definere hvordan informasjonen som skal hentes ser vha `ElicitRequestParams.RequestSchema`
* Kalle `McpServer.ElicitAsync` med det definerte schema-et

Kodeeksempler er tilgjengelige i denne [artikkelen](https://devblogs.microsoft.com/dotnet/mcp-csharp-sdk-2025-06-18-update/), men legg merke til at `IMcpServer` er deprecated til fordel for `McpServer`.

Bruk Elicitation til å gjøre følgende:
* Be bruker oppgi `Creator` og `Title` dersom det mangler
* Ber om ekstra bekreftelse ved sletting av todos hvor `Priority` er `High` eller `Urgent`
* Noen andre kule idéer? 😉
    
## Oppgave 4: Autentisering (bonus for nørds 🤓)

Per nå har API-et sånn ca. null sikkerhet, og vi kan se og redigere andre brukeres todos ved å sende et vilkårlig navn som `Creator`. Legg ved tokens på forespørslene og database-objektene for å hindre at uvedkommende får tilgang.

Hvis du har kommet så langt trenger du vel ikke så mye hjelp, men her er en [nyttig artikkel](https://auth0.com/blog/an-introduction-to-mcp-and-authorization/) 😉


Dobbel bonus: Eksponer todo-API-et via HTTPS i stedet for HTTP.


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

Test gjerne underveis vha MCP Inspector og GitHub Copilot. Prøv gjerne å utføre kompliserte oppgaver, som å få Copilot til å bryte ned en eksisterende todo i mer konkrete oppgaver, eller å be Copilot om å omprioritere eksisterende todos.

💡 Tips:
* Bruk grensesnittet til todo-API-et for å sjekke at tools fungerer som de skal, opprette testdata, etc.

## Oppgave 2: Lag prompts (bonus)

Serveren kan definere `Prompts` som agenter kan bruke. Dette er akkurat det det høres ut som. Man kan bruke dette til vanlige arbeidsflyter.

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

Her er noen eksempler på prompts man kan lage:
* Bryte ned en feature til oppgaver
* Prompt for å bryte ned en feature til oppgaver




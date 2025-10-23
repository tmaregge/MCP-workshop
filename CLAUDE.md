# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an ASP.NET Core 9.0 Web API project for a Todo application. It uses minimal APIs (no controllers) and is configured for development with OpenAPI support.

## Development Commands

### Build and Run
```bash
# Build the project
dotnet build

# Run the application (HTTP profile, port 5056)
dotnet run --launch-profile http

# Run with HTTPS (ports 7252 and 5056)
dotnet run --launch-profile https

# Watch mode for development (auto-reload on file changes)
dotnet watch run
```

### Testing HTTP Endpoints
The `TodoApi.http` file contains HTTP request examples for testing endpoints. The default application URL is `http://localhost:5056`.

### Database Migrations
```bash
# Add a new migration (after model changes)
dotnet ef migrations add <MigrationName>

# Apply migrations to the database
dotnet ef database update

# Remove the last migration (if not yet applied)
dotnet ef migrations remove

# View migration SQL without applying
dotnet ef migrations script
```

## Architecture

### Minimal API Structure
The application uses ASP.NET Core minimal APIs defined in `Program.cs`. All endpoints are registered using `app.MapGet()`, `app.MapPost()`, etc. rather than traditional controllers.

### Project Configuration
- **Target Framework**: .NET 9.0
- **Nullable Reference Types**: Enabled
- **Implicit Usings**: Enabled
- **OpenAPI**: Available in Development environment at `/openapi/v1.json`

### Data Models
Located in `Models/` directory:
- `Todo`: Main entity with Guid Id, title, description, creator (string), state, priority, dates, and tags
- `TodoState`: Enum with values Todo, Doing, Done
- `Priority`: Enum with values Low, Medium, High, Urgent

The `Todo` class includes:
- Guid-based unique identifier
- Required fields: Title, Creator
- Optional fields: Description, StartDate, DueDate, EndDate
- Collections: Tags (List<string>)
- Automatic timestamps: CreatedAt, UpdatedAt

### Configuration
- `appsettings.json`: Base configuration with connection strings
- `appsettings.Development.json`: Development-specific overrides
- `Properties/launchSettings.json`: Launch profiles with port configurations

### Data Access Layer

**Database**: SQLite database (`todos.db`) configured via Entity Framework Core 9.0

**DbContext**: `TodoDbContext` (in `Data/` directory)
- Configures entity mappings and relationships
- Enums stored as strings for readability
- Includes indexes on State, Creator, and CreatedAt for query performance
- Tags stored as comma-separated values

**Repository Pattern**: `ITodoRepository` interface with `TodoRepository` implementation (in `Repositories/` directory)
- Registered as scoped service in DI container
- Provides methods:
  - `GetAllAsync()`: Returns all todos ordered by creation date
  - `GetByIdAsync(Guid)`: Find todo by ID
  - `GetByCreatorAsync(string)`: Filter by creator
  - `GetByStateAsync(TodoState)`: Filter by state
  - `CreateAsync(Todo)`: Create new todo with auto-generated ID and timestamps
  - `UpdateAsync(Todo)`: Update existing todo and set UpdatedAt timestamp
  - `DeleteAsync(Guid)`: Delete todo by ID
  - `ExistsAsync(Guid)`: Check if todo exists

**Important**: When creating todos via the repository, the Id, CreatedAt, and UpdatedAt fields are automatically set. When updating, the UpdatedAt field is automatically updated.

## Adding New Endpoints

When adding minimal API endpoints in `Program.cs`:
1. Inject `ITodoRepository` into route handlers via parameter
2. Define route handlers before `app.Run()`
3. Use `.WithName()` to name endpoints for OpenAPI
4. Follow the pattern: `app.MapVerb("/route", async (ITodoRepository repo) => { ... })`
5. Return appropriate HTTP status codes (Ok, NotFound, Created, etc.)

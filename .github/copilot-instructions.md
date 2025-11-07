# RedSismica - AI Coding Agent Instructions

## Project Overview
RedSismica is a seismic network monitoring application built with **Avalonia UI** on **.NET 9.0**. While Avalonia is an MVVM framework, **this project uses it as MVC** - ViewModels act as Controllers that orchestrate business logic and view interaction. The system manages inspection orders for seismic stations, tracks seismograph status changes, and handles notification workflows for inspection personnel.

## Architecture & Key Components

### Application Structure
- **Entry Point**: `Program.cs` initializes mock data in `BaseDeDatosMock` and launches the Avalonia app
- **Authentication Flow**: `LoginWindow` → `MainWindow` (requires successful authentication)
- **Session Management**: Global `SesionManager` singleton maintains `SesionActual` throughout the app lifecycle
- **Data Layer**: `BaseDeDatosMock` in `Program.BaseDeDatosMock` serves as an in-memory database with test data

### MVC Pattern (Using MVVM Framework)
- **Controllers (named ViewModels)**: Inherit from `ViewModelBase` - these are controllers that manage business logic, not traditional MVVM ViewModels
- **View Resolution**: `ViewLocator` maps ViewModels to Views by name convention (`*ViewModel` → `*View`)
- **Compiled Bindings**: Enabled by default via `AvaloniaUseCompiledBindingsByDefault` in `.csproj`
- **Boundary Pattern**: Controllers like `GestorCierreOrdenInspeccion` take a boundary (View) as constructor parameter for bi-directional communication

### Domain Model Hierarchy
```
EstacionSismologica (Station)
  └─ Sismografo (Seismograph)
       └─ CambioEstado[] (State changes with timestamps)
            └─ Estado + MotivoFueraServicio[] (Reasons for out-of-service)

OrdenDeInspeccion (Inspection Order)
  └─ ResponsableInspeccion (Usuario)
  └─ Estado (Current state: "Completamente Realizada", "Cerrada", etc.)
  └─ EstacionSismologica
```

### Critical State Management & Design Patterns
- **State Pattern**: `Estado` class implements state-specific behavior with `Ambito` scoping ("Orden de Inspección" vs "Sismografo") and methods like `EsCompletamenteRealizada()`, `EsCerrada()`
- **Singleton Pattern**: `SesionManager` provides global session state access via static `SesionActual` property
- **Primary Constructor Pattern**: All models use C# 12 primary constructors with parameter validation
- **State Transitions**: `OrdenDeInspeccion.Cerrar()` updates state and marks seismograph as out-of-service via `EstacionSismologica.PonerSismografoEnFueraDeServicio()`
- **OOP Best Practices**: Models use encapsulation (private setters), behavior-rich domain objects, and intention-revealing method names

## Planned Architecture Evolution

### PostgreSQL Database Integration (Next Steps)
- **Current State**: In-memory `BaseDeDatosMock` provides test data
- **Migration Plan**: Create domain-to-database mapper layer for PostgreSQL
- **Data Access Pattern**: Implement repository pattern or data mapper to separate domain logic from persistence
- **Database Schema**: Will mirror domain model hierarchy (EstacionSismologica → Sismografo → CambioEstado)

### Design Patterns to Apply
- **State Pattern**: Already implemented in `Estado` class - expand as needed
- **Singleton Pattern**: `SesionManager` singleton - consider for database connection management
- **Repository Pattern**: Planned for PostgreSQL data access layer
- **Boundary/Interface Pattern**: Continue using for View-Controller communication
- **DTO Pattern**: Already using `DatosOrdenInspeccion` - expand for database layer

## Developer Workflows

### Building & Running
- **Build**: Use VS Code task `build` or `dotnet build RedSismica.sln`
- **Run**: `dotnet run --project RedSismica/RedSismica.csproj`
- **Watch Mode**: Use task `watch` for hot reload during development

### Test Credentials
Hardcoded in `Program.InicializarDatosDePrueba()`:
- Username: `jlinares` / Password: `123` (RI role)
- Username: `mperez` / Password: `123` (RI role)
- Username: `cgomez` / Password: `123` (RI role)

### Key Dependencies
- **Avalonia 11.3.0**: Cross-platform UI framework
- **CommunityToolkit.Mvvm 8.2.1**: For ObservableObject and MVVM infrastructure
- **MessageBox.Avalonia 3.2.0**: Modal dialogs (see `VentanaCierreOrden` async dialogs)

## Project-Specific Conventions

### Naming & Language
- **Spanish Domain Terms**: Models use Spanish names (`OrdenDeInspeccion`, `EstacionSismologica`, `ResponsableInspeccion`)
- **View Naming**: Windows use Spanish (`VentanaCierreOrden` = Order Closure Window)
- **Estado Literals**: State names are string literals in Spanish ("Completamente Realizada", "Fuera de Servicio")

### Code Patterns
1. **Boundary Interaction**: Controllers (ViewModels) call boundary methods like `boundary.MostrarMensaje()`, `boundary.PedirConfirmacion()` (async)
2. **Data Transfer Objects**: Use `DatosOrdenInspeccion` for UI-safe data projection from domain models
3. **Validation Methods**: Static validation methods in controllers (e.g., `ValidarDatosCierre()`)
4. **Debug Tracing**: Extensive `Debug.WriteLine()` statements in `GestorCierreOrdenInspeccion` for workflow debugging
5. **LINQ Query Pattern**: Use collection expressions `[..]` with LINQ for filtering/projecting data
6. **Encapsulation**: Models use private setters and expose behavior through methods, not property manipulation
7. **Intention-Revealing Names**: Methods like `EsCompletamenteRealizada()`, `PonerSismografoEnFueraDeServicio()` clearly express intent

### Window Management
- **LoginWindow**: Modal workflow blocks app until closed; `IsLoginSuccessful` property controls navigation
- **MainWindow**: Maximized by default (`WindowState="Maximized"`)
- **Dynamic Sizing**: `VentanaCierreOrden` sizes to 80% of screen working area on initialization
- **Session Controls**: Logout button recreates `LoginWindow` and resets `SesionManager`

### UI Patterns
- **DataGrid**: Used extensively in `VentanaCierreOrden.axaml` for displaying inspection orders
- **Simple Theme**: Project uses Avalonia's `SimpleTheme` (not Fluent) with DataGrid styles
- **Event Handlers**: Code-behind event handlers (not Commands) for button clicks
- **Async Dialogs**: MessageBox.Avalonia for confirmations (`await MessageBoxManager.GetMessageBoxStandard(...).ShowAsync()`)

## Common Tasks

### Adding New Estado (State)
1. Add to `estados` list in `Program.InicializarDatosDePrueba()` (later: database seeding)
2. Add behavior method to `Estado` class following State Pattern (e.g., `EsNuevoEstado()`)
3. Update relevant controllers that filter by state
4. Follow OOP principles: encapsulate state-specific behavior in the `Estado` class

### Adding New View/Controller
1. Create Controller in `ViewModels/` inheriting `ViewModelBase` (despite the name, it's a controller)
2. Create `.axaml` and `.axaml.cs` in `Views/` with matching name
3. ViewLocator automatically resolves based on naming convention
4. If boundary pattern needed, pass View instance to controller constructor
5. Apply separation of concerns: keep UI logic in View, business logic in Controller, data in Models

### Preparing for PostgreSQL Migration
1. Keep domain models database-agnostic (no entity framework attributes yet)
2. Use DTOs for data transfer between layers
3. Design repositories/mappers to translate between domain objects and database entities
4. Consider using interfaces for data access to maintain testability

### Email Notifications
- `GestorCierreOrdenInspeccion.EnviarEmails()` formats change notifications
- Currently logs to Debug output (not implemented mail sending)
- Email template includes: Sismografo ID, Estado, DateTime, formatted MotivoFueraServicio list

## Important Files
- **Program.cs**: Mock data initialization, app entry point
- **App.axaml.cs**: Authentication flow, DisableAvaloniaDataAnnotationValidation() called on startup
- **SesionManager.cs**: Global session state singleton
- **GestorCierreOrdenInspeccion.cs**: Core business logic for order closure workflow
- **VentanaCierreOrden.axaml.cs**: Main inspection order UI with multi-step async workflow

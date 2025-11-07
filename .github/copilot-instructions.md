# RedSismica - AI Coding Agent Instructions

## Project Overview
RedSismica is a seismic network monitoring application built with **Avalonia UI** on **.NET 9.0**. While Avalonia is an MVVM framework, **this project uses it as MVC** - ViewModels act as Controllers that orchestrate business logic and view interaction. The system manages inspection orders for seismic stations, tracks seismograph status changes, and handles notification workflows for inspection personnel.

## Architecture & Key Components

### Application Structure
- **Entry Point**: `Program.cs` initializes SQLite database and launches the Avalonia app
- **Authentication Flow**: `LoginWindow` → `MainWindow` (requires successful authentication)
- **Session Management**: Global `SesionManager` singleton maintains `SesionActual` throughout the app lifecycle
- **Data Layer**: SQLite database with Repository Pattern for data access (located at `RedSismica/Database/redsismica.db`)

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

## Database Architecture

### SQLite Persistence Layer
- **Database Location**: `RedSismica/Database/redsismica.db` (project root, not build output)
- **Schema Management**: SQL scripts in `Database/schema.sql` and `Database/seed.sql`
- **Initialization**: `DatabaseInitializer` automatically creates and seeds database on first run

### Repository Pattern Implementation
- **DataContext**: `RedSismicaDataContext.Create()` provides unified access to all repositories
- **Repositories**: 
  - `EstadoRepository` - Estado entities by ambito
  - `UsuarioRepository` - User authentication and management
  - `SismografoRepository` - Seismograph data with estado updates
  - `EstacionSismologicaRepository` - Stations with automatic sismografo loading
  - `OrdenDeInspeccionRepository` - Complex queries with relationship loading
- **Materialization**: Database rows → Domain objects (with automatic relationship loading)
- **Dematerialization**: Domain objects → Database persistence (via Update methods)

### Design Patterns Applied
- **Repository Pattern**: Encapsulates data access logic
- **Unit of Work**: `RedSismicaDataContext` coordinates multiple repository operations
- **Data Mapper**: Separates domain objects from database representation
- **State Pattern**: Already implemented in `Estado` class
- **Singleton Pattern**: `SesionManager` for global session state
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
1. **Repository Access**: Controllers create `RedSismicaDataContext.Create()` to access repositories
2. **Boundary Interaction**: Controllers (ViewModels) call boundary methods like `boundary.MostrarMensaje()`, `boundary.PedirConfirmacion()` (async)
3. **Data Transfer Objects**: Use `DatosOrdenInspeccion` for UI-safe data projection from domain models
4. **Validation Methods**: Static validation methods in controllers (e.g., `ValidarDatosCierre()`)
5. **Debug Tracing**: Extensive `Debug.WriteLine()` statements in `GestorCierreOrdenInspeccion` for workflow debugging
6. **Domain-First Updates**: Update domain objects first, then persist via repository (e.g., `orden.Cerrar()` then `context.Ordenes.Update()`)
7. **Encapsulation**: Models use private setters and expose behavior through methods, not property manipulation
8. **Intention-Revealing Names**: Methods like `EsCompletamenteRealizada()`, `PonerSismografoEnFueraDeServicio()` clearly express intent

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
1. Add to `Database/seed.sql` for initial data
2. Add behavior method to `Estado` class following State Pattern (e.g., `EsNuevoEstado()`)
3. Update relevant controllers that filter by state using `context.Estados.GetByNombreAndAmbito()`
4. Follow OOP principles: encapsulate state-specific behavior in the `Estado` class

### Adding New View/Controller
1. Create Controller in `ViewModels/` inheriting `ViewModelBase` (despite the name, it's a controller)
2. Create `RedSismicaDataContext` instance in controller for database access
3. Create `.axaml` and `.axaml.cs` in `Views/` with matching name
4. ViewLocator automatically resolves based on naming convention
5. If boundary pattern needed, pass View instance to controller constructor
6. Apply separation of concerns: keep UI logic in View, business logic in Controller, data in Models, persistence in Repositories

### Working with Database
1. **Loading Data**: `var context = RedSismicaDataContext.Create(); var usuarios = context.Usuarios.GetAll();`
2. **Authentication**: `var usuario = context.Usuarios.Authenticate(username, password);`
3. **Complex Queries**: Use repository-specific methods (e.g., `GetCompletamenteRealizadasByResponsable()`)
4. **Updating Data**: Update domain object first, then call repository Update method
5. **Database Location**: `RedSismica/Database/redsismica.db` (can be opened with SQLite browser tools)

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

Do not create readme or documentation files unless explicitly instructed.
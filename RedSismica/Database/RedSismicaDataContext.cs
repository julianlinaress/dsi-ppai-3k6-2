using RedSismica.Database.Repositories;

namespace RedSismica.Database;

/// <summary>
/// Unified data context providing access to all repositories.
/// Acts as the main entry point for database operations.
/// Follows the Unit of Work pattern.
/// </summary>
public class RedSismicaDataContext
{
    private readonly string _connectionString;

    // Repository instances (lazy-loaded)
    private EstadoRepository? _estadoRepository;
    private UsuarioRepository? _usuarioRepository;
    private SismografoRepository? _sismografoRepository;
    private EstacionSismologicaRepository? _estacionRepository;
    private OrdenDeInspeccionRepository? _ordenRepository;

    public RedSismicaDataContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Gets the Estado repository
    /// </summary>
    public EstadoRepository Estados
    {
        get
        {
            _estadoRepository ??= new EstadoRepository(_connectionString);
            return _estadoRepository;
        }
    }

    /// <summary>
    /// Gets the Usuario repository
    /// </summary>
    public UsuarioRepository Usuarios
    {
        get
        {
            _usuarioRepository ??= new UsuarioRepository(_connectionString);
            return _usuarioRepository;
        }
    }

    /// <summary>
    /// Gets the Sismografo repository
    /// </summary>
    public SismografoRepository Sismografos
    {
        get
        {
            _sismografoRepository ??= new SismografoRepository(_connectionString);
            return _sismografoRepository;
        }
    }

    /// <summary>
    /// Gets the EstacionSismologica repository
    /// </summary>
    public EstacionSismologicaRepository Estaciones
    {
        get
        {
            _estacionRepository ??= new EstacionSismologicaRepository(_connectionString);
            return _estacionRepository;
        }
    }

    /// <summary>
    /// Gets the OrdenDeInspeccion repository
    /// </summary>
    public OrdenDeInspeccionRepository Ordenes
    {
        get
        {
            _ordenRepository ??= new OrdenDeInspeccionRepository(_connectionString);
            return _ordenRepository;
        }
    }

    /// <summary>
    /// Creates a new data context instance with the default connection string
    /// </summary>
    public static RedSismicaDataContext Create()
    {
        return new RedSismicaDataContext(DatabaseInitializer.ConnectionString);
    }
}

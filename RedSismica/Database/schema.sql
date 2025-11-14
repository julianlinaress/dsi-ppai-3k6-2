-- ============================================================================
-- Drop tables if they exist (for clean re-creation)
-- ============================================================================

DROP TABLE IF EXISTS CambioEstadoMotivoFueraServicio;
DROP TABLE IF EXISTS MotivoFueraServicio;
DROP TABLE IF EXISTS MotivoTipo;
DROP TABLE IF EXISTS CambioEstado;
DROP TABLE IF EXISTS OrdenDeInspeccion;
DROP TABLE IF EXISTS EstacionSismologica;
DROP TABLE IF EXISTS Sismografo;
DROP TABLE IF EXISTS Usuario;
DROP TABLE IF EXISTS Empleado;
DROP TABLE IF EXISTS Rol;
DROP TABLE IF EXISTS EstadoSismografo;
DROP TABLE IF EXISTS EstadoOrden;

-- ============================================================================
-- Core Catalog Tables
-- ============================================================================

-- Estados para Ordenes de Inspección
CREATE TABLE EstadoOrden (
    EstadoOrdenId INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre VARCHAR(100) NOT NULL UNIQUE
);

-- Estados para Sismógrafos
CREATE TABLE EstadoSismografo (
    EstadoSismografoId INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre VARCHAR(100) NOT NULL UNIQUE
);

-- Roles del sistema
CREATE TABLE Rol (
    RolId INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre VARCHAR(100) NOT NULL UNIQUE,
    DescripcionRol TEXT
);

-- ============================================================================
-- Personnel Tables
-- ============================================================================

-- Empleados
CREATE TABLE Empleado (
    EmpleadoId INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Telefono VARCHAR(20),
    Mail VARCHAR(255) NOT NULL,
    RolId INTEGER NOT NULL,
    FOREIGN KEY (RolId) REFERENCES Rol(RolId),
    UNIQUE(Mail)
);

-- Usuarios del sistema (credentials)
CREATE TABLE Usuario (
    UsuarioId INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre VARCHAR(100) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL, -- In production, this should be hashed
    EsRi BOOLEAN NOT NULL DEFAULT 0,
    EmpleadoId INTEGER,
    FOREIGN KEY (EmpleadoId) REFERENCES Empleado(EmpleadoId)
);

-- ============================================================================
-- Seismic Equipment Tables
-- ============================================================================

-- Sismógrafos
-- EstadoSismografoId is kept synchronized with the active CambioEstado record (FechaHoraFin IS NULL)
-- This provides efficient querying while maintaining state history in CambioEstado
CREATE TABLE Sismografo (
    SismografoId INTEGER PRIMARY KEY AUTOINCREMENT,
    IdentificadorSismografo INTEGER NOT NULL UNIQUE,
    Nombre VARCHAR(200) NOT NULL,
    EstadoSismografoId INTEGER NOT NULL,
    FOREIGN KEY (EstadoSismografoId) REFERENCES EstadoSismografo(EstadoSismografoId)
);

-- Estaciones Sismológicas
CREATE TABLE EstacionSismologica (
    EstacionId INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre VARCHAR(200) NOT NULL UNIQUE,
    SismografoId INTEGER NOT NULL UNIQUE, -- One-to-one relationship
    FOREIGN KEY (SismografoId) REFERENCES Sismografo(SismografoId)
);

-- ============================================================================
-- State Change Tracking
-- ============================================================================

-- Tipos de Motivos para poner fuera de servicio
CREATE TABLE MotivoTipo (
    MotivoTipoId INTEGER PRIMARY KEY AUTOINCREMENT,
    Descripcion VARCHAR(200) NOT NULL UNIQUE
);

-- Cambios de Estado del Sismógrafo
CREATE TABLE CambioEstado (
    CambioEstadoId INTEGER PRIMARY KEY AUTOINCREMENT,
    SismografoId INTEGER NOT NULL,
    EstadoSismografoId INTEGER NOT NULL,
    FechaHoraInicio DATETIME NOT NULL,
    FechaHoraFin DATETIME,
    FOREIGN KEY (SismografoId) REFERENCES Sismografo(SismografoId),
    FOREIGN KEY (EstadoSismografoId) REFERENCES EstadoSismografo(EstadoSismografoId),
    CHECK (FechaHoraFin IS NULL OR FechaHoraFin >= FechaHoraInicio)
);

-- Motivos Fuera de Servicio (asociados a cambios de estado)
CREATE TABLE MotivoFueraServicio (
    MotivoFueraServicioId INTEGER PRIMARY KEY AUTOINCREMENT,
    CambioEstadoId INTEGER NOT NULL,
    MotivoTipoId INTEGER NOT NULL,
    Comentario TEXT,
    FOREIGN KEY (CambioEstadoId) REFERENCES CambioEstado(CambioEstadoId),
    FOREIGN KEY (MotivoTipoId) REFERENCES MotivoTipo(MotivoTipoId)
);

-- ============================================================================
-- Inspection Orders
-- ============================================================================

-- Órdenes de Inspección
CREATE TABLE OrdenDeInspeccion (
    OrdenId INTEGER PRIMARY KEY AUTOINCREMENT,
    NumeroOrden INTEGER NOT NULL UNIQUE,
    FechaFinalizacion DATETIME NOT NULL,
    FechaHoraCierre DATETIME,
    ResponsableInspeccionId INTEGER NOT NULL,
    EstadoOrdenId INTEGER NOT NULL,
    EstacionId INTEGER NOT NULL,
    FOREIGN KEY (ResponsableInspeccionId) REFERENCES Usuario(UsuarioId),
    FOREIGN KEY (EstadoOrdenId) REFERENCES EstadoOrden(EstadoOrdenId),
    FOREIGN KEY (EstacionId) REFERENCES EstacionSismologica(EstacionId)
);

-- ============================================================================
-- Indexes for Performance
-- ============================================================================

CREATE INDEX idx_usuario_nombre ON Usuario(Nombre);
CREATE INDEX idx_empleado_mail ON Empleado(Mail);
CREATE INDEX idx_cambio_estado_sismografo ON CambioEstado(SismografoId);
CREATE INDEX idx_cambio_estado_actual ON CambioEstado(SismografoId, FechaHoraFin) 
    WHERE FechaHoraFin IS NULL;
CREATE INDEX idx_orden_responsable ON OrdenDeInspeccion(ResponsableInspeccionId);
CREATE INDEX idx_orden_estado ON OrdenDeInspeccion(EstadoOrdenId);
CREATE INDEX idx_orden_numero ON OrdenDeInspeccion(NumeroOrden);
CREATE INDEX idx_sismografo_identificador ON Sismografo(IdentificadorSismografo);

-- ============================================================================
-- End of Schema
-- ============================================================================

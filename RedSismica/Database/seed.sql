-- Red Sísmica Database Seed Data
-- SQLite Initial Data Population
-- Author: AI Assistant
-- Date: 2025-11-06

-- ============================================================================
-- Seed Estados (States)
-- ============================================================================

INSERT INTO Estado (Nombre, Ambito) VALUES
    ('Completamente Realizada', 'Orden de Inspección'),
    ('En Proceso', 'Orden de Inspección'),
    ('Algun Estado', 'Orden de Inspección'),
    ('Estado Inicial', 'Orden de Inspección'),
    ('Cerrada', 'Orden de Inspección'),
    ('Fuera de Servicio', 'Sismografo'),
    ('En Servicio', 'Sismografo');

-- ============================================================================
-- Seed Roles
-- ============================================================================

INSERT INTO Rol (Nombre, DescripcionRol) VALUES
    ('Responsable de Inspección', 'Responsable de realizar y gestionar inspecciones de estaciones sismológicas'),
    ('Otro', 'Otros roles del sistema');

-- ============================================================================
-- Seed Empleados
-- ============================================================================

INSERT INTO Empleado (Nombre, Apellido, Telefono, Mail, RolId) VALUES
    ('Julian', 'Linares', '12345678', 'julian@linares.com.ar', 1);
    -- Uncomment these when needed:
    -- ('Mauro', 'Bastasini', '12345678', 'maurobastasiniprof@gmail.com', 1),
    -- ('Inés', 'Haefeli', '12345678', 'ineshaefeli@gmail.com', 1),
    -- ('Huenu', 'Capdevila', '12345678', 'huecap7@gmail.com', 1);

-- ============================================================================
-- Seed Usuarios (Credentials)
-- NOTE: In production, passwords should be hashed!
-- ============================================================================

INSERT INTO Usuario (Nombre, Password, EsRi, EmpleadoId) VALUES
    ('jlinares', '123', 1, 1),
    ('mperez', '123', 1, NULL),
    ('cgomez', '123', 1, NULL);

-- ============================================================================
-- Seed Sismógrafos
-- Starting ID counter at 18122021 as per original code
-- ============================================================================

INSERT INTO Sismografo (IdentificadorSismografo, Nombre, EstadoId) VALUES
    (18122021, 'Sismógrafo A123', 7), -- En Servicio
    (18122022, 'Sismógrafo B456', 7); -- En Servicio

-- ============================================================================
-- Seed Estaciones Sismológicas
-- ============================================================================

INSERT INTO EstacionSismologica (Nombre, SismografoId) VALUES
    ('Estación Norte', 1),
    ('Estación Sur', 2);

-- ============================================================================
-- Seed Órdenes de Inspección
-- Using date calculations relative to current date
-- ============================================================================

INSERT INTO OrdenDeInspeccion (NumeroOrden, FechaFinalizacion, FechaHoraCierre, ResponsableInspeccionId, EstadoId, EstacionId) VALUES
    (1, datetime('now', '-5 days'), NULL, 1, 1, 1),  -- jlinares, Completamente Realizada
    (2, datetime('now', '-7 days'), NULL, 1, 1, 1),  -- jlinares, Completamente Realizada
    (3, datetime('now', '-12 days'), NULL, 2, 1, 1), -- mperez, Completamente Realizada
    (4, datetime('now', '-23 days'), NULL, 1, 1, 1), -- jlinares, Completamente Realizada
    (5, datetime('now', '-54 days'), NULL, 2, 1, 1), -- mperez, Completamente Realizada
    (6, datetime('now', '-2 days'), NULL, 2, 1, 1),  -- mperez, Completamente Realizada
    (7, datetime('now', '-3 days'), NULL, 1, 1, 1),  -- jlinares, Completamente Realizada
    (8, datetime('now', '-3 days'), NULL, 2, 1, 2),  -- mperez, Completamente Realizada, Estación Sur
    (9, datetime('now', '-1 days'), NULL, 2, 2, 1);  -- mperez, En Proceso

-- ============================================================================
-- Seed Tipos de Motivos
-- ============================================================================

INSERT INTO MotivoTipo (Descripcion) VALUES
    ('Reparacion'),
    ('Renovacion'),
    ('Cambio de Sismografo'),
    ('Otro');

-- ============================================================================
-- Verify Data
-- ============================================================================

-- Uncomment to verify inserted data
-- SELECT 'Estados' as Tabla, COUNT(*) as Cantidad FROM Estado
-- UNION ALL
-- SELECT 'Roles', COUNT(*) FROM Rol
-- UNION ALL
-- SELECT 'Empleados', COUNT(*) FROM Empleado
-- UNION ALL
-- SELECT 'Usuarios', COUNT(*) FROM Usuario
-- UNION ALL
-- SELECT 'Sismografos', COUNT(*) FROM Sismografo
-- UNION ALL
-- SELECT 'Estaciones', COUNT(*) FROM EstacionSismologica
-- UNION ALL
-- SELECT 'Ordenes', COUNT(*) FROM OrdenDeInspeccion
-- UNION ALL
-- SELECT 'MotivoTipos', COUNT(*) FROM MotivoTipo;

-- ============================================================================
-- End of Seed Data
-- ============================================================================

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
    ('Inhabilitado', 'Sismografo');

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
-- EstadoId reflects current state and is kept synchronized with active CambioEstado
-- EstadoId 6 = Fuera de Servicio, EstadoId 7 = Inhabilitado
-- ============================================================================

INSERT INTO Sismografo (IdentificadorSismografo, Nombre, EstadoId) VALUES
    (18122021, 'Sismógrafo A123', 7),  -- Inhabilitado (matches CambioEstado)
    (18122022, 'Sismógrafo B456', 6),  -- Fuera de Servicio (matches CambioEstado)
    (18122023, 'Sismógrafo C789', 7),  -- Inhabilitado (matches CambioEstado)
    (18122024, 'Sismógrafo D012', 7),  -- Inhabilitado (matches CambioEstado)
    (18122025, 'Sismógrafo E345', 6),  -- Fuera de Servicio (matches CambioEstado)
    (18122026, 'Sismógrafo F678', 7);  -- Inhabilitado (matches CambioEstado)

-- ============================================================================
-- Seed Estaciones Sismológicas
-- MUST be created BEFORE CambioEstado because OrdenDeInspeccion needs EstacionId
-- ============================================================================

INSERT INTO EstacionSismologica (Nombre, SismografoId) VALUES
    ('Estación Norte', 1),
    ('Estación Sur', 2),
    ('Estación Este', 3),
    ('Estación Oeste', 4),
    ('Estación Centro', 5),
    ('Estación Patagonia', 6);

-- ============================================================================
-- Seed Tipos de Motivos
-- MUST be created BEFORE MotivoFueraServicio
-- ============================================================================

INSERT INTO MotivoTipo (Descripcion) VALUES
    ('Reparacion'),
    ('Renovacion'),
    ('Cambio de Sismografo'),
    ('Otro');

-- ============================================================================
-- Seed Cambios de Estado (State Change History)
-- ============================================================================

-- Sismógrafo A123 - Historial: Inhabilitado -> Fuera de Servicio -> Inhabilitado
INSERT INTO CambioEstado (FechaHoraInicio, FechaHoraFin, SismografoId, EstadoId) VALUES
    (datetime('now', '-120 days'), datetime('now', '-60 days'), 1, 7), -- Inhabilitado
    (datetime('now', '-60 days'), datetime('now', '-30 days'), 1, 6),  -- Fuera de Servicio
    (datetime('now', '-30 days'), NULL, 1, 7);                          -- Inhabilitado (actual)

-- Sismógrafo B456 - Historial: Inhabilitado -> Fuera de Servicio
INSERT INTO CambioEstado (FechaHoraInicio, FechaHoraFin, SismografoId, EstadoId) VALUES
    (datetime('now', '-90 days'), datetime('now', '-15 days'), 2, 7),  -- Inhabilitado
    (datetime('now', '-15 days'), NULL, 2, 6);                          -- Fuera de Servicio (actual)

-- Sismógrafo C789 - Historial: Solo Inhabilitado
INSERT INTO CambioEstado (FechaHoraInicio, FechaHoraFin, SismografoId, EstadoId) VALUES
    (datetime('now', '-180 days'), NULL, 3, 7);                         -- Inhabilitado (actual)

-- Sismógrafo D012 - Historial: Inhabilitado todo el tiempo
INSERT INTO CambioEstado (FechaHoraInicio, FechaHoraFin, SismografoId, EstadoId) VALUES
    (datetime('now', '-200 days'), NULL, 4, 7);                         -- Inhabilitado (actual)

-- Sismógrafo E345 - Historial: Inhabilitado -> Fuera de Servicio -> Inhabilitado -> Fuera de Servicio
INSERT INTO CambioEstado (FechaHoraInicio, FechaHoraFin, SismografoId, EstadoId) VALUES
    (datetime('now', '-150 days'), datetime('now', '-100 days'), 5, 7), -- Inhabilitado
    (datetime('now', '-100 days'), datetime('now', '-50 days'), 5, 6),  -- Fuera de Servicio
    (datetime('now', '-50 days'), datetime('now', '-10 days'), 5, 7),   -- Inhabilitado
    (datetime('now', '-10 days'), NULL, 5, 6);                           -- Fuera de Servicio (actual)

-- Sismógrafo F678 - Historial: Inhabilitado desde el inicio
INSERT INTO CambioEstado (FechaHoraInicio, FechaHoraFin, SismografoId, EstadoId) VALUES
    (datetime('now', '-365 days'), NULL, 6, 7);                         -- Inhabilitado (actual)

-- ============================================================================
-- Seed Motivos Fuera de Servicio
-- ============================================================================

-- Motivos para el segundo cambio de estado de A123 (Fuera de Servicio)
INSERT INTO MotivoFueraServicio (Comentario, MotivoTipoId, CambioEstadoId) VALUES
    ('Falla en sensor principal', 1, 2),
    ('Requiere calibración urgente', 4, 2);

-- Motivos para el estado actual de B456 (Fuera de Servicio)
INSERT INTO MotivoFueraServicio (Comentario, MotivoTipoId, CambioEstadoId) VALUES
    ('Actualización de firmware', 2, 4),
    ('Reemplazo de componentes', 1, 4);

-- Motivos para el segundo cambio de E345 (primer Fuera de Servicio)
INSERT INTO MotivoFueraServicio (Comentario, MotivoTipoId, CambioEstadoId) VALUES
    ('Mantenimiento preventivo', 1, 6);

-- Motivos para el cuarto cambio de E345 (segundo Fuera de Servicio, actual)
INSERT INTO MotivoFueraServicio (Comentario, MotivoTipoId, CambioEstadoId) VALUES
    ('Daño por condiciones climáticas', 1, 8),
    ('Cambio completo de equipo', 3, 8);

-- ============================================================================
-- Seed Órdenes de Inspección
-- Using date calculations relative to current date
-- ============================================================================

INSERT INTO OrdenDeInspeccion (NumeroOrden, FechaFinalizacion, FechaHoraCierre, ResponsableInspeccionId, EstadoId, EstacionId) VALUES
    (1, datetime('now', '-5 days'), NULL, 1, 1, 1),  -- jlinares, Completamente Realizada, Estación Norte
    (3, datetime('now', '-12 days'), NULL, 2, 1, 3), -- mperez, Completamente Realizada, Estación Este
    (4, datetime('now', '-23 days'), NULL, 1, 1, 4), -- jlinares, Completamente Realizada, Estación Oeste
    (5, datetime('now', '-54 days'), NULL, 2, 1, 5), -- mperez, Completamente Realizada, Estación Centro
    (6, datetime('now', '-2 days'), NULL, 2, 1, 6),  -- mperez, Completamente Realizada, Estación Patagonia
    (8, datetime('now', '-3 days'), NULL, 2, 1, 2);  -- mperez, Completamente Realizada, Estación Sur

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

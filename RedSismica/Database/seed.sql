-- Red Sísmica Database Seed Data
-- SQLite Initial Data Population
-- Author: AI Assistant
-- Date: 2025-11-06

-- ============================================================================
-- Seed Estados (States)
-- ============================================================================

INSERT INTO EstadoOrden (Nombre) VALUES
    ('Completamente Realizada'),
    ('En Proceso'),
    ('Algun Estado'),
    ('Estado Inicial'),
    ('Cerrada');

INSERT INTO EstadoSismografo (Nombre) VALUES
    ('Fuera de Servicio'),
    ('Inhabilitado'),
    ('Disponible'),
    ('En Instalación'),
    ('Reclamado'),
    ('En Línea');

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
    ('mperez', '123', 0, NULL),
    ('cgomez', '123', 1, NULL);

-- ============================================================================
-- Seed Sismógrafos
-- Starting ID counter at 18122021 as per original code
-- EstadoSismografoId reflects current state and is kept synchronized with active CambioEstado
-- They reference entries inserted in EstadoSismografo
-- ============================================================================

INSERT INTO Sismografo (IdentificadorSismografo, Nombre, EstadoSismografoId) VALUES
    (18122021, 'Sismógrafo A123', (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Inhabilitado')),
    (18122022, 'Sismógrafo B456', (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Fuera de Servicio')),
    (18122023, 'Sismógrafo C789', (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Inhabilitado')),
    (18122024, 'Sismógrafo D012', (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Inhabilitado')),
    (18122025, 'Sismógrafo E345', (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Fuera de Servicio')),
    (18122026, 'Sismógrafo F678', (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Inhabilitado'));

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
INSERT INTO CambioEstado (FechaHoraInicio, FechaHoraFin, SismografoId, EstadoSismografoId) VALUES
    (datetime('now', '-120 days'), datetime('now', '-60 days'), 1, (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Inhabilitado')),
    (datetime('now', '-60 days'), datetime('now', '-30 days'), 1, (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Fuera de Servicio')),
    (datetime('now', '-30 days'), NULL, 1, (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Inhabilitado'));

-- Sismógrafo B456 - Historial: Inhabilitado -> Fuera de Servicio
INSERT INTO CambioEstado (FechaHoraInicio, FechaHoraFin, SismografoId, EstadoSismografoId) VALUES
    (datetime('now', '-90 days'), datetime('now', '-15 days'), 2, (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Inhabilitado')),
    (datetime('now', '-15 days'), NULL, 2, (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Fuera de Servicio'));

-- Sismógrafo C789 - Historial: Solo Inhabilitado
INSERT INTO CambioEstado (FechaHoraInicio, FechaHoraFin, SismografoId, EstadoSismografoId) VALUES
    (datetime('now', '-180 days'), NULL, 3, (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Inhabilitado'));

-- Sismógrafo D012 - Historial: Inhabilitado todo el tiempo
INSERT INTO CambioEstado (FechaHoraInicio, FechaHoraFin, SismografoId, EstadoSismografoId) VALUES
    (datetime('now', '-200 days'), NULL, 4, (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Inhabilitado'));

-- Sismógrafo E345 - Historial: Inhabilitado -> Fuera de Servicio -> Inhabilitado -> Fuera de Servicio
INSERT INTO CambioEstado (FechaHoraInicio, FechaHoraFin, SismografoId, EstadoSismografoId) VALUES
    (datetime('now', '-150 days'), datetime('now', '-100 days'), 5, (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Inhabilitado')),
    (datetime('now', '-100 days'), datetime('now', '-50 days'), 5, (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Fuera de Servicio')),
    (datetime('now', '-50 days'), datetime('now', '-10 days'), 5, (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Inhabilitado')),
    (datetime('now', '-10 days'), NULL, 5, (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Fuera de Servicio'));

-- Sismógrafo F678 - Historial: Inhabilitado desde el inicio
INSERT INTO CambioEstado (FechaHoraInicio, FechaHoraFin, SismografoId, EstadoSismografoId) VALUES
    (datetime('now', '-365 days'), NULL, 6, (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Inhabilitado'));

-- ============================================================================
-- Seed Motivos Fuera de Servicio
-- ============================================================================

-- Motivos para el cambio de A123 (Fuera de Servicio)
INSERT INTO MotivoFueraServicio (Comentario, MotivoTipoId, CambioEstadoId)
SELECT 'Falla en sensor principal',
       (SELECT MotivoTipoId FROM MotivoTipo WHERE Descripcion = 'Reparacion'),
       CambioEstadoId
FROM CambioEstado
WHERE SismografoId = 1
  AND EstadoSismografoId = (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Fuera de Servicio')
  AND FechaHoraInicio = datetime('now', '-60 days');

INSERT INTO MotivoFueraServicio (Comentario, MotivoTipoId, CambioEstadoId)
SELECT 'Requiere calibración urgente',
       (SELECT MotivoTipoId FROM MotivoTipo WHERE Descripcion = 'Otro'),
       CambioEstadoId
FROM CambioEstado
WHERE SismografoId = 1
  AND EstadoSismografoId = (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Fuera de Servicio')
  AND FechaHoraInicio = datetime('now', '-60 days');

-- Motivos para el estado actual de B456 (Fuera de Servicio)
INSERT INTO MotivoFueraServicio (Comentario, MotivoTipoId, CambioEstadoId)
SELECT 'Actualización de firmware',
       (SELECT MotivoTipoId FROM MotivoTipo WHERE Descripcion = 'Renovacion'),
       CambioEstadoId
FROM CambioEstado
WHERE SismografoId = 2
  AND EstadoSismografoId = (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Fuera de Servicio')
  AND FechaHoraInicio = datetime('now', '-15 days');

INSERT INTO MotivoFueraServicio (Comentario, MotivoTipoId, CambioEstadoId)
SELECT 'Reemplazo de componentes',
       (SELECT MotivoTipoId FROM MotivoTipo WHERE Descripcion = 'Reparacion'),
       CambioEstadoId
FROM CambioEstado
WHERE SismografoId = 2
  AND EstadoSismografoId = (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Fuera de Servicio')
  AND FechaHoraInicio = datetime('now', '-15 days');

-- Motivos para el primer Fuera de Servicio de E345
INSERT INTO MotivoFueraServicio (Comentario, MotivoTipoId, CambioEstadoId)
SELECT 'Mantenimiento preventivo',
       (SELECT MotivoTipoId FROM MotivoTipo WHERE Descripcion = 'Reparacion'),
       CambioEstadoId
FROM CambioEstado
WHERE SismografoId = 5
  AND EstadoSismografoId = (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Fuera de Servicio')
  AND FechaHoraInicio = datetime('now', '-100 days');

-- Motivos para el Fuera de Servicio actual de E345
INSERT INTO MotivoFueraServicio (Comentario, MotivoTipoId, CambioEstadoId)
SELECT 'Daño por condiciones climáticas',
       (SELECT MotivoTipoId FROM MotivoTipo WHERE Descripcion = 'Reparacion'),
       CambioEstadoId
FROM CambioEstado
WHERE SismografoId = 5
  AND EstadoSismografoId = (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Fuera de Servicio')
  AND FechaHoraInicio = datetime('now', '-10 days');

INSERT INTO MotivoFueraServicio (Comentario, MotivoTipoId, CambioEstadoId)
SELECT 'Cambio completo de equipo',
       (SELECT MotivoTipoId FROM MotivoTipo WHERE Descripcion = 'Cambio de Sismografo'),
       CambioEstadoId
FROM CambioEstado
WHERE SismografoId = 5
  AND EstadoSismografoId = (SELECT EstadoSismografoId FROM EstadoSismografo WHERE Nombre = 'Fuera de Servicio')
  AND FechaHoraInicio = datetime('now', '-10 days');

-- ============================================================================
-- Seed Órdenes de Inspección
-- Using date calculations relative to current date
-- ============================================================================

INSERT INTO OrdenDeInspeccion (NumeroOrden, FechaFinalizacion, FechaHoraCierre, ResponsableInspeccionId, EstadoOrdenId, EstacionId) VALUES
    (1, datetime('now', '-5 days'), NULL, 1, (SELECT EstadoOrdenId FROM EstadoOrden WHERE Nombre = 'Completamente Realizada'), 1),
    (3, datetime('now', '-12 days'), NULL, 2, (SELECT EstadoOrdenId FROM EstadoOrden WHERE Nombre = 'Completamente Realizada'), 3),
    (4, datetime('now', '-23 days'), NULL, 1, (SELECT EstadoOrdenId FROM EstadoOrden WHERE Nombre = 'Completamente Realizada'), 4),
    (5, datetime('now', '-54 days'), NULL, 2, (SELECT EstadoOrdenId FROM EstadoOrden WHERE Nombre = 'Completamente Realizada'), 5),
    (6, datetime('now', '-2 days'), NULL, 2, (SELECT EstadoOrdenId FROM EstadoOrden WHERE Nombre = 'Completamente Realizada'), 6),
    (8, datetime('now', '-3 days'), NULL, 2, (SELECT EstadoOrdenId FROM EstadoOrden WHERE Nombre = 'Completamente Realizada'), 2);

-- ============================================================================
-- Verify Data
-- ============================================================================

-- Uncomment to verify inserted data
-- SELECT 'EstadosOrden' as Tabla, COUNT(*) as Cantidad FROM EstadoOrden
-- UNION ALL
-- SELECT 'EstadosSismografo', COUNT(*) FROM EstadoSismografo
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

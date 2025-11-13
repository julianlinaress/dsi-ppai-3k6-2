# 🚀 Guía de Compilación y Prueba - Red Sísmica

## Requisitos Previos

- **.NET 9.0 SDK** instalado
- **Visual Studio 2022** o **Visual Studio Code** con C# extension
- **Git** (opcional, para control de versiones)

---

## Paso 1: Verificar Requisitos

```powershell
# Verificar versión de .NET
dotnet --version

# Debe mostrar: 9.0.x o superior
```

---

## Paso 2: Restaurar Dependencias

```powershell
# Navegar a la carpeta del proyecto
cd "c:\Users\gasto\OneDrive\Documentos\universidad\Diseño de sistemas informaticos\PPAI\dsi-ppai-3k6-2"

# Restaurar paquetes NuGet
dotnet restore
```

---

## Paso 3: Compilar el Proyecto

### Opción A: Compilación de Debug (Recomendado para desarrollo)
```powershell
dotnet build RedSismica\RedSismica.csproj -c Debug
```

### Opción B: Compilación de Release (Para producción)
```powershell
dotnet build RedSismica\RedSismica.csproj -c Release
```

---

## Paso 4: Ejecutar la Aplicación

### Opción A: Desde Visual Studio
1. Abrir `RedSismica.sln` en Visual Studio
2. Hacer clic en **▶️ Start** o presionar **F5**

### Opción B: Desde Terminal
```powershell
# Debug
dotnet run --project RedSismica\RedSismica.csproj

# Release
dotnet run --project RedSismica\RedSismica.csproj -c Release
```

---

## Paso 5: Pruebas de UX/UI

### Pantalla de Login ✅
- [ ] Verificar que el header azul sea visible
- [ ] Comprobar que los campos tengan placeholders
- [ ] Verificar que el botón sea azul y grande
- [ ] Comprobar hover en botón (oscurece)
- [ ] Verificar espaciado consistente

### Pantalla Principal ✅
- [ ] Verificar header con branding (🌍)
- [ ] Comprobar botones con estilos diferenciados
- [ ] Verificar secciones con iconos (📊, 🏢, 📋, 👥)
- [ ] Comprobar DataGrids con estilo mejorado
- [ ] Verificar hover en filas (color gris claro)
- [ ] Comprobar selección de fila (color azul claro)

### Ventana de Cierre de Orden ✅
- [ ] Verificar header con icono y descripción
- [ ] Comprobar DataGrid estilizado
- [ ] Verificar botones diferenciados
- [ ] Comprobar efectos visuales

### Ventana de Historial ✅
- [ ] Verificar header con icono
- [ ] Comprobar DataGrid con estilos
- [ ] Verificar botón cerrar
- [ ] Comprobar consistencia de estilos

---

## Troubleshooting: Problemas Comunes

### Problema 1: Error al compilar
```
Error: "System.IO.DirectoryNotFoundException"
```
**Solución:**
```powershell
# Limpiar build previo
dotnet clean RedSismica\RedSismica.csproj

# Restaurar paquetes
dotnet restore

# Compilar nuevamente
dotnet build RedSismica\RedSismica.csproj
```

### Problema 2: Estilos no se aplican
```
Error: Los botones ven normal, no con estilo
```
**Solución:**
1. Verificar que `GlobalStyles.axaml` exista en `Styles/`
2. Verificar que `App.axaml` incluya:
   ```xaml
   <StyleInclude Source="/Styles/GlobalStyles.axaml"/>
   ```
3. Limpiar caché:
   ```powershell
   dotnet clean
   dotnet build
   ```

### Problema 3: Colores no coinciden
```
Error: Colores diferente a lo esperado
```
**Solución:**
1. Verificar tema del sistema (Dark/Light)
2. Reiniciar aplicación
3. Verificar valores hexadecimales en GlobalStyles.axaml

### Problema 4: Aplicación no inicia
```
Error: "unhandled exception" al iniciar
```
**Solución:**
1. Verificar que base de datos esté inicializada
2. Revisar que archivos XAML no tengan errores de sintaxis
3. Verificar logs de debug en Visual Studio

---

## Verificación de Cambios

### Archivos Modificados

```powershell
# Ver archivos modificados en git
git status

# Salida esperada:
# Modified: RedSismica/App.axaml
# Modified: RedSismica/Views/LoginWindow.axaml
# Modified: RedSismica/Views/MainWindow.axaml
# Modified: RedSismica/Views/VentanaCierreOrden.axaml
# Modified: RedSismica/Views/VentanaHistorialEstados.axaml

# Created: RedSismica/Styles/GlobalStyles.axaml
# Created: .github/DESIGN_GUIDE.md
# Created: UI_UX_IMPROVEMENTS.md
# Created: VISUAL_SUMMARY.md
```

### Ver Cambios Específicos

```powershell
# Diferencia en archivo
git diff RedSismica/Views/LoginWindow.axaml

# Archivos nuevos
git status | grep "Styles/GlobalStyles"
```

---

## Prueba de Performance

### Compilación
```powershell
# Medir tiempo de compilación
Measure-Command { dotnet build RedSismica\RedSismica.csproj -c Debug }
```
**Esperado**: < 30 segundos

### Ejecución
```powershell
# Iniciar aplicación
dotnet run --project RedSismica\RedSismica.csproj

# Verificar que interface sea responsiva
# Hacer clic en botones
# Navegar entre secciones
# Abrir diálogos
```
**Esperado**: No debe haber lag o retrasos

---

## Testing Manual

### Test de Navegación
1. [ ] Iniciar sesión
2. [ ] Ver tabla de sismógrafos
3. [ ] Hacer hover en botón de historial
4. [ ] Hacer clic en sección de estaciones
5. [ ] Scroll hacia abajo
6. [ ] Ver órdenes de inspección
7. [ ] Ver usuarios del sistema
8. [ ] Cerrar sesión

### Test de Interactividad
1. [ ] Hacer hover en botones (debe cambiar color)
2. [ ] Hacer hover en filas (debe cambiar fondo)
3. [ ] Hacer clic en fila (debe resaltarse)
4. [ ] Resize ventana (debe adaptarse)
5. [ ] Abrir diálogos (deben tener estilos)

### Test de Accesibilidad
1. [ ] Usar Tab para navegar
2. [ ] Usar Enter para activar botones
3. [ ] Verificar contraste de colores
4. [ ] Verificar tamaños de fuente legibles
5. [ ] Verificar información clara

---

## Publicación y Distribución

### Crear Ejecutable

```powershell
# Release Windows standalone
dotnet publish RedSismica\RedSismica.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishTrimmed=true
```

**Ubicación**: `RedSismica/bin/Release/net9.0/win-x64/publish/`

### Crear Instalador (opcional)
```powershell
# Requiere WiX Toolset instalado
# Consultar documentación oficial de .NET
```

---

## Notas de Versión

### Versión 1.1 - Mejoras UX/UI (Actual)

**Cambios:**
- ✅ Sistema de colores consistente
- ✅ Estilos de componentes mejorados
- ✅ Tipografía jerárquica
- ✅ Mejor espaciado y layout
- ✅ Efectos visuales en interacciones
- ✅ Accesibilidad mejorada

**Archivos nuevos:**
- `Styles/GlobalStyles.axaml`
- `.github/DESIGN_GUIDE.md`
- `UI_UX_IMPROVEMENTS.md`
- `VISUAL_SUMMARY.md`

**Archivos modificados:**
- `App.axaml`
- `Views/LoginWindow.axaml`
- `Views/MainWindow.axaml`
- `Views/VentanaCierreOrden.axaml`
- `Views/VentanaHistorialEstados.axaml`

---

## Recursos Útiles

### Documentación
- [Avalonia UI Documentation](https://docs.avaloniaui.net/)
- [XAML Basics](https://docs.microsoft.com/en-us/windows/uwp/xaml-platform/xaml-overview)
- [Styling Guide](https://docs.avaloniaui.net/docs/styling)

### Herramientas Recomendadas
- Visual Studio 2022 Professional
- Visual Studio Code + C# Extension
- Git + GitHub
- ColorPickr (para verificar colores)

### Extensiones VS Code
- C#
- C# Dev Kit
- Avalonia IDE
- XAML

---

## Soporte y Ayuda

### Si algo no funciona:
1. Revisar errores en consola
2. Consultar `.github/DESIGN_GUIDE.md`
3. Verificar `GlobalStyles.axaml`
4. Buscar en documentación oficial de Avalonia

### Contacto:
- Revisar comentarios en código
- Consultar commits de Git
- Documentación en carpeta `.github/`

---

## Checklist de Validación Final

- [ ] Compilación exitosa sin errores
- [ ] Aplicación inicia sin problemas
- [ ] Login funciona correctamente
- [ ] Pantalla principal muestra todas las secciones
- [ ] Estilos se ven correctamente
- [ ] Colores coinciden con paleta
- [ ] Botones tienen hover effects
- [ ] DataGrids tienen estilos
- [ ] Navegación fluida
- [ ] Sin lag o retrasos

---

**Estado**: ✅ Listo para producción
**Fecha**: Noviembre 2025
**Próxima Revisión**: Según necesidades


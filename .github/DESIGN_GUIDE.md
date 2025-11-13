# Estándares y Guía de Diseño - Red Sísmica

## Filosofía de Diseño

Esta guía establece los estándares visuales y de UX para mantener la consistencia en el proyecto Red Sísmica.

---

## 1. Paleta de Colores

### Colores Principales
```
Primario:         #2563EB (Azul)
Primario Oscuro:  #1D4ED8 (Azul oscuro)
Primario Claro:   #DBEAFE (Azul claro)
Secundario:       #10B981 (Verde)
Peligro:          #EF4444 (Rojo)
Advertencia:      #F59E0B (Naranja)
```

### Colores Neutrales
```
Texto Principal:      #1F2937 (Gris oscuro)
Texto Secundario:     #6B7280 (Gris medio)
Fondo Primario:       #FFFFFF (Blanco)
Fondo Secundario:     #F9FAFB (Gris muy claro)
Bordes:              #E5E7EB (Gris claro)
Deshabilitado:       #D1D5DB (Gris)
```

### Uso de Colores
- **Primario**: Acciones principales, botones destacados
- **Secundario**: Acciones positivas, confirmación
- **Peligro**: Acciones destructivas, errores
- **Advertencia**: Alertas, información importante
- **Neutrales**: Fondos, textos, bordes

---

## 2. Tipografía

### Escala Tipográfica
```
Títulos de Página:        28px, Bold
Títulos de Sección:       18px, SemiBold
Subtítulos:               16px, Medium
Texto Base:               14px, Regular
Etiquetas/Labels:         13px, Medium
Texto Pequeño:            12px, Regular
```

### Familias de Fuentes
- Usar la fuente por defecto del sistema (Avalonia)
- Evitar fuentes muy decorativas
- Mantener consistencia en toda la aplicación

---

## 3. Espaciado

### Unidades de Espaciado
```
XXS: 4px
XS:  8px
S:   12px
M:   16px
L:   20px
XL:  32px
XXL: 40px
```

### Reglas de Espaciado
- Márgenes entre secciones: `L` (20px) o `XL` (32px)
- Padding dentro de componentes: `S` (12px) o `M` (16px)
- Espaciado entre elementos: `S` (12px)
- Espaciado dentro de formularios: `XS` (8px) entre label e input

---

## 4. Componentes

### Botones

#### Primarios
```xaml
<Button Classes="Primary" Content="Guardar" />
```
- Fondo: #2563EB
- Texto: Blanco
- Padding: 12px horizontal, 8px vertical
- FontSize: 14px, SemiBold
- CornerRadius: 6px
- Hover: #1D4ED8

#### Secundarios
```xaml
<Button Classes="Secondary" Content="Cancelar" />
```
- Fondo: #F9FAFB
- Texto: #1F2937
- Borde: 1px #E5E7EB
- Padding: 12px, 8px
- FontSize: 14px, SemiBold
- Hover: #E5E7EB

#### Peligro
```xaml
<Button Classes="Danger" Content="Eliminar" />
```
- Fondo: #EF4444
- Texto: Blanco
- Padding: 12px, 8px
- FontSize: 14px, SemiBold
- Hover: #DC2626

### TextBox
```xaml
<TextBox Watermark="Ingrese..." />
```
- Padding: 10px
- BorderThickness: 1px
- BorderBrush: #E5E7EB
- CornerRadius: 6px
- Focus: BorderThickness 2px, BorderBrush #2563EB

### DataGrid
- BorderThickness: 1px
- BorderBrush: #E5E7EB
- GridLinesVisibility: Horizontal
- CornerRadius: 6px

#### Estilos de Fila
- Normal: Fondo transparente
- Hover: Fondo #F3F4F6
- Selected: Fondo #DBEAFE, Texto #1F2937

#### Encabezados
- Fondo: #F3F4F6
- Texto: #1F2937
- FontWeight: SemiBold
- FontSize: 13px

---

## 5. Layouts

### Header de Ventana
```xaml
<Border Background="White" BorderBrush="#E5E7EB" BorderThickness="0,0,0,1" Padding="20">
    <Grid ColumnDefinitions="*,Auto" Height="70">
        <!-- Contenido -->
    </Grid>
</Border>
```

### Contenedor de Contenido
```xaml
<ScrollViewer VerticalScrollBarVisibility="Auto">
    <Grid Margin="20">
        <!-- Secciones con StackPanel -->
    </Grid>
</ScrollViewer>
```

### Sección
```xaml
<StackPanel Spacing="12" Margin="0,0,0,16">
    <TextBlock Text="Título" Classes="SectionTitle" />
    <!-- Contenido -->
</StackPanel>
```

### Grupo de Botones
```xaml
<StackPanel Orientation="Horizontal" Spacing="12">
    <Button Classes="Primary" Content="Confirmar" />
    <Button Classes="Secondary" Content="Cancelar" />
</StackPanel>
```

---

## 6. Responsiveness

### Tamaños Mínimos de Ventanas
- Login: 420px ancho, content-height
- Principal: 1024px ancho, 768px alto (recomendado)
- Diálogos: 800px-950px ancho, 500px-650px alto

### Consideraciones
- Usar `*` en Grid ColumnDefinitions para columnas flexibles
- Siempre incluir ScrollViewer en contenido variable
- Mantener márgenes mínimos (20px en bordes)

---

## 7. Estados y Feedback

### Estados de Componentes
```
Normal:       Fondo base, texto base
Hover:        Fondo más claro, cursor hand
Focus:        Borde primario más visible
Active:       Fondo primario claro
Disabled:     Opacidad 50%, color gris
```

### Indicadores de Estado
- **Éxito**: Verde (#10B981)
- **Error**: Rojo (#EF4444)
- **Advertencia**: Naranja (#F59E0B)
- **Información**: Azul (#2563EB)

---

## 8. Iconografía

### Convenciones Emoji
Se usan emojis para:
- 🌍 Branding principal
- 📊 Datos/Sismógrafos
- 🏢 Infraestructura/Estaciones
- 📋 Documentos/Órdenes
- 👥 Usuarios
- 📜 Historial
- ✓ Confirmar
- ❌ Cancelar
- 🔄 Actualizar
- ➕ Agregar
- 🚪 Logout

### Recomendación Futura
Considerar usar FontAwesome o Material Icons para mejor consistencia y profesionalismo.

---

## 9. Accesibilidad

### Contraste
- Relación de contraste mínimo 4.5:1 para textos pequeños
- Verificar con herramientas como WebAIM Contrast Checker

### Tamaños
- Texto mínimo: 12px
- Botones mínimos: 42px altura
- Espacios clickeables: mínimo 44x44px

### Navegación
- Tab order lógico
- Labels claros en todos los inputs
- Mensajes de error descriptivos

---

## 10. Guía de Implementación

### Paso 1: Importar Estilos
```xaml
<Application.Styles>
    <SimpleTheme/>
    <StyleInclude Source="avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml"/>
    <StyleInclude Source="/Styles/GlobalStyles.axaml"/>
</Application.Styles>
```

### Paso 2: Usar Estilos Predefinidos
```xaml
<!-- Botón primario -->
<Button Classes="Primary" Content="Acción" />

<!-- TextBlock sección -->
<TextBlock Classes="SectionTitle" Text="Sección" />

<!-- DataGrid -->
<DataGrid Background="White" BorderBrush="#E5E7EB" BorderThickness="1" />
```

### Paso 3: Mantener Consistencia
- Siempre usar colores de la paleta
- No hardcodear colores
- Respetar espaciados definidos
- Seguir jerarquía tipográfica

---

## 11. Checklist de Nuevos Componentes

Al agregar nuevos elementos, verificar:

- [ ] ¿Se usa color de la paleta?
- [ ] ¿Tiene espaciado consistente?
- [ ] ¿Sigue jerarquía tipográfica?
- [ ] ¿Tiene estados visuales (hover, focus)?
- [ ] ¿Es accesible?
- [ ] ¿Es consistente con el resto?
- [ ] ¿Tiene margin/padding adecuado?
- [ ] ¿Se entiende claramente su función?

---

## 12. Ejemplos de Componentes

### Formulario Completo
```xaml
<StackPanel Spacing="20">
    <!-- Campo 1 -->
    <StackPanel Spacing="8">
        <TextBlock Text="Usuario" Classes="Label" />
        <TextBox Watermark="Ingrese usuario" />
    </StackPanel>
    
    <!-- Campo 2 -->
    <StackPanel Spacing="8">
        <TextBlock Text="Contraseña" Classes="Label" />
        <TextBox Watermark="Ingrese contraseña" PasswordChar="*" />
    </StackPanel>
    
    <!-- Botones -->
    <StackPanel Orientation="Horizontal" Spacing="12" Margin="0,8,0,0">
        <Button Classes="Primary" Content="Guardar" />
        <Button Classes="Secondary" Content="Cancelar" />
    </StackPanel>
</StackPanel>
```

### Sección de Datos
```xaml
<StackPanel Spacing="12">
    <TextBlock Text="📊 Datos" Classes="SectionTitle" />
    <DataGrid Name="DataGrid"
              AutoGenerateColumns="False"
              IsReadOnly="True"
              Background="White"
              BorderBrush="#E5E7EB"
              BorderThickness="1"
              CornerRadius="6"
              Height="200">
        <!-- Columns -->
    </DataGrid>
</StackPanel>
```

---

## 13. Troubleshooting Común

### Problema: Colores no se aplican
**Solución**: Verificar que GlobalStyles.axaml esté incluido en App.axaml

### Problema: Estilos no son consistentes
**Solución**: Usar clases predefinidas (Primary, Secondary, etc.)

### Problema: TextBox no tiene focus visible
**Solución**: Agregar BorderThickness "2" en el estilo focus

### Problema: DataGrid filas muy pequeñas
**Solución**: Aumentar Height del DataGrid o usar RowHeight

---

## 14. Contacto y Soporte

Para preguntas o mejoras:
1. Revisar este documento
2. Revisar UI_UX_IMPROVEMENTS.md
3. Consultar GlobalStyles.axaml

---

**Última actualización**: Noviembre 2025
**Versión**: 1.0


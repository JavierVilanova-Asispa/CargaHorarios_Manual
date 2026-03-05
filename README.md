# Cargador Horario Semanal - ASP.NET Core 8

Aplicación web desarrollada en C# .NET 8 para la gestión de carga de horarios semanales.

## Características

- **Interfaz similar a aplicación de escritorio Windows clásica**
- **Tipos de carga**: Coordinadora, Distrito, o Todos
- **Calendario mensual interactivo** con navegación entre meses
- **Parámetros configurables**: año, número de semana, días, etc.
- **Conexión a base de datos SQL Server**
- **Listo para implementar la lógica de generación**

## Requisitos

- Visual Studio 2022 o superior
- .NET 8 SDK
- SQL Server (para base de datos)

## Estructura del Proyecto

```
CargadorHorario.Web/
├── Controllers/
│   └── HomeController.cs          # Controlador principal
├── Models/
│   └── CargaHorarioViewModel.cs   # Modelo de vista
├── Services/
│   ├── ICargaHorarioService.cs    # Interfaz del servicio
│   └── CargaHorarioService.cs     # Lógica de negocio
├── Data/
│   ├── IDataService.cs            # Interfaz de acceso a datos
│   └── DataService.cs             # Implementación BD
├── Views/
│   ├── Home/
│   │   └── Index.cshtml           # Vista principal
│   └── Shared/
│       └── _Layout.cshtml         # Layout con estilos
├── Program.cs                      # Configuración
├── appsettings.json                # Configuración producción
└── appsettings.Development.json    # Configuración desarrollo
```

## Configuración

### 1. Cadena de Conexión

Edita `appsettings.json` o `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=TU_BD;User Id=USUARIO;Password=PASSWORD;TrustServerCertificate=True;"
  }
}
```

### 2. Configurar Financiaciones

En `CargaHorarioService.cs`, método `ObtenerFinanciacionesAsync()`:

```csharp
var consulta = @"
    SELECT CodigoFinanciacion AS codigo, 
           NombreFinanciacion AS nombre 
    FROM Financiaciones
    ORDER BY NombreFinanciacion";
```

Reemplaza con tu consulta SQL real.

## Implementar la Lógica de Generación

El método `GenerarCargaHorarioAsync` en `CargaHorarioService.cs` está preparado para que implementes tu lógica:

```csharp
public async Task<string> GenerarCargaHorarioAsync(CargaHorarioViewModel model)
{
    // Aquí tienes acceso a todos los parámetros:
    // - model.TipoCarga (Coordinadora, Distrito, Todos)
    // - model.FinanciacionSeleccionada
    // - model.CodPrograma
    // - model.Anio
    // - model.Mes
    // - model.NumSemanaGeneral
    // - model.PrimerDiaSemanaGeneral
    // - model.NumSemanasAGenerar
    // - model.FechaDia
    // - model.ExpAuxiliar
    // - model.ActualizarTablaDiaSemana
    // - model.ProyeccionSeparada
    
    // TODO: Implementa tu lógica aquí
    // Ejemplo:
    var parametros = new Dictionary<string, object>
    {
        { "@TipoCarga", (int)model.TipoCarga },
        { "@Financiacion", model.FinanciacionSeleccionada ?? string.Empty },
        { "@CodPrograma", model.CodPrograma ?? string.Empty },
        { "@Anio", model.Anio },
        { "@Mes", model.Mes }
        // ... más parámetros
    };
    
    var resultado = await _dataService.EjecutarProcedimientoAsync(
        "sp_GenerarCargaHorario", 
        parametros
    );
    
    return "Carga generada exitosamente";
}
```

## Ejecutar la Aplicación

### Desde Visual Studio 2022

1. Abre `CargadorHorario.Web.sln`
2. Configura la cadena de conexión
3. Presiona F5

### Desde línea de comandos

```bash
cd [ruta-del-proyecto]
dotnet restore
dotnet run
```

La aplicación estará disponible en: `https://localhost:5001`

## Características de la Interfaz

### Calendario Interactivo

- **Navegación**: Botones ◄ y ► para cambiar de mes
- **Día actual**: Resaltado en amarillo
- **Selección**: Click en cualquier día para seleccionarlo
- **Visualización**: Formato de calendario mensual estándar

### Tipo de Carga

- **COORDINADORA**: Carga por coordinadora específica
- **DISTRITO**: Carga por distrito
- **TODOS**: Carga general

### Parámetros Disponibles

1. **FINANCIACIÓN**: Desplegable con financiaciones de BD
2. **CodPrograma**: Código de programa libre
3. **Año**: Año de la carga
4. **NumSemana general**: Número de semana
5. **1er día semana general**: Primer día
6. **Nº semanas a generar**: Cantidad de semanas (mínimo 1)
7. **Día**: Fecha específica en formato dd/mm/aaaa
8. **ExpAuxiliar**: Expresión auxiliar
9. **Actualizar tabla TBL_DiaSemana**: Checkbox
10. **Proyección separada**: Checkbox

### Botones

- **GENERAR**: Ejecuta la carga (llama a `GenerarCargaHorarioAsync`)
- **SALIR**: Cierra o recarga la aplicación

## Acceso a Base de Datos

El servicio `DataService` proporciona 4 métodos:

### 1. Ejecutar Consulta (SELECT)
```csharp
var consulta = "SELECT * FROM Tabla WHERE Campo = @Valor";
var parametros = new Dictionary<string, object> { { "@Valor", valor } };
var resultado = await _dataService.EjecutarConsultaAsync(consulta, parametros);
```

### 2. Ejecutar Procedimiento Almacenado
```csharp
var parametros = new Dictionary<string, object>
{
    { "@Param1", valor1 },
    { "@Param2", valor2 }
};
var resultado = await _dataService.EjecutarProcedimientoAsync("sp_Nombre", parametros);
```

### 3. Ejecutar Comando (INSERT, UPDATE, DELETE)
```csharp
var comando = "UPDATE Tabla SET Campo = @Valor WHERE Id = @Id";
var parametros = new Dictionary<string, object>
{
    { "@Valor", nuevoValor },
    { "@Id", id }
};
var filasAfectadas = await _dataService.EjecutarComandoAsync(comando, parametros);
```

### 4. Ejecutar Escalar (COUNT, MAX, etc.)
```csharp
var consulta = "SELECT COUNT(*) FROM Tabla";
var total = await _dataService.EjecutarEscalarAsync(consulta);
```

## Logs

La aplicación registra automáticamente:
- Inicio y fin de generación de carga
- Parámetros utilizados
- Errores que ocurran
- Consultas ejecutadas

Los logs aparecen en la consola durante el desarrollo.

## Personalización

### Cambiar el Mensaje Inicial

En `HomeController.cs`, método `Index()`:

```csharp
Mensaje = "Tu mensaje personalizado aquí"
```

### Modificar Estilos

Los estilos están en `Views/Shared/_Layout.cshtml` dentro de la etiqueta `<style>`.

Para cambiar colores, tamaños, etc., edita las clases CSS correspondientes.

### Agregar Validaciones

En el controlador, antes de llamar al servicio:

```csharp
if (string.IsNullOrWhiteSpace(model.CodPrograma))
{
    model.Mensaje = "El código de programa es obligatorio";
    model.EsError = true;
    return View("Index", model);
}
```

## Notas Importantes

1. **La lógica de generación está pendiente de implementar** en `GenerarCargaHorarioAsync()`
2. **Configura tu cadena de conexión** antes de ejecutar
3. **La tabla de Financiaciones** debe existir o usa datos de ejemplo
4. **Todos los parámetros del formulario** están disponibles en el modelo

## Soporte

Para problemas o dudas:
1. Revisa los logs en la consola
2. Verifica la cadena de conexión
3. Asegúrate de que las tablas existen en la BD

## Tecnologías

- ASP.NET Core 8
- C# 12
- Razor Pages
- Microsoft.Data.SqlClient 5.2.0
- CSS3 (estilo Windows clásico)

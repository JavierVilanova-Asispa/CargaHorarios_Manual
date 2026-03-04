# Sistema de Carga de Datos - ASP.NET Core 8

Aplicación web desarrollada en C# .NET 8 para gestionar la carga de datos con diferentes opciones.

## Características

- **Carga Total**: Ejecuta una carga completa de datos
- **Carga por Distrito**: Permite seleccionar un distrito específico para la carga
- **Carga por Coordinadora**: Permite seleccionar una coordinadora específica
- **Tarea Programada**: Ejecuta automáticamente un método cada 7 horas

## Requisitos

- Visual Studio 2022 o superior
- .NET 8 SDK
- Windows, Linux o macOS

## Estructura del Proyecto

```
CargaDatos.Web/
├── Controllers/
│   └── HomeController.cs          # Controlador principal
├── Models/
│   └── CargaDatosViewModel.cs     # Modelo de vista
├── Services/
│   ├── ICargaDatosService.cs      # Interfaz del servicio
│   ├── CargaDatosService.cs       # Implementación del servicio
│   └── CargaDatosProgramadaService.cs  # Servicio en segundo plano (cada 7 horas)
├── Views/
│   ├── Home/
│   │   └── Index.cshtml           # Vista principal
│   └── Shared/
│       └── _Layout.cshtml         # Layout
├── Program.cs                      # Configuración de la aplicación
└── CargaDatos.Web.csproj          # Archivo de proyecto
```

## Cómo usar

### Abrir en Visual Studio 2022

1. Abre Visual Studio 2022
2. Selecciona "Abrir un proyecto o solución"
3. Navega a la carpeta del proyecto y selecciona `CargaDatos.Web.csproj`
4. Presiona F5 para ejecutar la aplicación

### Ejecutar desde línea de comandos

```bash
cd [ruta-del-proyecto]
dotnet run
```

La aplicación se ejecutará en: `https://localhost:5001` o `http://localhost:5000`

## Funcionalidades

### 1. Interfaz Web

La interfaz permite:
- Seleccionar el tipo de carga mediante radio buttons
- Mostrar desplegables dinámicos según la selección
- Ejecutar la carga con un solo clic
- Ver mensajes de éxito o error

### 2. Servicio de Carga

El servicio `CargaDatosService` implementa tres métodos:
- `CargarDatosTotal()`: Para carga completa
- `CargarDatosPorDistrito(distrito)`: Para carga por distrito
- `CargarDatosPorCoordinadora(coordinadora)`: Para carga por coordinadora

**IMPORTANTE**: Los métodos están marcados con `// TODO:` para que implementes tu lógica específica.

### 3. Tarea Programada

El servicio `CargaDatosProgramadaService` ejecuta automáticamente el método `EjecutarCargaProgramada()` cada 7 horas.

**IMPORTANTE**: El método está vacío y marcado con `// TODO:` para que implementes tu lógica.

## Personalización

### Cambiar los Distritos

Edita el método `ObtenerDistritos()` en `CargaDatosService.cs`:

```csharp
public List<string> ObtenerDistritos()
{
    return new List<string>
    {
        "Tu Distrito 1",
        "Tu Distrito 2",
        // ...
    };
}
```

### Cambiar las Coordinadoras

Edita el método `ObtenerCoordinadoras()` en `CargaDatosService.cs`:

```csharp
public List<string> ObtenerCoordinadoras()
{
    return new List<string>
    {
        "Tu Coordinadora 1",
        "Tu Coordinadora 2",
        // ...
    };
}
```

### Implementar la Lógica de Carga

Busca los comentarios `// TODO:` en `CargaDatosService.cs` e implementa tu lógica:

```csharp
public async Task<string> CargarDatosTotal()
{
    // TODO: Implementar lógica de carga total
    // Ejemplo:
    // - Conectar a base de datos
    // - Ejecutar procedimientos almacenados
    // - Procesar archivos
    // - etc.
}
```

### Implementar la Tarea Programada

Edita el método `EjecutarCargaProgramada()` en `CargaDatosProgramadaService.cs`:

```csharp
private async Task EjecutarCargaProgramada()
{
    // TODO: Implementar tu lógica aquí
    // Este método se ejecuta automáticamente cada 7 horas
}
```

### Cambiar el Intervalo de Ejecución

Si deseas cambiar las 7 horas a otro intervalo, edita la línea en `CargaDatosProgramadaService.cs`:

```csharp
private readonly TimeSpan _intervalo = TimeSpan.FromHours(7); // Cambiar aquí
```

## Logs

La aplicación registra logs automáticamente:
- En desarrollo: Se muestran en la consola
- En producción: Se pueden configurar diferentes destinos (archivo, base de datos, etc.)

Los logs incluyen:
- Inicio y fin de cada carga
- Ejecución de la tarea programada
- Errores que puedan ocurrir

## Tecnologías Utilizadas

- ASP.NET Core 8
- Bootstrap 5.3
- Font Awesome 6.4
- Razor Pages
- Dependency Injection
- Background Services (Hosted Services)

## Notas

- La tarea programada se inicia automáticamente al ejecutar la aplicación
- Los logs se muestran en la consola durante el desarrollo
- La interfaz es totalmente responsive y funciona en dispositivos móviles
- Los desplegables se actualizan dinámicamente según la selección

## Soporte

Para cualquier duda o problema, revisa los logs de la aplicación o contacta al equipo de desarrollo.

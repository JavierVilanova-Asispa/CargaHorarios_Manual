# SOLUCIÓN DE PROBLEMAS - ICargaDatosService no definido

Si obtienes el error "ICargaDatosService no está definido", sigue estos pasos:

## Opción 1: Verificar la estructura de carpetas

Asegúrate de que tienes esta estructura EXACTA:

```
CargadorHorario.Web/
├── CargadorHorario.Web.sln
├── CargadorHorario.Web.csproj
├── Program.cs
├── GlobalUsings.cs
├── appsettings.json
├── appsettings.Development.json
├── Controllers/
│   └── HomeController.cs
├── Models/
│   └── CargaDatosViewModel.cs
├── Services/
│   ├── ICargaDatosService.cs          ← ESTE ARCHIVO DEBE EXISTIR
│   ├── CargaDatosService.cs
│   └── CargaDatosProgramadaService.cs
└── Views/
    ├── _ViewImports.cshtml
    ├── _ViewStart.cshtml
    ├── Home/
    │   └── Index.cshtml
    └── Shared/
        └── _Layout.cshtml
```

## Opción 2: Limpiar y Reconstruir en Visual Studio

1. Cierra Visual Studio completamente
2. Elimina las carpetas `bin` y `obj` si existen
3. Abre Visual Studio 2022
4. Abre el archivo `CargadorHorario.Web.sln`
5. Ve a `Build` → `Clean Solution`
6. Ve a `Build` → `Rebuild Solution`

## Opción 3: Desde la línea de comandos

Abre una terminal en la carpeta del proyecto y ejecuta:

```bash
# Limpiar
dotnet clean

# Restaurar paquetes
dotnet restore

# Compilar
dotnet build
```

Si hay errores, míralos detenidamente. El output te dirá exactamente qué archivo falta.

## Opción 4: Verificar que los archivos existen

En Visual Studio, en el Solution Explorer, deberías ver:

- ▼ CargadorHorario.Web
  - ▼ Services
    - ICargaDatosService.cs  ← ¿Lo ves aquí?
    - CargaDatosService.cs
    - CargaDatosProgramadaService.cs

Si NO ves la carpeta Services o los archivos dentro:
1. Click derecho en el proyecto → "Add" → "Existing Item"
2. Navega a la carpeta Services
3. Selecciona los tres archivos .cs
4. Click "Add"

## Opción 5: Verificar el contenido de ICargaDatosService.cs

Abre el archivo `Services/ICargaDatosService.cs` y verifica que contenga:

```csharp
namespace CargadorHorario.Web.Services
{
    public interface ICargaDatosService
    {
        Task<string> CargarDatosTotal();
        Task<string> CargarDatosPorDistrito(string distrito);
        Task<string> CargarDatosPorCoordinadora(string coordinadora);
        List<string> ObtenerDistritos();
        List<string> ObtenerCoordinadoras();
    }
}
```

## Opción 6: Recargar el proyecto

En Visual Studio:
1. Click derecho en el proyecto "CargadorHorario.Web"
2. Selecciona "Unload Project"
3. Click derecho nuevamente
4. Selecciona "Reload Project"

## Si NADA funciona

Descarga nuevamente todos los archivos y asegúrate de:
1. Extraer TODO el contenido manteniendo la estructura de carpetas
2. No renombrar ningún archivo
3. Abrir el archivo .sln (NO el .csproj directamente)

## Verificación rápida desde la línea de comandos

```bash
# Ir a la carpeta del proyecto
cd ruta/a/CargadorHorario.Web

# Verificar que el archivo existe
dir Services\ICargaDatosService.cs    # En Windows
ls Services/ICargaDatosService.cs      # En Linux/Mac

# Compilar para ver errores específicos
dotnet build
```

El comando `dotnet build` te dirá EXACTAMENTE qué falta o qué está mal.

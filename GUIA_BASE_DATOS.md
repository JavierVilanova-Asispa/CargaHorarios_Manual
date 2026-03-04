# Guía de Uso - Acceso a Base de Datos

## Configuración de la Cadena de Conexión

### 1. appsettings.json (Producción)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=TU_BASE_DATOS;User Id=TU_USUARIO;Password=TU_PASSWORD;TrustServerCertificate=True;"
  }
}
```

### 2. appsettings.Development.json (Desarrollo)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CargaDatos;User Id=sa;Password=TuPassword123;TrustServerCertificate=True;"
  }
}
```

**IMPORTANTE:** Reemplaza los valores según tu configuración:
- `TU_SERVIDOR`: Nombre o IP de tu servidor SQL Server
- `TU_BASE_DATOS`: Nombre de tu base de datos
- `TU_USUARIO`: Usuario de SQL Server
- `TU_PASSWORD`: Contraseña del usuario

### Formatos de Cadena de Conexión

#### Autenticación de SQL Server
```
Server=localhost;Database=MiDB;User Id=sa;Password=MiPassword;TrustServerCertificate=True;
```

#### Autenticación de Windows
```
Server=localhost;Database=MiDB;Integrated Security=True;TrustServerCertificate=True;
```

#### Azure SQL Database
```
Server=tcp:miservidor.database.windows.net,1433;Initial Catalog=MiDB;Persist Security Info=False;User ID=miusuario;Password=mipassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

## Funciones Disponibles en DataService

El `DataService` proporciona 4 métodos genéricos para acceso a datos:

### 1. EjecutarConsultaAsync - SELECT
Ejecuta una consulta SELECT y devuelve un `DataTable`.

```csharp
// Ejemplo sin parámetros
var consulta = "SELECT * FROM Empleados";
var resultado = await _dataService.EjecutarConsultaAsync(consulta);

// Ejemplo con parámetros
var consultaConParametros = "SELECT * FROM Empleados WHERE Departamento = @Depto AND Activo = @Activo";
var parametros = new Dictionary<string, object>
{
    { "@Depto", "Ventas" },
    { "@Activo", true }
};
var resultado = await _dataService.EjecutarConsultaAsync(consultaConParametros, parametros);

// Acceder a los datos
foreach (DataRow fila in resultado.Rows)
{
    var nombre = fila["Nombre"].ToString();
    var salario = Convert.ToDecimal(fila["Salario"]);
    // ...
}
```

### 2. EjecutarProcedimientoAsync - Stored Procedure
Ejecuta un procedimiento almacenado y devuelve un `DataTable`.

```csharp
// Ejemplo básico
var nombreProcedimiento = "sp_ObtenerVentas";
var resultado = await _dataService.EjecutarProcedimientoAsync(nombreProcedimiento);

// Ejemplo con parámetros
var nombreProcedimiento = "sp_ObtenerVentasPorFecha";
var parametros = new Dictionary<string, object>
{
    { "@FechaInicio", new DateTime(2024, 1, 1) },
    { "@FechaFin", DateTime.Now },
    { "@Sucursal", "Madrid" }
};
var resultado = await _dataService.EjecutarProcedimientoAsync(nombreProcedimiento, parametros);
```

### 3. EjecutarComandoAsync - INSERT, UPDATE, DELETE
Ejecuta comandos de modificación y devuelve el número de filas afectadas.

```csharp
// INSERT
var insertComando = "INSERT INTO Clientes (Nombre, Email) VALUES (@Nombre, @Email)";
var parametrosInsert = new Dictionary<string, object>
{
    { "@Nombre", "Juan Pérez" },
    { "@Email", "juan@example.com" }
};
var filasInsertadas = await _dataService.EjecutarComandoAsync(insertComando, parametrosInsert);

// UPDATE
var updateComando = "UPDATE Productos SET Precio = @Precio WHERE Id = @Id";
var parametrosUpdate = new Dictionary<string, object>
{
    { "@Precio", 99.99 },
    { "@Id", 123 }
};
var filasActualizadas = await _dataService.EjecutarComandoAsync(updateComando, parametrosUpdate);

// DELETE
var deleteComando = "DELETE FROM Temporal WHERE Fecha < @Fecha";
var parametrosDelete = new Dictionary<string, object>
{
    { "@Fecha", DateTime.Now.AddDays(-30) }
};
var filasEliminadas = await _dataService.EjecutarComandoAsync(deleteComando, parametrosDelete);
```

### 4. EjecutarEscalarAsync - Valor Único
Ejecuta una consulta que devuelve un solo valor (primera columna de la primera fila).

```csharp
// Contar registros
var consultaCount = "SELECT COUNT(*) FROM Pedidos WHERE Estado = @Estado";
var parametros = new Dictionary<string, object>
{
    { "@Estado", "Pendiente" }
};
var totalPendientes = await _dataService.EjecutarEscalarAsync(consultaCount, parametros);
var total = Convert.ToInt32(totalPendientes);

// Obtener un máximo
var consultaMax = "SELECT MAX(Precio) FROM Productos";
var precioMaximo = await _dataService.EjecutarEscalarAsync(consultaMax);

// Verificar existencia
var consultaExiste = "SELECT COUNT(*) FROM Usuarios WHERE Email = @Email";
var parametrosEmail = new Dictionary<string, object>
{
    { "@Email", "usuario@example.com" }
};
var existe = Convert.ToInt32(await _dataService.EjecutarEscalarAsync(consultaExiste, parametrosEmail)) > 0;
```

## Ejemplos Completos de Uso

### Ejemplo 1: Cargar datos desde la base de datos
```csharp
public async Task<List<Distrito>> ObtenerDistritosDesdeDB()
{
    var consulta = "SELECT Id, Nombre, Codigo FROM Distritos WHERE Activo = 1 ORDER BY Nombre";
    var resultado = await _dataService.EjecutarConsultaAsync(consulta);
    
    var distritos = new List<Distrito>();
    foreach (DataRow fila in resultado.Rows)
    {
        distritos.Add(new Distrito
        {
            Id = Convert.ToInt32(fila["Id"]),
            Nombre = fila["Nombre"].ToString(),
            Codigo = fila["Codigo"].ToString()
        });
    }
    
    return distritos;
}
```

### Ejemplo 2: Ejecutar un procedimiento almacenado complejo
```csharp
public async Task<string> ProcesarCargaMasiva(int lote)
{
    var nombreProcedimiento = "sp_ProcesarCargaMasiva";
    var parametros = new Dictionary<string, object>
    {
        { "@LoteId", lote },
        { "@FechaProceso", DateTime.Now },
        { "@Usuario", "Sistema" },
        { "@ValidarDatos", true }
    };
    
    var resultado = await _dataService.EjecutarProcedimientoAsync(nombreProcedimiento, parametros);
    
    // El procedimiento puede devolver información de resultado
    if (resultado.Rows.Count > 0)
    {
        var registrosProcesados = Convert.ToInt32(resultado.Rows[0]["RegistrosProcesados"]);
        var errores = Convert.ToInt32(resultado.Rows[0]["Errores"]);
        
        return $"Procesados: {registrosProcesados}, Errores: {errores}";
    }
    
    return "Proceso completado";
}
```

### Ejemplo 3: Transacción (múltiples operaciones)
```csharp
public async Task<bool> GuardarCargaCompleta(CargaCompleta carga)
{
    try
    {
        // 1. Insertar cabecera
        var insertCabecera = @"
            INSERT INTO CargaCabecera (Fecha, Tipo, Usuario) 
            OUTPUT INSERTED.Id
            VALUES (@Fecha, @Tipo, @Usuario)";
        
        var parametrosCabecera = new Dictionary<string, object>
        {
            { "@Fecha", DateTime.Now },
            { "@Tipo", carga.Tipo },
            { "@Usuario", carga.Usuario }
        };
        
        var idCabecera = await _dataService.EjecutarEscalarAsync(insertCabecera, parametrosCabecera);
        
        // 2. Insertar detalles
        var insertDetalle = @"
            INSERT INTO CargaDetalle (CargaId, Item, Valor) 
            VALUES (@CargaId, @Item, @Valor)";
        
        foreach (var detalle in carga.Detalles)
        {
            var parametrosDetalle = new Dictionary<string, object>
            {
                { "@CargaId", idCabecera },
                { "@Item", detalle.Item },
                { "@Valor", detalle.Valor }
            };
            
            await _dataService.EjecutarComandoAsync(insertDetalle, parametrosDetalle);
        }
        
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al guardar carga completa");
        return false;
    }
}
```

### Ejemplo 4: Manejo de valores nulos
```csharp
var consulta = "SELECT Id, Nombre, Email, Telefono FROM Clientes WHERE Id = @Id";
var parametros = new Dictionary<string, object>
{
    { "@Id", clienteId }
};

var resultado = await _dataService.EjecutarConsultaAsync(consulta, parametros);

if (resultado.Rows.Count > 0)
{
    var fila = resultado.Rows[0];
    
    // Forma segura de manejar nulos
    var nombre = fila["Nombre"] != DBNull.Value ? fila["Nombre"].ToString() : string.Empty;
    var email = fila["Email"] != DBNull.Value ? fila["Email"].ToString() : null;
    var telefono = fila["Telefono"] != DBNull.Value ? fila["Telefono"].ToString() : "Sin teléfono";
}
```

## Mejores Prácticas

### 1. Siempre usar parámetros (previene SQL Injection)
```csharp
// ❌ MAL - Vulnerable a SQL Injection
var consulta = $"SELECT * FROM Usuarios WHERE Email = '{email}'";

// ✅ BIEN - Seguro
var consulta = "SELECT * FROM Usuarios WHERE Email = @Email";
var parametros = new Dictionary<string, object> { { "@Email", email } };
```

### 2. Manejar excepciones apropiadamente
```csharp
try
{
    var resultado = await _dataService.EjecutarConsultaAsync(consulta, parametros);
    // Procesar resultado
}
catch (SqlException ex)
{
    _logger.LogError(ex, "Error de SQL Server: {Mensaje}", ex.Message);
    throw new ApplicationException("Error al acceder a la base de datos", ex);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error inesperado");
    throw;
}
```

### 3. Validar datos antes de enviar
```csharp
if (string.IsNullOrWhiteSpace(distrito))
{
    throw new ArgumentException("El distrito no puede estar vacío", nameof(distrito));
}

var consulta = "SELECT * FROM Datos WHERE Distrito = @Distrito";
var parametros = new Dictionary<string, object> { { "@Distrito", distrito } };
```

### 4. Registrar operaciones importantes
```csharp
_logger.LogInformation("Ejecutando carga para distrito: {Distrito}", distrito);
var resultado = await _dataService.EjecutarProcedimientoAsync("sp_CargarDistrito", parametros);
_logger.LogInformation("Carga completada. Registros: {Count}", resultado.Rows.Count);
```

## Notas Importantes

1. **DataService usa `Scoped` lifetime** - Se crea una instancia por request HTTP
2. **Las conexiones se cierran automáticamente** gracias al `using`
3. **Los parámetros aceptan valores null** - se convierten automáticamente a `DBNull.Value`
4. **Todas las operaciones son asíncronas** - siempre usar `await`
5. **Los logs se registran automáticamente** en todas las operaciones

## Troubleshooting

### Error: "Cannot open database"
- Verifica la cadena de conexión
- Asegúrate de que el servidor SQL está ejecutándose
- Verifica que el usuario tiene permisos

### Error: "Login failed for user"
- Verifica usuario y contraseña en la cadena de conexión
- Asegúrate de que el usuario tiene acceso a la base de datos

### Error: "Invalid object name"
- Verifica que la tabla/procedimiento existe
- Verifica el esquema (dbo.NombreTabla)

### Error: "Must declare the scalar variable"
- Asegúrate de que todos los parámetros empiezan con @
- Verifica que los nombres de parámetros coinciden exactamente

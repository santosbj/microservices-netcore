# Microservicios Solution

Solución de microservicios desarrollada en .NET 9 que incluye servicios para consultar el precio del dólar y información de temperatura, con un API Gateway como punto de entrada principal.

## 🏗️ Arquitectura

### Servicios

1. **DolarService** - Puerto 5104 (HTTP) / 7214 (HTTPS)
   - Endpoint: `/api/dolar`
   - Función: Consultar el precio del dólar

2. **TemperaturaService** - Puerto 5050 (HTTP) / 7241 (HTTPS)
   - Endpoint: `/api/temperatura/{ciudad}`
   - Función: Consultar información de temperatura por ciudad

3. **GatewayService** - Puerto 5071 (HTTP) / 7189 (HTTPS)
   - **Función**: API Gateway con controladores manuales que exponen todos los endpoints
   - **Endpoints disponibles**:
     - `GET /api/Info` - Información general del Gateway (público)
     - `GET /api/Info/status` - Estado de los microservicios (requiere auth)
     - `GET /api/Dolar` - Consultar precio del dólar (proxy a DolarService)
     - `GET /api/Temperatura/{ciudad}` - Consultar temperatura de una ciudad
     - `GET /api/Temperatura/multiple/{ciudades}` - Consultar múltiples ciudades
   - **Documentación**: Swagger UI unificado con todos los endpoints

## 🚀 Cómo Ejecutar

### Opción 1: Usando VS Code (Recomendado)

1. **Ejecutar toda la solución:**
   - Abrir VS Code
   - Ir a Run and Debug (Ctrl+Shift+D)
   - Seleccionar "Microservicios Completos (HTTP)"
   - Presionar F5

2. **Ejecutar servicios individuales:**
   - Seleccionar la configuración del servicio específico
   - Presionar F5

3. **Modo desarrollo con hot reload:**
   - Ctrl+Shift+P → "Tasks: Run Task"
   - Seleccionar `watch-[ServiceName]`

### Opción 2: Línea de comandos

```bash
# Restaurar dependencias
dotnet restore

# Construir toda la solución
dotnet build

# Ejecutar servicios individualmente (en terminales separados)
cd DolarService && dotnet run
cd TemperaturaService && dotnet run  
cd GatewayService && dotnet run
```

## 📖 Documentación API

### Swagger UI

Cada servicio incluye documentación automática de Swagger:

- **DolarService**: http://localhost:5104
- **TemperaturaService**: http://localhost:5050  
- **GatewayService**: http://localhost:5071

### Health Checks y Monitoreo

Cada servicio incluye endpoints de health check y una interfaz visual para monitoreo:

#### **Health Checks Endpoints**
- **DolarService**: http://localhost:5104/health
- **TemperaturaService**: http://localhost:5050/health
- **GatewayService**: http://localhost:5071/health

#### **Health Checks UI (Interfaz Visual)**
- **DolarService**: http://localhost:5104/healthchecks-ui
- **TemperaturaService**: http://localhost:5050/healthchecks-ui  
- **GatewayService**: http://localhost:5071/healthchecks-ui *(Monitorea todos los servicios)*

La UI de health checks proporciona:
- ✅ **Monitoreo en tiempo real** de todos los servicios
- 📊 **Historial de estado** con gráficos
- 🔴🟡🟢 **Indicadores visuales** de salud
- ⏱️ **Tiempo de respuesta** de cada servicio
- 📈 **Estadísticas** de disponibilidad

## 🔐 Autenticación

La solución utiliza JWT (JSON Web Tokens) para autenticación:

- Todos los endpoints están protegidos con autenticación JWT
- El token se debe incluir en el header: `Authorization: Bearer {token}`
- El GatewayService reenvía automáticamente el JWT a los microservicios

## 🔧 Configuración

### Variables de Entorno

Cada servicio puede configurarse usando:

- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ASPNETCORE_URLS`: URLs donde el servicio escucha

### Configuración JWT

Los parámetros JWT se configuran en `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "your-issuer", 
    "Audience": "your-audience"
  }
}
```

## 📁 Estructura del Proyecto

```
MicroserviciosSolution/
├── DolarService/
│   ├── Controllers/
│   ├── Models/
│   └── Program.cs
├── TemperaturaService/
│   ├── Controllers/
│   ├── Models/
│   └── Program.cs
├── GatewayService/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Services/
│   └── Program.cs
└── .vscode/
    ├── launch.json
    ├── tasks.json
    └── settings.json
```

## 🛠️ Desarrollo

### Tareas disponibles en VS Code

- `build-all` - Construir toda la solución
- `clean-all` - Limpiar archivos de compilación  
- `restore-all` - Restaurar paquetes NuGet
- `watch-[ServiceName]` - Modo desarrollo con hot reload
- `run-all-services` - Ejecutar todos los servicios

### Características incluidas

- ✅ Swagger UI con autenticación JWT en cada servicio
- ✅ **Gateway unificado** con todos los endpoints en un solo Swagger
- ✅ Health Checks en todos los servicios
- ✅ Hot Reload para desarrollo
- ✅ **Controladores manuales** en Gateway (sin proxy reverso)
- ✅ Autenticación JWT centralizada
- ✅ Logging estructurado
- ✅ Configuración por entorno
- ✅ **Endpoint de información** del Gateway (`/api/Info`)
- ✅ **Monitoreo de estado** de microservicios (`/api/Info/status`)

## 🌐 URLs Importantes

### Desarrollo (HTTP)
- **Gateway**: http://localhost:5071
- **Dólar**: http://localhost:5104
- **Temperatura**: http://localhost:5050

### Health Checks
- **Gateway**: http://localhost:5071/health
- **Dólar**: http://localhost:5104/health  
- **Temperatura**: http://localhost:5050/health

## 📦 Dependencias Principales

- .NET 9.0
- ASP.NET Core
- Swashbuckle.AspNetCore (Swagger)
- Microsoft.AspNetCore.Authentication.JwtBearer
- AspNetCore.HealthChecks.UI
- Microsoft.Extensions.Diagnostics.HealthChecks

---

¡La solución está lista para desarrollo y puede escalarse fácilmente agregando más microservicios!

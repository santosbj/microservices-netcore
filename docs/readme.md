# Solución de Microservicios con ASP.NET Core
Arquitectura de microservicios con ASP.NET Core que consta de tres servicios:

## Estructura del Proyecto
1. DolarService : Microservicio que retorna el precio actual del dólar
   
   - Implementa autenticación JWT
   - Expone un endpoint /api/dolar que devuelve un precio aleatorio del dólar
2. TemperaturaService : Microservicio que retorna la temperatura actual de una ciudad
   
   - Implementa autenticación JWT
   - Expone un endpoint /api/temperatura/{ciudad} que devuelve una temperatura aleatoria para la ciudad especificada
3. GatewayService : Servicio API Gateway que actúa como punto de entrada
   
   - Implementa autenticación JWT
   - Utiliza YARP como proxy inverso para redirigir las solicitudes a los microservicios
   - Proporciona un endpoint /api/gateway/token para generar tokens JWT
   - Reenvía el token JWT a los microservicios
## Características Implementadas
- Autenticación JWT : Todos los servicios utilizan JWT para autenticación
- Proxy Inverso : El Gateway utiliza YARP para redirigir las solicitudes a los microservicios
- Middleware Personalizado : Se implementó middleware para reenviar tokens JWT
- Transformadores de Solicitudes : Se implementaron transformadores para modificar las solicitudes antes de enviarlas a los microservicios
## Flujo de Trabajo
1. El cliente solicita un token JWT al Gateway proporcionando un nombre de usuario y una fecha
2. El Gateway genera un token JWT con estos datos
3. El cliente utiliza este token para hacer solicitudes al Gateway
4. El Gateway valida el token y reenvía la solicitud al microservicio correspondiente, incluyendo el token JWT
5. El microservicio valida el token JWT y procesa la solicitud
6. La respuesta se devuelve al cliente a través del Gateway
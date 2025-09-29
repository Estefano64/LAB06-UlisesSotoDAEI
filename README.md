# LAB06 - JWT Authentication with Database

Este proyecto implementa autenticación JWT usando Entity Framework Core con MySQL y contraseñas encriptadas con BCrypt.

## Características Implementadas

- ✅ Tabla de usuarios en base de datos MySQL
- ✅ Contraseñas encriptadas con BCrypt (factor 11)
- ✅ Autenticación JWT con base de datos en lugar de credenciales hardcoded
- ✅ Múltiples políticas de autorización por roles
- ✅ Entity Framework Core con Pomelo MySQL provider
- ✅ Swagger UI integrado con autenticación JWT

## Configuración de Base de Datos

### 1. Crear la tabla Users

Ejecuta el script SQL ubicado en `Scripts/CreateUsersTable.sql` en tu base de datos MySQL `universidad`:

```sql
-- Ver el archivo Scripts/CreateUsersTable.sql para el script completo
CREATE TABLE Users (
    id_user INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(50) NOT NULL DEFAULT 'User',
    email VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE
);
```

### 2. Usuarios de prueba

Se incluyen 3 usuarios predefinidos:

| Usuario | Contraseña | Rol | Email |
|---------|------------|-----|-------|
| admin | admin123 | Admin | admin@universidad.com |
| user1 | user123 | User | user1@universidad.com |
| profesor1 | teacher123 | Teacher | profesor1@universidad.com |

## Endpoints de la API

### Autenticación

#### POST `/api/auth/login`
Autentica un usuario y devuelve un token JWT.

**Request Body:**
```json
{
  "user": "admin",
  "password": "admin123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires": "2024-01-01T12:30:00Z",
  "user": "admin",
  "role": "Admin",
  "message": "Autenticación exitosa"
}
```

### Endpoints Protegidos

#### GET `/api/auth/admin-only`
- **Política:** `AdminOnly`
- **Roles permitidos:** Admin
- **Requiere:** Token JWT válido con rol Admin

#### GET `/api/auth/teacher-admin`
- **Política:** `TeacherOrAdmin`
- **Roles permitidos:** Teacher, Admin
- **Requiere:** Token JWT válido con rol Teacher o Admin

#### GET `/api/auth/all-users`
- **Política:** `AllUsers`
- **Roles permitidos:** User, Teacher, Admin
- **Requiere:** Token JWT válido con cualquier rol

#### GET `/api/auth/protected`
- **Autorización:** Solo rol Admin (para compatibilidad)
- **Requiere:** Token JWT válido con rol Admin

## Políticas de Autorización

1. **AdminOnly**: Solo usuarios con rol `Admin`
2. **TeacherOrAdmin**: Usuarios con rol `Teacher` o `Admin`
3. **AllUsers**: Todos los usuarios autenticados (`User`, `Teacher`, `Admin`)

## Configuración JWT

Los settings de JWT se encuentran en `appsettings.json`:

```json
{
  "Jwt": {
    "SecretKey": "TuClaveSecretaSuperSegura123456789012345678901234567890",
    "Issuer": "LAB06-Universidad",
    "Audience": "LAB06-Students",
    "ExpiryInMinutes": 30
  }
}
```

## Cómo usar la API

### 1. Obtener token JWT

```bash
curl -X POST "https://localhost:7000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"user":"admin","password":"admin123"}'
```

### 2. Usar token en requests

```bash
curl -X GET "https://localhost:7000/api/auth/admin-only" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### 3. Usar Swagger UI

1. Navega a `https://localhost:7000/swagger`
2. Haz clic en "Authorize"
3. Ingresa el token en formato: `Bearer YOUR_TOKEN_HERE`
4. Prueba los diferentes endpoints

## Estructura del Proyecto

```
LAB06-UlisesSoto/
├── Controllers/
│   └── AuthController.cs          # Controlador de autenticación
├── Data/
│   └── ApplicationDbContext.cs    # Contexto de Entity Framework
├── DTOs/
│   └── AuthDtos.cs               # DTOs para autenticación
├── Models/
│   └── User.cs                   # Modelo de usuario
├── Services/
│   ├── Interfaces/
│   │   ├── IAuthService.cs       # Interfaz del servicio de autenticación
│   │   └── IPasswordService.cs   # Interfaz del servicio de contraseñas
│   └── Implementations/
│       ├── AuthService.cs        # Implementación del servicio de autenticación
│       └── PasswordService.cs    # Implementación del servicio de contraseñas
├── Scripts/
│   ├── CreateUsersTable.sql      # Script SQL para crear tabla de usuarios
│   └── PasswordHashGenerator.cs  # Generador de hashes (utilidad)
└── README.md                     # Este archivo
```

## Tecnologías Utilizadas

- **ASP.NET Core 9.0**
- **Entity Framework Core 9.0**
- **Pomelo.EntityFrameworkCore.MySql 9.0.0**
- **BCrypt.Net-Next 4.0.3** (para encriptación de contraseñas)
- **Microsoft.AspNetCore.Authentication.JwtBearer 9.0.9**
- **System.IdentityModel.Tokens.Jwt 8.14.0**
- **Swashbuckle.AspNetCore 9.0.4** (Swagger UI)

## Seguridad

- Las contraseñas se almacenan encriptadas usando BCrypt con factor 11
- Los tokens JWT tienen expiración configurable (30 minutos por defecto)
- Se valida issuer, audience y lifetime de los tokens
- Diferentes niveles de autorización por roles

## Notas de Desarrollo

- La conexión a MySQL está configurada en `appsettings.json`
- Se incluyen logs detallados para debugging
- El proyecto usa inyección de dependencias para todos los servicios
- Se mantiene compatibilidad con la implementación anterior
# Backend - ASP.NET Core Web API

API REST para el sistema de gestión de normativas conectada a Supabase.

## Requisitos

- .NET 8.0 SDK o superior
- Credenciales de Supabase (URL y Anon Key)

## Estructura del Proyecto

```
Backend/
├── Controllers/          # Controladores REST API
├── Services/            # Interfaces y implementaciones de servicios
│   └── Implementations/
├── Entities/            # Modelos de dominio
├── Models/              # DTOs para requestas/respuestas
├── Data/                # Conexión y acceso a Supabase
└── Utilities/           # Funciones auxiliares
```

## Configuración

### 1. Variables de Entorno

Crea un archivo `.env` en la raíz del proyecto Backend:

```env
Supabase__Url=https://your-project.supabase.co
Supabase__AnonKey=your-anon-key
Supabase__ServiceRoleKey=your-service-role-key
```

### 2. Actualizar appsettings.json

Edita `appsettings.json` con tus credenciales de Supabase:

```json
{
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "AnonKey": "your-anon-key",
    "ServiceRoleKey": "your-service-role-key"
  }
}
```

## Ejecutar el Backend

```bash
cd Backend
dotnet run
```

El API estará disponible en `https://localhost:5000` y Swagger en `https://localhost:5000/swagger`

## Endpoints Disponibles

### Regulations

- `GET /api/regulations` - Obtener todas las normativas (admin)
- `GET /api/regulations/published` - Obtener normativas publicadas (público)
- `GET /api/regulations/{id}` - Obtener normativa por ID
- `GET /api/regulations/search/{searchText}?type=DECREE` - Buscar normativas
- `POST /api/regulations` - Crear nueva normativa
- `PUT /api/regulations/{id}` - Actualizar normativa (completo)
- `PATCH /api/regulations/{id}` - Actualizar normativa (parcial)
- `DELETE /api/regulations/{id}` - Eliminar normativa

## Tipos de Datos

### Regulate States
- `DRAFT` - Borrador
- `REVIEW` - En revisión
- `PUBLISHED` - Publicada
- `ARCHIVED` - Archivada

### Regulation Types
- `DECREE` - Decreto
- `RESOLUTION` - Resolución
- `ORDINANCE` - Ordenanza
- `TRIBUNAL_RESOLUTION` - Resolución Tribunal
- `BID` - Licitación

### Legal Status
- `VIGENTE` - Vigente
- `PARCIAL` - Parcialmente vigente
- `SIN_ESTADO` - Sin estado

## Base de Datos (Supabase)

La tabla `regulations` debe tener las siguientes columnas:

```sql
CREATE TABLE regulations (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  special_number TEXT NOT NULL,
  reference TEXT NOT NULL,
  type TEXT DEFAULT 'DECREE',
  state TEXT DEFAULT 'DRAFT',
  legal_status TEXT DEFAULT 'SIN_ESTADO',
  content TEXT,
  keywords TEXT[] DEFAULT ARRAY[]::TEXT[],
  publication_date TIMESTAMP DEFAULT NOW(),
  file_url TEXT,
  pdf_url TEXT,
  created_at TIMESTAMP DEFAULT NOW(),
  updated_at TIMESTAMP DEFAULT NOW()
);
```

## Conexión Frontend-Backend

El frontend React está configurado para conectar a este backend. Actualiza la URL base en el frontend si es necesario:

```typescript
// En src/lib/supabaseClient.ts o similar
const API_BASE = process.env.REACT_APP_API_URL || 'https://localhost:5000/api';
```

## Desarrollo

Para cambios en caliente durante desarrollo:

```bash
dotnet watch run
```

## Build para Producción

```bash
dotnet publish -c Release -o ./publish
```

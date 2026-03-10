# Integración Frontend-Backend

## Configuración de la URL Base

En el frontend React, configura la URL base del API:

### 1. Crear archivo `.env` en `frontend/`

```env
REACT_APP_API_BASE_URL=https://localhost:5000/api
REACT_APP_API_BASE_URL_PRODUCTION=https://your-api-domain.com/api
```

### 2. Crear servicio API consolidado

Archivo: `frontend/src/lib/apiClient.ts`

```typescript
const API_BASE = process.env.NODE_ENV === 'production' 
  ? process.env.REACT_APP_API_BASE_URL_PRODUCTION 
  : process.env.REACT_APP_API_BASE_URL || 'https://localhost:5000/api';

export const apiClient = {
  async get<T>(endpoint: string): Promise<T> {
    const res = await fetch(`${API_BASE}${endpoint}`);
    if (!res.ok) throw new Error(`API Error: ${res.statusText}`);
    return res.json();
  },

  async post<T>(endpoint: string, data: unknown): Promise<T> {
    const res = await fetch(`${API_BASE}${endpoint}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    if (!res.ok) throw new Error(`API Error: ${res.statusText}`);
    return res.json();
  },

  async put<T>(endpoint: string, data: unknown): Promise<T> {
    const res = await fetch(`${API_BASE}${endpoint}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    if (!res.ok) throw new Error(`API Error: ${res.statusText}`);
    return res.json();
  },

  async patch<T>(endpoint: string, data: unknown): Promise<T> {
    const res = await fetch(`${API_BASE}${endpoint}`, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    if (!res.ok) throw new Error(`API Error: ${res.statusText}`);
    return res.json();
  },

  async delete<T>(endpoint: string): Promise<T> {
    const res = await fetch(`${API_BASE}${endpoint}`, { method: 'DELETE' });
    if (!res.ok) throw new Error(`API Error: ${res.statusText}`);
    return res.json();
  },
};
```

### 3. Actualizar servicios del frontend

Reemplaza las llamadas mock con llamadas al API:

**Archivo: `frontend/src/lib/regulationService.ts`**

```typescript
import { apiClient } from './apiClient';
import { Regulation } from '../types';

export const regulationService = {
  async getAll(): Promise<Regulation[]> {
    const response = await apiClient.get('/regulations');
    return response.data || [];
  },

  async getPublished(): Promise<Regulation[]> {
    const response = await apiClient.get('/regulations/published');
    return response.data || [];
  },

  async getById(id: string): Promise<Regulation> {
    const response = await apiClient.get(`/regulations/${id}`);
    return response.data;
  },

  async create(data: Partial<Regulation>): Promise<Regulation> {
    const response = await apiClient.post('/regulations', data);
    return response.data;
  },

  async update(id: string, data: Partial<Regulation>): Promise<Regulation> {
    const response = await apiClient.put(`/regulations/${id}`, data);
    return response.data;
  },

  async patch(id: string, data: Partial<Regulation>): Promise<Regulation> {
    const response = await apiClient.patch(`/regulations/${id}`, data);
    return response.data;
  },

  async delete(id: string): Promise<boolean> {
    const response = await apiClient.delete(`/regulations/${id}`);
    return response.success;
  },

  async search(text: string, type?: string): Promise<Regulation[]> {
    const query = type ? `?type=${type}` : '';
    const response = await apiClient.get(`/regulations/search/${encodeURIComponent(text)}${query}`);
    return response.data || [];
  },
};
```

## CORS Configuration

El backend ya está configurado con CORS para permitir todas las origins. En producción, actualiza `Program.cs`:

```csharp
options.AddPolicy("AllowFrontend", policy =>
{
    policy
        .WithOrigins("https://your-frontend-domain.com")
        .AllowAnyMethod()
        .AllowAnyHeader();
});
```

## Autenticación (Opcional - Futuro)

Cuando implementes autenticación JWT:

1. El backend emitirá tokens JWT
2. El frontend guardará el token en localStorage/sessionStorage
3. Todas las peticiones incluirán el header `Authorization: Bearer {token}`

```typescript
// Modificar apiClient para incluir JWT
async function callApi(endpoint: string, options: RequestInit = {}) {
  const token = localStorage.getItem('token');
  const headers = {
    'Content-Type': 'application/json',
    ...(token && { 'Authorization': `Bearer ${token}` }),
    ...options.headers,
  };

  return fetch(`${API_BASE}${endpoint}`, { ...options, headers });
}
```

## Testing

### Backend
```bash
cd Backend
dotnet test
```

### Frontend
```bash
cd frontend
npm test
```

## Despliegue

### Backend (Azure/Heroku)
```bash
dotnet publish -c Release -o ./publish
```

### Frontend (Vercel)
```bash
npm run build
vercel --prod
```

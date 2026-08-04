# Proyecto Final – Conexión a Base de Datos

## 📌 Tecnologías

- **PostgreSQL 15** – motor de base de datos
- **Entity Framework Core 8** – ORM y manejo de migraciones
- **Npgsql** – driver de PostgreSQL para ADO.NET

---

## 1️⃣ Instalación de dependencias

Ejecutar en la terminal desde la raíz del proyecto:

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Npgsql
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet tool install --global dotnet-ef
```

## 2️⃣ Configuración de la cadena de conexión

Usar Secret Manager para guardar la conexión de forma segura:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:PostgresConnection" "Host=localhost;Database=proyecto_final;Username=postgres;Password=pw;Pooling=true"
dotnet user-secrets list
```

Esto evita poner datos sensibles en el código o repositorio.

## 3️⃣ actualizar la base:

```bash
dotnet ef database update
```

EF Core aplicará los cambios pendientes de forma segura.

## 🛠️ Cómo generar o realizar modificaciones

Para generar relaciones se recomienda ver el tutorial hasta abajo de este archivo.

### 1️⃣ Crear o modificar una entidad

1. Crear una nueva clase en `Models/CodeFirst` o modificar una existente:

```csharp
// Ejemplo de nueva entidad
public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
}
```

2. Agregar el DbSet correspondiente en AppDbContext:

```csharp
public DbSet<Cliente> Clientes { get; set; }
```

### 2️⃣ Crear una nueva migración

```bash
dotnet ef migrations add NombreDeLaMigracion
```

Esto generará un archivo .cs con los métodos Up() y Down() para aplicar o revertir cambios.

### 3️⃣ Revisar la migración

Abrir el archivo de migración generado en la carpeta Migrations.

Verificar que los cambios reflejen lo que querés aplicar a la base de datos.

### 4️⃣ Aplicar la migración a la base de datos

```bash
dotnet ef database update
```

EF Core aplicará los cambios pendientes de forma segura.

### 5️⃣ Revertir una migración (si es necesario)

Para deshacer la última migración antes de aplicarla:

```bash
dotnet ef migrations remove
```

Para volver la base de datos a un estado anterior (ej. a la migración Baseline):

```bash
dotnet ef database update Baseline
```

## RECOMENDACIONES

Convención recomendada: AddNuevoCampo, Delete..., Update...

[TUTORIAL DE EF RECOMENDADO](https://www.netmentor.es/entrada/introduccion-entity-framework-core?viewasvideo=true)

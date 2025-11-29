# Database Setup Guide for ASP.NET Project

## Overview
Your ASP.NET project is now configured to use Entity Framework Core with SQL Server. Here's how to set up an online database and connect your application.

---

## Step 1: Choose Your Online Database Provider

### ⭐ FREE OPTIONS (No Credit Card Required)

#### Option A: Railway.app (EASIEST - Recommended for Learning)
1. Go to [Railway.app](https://railway.app)
2. Sign up with GitHub (free account)
3. Create new project
4. Add "PostgreSQL" service
5. Click "Create"
6. Copy connection string from the "Connect" tab

**Pros**: 
- ✅ Completely free for learning
- ✅ Easy setup (5 minutes)
- ✅ No credit card needed
- ✅ PostgreSQL included

**Note**: You'll need to change your project to use PostgreSQL instead of SQL Server:
```csharp
// In Program.cs, replace UseSqlServer with:
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
```

And add NuGet package:
```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

---

#### Option B: Supabase (Free PostgreSQL + Auth)
1. Go to [Supabase.com](https://supabase.com)
2. Sign up with GitHub
3. Create new project
4. Go to Settings → Database
5. Copy connection string

**Pros**:
- ✅ Free tier with generous limits
- ✅ Built-in PostgreSQL
- ✅ No credit card for free tier

---

#### Option C: Render.com (Free PostgreSQL)
1. Go to [Render.com](https://render.com)
2. Sign up with GitHub
3. Create new PostgreSQL database
4. Copy connection string

**Pros**:
- ✅ Free PostgreSQL instance
- ✅ Automatic backups
- ✅ No credit card

---

### PAID BUT CHEAPEST OPTIONS

#### Option D: Azure SQL Database (Free Tier - Limited)
1. Go to [Azure Portal](https://portal.azure.com)
2. Sign up (requires credit card, but free $200 credit)
3. Click "Create a resource" → Search for "SQL Database"
4. Select **Free tier** (F2s_v2)
5. Configure:
   - **Database Name**: `asplab2db`
   - **Server**: Create new
   - **Location**: Select closest region
6. Click "Review + Create" → "Create"

**Note**: Free tier may expire after 12 months

---

### Option E: Local SQL Server (NO ONLINE ACCESS)
Install locally for development:
- **SQL Server Express**: https://www.microsoft.com/en-us/sql-server/sql-server-editions-express
- **Local SQLite** (even simpler): Uses local file, no server needed

**Pros**:
- ✅ Completely free
- ✅ Works offline
- ✅ No credit card

**Cons**:
- ❌ Not accessible online
- ❌ Only works on your computer

---

## Step 2: Get Your Connection String

### For PostgreSQL (Railway, Supabase, Render):
Connection string format:
```
Server=your-server.com;Port=5432;Database=your_db;User Id=your_user;Password=your_password;
```

Example (Railway):
```
postgresql://user:password@containers-us-west-93.railway.app:7542/railway
```

### For SQL Server (Azure):
```
Server=tcp:yourusername-asplab2.database.windows.net,1433;Initial Catalog=asplab2db;Persist Security Info=False;User ID=youradminusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

---

## Step 3: Setup Your Project for PostgreSQL (RECOMMENDED - FREE)

### Add PostgreSQL NuGet Package

Open terminal and run:
```bash
cd /path/to/ASP_Lab2
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

### Update Program.cs

Replace the SQL Server configuration with PostgreSQL:

```csharp
// Find this line:
// options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

// Replace with:
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
```

### Update appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-server.railway.app;Port=5432;Database=railway;Username=postgres;Password=your-password;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Update Your Migrations (Important!)

Your existing SQL Server migrations won't work with PostgreSQL. You have two options:

**Option 1: Delete and regenerate migrations**
```bash
# Remove the Migrations folder entirely
rm -r Migrations

# Generate new migrations for PostgreSQL
dotnet ef migrations add InitialCreate
```

**Option 2: Keep existing, let EF Core handle it**
```bash
# Just run this - EF Core will adapt
dotnet ef database update
```

---

## Step 4: Apply Database Migrations

1. **Install Entity Framework Core CLI (first time only)**:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

2. **Apply migrations to your database**:
   ```bash
   cd /path/to/ASP_Lab2
   dotnet ef database update
   ```

3. **Verify it worked**:
   - Check your database (Railway/Supabase/Render dashboard)
   - You should see a `Users` table created

---

## Step 5: Run Your Application

```bash
cd /path/to/ASP_Lab2
dotnet run
```

Your application will now:
- ✅ Use the online database for user accounts
- ✅ Store registration data persistently
- ✅ Support login with your registered accounts
- ✅ Work across multiple devices accessing the same database

---

## How It Works

### User Registration Flow:
1. User fills out registration form
2. `AccountController.Register()` creates a User object
3. Password is hashed using `PasswordHasher<User>`
4. User is saved to your online database via Entity Framework
5. User sees success message and redirects to login

### User Login Flow:
1. User enters credentials
2. `UserStore.FindByEmailAsync()` queries the database
3. Password is verified against the stored hash
4. Authentication cookie is created
5. User is logged in

---

## Useful Commands

```bash
# View your connection string in User Secrets
dotnet user-secrets list

# Clear user secrets
dotnet user-secrets clear

# Check database status
dotnet ef database info

# Create a new migration after model changes
dotnet ef migrations add YourMigrationName

# Remove last migration
dotnet ef migrations remove

# Revert database to specific migration
dotnet ef database update MigrationName
```

---

## Environment Variables (for production)

Create a `.env` file (or use CI/CD pipeline secrets):
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=your-connection-string
```

---

## Security Best Practices

1. ✅ **Never commit passwords** to version control
2. ✅ **Use User Secrets** for local development
3. ✅ **Use Azure Key Vault** for production secrets
4. ✅ **Enable database firewall** rules
5. ✅ **Use SSL/TLS** connections (already enabled in string)
6. ✅ **Regularly backup** your database
7. ✅ **Use strong passwords** (minimum 12 characters)

---

## Troubleshooting

| Error | Solution |
|-------|----------|
| "Cannot connect to server" | Check firewall rules in Azure Portal |
| "Login failed" | Verify connection string and credentials |
| "Database doesn't exist" | Run `dotnet ef database update` |
| "Migrations pending" | Run `dotnet ef database update` |
| "Timeout" | Increase Connection Timeout in connection string |

---

## Next Steps

- Add more models (Posts, Comments, etc.)
- Create migrations for new tables
- Implement role-based authorization
- Add email confirmation
- Set up automated backups

---

**Questions?** Refer to the [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)

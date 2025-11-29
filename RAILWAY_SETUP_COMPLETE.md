# Railway PostgreSQL Setup - Complete Instructions

## Your Database Connection Details

```
Host: junction.proxy.rlwy.net
Port: 39145
Database: railway
Username: postgres
Password: bqwWeNSQnQUVvDLLJPhpwDOQcbfuczEB
SSL: Required
```

## Changes Made to Your Project

✅ **appsettings.json** - Updated with Railway PostgreSQL connection string
✅ **projectAsp.csproj** - Replaced SQL Server with Npgsql.EntityFrameworkCore.PostgreSQL
✅ **Program.cs** - Changed UseSqlServer to UseNpgsql
✅ **Data/AppDbContext.cs** - Updated for PostgreSQL naming conventions
✅ **Migrations/** - Created fresh PostgreSQL migrations

---

## What Tables You Need

### Users Table (Automatically Created)

The migration will automatically create this table:

```sql
CREATE TABLE users (
    id uuid PRIMARY KEY,
    email character varying(256) UNIQUE NOT NULL,
    password_hash text NOT NULL,
    display_name character varying(256),
    password_reset_token character varying(512),
    password_reset_token_expires timestamp with time zone
);
```

**Columns:**
- `id` - Unique identifier for each user
- `email` - User's email (unique)
- `password_hash` - Hashed password
- `display_name` - User's full name
- `password_reset_token` - Token for password reset
- `password_reset_token_expires` - When reset token expires

---

## Next Steps - Apply Migrations

### Step 1: Install Entity Framework CLI (First Time Only)

```bash
dotnet tool install --global dotnet-ef
```

### Step 2: Run Database Update

```bash
cd "/Users/victorrudinciuc/Colegiu/Moraru Magdalena/ASP/ASP_Lab2"
dotnet ef database update
```

This will:
1. Create the `users` table
2. Create the unique index on `email`
3. Your database is ready!

### Step 3: Run Your Application

```bash
dotnet run
```

Open: https://localhost:5001

---

## Testing

1. **Register a new account**
   - Go to /Account/Register
   - Fill in credentials
   - Click Register

2. **Check Railway Dashboard**
   - Go to https://railway.app
   - Click your project → PostgreSQL
   - Go to "Data" tab
   - Run query: `SELECT * FROM users;`
   - You should see your registered user!

3. **Login**
   - Go to /Account/Login
   - Use the credentials you just registered
   - You should be logged in

---

## Common Issues & Solutions

| Error | Solution |
|-------|----------|
| "Connection refused" | Check Railway dashboard - is PostgreSQL running? |
| "password authentication failed" | Password might be wrong - copy fresh from Railway Connect tab |
| "Database 'railway' does not exist" | Run `dotnet ef database update` to create it |
| "Port 5001 already in use" | Change port in launchSettings.json or kill process on port 5001 |

---

## Important: Keep Your Password Safe!

Your password `bqwWeNSQnQUVvDLLJPhpwDOQcbfuczEB` is in appsettings.json. 

**For production**, use User Secrets instead:

```bash
# Set up secrets
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=junction.proxy.rlwy.net;Port=39145;Database=railway;Username=postgres;Password=bqwWeNSQnQUVvDLLJPhpwDOQcbfuczEB;SSL Mode=Require;"
```

Then in appsettings.json, secrets will override the value automatically.

---

## Available Commands

```bash
# Check pending migrations
dotnet ef migrations list

# View database info
dotnet ef database info

# Add new migration after model changes
dotnet ef migrations add YourMigrationName

# Revert to previous migration
dotnet ef database update PreviousMigrationName

# View migrations script
dotnet ef migrations script
```

---

## Your Application Architecture

```
User Registration Form
    ↓
AccountController.Register()
    ↓
UserStore.CreateAsync(user)
    ↓
AppDbContext.Users.Add(user)
    ↓
PostgreSQL Database (Railway)
    ↓
users table
```

---

## Useful Railway Links

- Dashboard: https://railway.app
- PostgreSQL Connection: Railway App → PostgreSQL Service → Connect tab
- Monitor Usage: Railway App → Deployments → Logs

---

**All set!** Run `dotnet run` and test your application. 🚀

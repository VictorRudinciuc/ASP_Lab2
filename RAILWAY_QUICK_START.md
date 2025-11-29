# Quick Start: Railway.app (FREE - No Credit Card)

This is the **fastest way** to get a free online database working with your ASP.NET project.

## Step 1: Create Railway Database (5 minutes)

1. Go to https://railway.app
2. Click "Sign up" → Sign in with **GitHub**
3. Create a new project
4. Click "Add Service" → Select "PostgreSQL"
5. Wait for it to deploy (1-2 minutes)
6. Click on the PostgreSQL service
7. Go to the "Connect" tab
8. Copy the connection string that looks like:
   ```
   postgresql://user:password@containers-us-west-93.railway.app:7542/railway
   ```

## Step 2: Install PostgreSQL Package

Open your terminal and run:

```bash
cd /path/to/ASP_Lab2
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

## Step 3: Update Program.cs

Open `Program.cs` and find this line:
```csharp
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
```

Replace it with:
```csharp
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
```

The full section should look like:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

## Step 4: Update appsettings.json

Open `appsettings.json` and replace the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=containers-us-west-93.railway.app;Port=7542;Database=railway;Username=postgres;Password=your_password_here;"
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

**⚠️ IMPORTANT**: Replace `your_password_here` with the actual password from Railway.

## Step 5: Delete Old Migrations & Create New Ones

Since we're switching from SQL Server to PostgreSQL:

```bash
# Remove the old migrations folder
rm -r Migrations

# Create database (EF will create it if it doesn't exist)
dotnet ef database update

# If the above fails, create a new migration first:
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Step 6: Run Your App

```bash
dotnet run
```

Open https://localhost:5001 and test registration!

---

## Where to Find Your Connection String Again

If you lose it:
1. Go to https://railway.app
2. Click your project
3. Click the PostgreSQL service
4. Click "Connect" tab
5. Copy the connection string

---

## Testing

1. Go to http://localhost:5001
2. Register a new account
3. Log in with that account
4. Try logout/login
5. Check Railway dashboard → PostgreSQL → "Data" tab to see your users

---

## Environment Variables (Better Security)

Instead of putting the password in `appsettings.json`, use User Secrets:

```bash
# Initialize secrets (one time)
dotnet user-secrets init

# Store your connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "postgresql://user:password@containers-us-west-93.railway.app:7542/railway"
```

Now your password is safe and not in version control!

---

## Troubleshooting

### "Connection refused"
- Check if PostgreSQL service is running on Railway dashboard
- Verify the connection string is correct

### "Authentication failed"
- Double-check the password in your connection string
- Copy it fresh from Railway's Connect tab

### "Database doesn't exist"
- Run: `dotnet ef database update`
- This will create the database automatically

### Port already in use
- Railway uses port 5432 for PostgreSQL
- Your ASP.NET app runs on port 5001 (different port, no conflict)

---

## Limits (Railway Free Tier)

- ✅ 5GB storage included
- ✅ Unlimited databases
- ✅ Unlimited connections
- ⏰ Sleeps after 7 days of inactivity (restart anytime)

Perfect for learning and small projects!

---

**Done! Your accounts are now saved in the cloud.** 🎉

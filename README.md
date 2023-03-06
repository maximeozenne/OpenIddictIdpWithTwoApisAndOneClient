# Quickstart

## Configure local database

By default, my sample uses `PostgreSQL`. If you have a local instance of PostgreSQL on your machine listening on the port `5432`, then simply modify your `secrets.json` (right-click on the `IdentityProvider` project, then `Manage User Secrets`) to configure the `ConnectionString` :

```
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Port=5432;User Id=YOURID;Password=YOURPASSWORD;Database=YOURDATABASE;"
    }
}
```

The default values for `User Id` and `Database` are usually `postgres`.

You can also simply modify the `appsettings.json` but be careful if you create a merge request as some sensible data about your local database instance could become public.
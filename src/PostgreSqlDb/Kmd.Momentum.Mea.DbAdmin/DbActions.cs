using DbUp;
using Npgsql;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.DbAdmin
{
    public class DbActions
    {
        public async Task<int> CreateAsync(CreateOptions options)
        {
            Log.Verbose("Connecting to {ServerName} as {User}", options.ServerName, options.AdminUser);

            var adminConnString = GetConnectionString(options, options.AdminDatabase, options.AdminUser, options.AdminPassword);
            var createDb = false;

            using (var conn = new NpgsqlConnection(adminConnString))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (var cmd = conn.CreateCommand())
                {
                    Log.Verbose("Checking for login {User}", options.UserName);

                    cmd.CommandText = $"SELECT 1 FROM pg_catalog.pg_roles WHERE rolname = '{options.UserName}'";

                    string userAction;
                    if (await cmd.ExecuteScalarAsync().ConfigureAwait(false) != null)
                    {
                        Log.Information("Login {User} already exists", options.UserName);
                        userAction = "ALTER ROLE";
                    }
                    else
                    {
                        Log.Information("Creating login {User}", options.UserName);
                        userAction = "CREATE USER";
                    }

                    cmd.CommandText = $"{userAction} \"{options.UserName}\" WITH LOGIN CREATEDB PASSWORD '{options.Password}'";

                    await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                    Log.Verbose("Checking for database {Database}", options.DatabaseName);

                    cmd.CommandText = $"SELECT 1 FROM pg_catalog.pg_database WHERE datname = '{options.DatabaseName}'";
                    if (await cmd.ExecuteScalarAsync().ConfigureAwait(false) != null)
                    {
                        Log.Information("Database {Database} already exists", options.DatabaseName);
                    }
                    else
                    {
                        createDb = true;
                    }
                }
            }

            if (createDb)
            {
                Log.Verbose("Connecting to {ServerName} as {User}", options.ServerName, options.UserName);

                var userConnString = GetConnectionString(options, options.AdminDatabase, options.UserName, options.Password);

                using (var conn = new NpgsqlConnection(userConnString))
                {
                    await conn.OpenAsync().ConfigureAwait(false);

                    using (var cmd = conn.CreateCommand())
                    {
                        Log.Information("Creating database {Database}", options.DatabaseName);

                        cmd.CommandText = $"CREATE DATABASE \"{options.DatabaseName}\" WITH OWNER = \"{options.UserName}\" ENCODING = 'UTF8'";

                        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }

            await AddExtensionAsync(options);

            return 0;
        }

        private class DeleteItem
        {
            public string Database { get; }
            public string Owner { get; }

            public DeleteItem(string database, string owner)
            {
                Database = database;
                Owner = owner;
            }
        }

        public async Task<int> DeleteAsync(DeleteOptions options)
        {
            string lessThan;

            if (!ValidateOptions(options, out lessThan))
            {
                return -1;
            }

            var regex = string.IsNullOrEmpty(options.Regex)
                    ? null
                    : new Regex(options.Regex, RegexOptions.IgnoreCase);

            Log.Verbose("Connecting to {ServerName} as {User}", options.ServerName, options.AdminUser);

            var adminConnString = GetConnectionString(options, options.AdminDatabase, options.AdminUser, options.AdminPassword);

            using (var conn = new NpgsqlConnection(adminConnString))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                var databasesToDelete = await FindDatabases(conn, options.DatabaseName, regex, lessThan).ConfigureAwait(false);

                if (databasesToDelete.Count == 0)
                {
                    Log.Information("No databases found to delete");
                    return 0;
                }

                Log.Information("Found {Count} database(s) to delete", databasesToDelete.Count);

                foreach (var delete in databasesToDelete)
                {
                    await DeleteDatabase(conn, delete).ConfigureAwait(false);
                }
            }

            return 0;
        }

        private bool ValidateOptions(DeleteOptions options, out string expiryFormat)
        {
            expiryFormat = null;

            if (string.IsNullOrEmpty(options.DatabaseName)
                && string.IsNullOrEmpty(options.Regex) &&
                options.ExpiryMinutes != null)
            {
                Log.Error("You must either specify a {DatabaseName}, {Regex} or {ExpiryMinutes} to delete",
                    nameof(options.DatabaseName),
                    nameof(options.Regex),
                    nameof(options.ExpiryMinutes));
                return false;
            }

            if (options.ExpiryMinutes != null)
            {
                if (string.IsNullOrWhiteSpace(options.ExpiryFormat))
                {
                    Log.Error("An {ExpiryFormat} is required to delete by {ExpiryMinutes}",
                        nameof(options.ExpiryMinutes),
                        nameof(options.ExpiryFormat));
                    return false;
                }

                try
                {
                    expiryFormat = DateTime.UtcNow
                        .AddMinutes(0 - options.ExpiryMinutes.Value)
                        .ToString(options.ExpiryFormat, CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    Log.Error("Invalid format {ExpiryFormat}", options.ExpiryFormat);
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(options.DatabaseName))
            {
                Log.Information("Looking for a database named {Name}", options.DatabaseName);
            }

            if (!string.IsNullOrEmpty(options.Regex))
            {
                Log.Information("Looking for databases matching {Regex}", options.Regex);
            }

            if (options.ExpiryMinutes != null)
            {
                Log.Information("Looking for databases older than {ExpiryMinutes} mins, based on name < {Format}", options.ExpiryMinutes, expiryFormat);
            }

            return true;
        }

        private static async Task DeleteDatabase(NpgsqlConnection conn, DeleteItem delete)
        {
            Log.Information("Deleting database {Database}", delete.Database);

            using (var cmd = conn.CreateCommand())
            {
                Log.Verbose("Revoking access to {Database}", delete.Database);

                cmd.CommandText = $"REVOKE CONNECT ON DATABASE \"{delete.Database}\" FROM public";

                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                Log.Verbose("Disconnecting existing connections to {Database}", delete.Database);

                cmd.CommandText = $"SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '{delete.Database}'";

                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    var count = 0;

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        count++;
                    }

                    Log.Verbose("Disconnected {Count} connection(s)", count);
                }

                Log.Verbose("Dropping database {Database}", delete.Database);

                cmd.CommandText = $"DROP DATABASE \"{delete.Database}\"";

                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                try
                {
                    Log.Verbose("Dropping owner role {Owner}", delete.Owner);

                    cmd.CommandText = $"DROP ROLE \"{delete.Owner}\"";

                    await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log.Warning("Unable to drop database owner {Owner}, {Message}", delete.Owner, ex.Message);
                }
            }
        }

        private static async Task<List<DeleteItem>> FindDatabases(NpgsqlConnection conn, string databaseName, Regex regex, string lessThan)
        {
            var databasesToDelete = new List<DeleteItem>();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT d.datname, pg_catalog.pg_get_userbyid(d.datdba) FROM pg_catalog.pg_database d ORDER BY d.datname";
                var databases = string.IsNullOrEmpty(databaseName) ? null : databaseName.Split(',');

                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var dbName = reader.GetString(0);
                        var owner = reader.GetString(1);

                        if (databases != null && databases.Contains(dbName))
                        {
                            databasesToDelete.Add(new DeleteItem(dbName, owner));
                            continue;
                        }

                        if (regex != null && regex.IsMatch(dbName))
                        {
                            if (lessThan == null || string.Compare(dbName, lessThan, StringComparison.OrdinalIgnoreCase) < 0)
                            {
                                databasesToDelete.Add(new DeleteItem(dbName, owner));
                            }
                        }
                        else if (lessThan != null && string.Compare(dbName, lessThan, StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            databasesToDelete.Add(new DeleteItem(dbName, owner));
                        }
                    }
                }
            }

            return databasesToDelete;
        }

        private string GetConnectionString(CommonOptions options, string database, string user, string password)
        {
            var connString = options.ConnectionTemplate;

            connString = connString.Replace("{Server}", options.ServerName, StringComparison.OrdinalIgnoreCase);
            connString = connString.Replace("{Database}", database, StringComparison.OrdinalIgnoreCase);
            connString = connString.Replace("{User}", user, StringComparison.OrdinalIgnoreCase);
            connString = connString.Replace("{Password}", password, StringComparison.OrdinalIgnoreCase);

            return connString;
        }

        public Task<int> MigrateAsync(MigrateOptions options)
        {
            Log.Verbose("Connecting to {ServerName} for {Database} as {User}", options.ServerName, options.DatabaseName, options.UserName);
            var userConnString = GetConnectionString(options, options.DatabaseName, options.UserName, options.Password);
            var location = Assembly.GetEntryAssembly().Location;
            var directory = options.Folder;
            var dbUpgradeEngine = DeployChanges.To
                        .PostgresqlDatabase(userConnString)
                        .WithScriptsFromFileSystem(directory)
                        .LogToConsole()
                        .LogToAutodetectedLog()
                        .LogScriptOutput()
                        .WithVariablesDisabled()
                        .WithTransaction()
                        .Build();

            if (dbUpgradeEngine.IsUpgradeRequired())
            {
                var result = dbUpgradeEngine.PerformUpgrade();
                if (result.Successful)
                    Log.Information("Migration Executed  Successfully");
                else
                {
                    Log.Error(result.Error, "Error occurred while running database migrations.");
                    return Task.FromResult<int>(1);
                }
            }
            return Task.FromResult<int>(0);
        }

        private async Task AddExtensionAsync(CreateOptions options)
        {
            Log.Verbose("Connecting to {ServerName} for {Database} as {User}", options.ServerName, options.DatabaseName, options.AdminUser);
            var userConnString = GetConnectionString(options, options.DatabaseName, options.AdminUser, options.AdminPassword);

            using (var conn = new NpgsqlConnection(userConnString))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (var cmd = conn.CreateCommand())
                {
                    Log.Information("Adding extension uuid-ossp for {Database}", options.DatabaseName);
                    cmd.CommandText = $"CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";";
                    await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
        }
    }
}

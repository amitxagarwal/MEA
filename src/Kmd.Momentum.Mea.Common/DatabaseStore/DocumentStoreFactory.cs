using Marten;
using Marten.Services;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Npgsql.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    internal sealed class DocumentStoreFactory
    {
        private static readonly ReaderWriterLockSlim sharedLock = new ReaderWriterLockSlim();
        private static Marten.DocumentStore sharedStore;
        private static string sharedConnection = string.Empty;

        private readonly DocumentMappableSerializationBinder serializationBinder;
        private readonly IDocumentStoreAssemblyDiscoverer discoverer;
        private readonly string connectionString;

        static DocumentStoreFactory()
        {
            try
            {
                NpgsqlLogManager.Provider = new SerilogNpgsqlLogProvider();
            }
            catch (Exception ex)
            {
                // It appears in cases when the schema definition isn't setup properly (maybe?) this method is getting called multiple times.
                // You can only call NpgsqlLogManager.Provider before doing any other Npgsql call and so you can get a nasty exception.
                Log.Error(ex, "Unexpected error occurred while setting up the NpgsqlLogManager");
            }
        }

        public DocumentStoreFactory(
            IConfiguration configuration,
            DocumentMappableSerializationBinder serializationBinder,
            IDocumentStoreAssemblyDiscoverer discoverer)
        {
            this.serializationBinder = serializationBinder ?? throw new ArgumentNullException(nameof(serializationBinder));
            this.discoverer = discoverer ?? throw new ArgumentNullException(nameof(discoverer));

            connectionString = configuration["ConnectionStrings:LogicDatabase"];
        }

        public IScopedDocumentStore GetStore()
        {
            return new ScopedDocumentStore(
                    GetSharedStore(connectionString, serializationBinder, discoverer)
                );
        }

        private static Marten.DocumentStore GetSharedStore(
            string connString,
            DocumentMappableSerializationBinder serializationBinder,
            IDocumentStoreAssemblyDiscoverer discoverer)
        {
            sharedLock.EnterUpgradeableReadLock();
            try
            {
                if (sharedStore != null && sharedConnection == connString)
                {
                    return sharedStore;
                }

                sharedLock.EnterWriteLock();
                try
                {
                    var store = CreateNewStore(connString, serializationBinder, discoverer);

                    ScopedDocumentStore.ClearMappedTenancies();

                    sharedStore = store;
                    sharedConnection = connString;

                    return store;
                }
                finally
                {
                    sharedLock.ExitWriteLock();
                }
            }
            finally
            {
                sharedLock.ExitUpgradeableReadLock();
            }
        }

        private static Marten.DocumentStore CreateNewStore(string connString, DocumentMappableSerializationBinder serializationBinder,
            IDocumentStoreAssemblyDiscoverer discoverer)
        {
            var store = Marten.DocumentStore.For(options =>
            {
                options.Logger(new SerilogMartenLogger());

                var serializer = new Marten.Services.JsonNetSerializer
                {
                    EnumStorage = EnumStorage.AsString
                };

                serializer.Customize(x => x.SerializationBinder = serializationBinder);

                options.PLV8Enabled = false; // Not installed by default in PostgreSQL 11

                options.AutoCreateSchemaObjects = AutoCreate.None;

                options.Connection(connString);

                options.Serializer(serializer);

                foreach (var (docType, attr) in discoverer.DiscoverAutoDocumentCollectionTypes())
                {
                    options.Storage.MappingFor(docType);
                }

                foreach (var cfgType in discoverer.DiscoverDocumentStoreConfigurers())
                {
                    ((IDocumentStoreConfiguration)Activator.CreateInstance(cfgType)).Configure(options);
                }
            });

            return store;
        }


        private class SerilogNpgsqlLogProvider : INpgsqlLoggingProvider
        {
            public NpgsqlLogger CreateLogger(string name)
            {
                return new SerilogNpgsqlLogger();
            }
        }

        private class SerilogNpgsqlLogger : NpgsqlLogger
        {
            private const NpgsqlLogLevel MinimumLogLevel = NpgsqlLogLevel.Info;

            public override bool IsEnabled(NpgsqlLogLevel level)
            {
                return level >= MinimumLogLevel && Serilog.Log.IsEnabled(MapLevel(level));
            }

            public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception exception = null)
            {
                if (level >= MinimumLogLevel)
                {
                    Serilog.Log
                        .ForContext("ConnectorId", connectorId)
                        .Write(MapLevel(level), exception, "Npgsql: {Message}", msg);
                }
            }

            private static LogEventLevel MapLevel(NpgsqlLogLevel level)
            {
                switch (level)
                {
                    case NpgsqlLogLevel.Fatal:
                        return LogEventLevel.Fatal;
                    case NpgsqlLogLevel.Error:
                        return LogEventLevel.Error;
                    case NpgsqlLogLevel.Warn:
                        return LogEventLevel.Warning;
                    case NpgsqlLogLevel.Info:
                        return LogEventLevel.Information;
                    case NpgsqlLogLevel.Debug:
                        return LogEventLevel.Debug;
                    default:
                        return LogEventLevel.Verbose;
                }
            }
        }

        private class SerilogMartenLogger : IMartenLogger
        {
            private readonly IMartenSessionLogger sharedLogger = new SerilogMartenSessionLogger();

            public void SchemaChange(string sql)
            {
                Log.Information("Schema change: {SQL}", sql);
            }

            public IMartenSessionLogger StartSession(IQuerySession session)
            {
                return sharedLogger;
            }
        }

        private class SerilogMartenSessionLogger : IMartenSessionLogger
        {
            public void LogFailure(NpgsqlCommand command, Exception ex)
            {
                Log.Error(ex, "Error executing command: {Command}", command.CommandText);
            }

            public void LogSuccess(NpgsqlCommand command)
            {
                // Do nothing
            }

            public void RecordSavedChanges(IDocumentSession session, IChangeSet commit)
            {
                // Do nothing
            }
        }
    }
}
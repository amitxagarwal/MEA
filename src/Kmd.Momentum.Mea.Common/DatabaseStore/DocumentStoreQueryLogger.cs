using System;
using System.Diagnostics;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    internal sealed class DocumentStoreQueryLogger : IDocumentStoreQueryLogger
    {
        private sealed class DocumentStoreQueryLoggerOperation : IDocumentStoreQueryLoggerOperation
        {
            private readonly string operation;
            private readonly Stopwatch stopWatch;

            public DocumentStoreQueryLoggerOperation(string operation)
            {
                this.operation = operation;
                this.stopWatch = new Stopwatch();
                this.stopWatch.Start();
            }

            public void Dispose()
            {
                stopWatch.Stop();

                Serilog.Log.Verbose("Database operation '{Operation}' in {Elapsed}ms",
                    operation,
                    stopWatch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond);
            }
        }

        public IDocumentStoreQueryLoggerOperation BeginOperation(string operation)
        {
            return new DocumentStoreQueryLoggerOperation(operation);
        }
    }
}

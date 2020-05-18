using CommandLine;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.DbAdmin
{
    static class Program
    {
        static async Task<int> Main(string[] args)
        {
            var minLogLevelSwitch = new Serilog.Core.LoggingLevelSwitch();

            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.ControlledBy(minLogLevelSwitch)
                            .WriteTo.Console()
                            .CreateLogger();

            try
            {
                var helpWriter = new StringWriter();
                var commandLineParser = new Parser(s =>
                {
                    s.HelpWriter = helpWriter;
                    s.CaseSensitive = false;
                    s.CaseInsensitiveEnumValues = true;
                });

                var actions = new DbActions();

                var result = await commandLineParser.ParseArguments<CreateOptions, DeleteOptions, MigrateOptions>(args)
                        .WithParsed((CommonOptions o) =>
                        {
                            minLogLevelSwitch.MinimumLevel = o.Verbosity;
                            Log.Verbose("Started with arguments {Arguments}", args);
                            Log.Verbose("The parsed options are {@Parsed}", o);
                        })
                        .MapResult(
                          (CreateOptions opts) => actions.CreateAsync(opts),
                          (DeleteOptions opts) => actions.DeleteAsync(opts),
                          (MigrateOptions opts) => actions.MigrateAsync(opts),
                          errs =>
                          {
                              Console.WriteLine(helpWriter.ToString());
                              return Task.FromResult(2);
                          }
                       ).ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error: {Message}", ex.Message);
                return -1;
            }
        }
    }
}



using CommandLine;
using Serilog.Events;

namespace Kmd.Momentum.Mea.DbAdmin
{
    public class CommonOptions
    {
        const string AzureTemplate = "Server={Server}.postgres.database.azure.com;" +
                              "Database={Database};Port=5432;User Id={User}@{Server};" +
                              "Password={Password};Ssl Mode=Require;";

        [Option('v', "verbosity", Required = false, Default = LogEventLevel.Information, HelpText = "The logging level.")]
        public LogEventLevel Verbosity { get; set; }

        [Option('t', "template", Required = false, Default = AzureTemplate, HelpText = "The PostgreSQL server connection string template.")]
        public string ConnectionTemplate { get; set; }

        [Option('s', "server", Required = true, HelpText = "The postgresql server.")]
        public string ServerName { get; set; }

        [Option('x', "admindb", Required = false, Default = "postgres", HelpText = "The admin database.")]
        public string AdminDatabase { get; set; }

        [Option('a', "adminuser", Required = false, Default = "postgres", HelpText = "The admin user account.")]
        public string AdminUser { get; set; }

        [Option('w', "adminpass", Required = false, Default = "RtAhL8j9946W", HelpText = "The admin user password.")]
        public string AdminPassword { get; set; }
    }
}
using CommandLine;

namespace Kmd.Momentum.Mea.DbAdmin
{
    [Verb("migrate", HelpText = "Runs the change scripts that are needed to get your database up to date")]
    public class MigrateOptions : CommonOptions
    {
        [Option('u', "dbuser", Required = false, Default = "momentum", HelpText = "The database login account.")]
        public string UserName { get; set; }

        [Option('p', "dbpass", Required = false, Default = "oQTX2jPgOWwe", HelpText = "The database login password.")]
        public string Password { get; set; }

        [Option('d', "database", Required = false, Default = "momentum", HelpText = "The database to create.")]
        public string DatabaseName { get; set; }

        [Option('f', "folder", Required = false, Default = "MigrationScripts", HelpText = "The folder where the migrations scripts are.")]
        public string Folder { get; set; }
    }
}


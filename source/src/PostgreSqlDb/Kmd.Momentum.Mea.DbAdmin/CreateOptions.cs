using CommandLine;

namespace Kmd.Momentum.Mea.DbAdmin
{
    [Verb("create", HelpText = "Create a new database.")]
    public class CreateOptions : CommonOptions
    {
        [Option('u', "dbuser", Required = false, Default = "momentum", HelpText = "The database login account.")]
        public string UserName { get; set; }

        [Option('p', "dbpass", Required = false, Default = "oQTX2jPgOWwe", HelpText = "The database login password.")]
        public string Password { get; set; }

        [Option('d', "database", Required = false, Default = "momentum", HelpText = "The database to create.")]
        public string DatabaseName { get; set; }
    }
}
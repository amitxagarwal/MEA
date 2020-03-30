using CommandLine;

namespace Kmd.Momentum.Mea.DbAdmin
{
    [Verb("delete", HelpText = "Delete one or more databases. If options are used in conjunction, the query becomes \"name = <database> or (name ~ <regex> and name < <age>)\".")]
    public class DeleteOptions : CommonOptions
    {
        [Option('r', "regex", Required = false, HelpText = "Delete databases matching a regular expression.")]
        public string Regex { get; set; }

        [Option('e', "expiry", Required = false, HelpText = "The expiry age of the database in minutes.")]
        public int? ExpiryMinutes { get; set; }

        [Option('f', "expiryformat", Required = false, HelpText = "The format of the database name to age match against.")]
        public string ExpiryFormat { get; set; }

        [Option('d', "database", Required = false, HelpText = "The database to delete.")]
        public string DatabaseName { get; set; }
    }
}

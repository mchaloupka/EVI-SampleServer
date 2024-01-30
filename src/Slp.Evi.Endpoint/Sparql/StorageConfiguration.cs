namespace Slp.Evi.Endpoint.Sparql
{
    public enum DatabaseTypes
    {
        MsSql,
        MySql,
    }

    public class StorageConfiguration
    {
        public DatabaseTypes? DatabaseType { get; set; }
        public string ConnectionString { get; set; }
        public string MappingFilePath { get; set; }
        public int QueryTimeout { get; set; }
    }
}

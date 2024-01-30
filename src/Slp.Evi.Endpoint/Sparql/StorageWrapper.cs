using System;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slp.Evi.Storage.MsSql;
using Slp.Evi.Storage.MySql;
using TCode.r2rml4net;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;
using VDS.RDF.Storage;

namespace Slp.Evi.Endpoint.Sparql
{
    public class StorageWrapper
        : IStorageWrapper
    {
        private readonly IQueryableStorage _storage;

        public StorageWrapper(IOptions<StorageConfiguration> configuration, IHostEnvironment environment, ILoggerFactory loggerFactory)
        {
            var databaseType = configuration.Value.DatabaseType ?? DatabaseTypes.MsSql;

            var connectionString = configuration.Value.ConnectionString;
            var mappingFilePath = configuration.Value.MappingFilePath;
            var path = Path.Combine(environment.ContentRootPath, mappingFilePath);

            IR2RML mapping;
            using (var fs = new FileStream(path, FileMode.Open))
            {
                mapping = R2RMLLoader.Load(fs, new MappingOptions());
            }

            switch (databaseType)
            {
                case DatabaseTypes.MsSql:
                    _storage = new MsSqlEviStorage(mapping, connectionString, configuration.Value.QueryTimeout);
                    break;
                case DatabaseTypes.MySql:
                    _storage = new MySqlEviStorage(mapping, connectionString, configuration.Value.QueryTimeout);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Not supported database ${databaseType}");
            }
        }

        public void Query(IRdfHandler rdfHandler, ISparqlResultsHandler sparqlResultsHandler, string query)
        {
            _storage.Query(rdfHandler, sparqlResultsHandler, query);
        }
    }
}

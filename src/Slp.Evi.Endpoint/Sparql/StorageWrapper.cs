using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slp.Evi.Storage;
using Slp.Evi.Storage.Bootstrap;
using Slp.Evi.Storage.Database.Vendor.MsSql;
using TCode.r2rml4net;
using TCode.r2rml4net.Mapping.Fluent;
using VDS.RDF;

namespace Slp.Evi.Endpoint.Sparql
{
    public class StorageWrapper
        : IStorageWrapper
    {
        private readonly EviQueryableStorage _storage;

        public StorageWrapper(IOptions<StorageConfiguration> configuration, IHostEnvironment environment, ILoggerFactory loggerFactory)
        {
            var connectionString = configuration.Value.ConnectionString;
            var mappingFilePath = configuration.Value.MappingFilePath;
            var path = Path.Combine(environment.ContentRootPath, mappingFilePath);

            IR2RML mapping;
            using (var fs = new FileStream(path, FileMode.Open))
            {
                mapping = R2RMLLoader.Load(fs);
            }

            var sqlFactory = new MsSqlDbFactory();
            _storage = new EviQueryableStorage(sqlFactory.CreateSqlDb(connectionString, configuration.Value.QueryTimeout), mapping, new DefaultEviQueryableStorageFactory(loggerFactory));
        }

        public void Query(IRdfHandler rdfHandler, ISparqlResultsHandler sparqlResultsHandler, string query)
        {
            _storage.Query(rdfHandler, sparqlResultsHandler, query);
        }
    }
}

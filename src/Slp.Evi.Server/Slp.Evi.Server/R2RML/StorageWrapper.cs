using System;
using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Slp.Evi.Storage;
using Slp.Evi.Storage.Bootstrap;
using Slp.Evi.Storage.Database.Vendor.MsSql;
using TCode.r2rml4net;
using TCode.r2rml4net.Mapping.Fluent;

namespace Slp.Evi.Server.R2RML
{
    /// <summary>
    /// Wrapper for the <see cref="R2RmlStorage"/>
    /// </summary>
    public class StorageWrapper
    {
        /// <summary>
        /// The storage
        /// </summary>
        private static EviQueryableStorage _storage = null;

        /// <summary>
        /// Gets the storage.
        /// </summary>
        /// <value>The storage.</value>
        public static EviQueryableStorage Storage { get { return _storage; } }

        /// <summary>
        /// Application start.
        /// </summary>
        public static void AppStart()
        {
            StartException = null;

            try
            {
                var mappingPath = System.Configuration.ConfigurationManager.AppSettings["r2rmlConfig"];
                ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["r2rmlstoreconnection"].ConnectionString;

                Mapping = null;

                var path = System.Web.Hosting.HostingEnvironment.MapPath(mappingPath);

                using (var fs = new FileStream(path, FileMode.Open))
                {
                    Mapping = R2RMLLoader.Load(fs);
                }

                var sqlFactory = new MsSqlDbFactory();
                _storage = new EviQueryableStorage(sqlFactory.CreateSqlDb(ConnectionString), Mapping, new DefaultEviQueryableStorageFactory(new NullLoggerFactory()));
            }
            catch (Exception e)
            {
                StartException = e;
            }
        }

        /// <summary>
        /// Application end.
        /// </summary>
        public static void AppEnd()
        {
            if (_storage != null)
                _storage.Dispose();
        }

        /// <summary>
        /// Gets the start exception.
        /// </summary>
        /// <value>The start exception.</value>
        public static Exception StartException { get; private set; }
        /// <summary>
        /// Gets the mapping.
        /// </summary>
        /// <value>The mapping.</value>
        public static IR2RML Mapping { get; private set; }
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public static string ConnectionString { get; private set; }
    }
}
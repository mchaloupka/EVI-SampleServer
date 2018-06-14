using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slp.Evi.Endpoint.Sparql
{
    public class StorageConfiguration
    {
        public string ConnectionString { get; set; }
        public string MappingFilePath { get; set; }
        public int QueryTimeout { get; set; }
    }
}

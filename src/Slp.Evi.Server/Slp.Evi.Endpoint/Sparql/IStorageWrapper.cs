using Slp.Evi.Endpoint.Controllers;
using VDS.RDF;

namespace Slp.Evi.Endpoint.Sparql
{
    public interface IStorageWrapper
    {
        void Query(IRdfHandler rdfHandler, ISparqlResultsHandler sparqlResultsHandler, string query);
    }
}

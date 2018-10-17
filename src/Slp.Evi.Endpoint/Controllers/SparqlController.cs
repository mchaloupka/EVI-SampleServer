using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Slp.Evi.Endpoint.Sparql;
using Slp.Evi.Endpoint.Sparql.Writers;

namespace Slp.Evi.Endpoint.Controllers
{
    [Route("api/[controller]")]
    public class SparqlController
        : Controller
    {
        private readonly ILogger<SparqlController> _logger;

        public SparqlController(ILogger<SparqlController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public void Get([FromQuery] string query, [FromServices]IStorageWrapper wrapper)
        {
            QueryImpl(query, wrapper);
        }

        [HttpPost]
        public void Post([FromForm] string query, [FromServices] IStorageWrapper wrapper)
        {
            QueryImpl(query, wrapper);
        }

        private void QueryImpl(string query, IStorageWrapper wrapper)
        {
            try
            {
                using (var turtleWriter = new TurtleRdfWriter(Response.Body))
                using (var xmlWriter = new XmlSparqlWriter(Response.Body))
                {
                    var responseWriterHelper = new ResponseWriterHelper(Response, xmlWriter, "application/xml",
                        turtleWriter, "application/turtle");
                    wrapper.Query(responseWriterHelper, responseWriterHelper, query);
                }
            }
            catch (NotImplementedException e)
            {
                _logger.LogError(new EventId(), e, "Query failed - not implemented");
                Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, "Query failed - another exception");
                throw;
            }
        }
    }
}

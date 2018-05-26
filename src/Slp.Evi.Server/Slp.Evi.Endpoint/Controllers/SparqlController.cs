using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // GET api/values
        [HttpGet]
        public void Get([FromQuery] string query, [FromServices]IStorageWrapper wrapper)
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
                Response.StatusCode = StatusCodes.Status400BadRequest;
                _logger.LogError(new EventId(), e, "Query failed - not implemented");
            }
        }
    }
}

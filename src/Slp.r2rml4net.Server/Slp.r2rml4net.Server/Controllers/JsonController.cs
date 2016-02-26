using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Slp.r2rml4net.Server.Models.Json;
using Slp.r2rml4net.Server.R2RML;
using Slp.r2rml4net.Server.Writers;
using TCode.r2rml4net;
using VDS.RDF;
using VDS.RDF.Query;

namespace Slp.r2rml4net.Server.Controllers
{
    /// <summary>
    /// Controller for methods called from JS
    /// </summary>
    public class JsonController : Controller
    {
        /// <summary>
        /// Gets the query result.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>ActionResult.</returns>
        /// <exception cref="System.Exception">Unknown query result</exception>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetQueryResult(string query)
        {
            var result = new JsonResultModel<QueryResultModel>();
            result.Data = new QueryResultModel();

            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var data = StorageWrapper.Storage.Query(query);
                sw.Stop();

                result.Data.TimeElapsedInMs = sw.ElapsedMilliseconds;

                if (data is Graph)
                {
                    var g = (Graph)data;

                    foreach (var triple in g.Triples)
                    {
                        var row = new QueryResultRowModel();
                        row.Bindings.Add(new QueryResultColumnModel()
                        {
                            Variable = "predicate",
                            Value = triple.Predicate.ToString()
                        });

                        row.Bindings.Add(new QueryResultColumnModel()
                        {
                            Variable = "subject",
                            Value = triple.Subject.ToString()
                        });

                        row.Bindings.Add(new QueryResultColumnModel()
                        {
                            Variable = "object",
                            Value = triple.Object.ToString()
                        });
                        result.Data.Rows.Add(row);
                    }

                    result.Data.Variables.Add("subject");
                    result.Data.Variables.Add("predicate");
                    result.Data.Variables.Add("object");
                }
                else if (data is SparqlResultSet)
                {
                    var resData = (SparqlResultSet)data;

                    foreach (var rowData in resData)
                    {
                        var row = new QueryResultRowModel();

                        foreach (var variable in rowData.Variables)
                        {
                            row.Bindings.Add(new QueryResultColumnModel()
                            {
                                Variable = variable,
                                Value = rowData[variable].ToString()
                            });
                        }

                        result.Data.Rows.Add(row);
                    }

                    result.Data.Variables.AddRange(resData.Variables);
                }
                else
                {
                    throw new Exception("Unknown query result");
                }

                result.Success = true;
                return Json(result);
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Data = null;
                result.ExceptionMessage = e.Message;
                result.ExceptionName = e.GetType().Name;
                return Json(result);
            }
        }

        /// <summary>
        /// Gets the query result.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>ActionResult.</returns>
        /// <exception cref="System.Exception">Unknown query result</exception>
        [HttpGet]
        [ValidateInput(false)]
        public void Query(string query)
        {
            try
            {
                using (var turtleWriter = new TurtleRdfWriter(Response.OutputStream))
                using(var xmlWriter = new XmlSparqlWriter(Response.OutputStream))
                {
                    var responseWriterHelper = new ResponseWriterHelper(Response, xmlWriter, "application/xml", turtleWriter, "text/turtle");
                    StorageWrapper.Storage.Query(responseWriterHelper, responseWriterHelper, query);
                }

                Response.End();
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                Response.Write("Query failed");
            }
        }

        /// <summary>
        /// Gets the URL helper.
        /// </summary>
        /// <returns>UrlHelper.</returns>
        private UrlHelper GetUrlHelper()
        {
            return new UrlHelper(HttpContext.Request.RequestContext);
        }

        public void Dump()
        {
            try
            {
                var mapping = StorageWrapper.Mapping;

                using (var connection = new SqlConnection(StorageWrapper.ConnectionString))
                using (var processor = new W3CR2RMLProcessor(connection))
                using (var turtleWriter = new TurtleRdfWriter(Response.OutputStream))
                {
                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.Buffer = false;
                    Response.BufferOutput = false;
                    Response.ContentType = "application/octet-stream";
                    Response.AppendHeader("Content-Type", "application/octet-stream");
                    Response.AppendHeader("Content-Disposition", "attachment; filename=Dump.ttl");
                    Response.Flush();
                    processor.GenerateTriples(mapping, turtleWriter);
                }

                Response.End();
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                Response.Write("Dump creation failed");
            }
        }
    }
}
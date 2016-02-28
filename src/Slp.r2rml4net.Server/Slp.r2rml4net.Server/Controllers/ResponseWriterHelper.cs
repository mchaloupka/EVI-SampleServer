using System;
using System.Web;
using Slp.r2rml4net.Server.Writers;
using VDS.RDF;
using VDS.RDF.Query;

namespace Slp.r2rml4net.Server.Controllers
{
    public class ResponseWriterHelper : IRdfHandler, ISparqlResultsHandler
    {
        private readonly HttpResponseBase _response;
        private readonly XmlSparqlWriter _sparqlWriter;
        private readonly string _sparqlContentType;
        private readonly TurtleRdfWriter _rdfWriter;
        private readonly string _rdfContentType;
        private bool _start;

        public ResponseWriterHelper(HttpResponseBase response, XmlSparqlWriter sparqlWriter, string sparqlContentType, TurtleRdfWriter rdfWriter, string rdfContentType)
        {
            _response = response;
            _sparqlWriter = sparqlWriter;
            _sparqlContentType = sparqlContentType;
            _rdfWriter = rdfWriter;
            _rdfContentType = rdfContentType;
            _start = true;
        }

        private void WriteCommonHeaders()
        {
            
        }

        private void HandleSparqlStart()
        {
            if (_start)
            {
                WriteCommonHeaders();

                _response.ContentType = _sparqlContentType;
                _response.Flush();

                _start = false;
            }
        }

        private void HandleRdfStart()
        {
            if (_start)
            {
                WriteCommonHeaders();

                _response.ContentType = _rdfContentType;
                _response.Flush();

                _start = false;
            }
        }

        #region IRdfHandler
        public IBlankNode CreateBlankNode()
        {
            HandleRdfStart();
            return ((INodeFactory) _rdfWriter).CreateBlankNode();
        }

        public IBlankNode CreateBlankNode(string nodeId)
        {
            HandleRdfStart();
            return ((INodeFactory) _rdfWriter).CreateBlankNode(nodeId);
        }

        public IGraphLiteralNode CreateGraphLiteralNode()
        {
            HandleRdfStart();
            return ((INodeFactory) _rdfWriter).CreateGraphLiteralNode();
        }

        public IGraphLiteralNode CreateGraphLiteralNode(IGraph subgraph)
        {
            HandleRdfStart();
            return ((INodeFactory) _rdfWriter).CreateGraphLiteralNode(subgraph);
        }

        public ILiteralNode CreateLiteralNode(string literal)
        {
            HandleRdfStart();
            return ((INodeFactory) _rdfWriter).CreateLiteralNode(literal);
        }

        public ILiteralNode CreateLiteralNode(string literal, string langspec)
        {
            HandleRdfStart();
            return ((INodeFactory) _rdfWriter).CreateLiteralNode(literal, langspec);
        }

        public IVariableNode CreateVariableNode(string varname)
        {
            HandleRdfStart();
            return ((INodeFactory) _rdfWriter).CreateVariableNode(varname);
        }

        public string GetNextBlankNodeID()
        {
            HandleRdfStart();
            return ((INodeFactory) _rdfWriter).GetNextBlankNodeID();
        }

        public IUriNode CreateUriNode(Uri uri)
        {
            HandleRdfStart();
            return ((INodeFactory) _rdfWriter).CreateUriNode(uri);
        }

        public ILiteralNode CreateLiteralNode(string literal, Uri datatype)
        {
            HandleRdfStart();
            return _rdfWriter.CreateLiteralNode(literal, datatype);
        }

        public void StartRdf()
        {
            HandleRdfStart();
            ((IRdfHandler) _rdfWriter).StartRdf();
        }

        public void EndRdf(bool ok)
        {
            HandleRdfStart();
            ((IRdfHandler) _rdfWriter).EndRdf(ok);
        }

        public bool HandleTriple(Triple t)
        {
            HandleRdfStart();
            return ((IRdfHandler) _rdfWriter).HandleTriple(t);
        }

        public bool AcceptsAll
        {
            get { return _rdfWriter.AcceptsAll; }
        }

        public bool HandleBaseUri(Uri baseUri)
        {
            HandleRdfStart();
            return ((IRdfHandler) _rdfWriter).HandleBaseUri(baseUri);
        }

        public bool HandleNamespace(string prefix, Uri namespaceUri)
        {
            HandleRdfStart();
            return ((IRdfHandler) _rdfWriter).HandleNamespace(prefix, namespaceUri);
        }
        #endregion

        #region ISparqlResultsHandler
        public void StartResults()
        {
            HandleSparqlStart();
            ((ISparqlResultsHandler) _sparqlWriter).StartResults();
        }

        public void EndResults(bool ok)
        {
            HandleSparqlStart();
            ((ISparqlResultsHandler) _sparqlWriter).EndResults(ok);
        }

        public void HandleBooleanResult(bool result)
        {
            HandleSparqlStart();
            ((ISparqlResultsHandler) _sparqlWriter).HandleBooleanResult(result);
        }

        public bool HandleVariable(string var)
        {
            HandleSparqlStart();
            return ((ISparqlResultsHandler) _sparqlWriter).HandleVariable(var);
        }

        public bool HandleResult(SparqlResult result)
        {
            HandleSparqlStart();
            return ((ISparqlResultsHandler) _sparqlWriter).HandleResult(result);
        }
        #endregion
    }
}
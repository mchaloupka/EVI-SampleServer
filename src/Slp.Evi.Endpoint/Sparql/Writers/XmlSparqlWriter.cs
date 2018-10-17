using System;
using System.IO;
using System.Text;
using System.Xml;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Parsing.Handlers;
using VDS.RDF.Query;
using VDS.RDF.Writing;

namespace Slp.Evi.Endpoint.Sparql.Writers
{
    public sealed class XmlSparqlWriter
        : BaseResultsHandler, IDisposable
    {

        private readonly XmlWriter _xmlWriter;
        private bool _firstResult;
        private bool _handledBoolean;

        public XmlSparqlWriter(Stream outputStream)
        {
            _firstResult = true;
            _handledBoolean = false;

            _xmlWriter = XmlWriter.Create(outputStream, new XmlWriterSettings()
            {
                Indent = false,
                CloseOutput = false,
                Encoding = new UTF8Encoding(false)
            });
        }

        protected override void StartResultsInternal()
        {
            base.StartResultsInternal();
            _xmlWriter.WriteStartDocument();
            _xmlWriter.WriteStartElement("sparql", SparqlSpecsHelper.SparqlNamespace);
            _xmlWriter.WriteStartElement("head");
        }

        protected override void HandleBooleanResultInternal(bool result)
        {
            _xmlWriter.WriteEndElement();
            _xmlWriter.WriteStartElement("boolean");
            _xmlWriter.WriteString(result.ToString().ToLower());
            _xmlWriter.WriteEndElement();

            _handledBoolean = true;
        }

        protected override bool HandleVariableInternal(string var)
        {
            _xmlWriter.WriteStartElement("variable");
            _xmlWriter.WriteAttributeString("name", var);
            _xmlWriter.WriteEndElement();

            return true;
        }

        protected override bool HandleResultInternal(SparqlResult result)
        {
            if (_firstResult)
            {
                _xmlWriter.WriteEndElement();
                _xmlWriter.WriteStartElement("results");
                _firstResult = false;
            }

            _xmlWriter.WriteStartElement("result");

            foreach (var variable in result.Variables)
            {
                _xmlWriter.WriteStartElement("binding");
                _xmlWriter.WriteAttributeString("name", variable);

                var node = result.Value(variable);

                switch (node.NodeType)
                {
                    case NodeType.Blank:
                        _xmlWriter.WriteStartElement("bnode");
                        _xmlWriter.WriteRaw(((IBlankNode)node).InternalID);
                        _xmlWriter.WriteEndElement();
                        break;
                    case NodeType.Uri:
                        _xmlWriter.WriteStartElement("uri");
                        _xmlWriter.WriteRaw(WriterHelper.EncodeForXml(((IUriNode)node).Uri.AbsoluteUri));
                        _xmlWriter.WriteEndElement();
                        break;
                    case NodeType.Literal:
                        _xmlWriter.WriteStartElement("literal");
                        ILiteralNode literal = (ILiteralNode)node;

                        if (!literal.Language.Equals(String.Empty))
                        {
                            _xmlWriter.WriteStartAttribute("xml", "lang", XmlSpecsHelper.NamespaceXml);
                            _xmlWriter.WriteRaw(literal.Language);
                            _xmlWriter.WriteEndAttribute();
                        }
                        else if (literal.DataType != null)
                        {
                            _xmlWriter.WriteStartAttribute("datatype");
                            _xmlWriter.WriteRaw(WriterHelper.EncodeForXml(literal.DataType.AbsoluteUri));
                            _xmlWriter.WriteEndAttribute();
                        }

                        _xmlWriter.WriteRaw(WriterHelper.EncodeForXml(literal.Value));
                        _xmlWriter.WriteEndElement();
                        break;

                    case NodeType.GraphLiteral:
                        throw new RdfOutputException("Result Sets which contain Graph Literal Nodes cannot be serialized in the SPARQL Query Results XML Format");
                    case NodeType.Variable:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _xmlWriter.WriteEndElement();
            }

            _xmlWriter.WriteEndElement();
            return true;
        }

        protected override void EndResultsInternal(bool ok)
        {
            if (!_firstResult)
            {
                _xmlWriter.WriteEndElement();
            }

            if (_firstResult && !_handledBoolean)
            {
                _xmlWriter.WriteStartElement("results");
                _xmlWriter.WriteEndElement();
            }

            _xmlWriter.WriteEndElement();
            _xmlWriter.WriteEndDocument();
        }

        public void Dispose()
        {
            _xmlWriter.Close();
            _xmlWriter.Dispose();
        }
    }
}

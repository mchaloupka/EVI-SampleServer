using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing.Handlers;
using VDS.RDF.Writing;
using VDS.RDF.Writing.Formatting;

namespace Slp.r2rml4net.Server.Writers
{
    public class TurtleRdfWriter : BaseRdfHandler, IDisposable
    {
        private readonly StreamWriter _outputStream;
        private readonly TurtleFormatter _turtleFormatter;

        public TurtleRdfWriter(Stream outputStream)
        {
            _outputStream = new StreamWriter(outputStream, Encoding.UTF8, 64 * 1024, true);
            _turtleFormatter = new TurtleFormatter();
        }

        public void Dispose()
        {
            _outputStream.Close();
            _outputStream.Dispose();
        }

        protected override bool HandleTripleInternal(Triple t)
        {
            _outputStream.Write(GenerateNodeOutput(t.Subject, TripleSegment.Subject));
            _outputStream.Write(" ");
            _outputStream.Write(GenerateNodeOutput(t.Predicate, TripleSegment.Predicate));
            _outputStream.Write(" ");
            _outputStream.Write(GenerateNodeOutput(t.Object, TripleSegment.Object));
            _outputStream.Write(".");
            _outputStream.WriteLine();
            return true;
        }

        public override bool AcceptsAll => true;

        private string GenerateNodeOutput(INode node, TripleSegment segment)
        {
            switch (node.NodeType)
            {
                case NodeType.Blank:
                    if (segment == TripleSegment.Predicate)
                    {
                        throw new RdfOutputException(WriterErrorMessages.BlankPredicatesUnserializable("Turtle"));
                    }

                    return Format(node, segment);
                case NodeType.Uri:
                    return Format(node, segment);
                case NodeType.Literal:
                    if (segment == TripleSegment.Subject) throw new RdfOutputException(WriterErrorMessages.LiteralSubjectsUnserializable("Turtle"));
                    if (segment == TripleSegment.Predicate) throw new RdfOutputException(WriterErrorMessages.LiteralPredicatesUnserializable("Turtle"));
                    return Format(node, segment);
                case NodeType.GraphLiteral:
                    throw new RdfOutputException(WriterErrorMessages.GraphLiteralsUnserializable("Turtle"));
                default:
                    throw new RdfOutputException(WriterErrorMessages.UnknownNodeTypeUnserializable("Turtle"));
            }
        }

        protected override bool HandleBaseUriInternal(Uri baseUri)
        {
            _outputStream.WriteLine("@base <{0}>.", FormatUri(baseUri));
            _outputStream.WriteLine();
            return true;
        }

        protected override bool HandleNamespaceInternal(string prefix, Uri namespaceUri)
        {
            _outputStream.WriteLine("@prefix {0}: <{1}>.", prefix, FormatUri(namespaceUri));

            return base.HandleNamespaceInternal(prefix, namespaceUri);
        }

        private string FormatUri(Uri baseUri)
        {
            return _turtleFormatter.FormatUri(baseUri);
        }

        private string Format(INode node, TripleSegment segment)
        {
            return _turtleFormatter.Format(node, segment);
        }
    }
}

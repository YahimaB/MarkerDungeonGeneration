using System.Collections.Generic;
using Generation;
using Graph;

namespace Analysis.Analyzers
{
    public class LinearAnalyzer : Analyzer
    {
        protected override string FileName => "Linearity";

        private readonly List<float> _values = new();

        public override void ProcessData(GenerationContext context)
        {
            var tree = context.SpanningTree;
            var vEdgeCount = new Dictionary<Vertex, int>();

            foreach (var edge in tree)
            {
                vEdgeCount.TryAdd(edge.V, 0);
                vEdgeCount.TryAdd(edge.U, 0);

                vEdgeCount[edge.V] += 1;
                vEdgeCount[edge.U] += 1;
            }

            var endNodes = 0;
            var nonLinearNodes = 0;
            foreach (var edgeCount in vEdgeCount.Values)
            {
                if (endNodes < 2 && edgeCount == 1)
                {
                    endNodes++;
                    continue;
                }

                if (edgeCount != 2)
                {
                    nonLinearNodes++;
                }
            }

            _values.Add((float)nonLinearNodes / (vEdgeCount.Count - endNodes));
        }

        public override void DumpData()
        {
            SaveToFile(new List<Column>
            {
                new() { Header = FileName, Values = _values },
            });
        }
    }
}
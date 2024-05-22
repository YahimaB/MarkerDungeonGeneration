using System.Collections.Generic;
using System.Linq;
using DungeonData;
using Generation;

namespace Analysis.Analyzers
{
    public class CorridorCountAnalyzer : Analyzer
    {
        protected override string FileName => "CorridorCount";

        private readonly List<float> _values = new();

        public override void ProcessData(GenerationContext context)
        {
            var corridorCount = context.SpanningTree.Count;

            _values.Add(corridorCount);
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
using System.Collections.Generic;
using System.Linq;
using Generation;
using Utils;

namespace Analysis.Analyzers
{
    public class CorridorLengthAnalyzer : Analyzer
    {
        protected override string FileName => "CorridorLength";

        private readonly List<float> _values = new();

        //TODO: add deviation
        public override void ProcessData(GenerationContext context)
        {
            var paths = context.Paths;

            var pathsCount = paths.Count();
            var pathsLength = paths.Sum(x => x.Count);

            _values.Add((float)pathsLength / pathsCount);
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
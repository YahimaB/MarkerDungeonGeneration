using System.Collections.Generic;
using Generation;

namespace Analysis.GeneticAnalyzers
{
    public class EdgesLengthAnalyzer: Analyzer
    {
        protected override string FileName => "EdgeLength";

        private readonly List<float> _values = new();

        public override void ProcessData(GenerationContext context)
        {
            var rates = context.Rates;

            var rate = rates["EdgeLengthRater"];

            _values.Add((float)rate);
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
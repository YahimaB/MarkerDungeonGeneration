using System.Collections.Generic;
using Generation;

namespace Analysis.GeneticAnalyzers
{
    public class EdgesFillingAnalyzer : Analyzer
    {
        protected override string FileName => "EdgesFilling";

        private readonly List<float> _values = new();

        public override void ProcessData(GenerationContext context)
        {
            var rates = context.Rates;

            var rate = rates["EdgesCountRater"];

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
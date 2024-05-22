using System.Collections.Generic;
using Generation;

namespace Analysis.Analyzers
{
    public class TestAnalyzer : Analyzer
    {
        protected override string FileName => "Test";

        public override void ProcessData(GenerationContext context) { }

        public override void DumpData()
        {
            var temp = new List<float> { 1f, 3f, 5f, 60f };
            var temp2 = new List<float> { 3f, 4f, 5f, 6f };

            SaveToFile(new List<Column>
            {
                new() { Header = FileName, Values = temp },
                new() { Header = "Temp2", Values = temp2 },
            });
        }
    }
}
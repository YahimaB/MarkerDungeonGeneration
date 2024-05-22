using System.Collections.Generic;
using System.Linq;
using DungeonData;
using Generation;

namespace Analysis.Analyzers
{
    public class TotalVolumeAnalyzer : Analyzer
    {
        protected override string FileName => "TotalVolume";

        private readonly List<float> _values = new();

        public override void ProcessData(GenerationContext context)
        {
            var grid = context.Grid;
            var volume = grid.Data.Count(x => x.Type is not CellType.None);
            _values.Add(volume);
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
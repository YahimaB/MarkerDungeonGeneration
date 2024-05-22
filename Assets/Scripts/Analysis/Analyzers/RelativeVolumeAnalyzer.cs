using System.Collections.Generic;
using System.Linq;
using DungeonData;
using Generation;

namespace Analysis.Analyzers
{
    public class RelativeVolumeAnalyzer : Analyzer
    {
        protected override string FileName => "RelativeVolume";

        private readonly List<float> _values = new();

        public override void ProcessData(GenerationContext context)
        {
            var grid = context.Grid;
            var volume = grid.Data.Count(x => x.Type is not CellType.None);
            var roomVolume = grid.Data.Count(x => x.Type is CellType.Room);
            var corridorVolume = grid.Data.Count(x => x.Type is CellType.Hallway);

            _values.Add((float)roomVolume / volume);
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
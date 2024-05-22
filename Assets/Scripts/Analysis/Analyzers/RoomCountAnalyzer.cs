using System.Collections.Generic;
using System.Linq;
using Generation;

namespace Analysis.Analyzers
{
    public class RoomCountAnalyzer : Analyzer
    {
        protected override string FileName => "RoomCount";

        private readonly List<float> _values = new();

        public override void ProcessData(GenerationContext context)
        {
            var roomCount = context.Rooms.Count();

            _values.Add(roomCount);
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
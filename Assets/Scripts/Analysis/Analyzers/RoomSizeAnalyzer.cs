using System.Collections.Generic;
using Generation;
using Utils;

namespace Analysis.Analyzers
{
    public class RoomSizeAnalyzer : Analyzer
    {
        protected override string FileName => "RoomSize";

        private readonly List<float> _values = new();

        public override void ProcessData(GenerationContext context)
        {
            var volumes = new List<float>();
            var targets = new List<float>();

            foreach (var room in context.Rooms)
            {
                volumes.Add(room.Bounds.GetBoundsVolume());

                var targetSize = room.TargetSize;
                targets.Add(targetSize.x * targetSize.y * targetSize.z);
            }

            var wape = volumes.MAPE(targets);
            _values.Add(wape);
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
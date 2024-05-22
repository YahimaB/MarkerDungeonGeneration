using System.Collections.Generic;
using System.Diagnostics;
using Generation;
using UnityEngine;

namespace Analysis.Analyzers
{
    public class TimeAnalyzer : Analyzer
    {
        protected override string FileName => "GenerationTime";

        private readonly List<float> _values = new();

        private readonly Stopwatch _timer = new();

        public override void ProcessData(GenerationContext context)
        {
            // var tree = context.SpanningTree;
        }

        public override void DumpData()
        {
            SaveToFile(new List<Column>
            {
                new() { Header = FileName, Values = _values },
            });
        }

        public void Start()
        {
            _timer.Restart();
            // Time.realtimeSinceStartup
        }

        public void Stop()
        {
            _timer.Stop();
            _values.Add(_timer.ElapsedMilliseconds);
        }
    }
}
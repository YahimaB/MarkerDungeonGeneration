using System.Collections.Generic;
using Generation;
using UnityEngine;
using Utils;

namespace Analysis.Analyzers
{
    public class AvailabilityAnalyzer : Analyzer
    {
        protected override string FileName => "Availability";

        private readonly List<float> _cValues = new();
        private readonly List<float> _klValues = new();

        public override void ProcessData(GenerationContext context)
        {
            var rates = context.Rates;

            var componentsRate = rates["GraphComponentRater"];
            var keyLockRate = rates["KeyLocksRater"];
            
            // var componentsRate = rates["compGen"];
            // var keyLockRate = rates["keyLockGen"];

            _cValues.Add((float)componentsRate);
            _klValues.Add((float)keyLockRate);
        }

        public override void DumpData()
        {
            SaveToFile(new List<Column>
            {
                new() { Header = "Availability", Values = _cValues },
                new() { Header = "KeyLocks", Values = _klValues },
            });
        }
    }
}
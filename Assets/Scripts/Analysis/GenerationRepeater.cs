using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Analysis.Analyzers;
using Analysis.GeneticAnalyzers;
using Generation;
using UnityEngine;
using Utils.ClassTypeReference;
using Debug = UnityEngine.Debug;

namespace Analysis
{
    public class GenerationRepeater : MonoBehaviour
    {
        [SerializeField] private bool _demoMode;
        [SerializeField] private int _totalGenerations = 100;
        [SerializeField] private int _stepCount = 10;
        [SerializeField] private Generator _generator;
        
        [SerializeField]
        [ClassExtends(typeof(Analyzer))]
        public List<ClassTypeReference> _analyzerTypes;

        private readonly Stopwatch _timer = new();

        private readonly List<Analyzer> _analyzers = new();
        private TimeAnalyzer _timeAnalyzer;

        private void Start()
        {
            _analyzers.Clear();
            
            foreach (var analyzerType in _analyzerTypes)
            {
                if (analyzerType?.Type == null)
                {
                    continue;
                }
                
                var analyzer = (Analyzer)Activator.CreateInstance(analyzerType.Type);
                _analyzers.Add(analyzer);
            }
            
            _timeAnalyzer = new TimeAnalyzer();
            _analyzers.Add(_timeAnalyzer);

            // _analyzers = new List<Analyzer>
            // {
            //     // new TestAnalyzer(),
            //     // new TotalVolumeAnalyzer(),
            //     // new RelativeVolumeAnalyzer(),
            //     // new RoomCountAnalyzer(),
            //     // new RoomSizeAnalyzer(), //TODO: deviation
            //     // new AvailabilityAnalyzer(),
            //     // new CorridorCountAnalyzer(),
            //     // new CorridorLengthAnalyzer(),
            //     // new LinearAnalyzer(),
            //     // new EdgesFillingAnalyzer(),
            //     // new BranchingRateAnalyzer(),
            //     // new SlopeRateAnalyzer(),
            //     new EdgesLengthAnalyzer(),
            //     _timeAnalyzer
            // };

            if (_demoMode)
            {
                RunDemoGeneration();
            }
            else
            {
                StartCoroutine(GenerationCoroutine());
            }
        }

        private void RunDemoGeneration()
        {
            _generator.StartGeneration(true);
        }

        private IEnumerator GenerationCoroutine()
        {
            _timer.Start();
            var startTime = Time.realtimeSinceStartup;
            var step = 0;

            yield return new WaitForEndOfFrame();
            while (step < _totalGenerations)
            {
                step++;
                if (step % _stepCount == 0)
                {
                    Debug.Log($"DONE: {step} / {_totalGenerations}");
                    Debug.Log($"STOPWATCH TIME: {_timer.Elapsed.TotalSeconds} seconds");
                    GC.Collect();
                    yield return new WaitForEndOfFrame();
                }
                // ClearObjects();

                try
                {
                    _timeAnalyzer.Start();
                    _generator.StartGeneration(false);
                    _timeAnalyzer.Stop();
                    // yield return new WaitForSecondsRealtime(2f);

                    var currentContext = GenerationContext.CurrentContext;
                    foreach (var analyzer in _analyzers)
                    {
                        analyzer.ProcessData(currentContext);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }
            }

            foreach (var analyzer in _analyzers)
            {
                analyzer.DumpData();
            }

            _timer.Stop();
            var endTime = Time.realtimeSinceStartup;

            Debug.Log($"STOPWATCH TIME: {_timer.Elapsed.TotalSeconds} seconds");
            Debug.Log($"REAL TIME: {endTime - startTime} seconds");

            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
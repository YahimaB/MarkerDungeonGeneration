using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Analysis.Analyzers;
using Analysis.GeneticAnalyzers;
using Generation;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Analysis
{
    public class GenerationRepeater : MonoBehaviour
    {
        [SerializeField] private bool _demoMode;
        [SerializeField] private int _totalGenerations = 100;
        [SerializeField] private int _stepCount = 10;
        [SerializeField] private Generator _generator;
        [SerializeField] private string _cellObjectTag;

        private readonly Stopwatch _timer = new();

        private List<Analyzer> _analyzers;
        private TimeAnalyzer _timeAnalyzer;

        private void Start()
        {
            _timeAnalyzer = new TimeAnalyzer();

            _analyzers = new List<Analyzer>
            {
                // new TestAnalyzer(),
                // new TotalVolumeAnalyzer(),
                // new RelativeVolumeAnalyzer(),
                // new RoomCountAnalyzer(),
                // new RoomSizeAnalyzer(), //TODO: deviation
                // new AvailabilityAnalyzer(),
                // new CorridorCountAnalyzer(),
                // new CorridorLengthAnalyzer(),
                // new LinearAnalyzer(),
                // new EdgesFillingAnalyzer(),
                // new BranchingRateAnalyzer(),
                // new SlopeRateAnalyzer(),
                new EdgesLengthAnalyzer(),
                _timeAnalyzer
            };

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

        private void ClearObjects()
        {
            var cellObjects = GameObject.FindGameObjectsWithTag(_cellObjectTag);
            for (var i = 0; i < cellObjects.Length; i++)
            {
                Destroy(cellObjects[i]);
            }
        }
    }
}
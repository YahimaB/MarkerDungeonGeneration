using System;
using System.Collections.Generic;
using System.Linq;
using Generation;
using Graph.GeneticSearch.Raters;
using Graph.Prim;
using UnityEngine;
using Utils;

namespace Graph.GeneticSearch
{
    public class GeneticSolver
    {
        public GenerationContext Context { get; private set; }
        public List<GeneticRater> Raters { get; private set; }

        public bool[] BestSln => _bestSln;

        private readonly List<Edge> _edges;
        private readonly int _numEdges;
        private readonly int _numPop; // should be even

        private readonly bool[][] _pop; // array of solutions[]
        private readonly double[] _errs;

        private readonly bool[] _bestSln;
        private double _bestErr;
        private int _gen;
        private int _compGen;
        private int _keyLockGen;
        
        public GeneticSolver(GenerationContext context, List<GeneticRater> raters, int numPop)
        {
            Context = context;
            Raters = raters;
            _edges = context.DeloneGraph.Edges;

            // _edges = edges.Select(x => new PrimEdge(x.U, x.V)).ToList();
            _numEdges = _edges.Count;
            _numPop = numPop;

            _bestErr = 0.0;
            _gen = 0;
            _compGen = -1;
            _keyLockGen = -1;

            _pop = new bool[_numPop][];
            for (var i = 0; i < _numPop; i++)
            {
                _pop[i] = new bool[_numEdges];
            }
            _errs = new double[_numPop];

            for (var i = 0; i < _numPop; i++)
            {
                RandomizeSolution(_pop[i]);
                _errs[i] = RateSolution(_pop[i]);
            }

            Array.Sort(_errs, _pop);

            _bestSln = new bool[_numEdges];
            for (var i = 0; i < _numEdges; i++)
            {
                _bestSln[i] = _pop[0][i];
            }

            _bestErr = _errs[0];
        }

        private void RandomizeSolution(bool[] sln)
        {
            for (var i = 0; i < sln.Length; i++)
            {
                sln[i] = GenRandom.NextBool();
                if (_edges[i] is PermanentEdge)
                {
                    sln[i] = true;
                }
            }
        }

        //Smaller is better
        private double RateSolution(bool[] sln)
        {
            var totalRate = 0.0;
            foreach (var rater in Raters)
            {
                var rate = rater.Rate(Context, sln);
                totalRate += rate * rater.Weight;

                if (_compGen <= 0 && rater is GraphComponentRater)
                {
                    if (rate == 0)
                    {
                        _compGen = _gen;
                    }
                }
                
                if (_keyLockGen <= 0 && rater is KeyLocksRater)
                {
                    if (rate == 0)
                    {
                        _keyLockGen = _gen;
                    }
                }
                
                if (totalRate < 0)
                {
                    totalRate = double.MaxValue;
                }
            }
            
            return totalRate;
        }

        public void Solve(int maxGen)
        {
            for (_gen = 1; _gen <= maxGen; ++_gen)
            {
                // 1. pick parent indexes
                int idx1, idx2;
                var flip = GenRandom.NextBool();
                if (flip)
                {

                    idx1 = GenRandom.Next(0, _numPop / 2);
                    idx2 = GenRandom.Next(_numPop / 2, _numPop);
                }
                else
                {
                    idx2 = GenRandom.Next(0, _numPop);
                    idx1 = GenRandom.Next(_numPop / 2, _numPop);
                }

                // 2. create a child
                var child = MakeChild(idx1, idx2);

                // 3. mutate
                Mutate(child);
                var childErr = RateSolution(child);

                // 4. found new best?
                if (childErr < _bestErr)
                {
                    // Debug.Log("New best solution found at gen " + gen);
                    for (var i = 0; i < child.Length; ++i)
                    {
                        _bestSln[i] = child[i];
                    }
                    _bestErr = childErr;
                }

                // 4. replace weak solution
                var idx = GenRandom.Next(_numPop / 2, _numPop);
                _pop[idx] = child;
                _errs[idx] = childErr;

                // 5. create an immigrant
                var imm = new bool[_numEdges];
                for (var i = 0; i < _numEdges; ++i)
                {
                    RandomizeSolution(imm);
                }
                var immErr = RateSolution(imm);

                // found new best?
                if (immErr < _bestErr)
                {
                    // Debug.Log("New best (immigrant) at gen " + gen);
                    for (int i = 0; i < imm.Length; ++i)
                    {
                        _bestSln[i] = imm[i];
                    }
                    _bestErr = immErr;
                }

                // 4. replace weak solution
                idx = GenRandom.Next(_numPop / 2, _numPop);
                _pop[idx] = imm;
                _errs[idx] = immErr;

                // 5. sort the new population
                Array.Sort(_errs, _pop);

                // if (_gen == maxGen && _bestErr >= GeneticRater.FailureValue && maxGen != 100000)
                // {
                //     maxGen++;
                // }
            }
        }

        private bool[] MakeChild(int idx1, int idx2) // crossover
        {
            var parent1 = _pop[idx1];
            var parent2 = _pop[idx2];
            var result = new bool[_numEdges];

            var k = 0;
            for (var i = 0; i < _numEdges / 2; ++i) // left parent1
            {
                result[k++] = parent1[i];
            }

            for (var i = _numEdges / 2; i < _numEdges; ++i) // right parent2
            {
                result[k++] = parent2[i];
            }

            return result;
        } // MakeChild

        private void Mutate(bool[] sln)
        {
            int idx;
            do
            {
                idx = GenRandom.Next(0, sln.Length);
            } while (_edges[idx] is PermanentEdge);
            sln[idx] = !sln[idx];
        }

        public void Show()
        {
            Debug.Log("Genetic Solution");
            // Debug.Log($"Error = {_bestErr} : {string.Join(" | ", _bestSln)}");
            
            // foreach (var rater in Raters)
            // {
            //     var rate = rater.Rate(Context, _bestSln);
            //     Debug.Log($"{rater.GetType()}: pure = {rate} ; weighted = {rate * rater.Weight}");
            // }
            
            Debug.Log($"genCount = {_gen} | compGen = {_compGen} | keyLockGen = {_keyLockGen}");
        }

        public void Dump()
        {
            // Show();
            
            var rates = new Dictionary<string, double>();
            
            foreach (var rater in Raters)
            {
                var rate = rater.Rate(Context, _bestSln);
                rates.Add($"{rater.GetType().Name}", rate);
                Debug.Log($"{rater.GetType()}: pure = {rate} ; weighted = {rate * rater.Weight}");
            }
            
            rates.Add("genCount", _gen);
            rates.Add("compGen", _compGen);
            rates.Add("keyLockGen", _keyLockGen);
            
            Context.SetRates(rates);
        }

        //TODO: Remove
        private readonly Dictionary<Vertex, HashSet<Vertex>> _g = new Dictionary<Vertex, HashSet<Vertex>>();
        private readonly HashSet<Vertex> _used = new HashSet<Vertex>();
        private readonly List<Vertex> _comp = new List<Vertex>();

        private int CountComponents(bool[] sln)
        {
            _g.Clear();
            _used.Clear();

            var vertices = Context.DeloneGraph.Vertices;
            foreach (var vertex in vertices)
            {
                _g[vertex] = new HashSet<Vertex>();
            }

            for (var i = 0; i < _numEdges; i++)
            {
                if (sln[i])
                {
                    var edge = _edges[i];
                    _g[edge.V].Add(edge.U);
                    _g[edge.U].Add(edge.V);
                }
            }

            var compCount = 0;
            foreach (var v in vertices)
            {
                if (!_used.Contains(v))
                {
                    _comp.Clear();
                    Dfs(v);
                    compCount++;
                }
            }

            return compCount;
        }

        private void Dfs(Vertex v)
        {
            _used.Add(v);
            _comp.Add(v);
            foreach (var v2 in _g[v])
            {
                if (!_used.Contains(v2))
                {
                    Dfs(v2);
                }
            }
        }
    }
}
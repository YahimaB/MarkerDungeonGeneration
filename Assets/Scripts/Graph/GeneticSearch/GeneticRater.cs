using System;
using Generation;
using UnityEngine;
using Utils.ClassTypeReference;

namespace Graph.GeneticSearch
{
    [Serializable]
    public abstract class GeneticRater
    {
        // [SerializeField]
        // private GeneticRaterMode _mode;
        public const double FailureValue = 100000000;

        public virtual float Weight => 1f;
        public virtual GeneticRaterMode Mode => GeneticRaterMode.Add;

        public abstract double Rate(GenerationContext context, bool[] sln);
    }

    [Serializable]
    public class GeneticRaterHolder
    {
        [SerializeField]
        [ClassExtends(typeof(GeneticRater))]
        public ClassTypeReference RaterType;

        [SerializeReference]
        public GeneticRater Rater = null;
    }

    public enum GeneticRaterMode
    {
        Add = 1,
        Multiply = 2,
    }
}
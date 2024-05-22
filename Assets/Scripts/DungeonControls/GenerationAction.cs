using System;
using DungeonData;
using Generation;
using UnityEngine;
using Utils.ClassTypeReference;

namespace DungeonControls
{
    [Serializable]
    public abstract class GenerationAction
    {
        public Room HostRoom { get; set; }
        
        public virtual void Run() { }

        protected GenerationContext GetContext()
        {
            return GenerationContext.CurrentContext;
        }
    }

    [Serializable]
    public class GenerationActionHolder
    {
        [SerializeField]
        [ClassExtends(typeof(GenerationAction))]
        public ClassTypeReference ActionType;

        [SerializeReference]
        public GenerationAction Action = null;
    }
}
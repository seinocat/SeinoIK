using System;
using System.Collections.Generic;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class IKSolverHeuristic : IKSolver
    {
        public int MaxIterations = 5;

        public Transform Target;
        
        public List<Bone> Bones = new List<Bone>();

        protected override void OnUpadete() {}

        
    }
}
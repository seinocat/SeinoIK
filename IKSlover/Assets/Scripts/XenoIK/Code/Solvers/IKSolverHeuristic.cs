using System;
using System.Collections.Generic;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class IKSolverHeuristic : IKSolver
    {
        public int maxIterations = 5;

        public int bonesCount;
        
        public Transform target;
        
        public List<Bone> bones = new List<Bone>();
        
        protected override void OnInitialize() { }
        protected override void OnUpdate(float deltaTime) {}

        protected void InitBones()
        {
            
        }
    }
}
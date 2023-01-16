using System;
using System.Collections.Generic;
using UnityEngine;

namespace XenoIK
{
    [Serializable]
    public class IKSolverLookAt : IKSolver
    {
        public Transform target;
        public LookAtBone head;
        public List<LookAtBone> eyes;
        public List<LookAtBone> spines;


        protected override void OnInitialize()
        {
            
        }

        protected override void OnUpdate(float deltaTime)
        {
            
        }
    }
}
using System;
using System.Collections.Generic;

namespace XenoIK
{
    [Serializable]
    public class IKSolverTwoBone : IKSolver
    {

        public List<Bone> TwoBoneList = new List<Bone>(){new Bone(), new Bone(), new Bone()};

        protected override void OnInitialize()
        {
            
        }

        protected override void OnUpdate(float deltaTime)
        {
            
        }

        public override void StoreDefaultLocalState()
        {
            
        }

        public override void FixTransform()
        {
            
        }
    }
}
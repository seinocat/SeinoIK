using System;
using UnityEditor;
using UnityEngine;
using XenoIK.Editor;

namespace XenoIK
{
    [CustomEditor(typeof(TwoBoneIK))]
    public class TwoBoneInspector : IKInspector
    {
        public TwoBoneIK script => target as TwoBoneIK;
        
        private IKSolverTwoBoneInspector Inspector => this.solverInspector as IKSolverTwoBoneInspector;
        
        
        protected override void OnInspectorEnable()
        {
            this.solverInspector = new IKSolverTwoBoneInspector();
        }

        protected override void DrawInspector()
        {
            this.Inspector.DrawInspector(this.solver, script);
        }

        protected override void OnModifty()
        {
            
        }

 
    }
}
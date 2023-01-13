using System;
using UnityEngine;

namespace XenoIK
{
    public class SolverManager : MonoBehaviour
    {
        protected virtual void InitialSolver(){}
        protected virtual void UpdateSolver(){}

        private void Start()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            InitialSolver();
        }
        
        private void LateUpdate()
        {
            UpdateSolver();
        }
    }
}
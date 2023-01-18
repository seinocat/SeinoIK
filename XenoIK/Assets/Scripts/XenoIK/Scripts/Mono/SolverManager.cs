using System;
using UnityEngine;

namespace XenoIK
{
    public class SolverManager : MonoBehaviour
    {
        protected virtual void InitialSolver(){}
        protected virtual void UpdateSolver(float deltaTime){}
        protected virtual void FixTransform(){}

        private void Start()
        {
            this.Initialize();
        }

        private void Update()
        {
            this.FixTransform();
        }

        private void Initialize()
        {
            this.InitialSolver();
        }
        
        private void LateUpdate()
        {
            this.UpdateSolver(Time.deltaTime);
        }
    }
}
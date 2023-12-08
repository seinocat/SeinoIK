using UnityEngine;

namespace SeinoIK
{
    public class SolverManager : MonoBehaviour
    {
        protected virtual void InitialSolver(){}
        protected virtual void UpdateSolver(float deltaTime){}
        protected virtual void FixTransform(){}

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            FixTransform();
        }

        private void Initialize()
        {
            InitialSolver();
        }
        
        private void LateUpdate()
        {
            UpdateSolver(Time.deltaTime);
        }
    }
}
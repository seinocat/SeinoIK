using System;
using UnityEngine;

namespace SeinoIK
{
    [Serializable]
    public class IKSolverFABR : IKSolverHeuristic
    {
        protected override void OnInitialize()
        {
            this.InitBones();
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (this.IKWeight == 0) return;
            if (this.target != null) this.IKPosition = this.target.position;
            for (int i = 0; i < this.maxIterations; i++)
            {
                this.Solve();
            }
        }
        
        private void Solve()
        {
            PreSolve();
            ForwardSolve();
            BackwardSolve(this.bones[0].Position);
            PostSolve();
        }

        private void PreSolve()
        {
            this.chainLength = 0;
            for (int i = 0; i < this.bones.Count; i++)
            {
                var bone = this.bones[i];
                bone.solverPosition = bone.Position;
                bone.solverRotation = bone.Rotation;

                if (i < this.bones.Count - 1)
                {
                    bone.length = (bone.Position - bones[i + 1].Position).magnitude;
                    bone.axis = Quaternion.Inverse(bone.Rotation) * (bones[i + 1].Position - bone.Position);
                    this.chainLength += bone.length;
                }
            }
        }
        
        private void PostSolve()
        {
            this.bones[0].Position = bones[0].solverPosition;
            for (int i = 0; i < this.bones.Count; i++)
            {
                if (i < this.bones.Count - 1) 
                    bones[i].Swing(bones[i+1].solverPosition);
            }
        }
        
        private void ForwardSolve()
        {
            var effectorBone =  this.bones[this.bones.Count - 1];
            effectorBone.solverPosition = Vector3.Lerp(effectorBone.solverPosition, this.IKPosition, this.IKWeight);
            
            for (int i = this.bones.Count - 2; i >= 0; i--)
            {
                var bone = this.bones[i];
                var nextBone = this.bones[i + 1];
                bone.solverPosition = SolveJoint(bone.solverPosition, nextBone.solverPosition, bone.length);
            }
        }

        private void BackwardSolve(Vector3 pos)
        {
            this.bones[0].solverPosition = pos;

            for (int i = 1; i < this.bones.Count; i++)
            {
                var bone = this.bones[i];
                var lastBone = this.bones[i - 1];
                this.bones[i].solverPosition = SolveJoint(bone.solverPosition, lastBone.solverPosition, lastBone.length);
            }
        }
        
        private Vector3 SolveJoint(Vector3 p1, Vector3 p2, float length)
        {
            return p2 + (p1 - p2).normalized * length;
        }

        
    }
}